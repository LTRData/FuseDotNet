using FuseDotNet;
using FuseDotNet.Extensions;
using MirrorFs;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

[SupportedOSPlatform("linux")]
[SupportedOSPlatform("freebsd")]
internal static class Program
{
    private static int assertions;

    private static void Main()
    {
        CheckFlags();
        CheckModes();
        var root = Path.Combine(Path.GetTempPath(), $"fusedotnet-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            CheckMirror(root);
            CheckConcurrentAppend(root);
            CheckWriteback(root);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
        Console.WriteLine($"PASS: {assertions} native-flag, mode and MirrorFs assertions");
    }

    private static void CheckFlags()
    {
        PosixOpenFlags[] values = [PosixOpenFlags.Read, PosixOpenFlags.Write, PosixOpenFlags.ReadWrite,
            PosixOpenFlags.AccessModes, PosixOpenFlags.NonBlocking, PosixOpenFlags.Append, PosixOpenFlags.Async,
            PosixOpenFlags.SynchronousWrites, PosixOpenFlags.NoFollowLinks, PosixOpenFlags.Create, PosixOpenFlags.Truncate,
            PosixOpenFlags.CreateNew, PosixOpenFlags.NoControllingTerminal, PosixOpenFlags.Direct,
            PosixOpenFlags.Directory, PosixOpenFlags.CloseOnExec, PosixOpenFlags.DataSync];
        for (var i = 0; i < values.Length; i++)
        {
            Equal(Native.open_flag_value((uint)i), (int)values[i], $"native flag {i}");
        }
        Func<PosixOpenFlags>[] bsdOnly = [() => PosixOpenFlags.SharedLock, () => PosixOpenFlags.ExclusiveLock,
            () => PosixOpenFlags.Execute, () => PosixOpenFlags.Search, () => PosixOpenFlags.TtyInit,
            () => PosixOpenFlags.Verify, () => PosixOpenFlags.ResolveBeneath];
        for (var i = 0; i < bsdOnly.Length; i++)
        {
            if (OperatingSystem.IsFreeBSD())
            {
                Equal(Native.open_flag_value((uint)(values.Length + i)), (int)bsdOnly[i](), "FreeBSD flag");
            }
            else
            {
                Throws<PlatformNotSupportedException>(() => bsdOnly[i](), "unsupported flag is not zero or another native bit");
            }
        }
        Equal((nuint)Marshal.SizeOf<PosixOpenFlags>(), Native.flags_size(), "native flag width");
        string[] fields = ["flags", "fh", "lock_owner", "poll_events"];
        for (var i = 0; i < fields.Length; i++)
        {
            Equal(Native.file_info_offset((uint)i), (nuint)Marshal.OffsetOf<FuseFileInfo>(fields[i]), fields[i]);
        }
        var info = new FuseFileInfo();
        var raw = Native.open_flag_value(2) | Native.open_flag_value(5) | unchecked((int)0x80000000);
        Native.set_flags(ref info, raw);
        Equal(raw, (int)info.flags, "native-to-managed unknown bits preserved");
        Equal(raw, Native.get_flags(ref info), "native storage unchanged");
        Equal(true, info.flags.HasFlag(PosixOpenFlags.Append), "append decoded");
        Equal(false, info.flags.HasFlag(PosixOpenFlags.Truncate), "append is not truncate");
        Equal(FileAccess.ReadWrite, info.flags.ToFileAccess(), "access bits decoded");
        Equal(raw, (int)(PosixOpenFlags)raw, "raw integer round trip");
        Equal((int)PosixOpenFlags.ReadWrite, (int)(info.flags & 3), "integer mask operator");
        Equal((int)PosixOpenFlags.ReadWrite, (int)(3 & info.flags), "reversed integer mask operator");
        Equal(raw & ~(int)PosixOpenFlags.Append, (int)(info.flags & ~PosixOpenFlags.Append), "clear one bit preserves others");
        Equal(true, (info.flags ^ PosixOpenFlags.Append) == (raw ^ (int)PosixOpenFlags.Append), "xor and integer equality");
        Equal(true, (PosixOpenFlags.ReadWrite | PosixOpenFlags.Append).HasFlag(PosixOpenFlags.ReadWrite | PosixOpenFlags.Append), "combined mask");
        Equal(false, PosixOpenFlags.DataSync.HasFlag(PosixOpenFlags.SynchronousWrites) && OperatingSystem.IsLinux(), "data sync is not full Linux sync");
        Equal(true, typeof(PosixOpenFlags).GetMember("DirSync").Length == 0, "no compatibility alias");
        Equal(FileShare.ReadWrite | FileShare.Delete, info.flags.ToFileShare(), "ordinary sharing");
        if (OperatingSystem.IsFreeBSD())
        {
            Equal(FileShare.Read | FileShare.Delete, PosixOpenFlags.SharedLock.ToFileShare(), "shared lock");
            Equal(FileShare.Delete, PosixOpenFlags.ExclusiveLock.ToFileShare(), "exclusive lock");
        }
    }

    private static void CheckModes()
    {
        for (var bits = 0; bits < 16; bits++)
        {
            PosixOpenFlags flags = 0;
            if ((bits & 1) != 0) flags |= PosixOpenFlags.Create;
            if ((bits & 2) != 0) flags |= PosixOpenFlags.CreateNew;
            if ((bits & 4) != 0) flags |= PosixOpenFlags.Truncate;
            if ((bits & 8) != 0) flags |= PosixOpenFlags.Append;
            var expected = (bits & 3) == 3 ? FileMode.CreateNew
                : (bits & 5) == 5 ? FileMode.Create
                : (bits & 1) != 0 ? FileMode.OpenOrCreate
                : (bits & 4) != 0 ? FileMode.Truncate : FileMode.Open;
            Equal(expected, flags.ToFileMode(), $"creation/append combination {bits}");
        }
        Equal(FileAccess.Read, PosixOpenFlags.Read.ToFileAccess(), "read access");
        Equal(FileAccess.Write, PosixOpenFlags.Write.ToFileAccess(), "write access");
        Equal(FileAccess.ReadWrite, PosixOpenFlags.ReadWrite.ToFileAccess(), "read/write access");
    }

    private static void CheckMirror(string root)
    {
        using var operations = new MirrorFsOperations(root);
        var path = Path.Combine(root, "file");
        File.WriteAllText(path, "seed");
        var info = Open(operations, "/file", PosixOpenFlags.Create | PosixOpenFlags.Write, create: true);
        Close(operations, ref info);
        Equal("seed", File.ReadAllText(path), "O_CREAT preserves contents");
        Throws<IOException>(() => Open(operations, "/file", PosixOpenFlags.Create | PosixOpenFlags.CreateNew | PosixOpenFlags.Write, create: true), "exclusive create fails");
        Equal("seed", File.ReadAllText(path), "exclusive failure preserves contents");
        info = Open(operations, "/file", PosixOpenFlags.CreateNew | PosixOpenFlags.Write);
        Close(operations, ref info);
        Equal("seed", File.ReadAllText(path), "O_EXCL alone does not create or truncate");

        info = Open(operations, "/file", PosixOpenFlags.ReadWrite | PosixOpenFlags.Append);
        try
        {
            Equal("seed", Read(operations, ref info), "read/write append permits reading");
            Write(operations, ref info, "A", 0);
            File.AppendAllText(path, "B");
            Write(operations, ref info, "C", 0);
            Equal("seedABC", Read(operations, ref info), "every append uses current EOF despite stale offset and external growth");
        }
        finally { Close(operations, ref info); }
        Equal("seedABC", File.ReadAllText(path), "append preserves contents");

        var missing = Path.Combine(root, "missing");
        Throws<PosixException>(() => Open(operations, "/missing", PosixOpenFlags.Append | PosixOpenFlags.ReadWrite), "append without create fails for missing file");
        Equal(false, File.Exists(missing), "append does not create implicitly");
        info = Open(operations, "/missing", PosixOpenFlags.Create | PosixOpenFlags.Append | PosixOpenFlags.ReadWrite, create: true);
        try { Write(operations, ref info, "new", 0); }
        finally { Close(operations, ref info); }
        Equal("new", File.ReadAllText(missing), "explicit append creation");
        Throws<PosixException>(() => Open(operations, "/missing", PosixOpenFlags.Create | PosixOpenFlags.CreateNew | PosixOpenFlags.Append | PosixOpenFlags.Write, create: true), "exclusive append create fails");
        Equal("new", File.ReadAllText(missing), "exclusive append failure preserves contents");

        info = Open(operations, "/file", PosixOpenFlags.Truncate | PosixOpenFlags.Write);
        Close(operations, ref info);
        Equal(0L, new FileInfo(path).Length, "truncate clears existing file");
        File.WriteAllText(path, "old");
        info = Open(operations, "/file", PosixOpenFlags.Truncate | PosixOpenFlags.Append | PosixOpenFlags.ReadWrite);
        try { Write(operations, ref info, "after", 500); }
        finally { Close(operations, ref info); }
        Equal("after", File.ReadAllText(path), "truncate and append both honored");
    }

    private static void CheckConcurrentAppend(string root)
    {
        using var operations = new MirrorFsOperations(root);
        var path = Path.Combine(root, "concurrent");
        File.WriteAllText(path, "");
        var first = Open(operations, "/concurrent", PosixOpenFlags.Write | PosixOpenFlags.Append);
        var second = Open(operations, "/concurrent", PosixOpenFlags.Write | PosixOpenFlags.Append);
        try
        {
            Parallel.Invoke(
                () => { for (var i = 0; i < 256; i++) Write(operations, ref first, new string('A', 63) + "\n", 0); },
                () => { for (var i = 0; i < 256; i++) Write(operations, ref second, new string('B', 63) + "\n", 0); });
        }
        finally
        {
            Close(operations, ref first);
            Close(operations, ref second);
        }
        var lines = File.ReadAllLines(path);
        Equal(512, lines.Length, "concurrent append keeps every record");
        Equal(256, lines.Count(line => line == new string('A', 63)), "first writer records intact");
        Equal(256, lines.Count(line => line == new string('B', 63)), "second writer records intact");
    }

    private static void CheckWriteback(string root)
    {
        using var operations = new MirrorFsOperations(root);
        var conn = new FuseConnInfo { want = FuseCapabilities.WritebackCache };
        operations.Init(ref conn);
        var path = Path.Combine(root, "writeback");
        File.WriteAllText(path, "abcdef");
        var info = Open(operations, "/writeback", PosixOpenFlags.Write | PosixOpenFlags.Append);
        try
        {
            Equal("abcdef", Read(operations, ref info), "writeback permits reads on originally write-only handles");
            Write(operations, ref info, "X", 2);
        }
        finally { Close(operations, ref info); }
        Equal("abXdef", File.ReadAllText(path), "writeback respects supplied offset instead of appending again");
    }

    private static FuseFileInfo Open(MirrorFsOperations operations, string path, PosixOpenFlags flags, bool create = false)
    {
        var ptr = Marshal.StringToCoTaskMemUTF8(path);
        try
        {
            var info = new FuseFileInfo();
            Native.set_flags(ref info, flags);
            var name = FuseHelper.SpanFromIntPtr(ptr);
            var result = create ? operations.Create(name, 438, ref info) : operations.Open(name, ref info);
            Equal(0, (int)result, "open succeeds");
            return info;
        }
        finally { Marshal.FreeCoTaskMem(ptr); }
    }

    private static unsafe void Write(MirrorFsOperations operations, ref FuseFileInfo info, string text, long position)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        fixed (byte* ptr = bytes)
        {
            var result = operations.Write(default, new((nint)ptr, bytes.Length), position, out var written, ref info);
            Equal(0, (int)result, "write result");
            Equal(bytes.Length, written, "write length");
        }
    }

    private static unsafe string Read(MirrorFsOperations operations, ref FuseFileInfo info)
    {
        var bytes = new byte[1024];
        fixed (byte* ptr = bytes)
        {
            Equal(0, (int)operations.Read(default, new((nint)ptr, bytes.Length), 0, out var length, ref info), "read result");
            return Encoding.UTF8.GetString(bytes, 0, length);
        }
    }

    private static void Close(MirrorFsOperations operations, ref FuseFileInfo info)
    {
        Equal(0, (int)operations.Release(default, ref info), "release result");
        Equal(true, info.Context is null, "release frees context");
    }

    private static void Throws<T>(Action action, string message) where T : Exception
    {
        try { action(); }
        catch (T) { Interlocked.Increment(ref assertions); return; }
        throw new InvalidOperationException(message);
    }

    private static void Equal<T>(T expected, T actual, string message)
    {
        Interlocked.Increment(ref assertions);
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException($"{message}: expected {expected}, got {actual}");
        }
    }

    private static class Native
    {
        [DllImport("open_flags_tests", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int open_flag_value(uint index);
        [DllImport("open_flags_tests", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void set_flags(ref FuseFileInfo info, int flags);
        [DllImport("open_flags_tests", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int get_flags(ref FuseFileInfo info);
        [DllImport("open_flags_tests", CallingConvention = CallingConvention.Cdecl)]
        internal static extern nuint flags_size();
        [DllImport("open_flags_tests", CallingConvention = CallingConvention.Cdecl)]
        internal static extern nuint file_info_offset(uint index);
    }
}
