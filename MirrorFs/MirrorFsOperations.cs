using FuseDotNet;
using FuseDotNet.Extensions;
using LTRData.Extensions.Native.Memory;

namespace MirrorFs;

internal class MirrorFsOperations(string basePath) : IFuseOperations
{
    public string BasePath { get; } = basePath;

    public string GetPath(ReadOnlyNativeMemory<byte> fileNamePtr)
        => Path.Join(BasePath, FuseHelper.GetString(fileNamePtr));

    public PosixResult Access(ReadOnlyNativeMemory<byte> fileNamePtr, PosixAccessMode mask)
    {
        var path = GetPath(fileNamePtr);

        var exists = File.Exists(path) || Directory.Exists(path);

        if (exists)
        {
            return PosixResult.Success;
        }
        else
        {
            return PosixResult.ENOENT;
        }
    }

    public PosixResult Create(ReadOnlyNativeMemory<byte> fileNamePtr, PosixFileMode mode, ref FuseFileInfo fileInfo) => PosixResult.ENOSYS;

    public void Dispose() { }

    public PosixResult Flush(ReadOnlyNativeMemory<byte> readOnlySpan, ref FuseFileInfo fileInfo) => PosixResult.ENOSYS;

    public PosixResult FSync(ReadOnlyNativeMemory<byte> fileNamePtr, bool datasync, ref FuseFileInfo fileInfo) => PosixResult.ENOSYS;

    public PosixResult FSyncDir(ReadOnlyNativeMemory<byte> fileNamePtr, bool datasync, ref FuseFileInfo fileInfo) => PosixResult.ENOSYS;

    public PosixResult GetAttr(ReadOnlyNativeMemory<byte> fileNamePtr, out FuseFileStat stat, ref FuseFileInfo fileInfo)
    {
        var path = GetPath(fileNamePtr);
        if (File.Exists(path))
        {
            var info = new FileInfo(GetPath(fileNamePtr));
            stat = new()
            {
                st_size = info.Length,
                st_birthtim = info.CreationTimeUtc,
                st_mtim = info.LastWriteTimeUtc,
                st_ctim = info.LastWriteTimeUtc,
                st_atim = info.LastAccessTimeUtc,
                st_mode = info.Attributes.ToPosixFileMode()
            };
            return PosixResult.Success;
        }
        else if (Directory.Exists(path))
        {
            var info = new DirectoryInfo(GetPath(fileNamePtr));
            stat = new()
            {
                st_birthtim = info.CreationTimeUtc,
                st_mtim = info.LastWriteTimeUtc,
                st_ctim = info.LastWriteTimeUtc,
                st_atim = info.LastAccessTimeUtc,
                st_mode = info.Attributes.ToPosixFileMode()
            };
            return PosixResult.Success;
        }
        else
        {
            stat = default;
            return PosixResult.ENOENT;
        }
    }

    public void Init(ref FuseConnInfo fuse_conn_info) { }
    
    public PosixResult IoCtl(ReadOnlyNativeMemory<byte> readOnlySpan, int cmd, nint arg, ref FuseFileInfo fileInfo, FuseIoctlFlags flags, nint data) => PosixResult.ENOSYS;
    
    public PosixResult Link(ReadOnlyNativeMemory<byte> from, ReadOnlyNativeMemory<byte> to) => PosixResult.ENOSYS;

    public PosixResult MkDir(ReadOnlyNativeMemory<byte> fileNamePtr, PosixFileMode mode) => PosixResult.ENOSYS;

    public PosixResult Open(ReadOnlyNativeMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo)
    {
        fileInfo.Context = File.Open(GetPath(fileNamePtr),
                                           fileInfo.flags.ToFileMode(),
                                           fileInfo.flags.ToFileAccess(),
                                           fileInfo.flags.ToFileShare());

        return PosixResult.Success;
    }
    
    public PosixResult OpenDir(ReadOnlyNativeMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo)
    {
        if (Directory.Exists(GetPath(fileNamePtr)))
        {
            return PosixResult.Success;
        }
        else
        {
            return PosixResult.ENOENT;
        }
    }

    public PosixResult Read(ReadOnlyNativeMemory<byte> fileNamePtr, NativeMemory<byte> buffer, long position, out int readLength, ref FuseFileInfo fileInfo)
    {
        if (fileInfo.Context is not Stream stream)
        {
            readLength = 0;
            return PosixResult.EBADF;
        }

        stream.Position = position;
        readLength = stream.Read(buffer.Span);

        return PosixResult.Success;
    }

    public PosixResult ReadDir(ReadOnlyNativeMemory<byte> fileNamePtr, out IEnumerable<FuseDirEntry> entries, ref FuseFileInfo fileInfo, long offset, FuseReadDirFlags flags)
    {
        var path = GetPath(fileNamePtr);

        var files = Directory.EnumerateFiles(path)
            .Select(entry => new FuseDirEntry(Path.GetFileName(entry), 0, 0, new() { st_mode = PosixFileMode.Regular }));

        var dirs = Directory.EnumerateDirectories(path)
            .Select(entry => new FuseDirEntry(Path.GetFileName(entry), 0, 0, new() { st_mode = PosixFileMode.Directory }));

        entries = FuseHelper.DotEntries.Concat(files).Concat(dirs);

        return PosixResult.Success;
    }

    public PosixResult ReadLink(ReadOnlyNativeMemory<byte> fileNamePtr, NativeMemory<byte> target) => PosixResult.ENOSYS;

    public PosixResult Release(ReadOnlyNativeMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo)
    {
        if (fileInfo.Context is not Stream stream)
        {
            return PosixResult.Success;
        }

        stream.Close();

        return PosixResult.Success;
    }

    public PosixResult ReleaseDir(ReadOnlyNativeMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo) => PosixResult.Success;

    public PosixResult Rename(ReadOnlyNativeMemory<byte> from, ReadOnlyNativeMemory<byte> to)
    {
        File.Move(GetPath(from), GetPath(to));
        return PosixResult.Success;
    }

    public PosixResult RmDir(ReadOnlyNativeMemory<byte> fileNamePtr)
    {
        Directory.Delete(GetPath(fileNamePtr));
        return PosixResult.Success;
    }

    public PosixResult StatFs(ReadOnlyNativeMemory<byte> fileNamePtr, out FuseVfsStat statvfs)
    {
        statvfs = default;
        return PosixResult.Success;
    }

    public PosixResult SymLink(ReadOnlyNativeMemory<byte> from, ReadOnlyNativeMemory<byte> to) => PosixResult.ENOSYS;

    public PosixResult Truncate(ReadOnlyNativeMemory<byte> fileNamePtr, long size)
    {
        using var file = File.Open(GetPath(fileNamePtr),
                                   FileMode.Open,
                                   FileAccess.ReadWrite,
                                   FileShare.ReadWrite | FileShare.Delete);

        file.SetLength(size);

        return PosixResult.Success;
    }

    public PosixResult Unlink(ReadOnlyNativeMemory<byte> fileNamePtr)
    {
        File.Delete(GetPath(fileNamePtr));
        return PosixResult.Success;
    }

    public PosixResult UTime(ReadOnlyNativeMemory<byte> fileNamePtr, TimeSpec atime, TimeSpec mtime, ref FuseFileInfo fileInfo)
    {
        var path = GetPath(fileNamePtr);
        File.SetLastAccessTimeUtc(path, atime.ToDateTime().UtcDateTime);
        File.SetLastWriteTimeUtc(path, mtime.ToDateTime().UtcDateTime);
        return PosixResult.Success;
    }

    public PosixResult Write(ReadOnlyNativeMemory<byte> fileNamePtr, ReadOnlyNativeMemory<byte> buffer, long position, out int writtenLength, ref FuseFileInfo fileInfo)
    {
        if (fileInfo.Context is not Stream stream)
        {
            writtenLength = 0;
            return PosixResult.EBADF;
        }

        stream.Position = position;
        stream.Write(buffer.Span);
        writtenLength = buffer.Length;

        return PosixResult.Success;
    }
}
