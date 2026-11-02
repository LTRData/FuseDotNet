using FuseDotNet;
using FuseDotNet.Extensions;
using FuseDotNet.Logging;
using FuseDotNet.Native;
using LTRData.Extensions.Native.Memory;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

[SupportedOSPlatform("linux")]
[SupportedOSPlatform("freebsd")]
internal static class Program
{
    private static int assertions;

    private static unsafe void Main()
    {
        string[] fields = ["set_gid", "gid", "set_uid", "uid", "set_mode", "umask",
            "entry_timeout", "negative_timeout", "attr_timeout", "intr", "intr_signal",
            "remember", "hard_remove", "use_ino", "readdir_ino", "direct_io", "kernel_cache", "auto_cache"];

        Equal((nuint)Marshal.SizeOf<FuseConfig>(), Native.config_prefix_size(), "configuration prefix size");
        for (var i = 0; i < fields.Length; i++)
        {
            Equal((nuint)Marshal.OffsetOf<FuseConfig>(fields[i]), Native.config_offset((uint)i), fields[i]);
        }

        var legacy = new TestOperations();
        var proxy = new FuseOperationProxy(legacy, new NullLogger());
        foreach (var flags in new uint[] { 1, 2, 3, 4, 0x80000000 })
        {
            Equal(-(int)PosixResult.EINVAL, Native.invoke_rename(proxy.rename, flags), "legacy rejects flags");
            Equal("source", legacy.Files["/from"], "source preserved");
            Equal("destination", legacy.Files["/to"], "destination preserved");
        }
        Equal(0, legacy.RenameCalls, "legacy callback not invoked for flags");
        Equal(0, Native.invoke_rename(proxy.rename, 0), "ordinary rename");
        Equal(1, legacy.RenameCalls, "ordinary rename dispatched once");
        Equal("source", legacy.Files["/to"], "ordinary rename reached correct paths");

        var extended = new ExtendedOperations();
        proxy = new(extended, new NullLogger());
        Equal(-(int)PosixResult.EEXIST, Native.invoke_rename(proxy.rename, 1), "no-replace result");
        Equal("destination", extended.Files["/to"], "no-replace preserves destination");
        Equal(FuseRenameFlags.NoReplace, extended.LastFlags, "no-replace flag forwarded");
        Equal(0, Native.invoke_rename(proxy.rename, 2), "exchange result");
        Equal("destination", extended.Files["/from"], "exchange source");
        Equal("source", extended.Files["/to"], "exchange destination");
        Equal(FuseRenameFlags.Exchange, extended.LastFlags, "exchange flag forwarded");
        Equal(0, Native.invoke_rename(proxy.rename, 0), "extended ordinary rename");
        Equal(0, extended.RenameCalls, "legacy overload not called");
        Equal(3, extended.FlaggedRenameCalls, "extended dispatch count");
        foreach (var flags in new uint[] { 3, 4, 0x80000000 })
        {
            Equal(-(int)PosixResult.EINVAL, Native.invoke_rename(proxy.rename, flags), "invalid flags rejected");
        }
        Equal(3, extended.FlaggedRenameCalls, "invalid flags never dispatched");
        extended.ThrowOnRename = true;
        Equal(-(int)PosixResult.EACCES, Native.invoke_rename(proxy.rename, 0), "exception translated at native boundary");

        CheckInit(new TestOperations(), extended: false);
        CheckInit(new ExtendedOperations(), extended: true);
        Console.WriteLine($"PASS: {assertions} callback and native-layout assertions");
    }

    private static unsafe void CheckInit(TestOperations operations, bool extended)
    {
        var buffer = Enumerable.Repeat((byte)0xA5, checked((int)Native.config_size() + 16)).ToArray();
        var proxy = new FuseOperationProxy(operations, new NullLogger());
        var conn = new FuseConnInfo { max_write = 123 };
        fixed (byte* bufferPtr = buffer)
        {
            var ptr = bufferPtr + 8;
            *(FuseConfig*)ptr = new() { entry_timeout = 1.25, attr_timeout = 2.5, auto_cache = 1 };
            var before = buffer.ToArray();
            Equal((nint)0, Native.invoke_init(proxy.init, ref conn, (nint)ptr), "init private data");
            Equal(extended ? 0 : 1, operations.InitCalls, "legacy init dispatch count");
            Equal(777u, conn.max_write, "connection write-back");
            if (operations is ExtendedOperations configured)
            {
                Equal(1, configured.ConfigInitCalls, "configured init dispatched exactly once");
                Equal(1.25, configured.InitialEntryTimeout, "existing native configuration received");
                Equal(1, ((FuseConfig*)ptr)->hard_remove, "hard_remove write-back");
                Equal(0.5, ((FuseConfig*)ptr)->attr_timeout, "timeout write-back");
                Equal(1, ((FuseConfig*)ptr)->auto_cache, "unmodified prefix field preserved");
            }
            else
            {
                Equal(true, buffer.SequenceEqual(before), "legacy init leaves all config bytes unchanged");
            }
            Equal(true, buffer.AsSpan(0, 8).SequenceEqual(before.AsSpan(0, 8)), "leading guard unchanged");
            var tail = 8 + Marshal.SizeOf<FuseConfig>();
            Equal(true, buffer.AsSpan(tail).SequenceEqual(before.AsSpan(tail)), "unrepresented native tail and guard unchanged");
        }
    }

    private static void Equal<T>(T expected, T actual, string message)
    {
        assertions++;
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException($"{message}: expected {expected}, got {actual}");
        }
    }

    private static class Native
    {
        [DllImport("callback_tests", CallingConvention = CallingConvention.Cdecl)]
        internal static extern nuint config_size();
        [DllImport("callback_tests", CallingConvention = CallingConvention.Cdecl)]
        internal static extern nuint config_prefix_size();
        [DllImport("callback_tests", CallingConvention = CallingConvention.Cdecl)]
        internal static extern nuint config_offset(uint index);
        [DllImport("callback_tests", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int invoke_rename(FuseOperations.fuse_f_rename callback, uint flags);
        [DllImport("callback_tests", CallingConvention = CallingConvention.Cdecl)]
        internal static extern nint invoke_init(FuseOperations.fuse_f_init callback, ref FuseConnInfo conn, nint config);
    }

    private sealed class ExtendedOperations : TestOperations, IFuseRenameWithFlags, IFuseInitWithConfig
    {
        public int FlaggedRenameCalls;
        public int ConfigInitCalls;
        public FuseRenameFlags LastFlags;
        public double InitialEntryTimeout;
        public bool ThrowOnRename;

        public PosixResult Rename(ReadOnlyNativeMemory<byte> from, ReadOnlyNativeMemory<byte> to, FuseRenameFlags flags)
        {
            FlaggedRenameCalls++;
            LastFlags = flags;
            if (ThrowOnRename)
            {
                throw new PosixException(PosixResult.EACCES);
            }
            var source = FuseHelper.GetString(from);
            var destination = FuseHelper.GetString(to);
            if (flags == FuseRenameFlags.NoReplace && Files.ContainsKey(destination))
            {
                return PosixResult.EEXIST;
            }
            if (flags == FuseRenameFlags.Exchange)
            {
                (Files[source], Files[destination]) = (Files[destination], Files[source]);
            }
            return PosixResult.Success;
        }

        public void Init(ref FuseConnInfo conn, ref FuseConfig config)
        {
            ConfigInitCalls++;
            InitialEntryTimeout = config.entry_timeout;
            conn.max_write = 777;
            config.hard_remove = 1;
            config.attr_timeout = 0.5;
        }
    }

    private class TestOperations : IFuseOperations
    {
        public int RenameCalls;
        public int InitCalls;
        public Dictionary<string, string> Files = new() { ["/from"] = "source", ["/to"] = "destination" };

        public PosixResult Rename(ReadOnlyNativeMemory<byte> from, ReadOnlyNativeMemory<byte> to)
        {
            RenameCalls++;
            var source = FuseHelper.GetString(from);
            Files[FuseHelper.GetString(to)] = Files[source];
            Files.Remove(source);
            return PosixResult.Success;
        }
        public void Init(ref FuseConnInfo conn) { InitCalls++; conn.max_write = 777; }
        public void Dispose() { }
        public PosixResult OpenDir(ReadOnlyNativeMemory<byte> path, ref FuseFileInfo info) => PosixResult.ENOSYS;
        public PosixResult GetAttr(ReadOnlyNativeMemory<byte> path, out FuseFileStat stat, ref FuseFileInfo info) { stat = default; return PosixResult.ENOSYS; }
        public PosixResult Read(ReadOnlyNativeMemory<byte> path, NativeMemory<byte> buffer, long position, out int length, ref FuseFileInfo info) { length = 0; return PosixResult.ENOSYS; }
        public PosixResult ReadDir(ReadOnlyNativeMemory<byte> path, out IEnumerable<FuseDirEntry> entries, ref FuseFileInfo info, long offset, FuseReadDirFlags flags) { entries = []; return PosixResult.ENOSYS; }
        public PosixResult Open(ReadOnlyNativeMemory<byte> path, ref FuseFileInfo info) => PosixResult.ENOSYS;
        public PosixResult Access(ReadOnlyNativeMemory<byte> path, PosixAccessMode mask) => PosixResult.ENOSYS;
        public PosixResult StatFs(ReadOnlyNativeMemory<byte> path, out FuseVfsStat stat) { stat = default; return PosixResult.ENOSYS; }
        public PosixResult FSyncDir(ReadOnlyNativeMemory<byte> path, bool data, ref FuseFileInfo info) => PosixResult.ENOSYS;
        public PosixResult ReadLink(ReadOnlyNativeMemory<byte> path, NativeMemory<byte> target) => PosixResult.ENOSYS;
        public PosixResult ReleaseDir(ReadOnlyNativeMemory<byte> path, ref FuseFileInfo info) => PosixResult.ENOSYS;
        public PosixResult Link(ReadOnlyNativeMemory<byte> from, ReadOnlyNativeMemory<byte> to) => PosixResult.ENOSYS;
        public PosixResult MkDir(ReadOnlyNativeMemory<byte> path, PosixFileMode mode) => PosixResult.ENOSYS;
        public PosixResult Release(ReadOnlyNativeMemory<byte> path, ref FuseFileInfo info) => PosixResult.ENOSYS;
        public PosixResult RmDir(ReadOnlyNativeMemory<byte> path) => PosixResult.ENOSYS;
        public PosixResult FSync(ReadOnlyNativeMemory<byte> path, bool data, ref FuseFileInfo info) => PosixResult.ENOSYS;
        public PosixResult Unlink(ReadOnlyNativeMemory<byte> path) => PosixResult.ENOSYS;
        public PosixResult Write(ReadOnlyNativeMemory<byte> path, ReadOnlyNativeMemory<byte> buffer, long position, out int length, ref FuseFileInfo info) { length = 0; return PosixResult.ENOSYS; }
        public PosixResult SymLink(ReadOnlyNativeMemory<byte> from, ReadOnlyNativeMemory<byte> to) => PosixResult.ENOSYS;
        public PosixResult Flush(ReadOnlyNativeMemory<byte> path, ref FuseFileInfo info) => PosixResult.ENOSYS;
        public PosixResult Truncate(ReadOnlyNativeMemory<byte> path, long size) => PosixResult.ENOSYS;
        public PosixResult UTime(ReadOnlyNativeMemory<byte> path, TimeSpec atime, TimeSpec mtime, ref FuseFileInfo info) => PosixResult.ENOSYS;
        public PosixResult Create(ReadOnlyNativeMemory<byte> path, int mode, ref FuseFileInfo info) => PosixResult.ENOSYS;
        public PosixResult IoCtl(ReadOnlyNativeMemory<byte> path, int cmd, nint arg, ref FuseFileInfo info, FuseIoctlFlags flags, nint data) => PosixResult.ENOSYS;
        public PosixResult ChMod(NativeMemory<byte> path, PosixFileMode mode) => PosixResult.ENOSYS;
        public PosixResult ChOwn(NativeMemory<byte> path, int uid, int gid) => PosixResult.ENOSYS;
        public PosixResult FAllocate(NativeMemory<byte> path, FuseAllocateMode mode, long offset, long length, ref FuseFileInfo info) => PosixResult.ENOSYS;
    }
}
