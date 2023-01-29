using FuseDotNet;
using FuseDotNet.Extensions;
using System.Runtime.InteropServices;
using System.Text;

namespace MirrorFs;

internal class MirrorFsOperations : IFuseOperations
{
    public string BasePath { get; }

    public MirrorFsOperations(string basePath)
    {
        BasePath = basePath;
    }

    public string GetPath(ReadOnlySpan<byte> fileNamePtr)
        => Path.Join(BasePath, Encoding.UTF8.GetString(fileNamePtr));

    public PosixResult Access(ReadOnlySpan<byte> fileNamePtr, PosixAccessMode mask)
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

    public PosixResult Create(ReadOnlySpan<byte> fileNamePtr, PosixFileMode mode, ref FuseFileInfo fileInfo) => PosixResult.ENOSYS;

    public void Dispose() { }

    public PosixResult Flush(ReadOnlySpan<byte> readOnlySpan, ref FuseFileInfo fileInfo) => PosixResult.ENOSYS;

    public PosixResult FSync(ReadOnlySpan<byte> fileNamePtr, bool datasync, ref FuseFileInfo fileInfo) => PosixResult.ENOSYS;

    public PosixResult FSyncDir(ReadOnlySpan<byte> fileNamePtr, bool datasync, ref FuseFileInfo fileInfo) => PosixResult.ENOSYS;

    public PosixResult GetAttr(ReadOnlySpan<byte> fileNamePtr, out FuseFileStat stat, ref FuseFileInfo fileInfo)
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
    
    public PosixResult IoCtl(ReadOnlySpan<byte> readOnlySpan, int cmd, nint arg, ref FuseFileInfo fileInfo, FuseIoctlFlags flags, nint data) => PosixResult.ENOSYS;
    
    public PosixResult Link(ReadOnlySpan<byte> from, ReadOnlySpan<byte> to) => PosixResult.ENOSYS;

    public PosixResult MkDir(ReadOnlySpan<byte> fileNamePtr, PosixFileMode mode) => PosixResult.ENOSYS;

    public PosixResult Open(ReadOnlySpan<byte> fileNamePtr, ref FuseFileInfo fileInfo)
    {
        fileInfo.Context = File.Open(GetPath(fileNamePtr),
                                           fileInfo.flags.ToFileMode(),
                                           fileInfo.flags.ToFileAccess(),
                                           fileInfo.flags.ToFileShare());

        return PosixResult.Success;
    }
    
    public PosixResult OpenDir(ReadOnlySpan<byte> fileNamePtr, ref FuseFileInfo fileInfo)
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

    public PosixResult Read(ReadOnlySpan<byte> fileNamePtr, Span<byte> buffer, long position, out int readLength, ref FuseFileInfo fileInfo)
    {
        if (fileInfo.Context is not Stream stream)
        {
            readLength = 0;
            return PosixResult.EBADF;
        }

        stream.Position = position;
        readLength = stream.Read(buffer);

        return PosixResult.Success;
    }

    public PosixResult ReadDir(ReadOnlySpan<byte> fileNamePtr, out IEnumerable<FuseDirEntry> entries, ref FuseFileInfo fileInfo, long offset, FuseReadDirFlags flags)
    {
        var path = GetPath(fileNamePtr);

        var files = Directory.EnumerateFiles(path)
            .Select(entry => new FuseDirEntry(Path.GetFileName(entry), 0, 0, new() { st_mode = PosixFileMode.Regular }));

        var dirs = Directory.EnumerateDirectories(path)
            .Select(entry => new FuseDirEntry(Path.GetFileName(entry), 0, 0, new() { st_mode = PosixFileMode.Directory }));

        entries = FuseHelper.DotEntries.Concat(files).Concat(dirs);

        return PosixResult.Success;
    }

    public PosixResult ReadLink(ReadOnlySpan<byte> fileNamePtr, Span<byte> target) => PosixResult.ENOSYS;

    public PosixResult Release(ReadOnlySpan<byte> fileNamePtr, ref FuseFileInfo fileInfo)
    {
        if (fileInfo.Context is not Stream stream)
        {
            return PosixResult.Success;
        }

        stream.Close();

        return PosixResult.Success;
    }

    public PosixResult ReleaseDir(ReadOnlySpan<byte> fileNamePtr, ref FuseFileInfo fileInfo) => PosixResult.Success;

    public PosixResult Rename(ReadOnlySpan<byte> from, ReadOnlySpan<byte> to)
    {
        File.Move(GetPath(from), GetPath(to));
        return PosixResult.Success;
    }

    public PosixResult RmDir(ReadOnlySpan<byte> fileNamePtr)
    {
        Directory.Delete(GetPath(fileNamePtr));
        return PosixResult.Success;
    }

    public PosixResult StatFs(ReadOnlySpan<byte> fileNamePtr, out FuseVfsStat statvfs)
    {
        statvfs = default;
        return PosixResult.Success;
    }

    public PosixResult SymLink(ReadOnlySpan<byte> from, ReadOnlySpan<byte> to) => PosixResult.ENOSYS;

    public PosixResult Truncate(ReadOnlySpan<byte> fileNamePtr, long size)
    {
        using var file = File.Open(GetPath(fileNamePtr),
                                   FileMode.Open,
                                   FileAccess.ReadWrite,
                                   FileShare.ReadWrite | FileShare.Delete);

        file.SetLength(size);

        return PosixResult.Success;
    }

    public PosixResult Unlink(ReadOnlySpan<byte> fileNamePtr)
    {
        File.Delete(GetPath(fileNamePtr));
        return PosixResult.Success;
    }

    public PosixResult UTime(ReadOnlySpan<byte> fileNamePtr, TimeSpec atime, TimeSpec mtime, ref FuseFileInfo fileInfo)
    {
        var path = GetPath(fileNamePtr);
        File.SetLastAccessTimeUtc(path, atime.ToDateTime().UtcDateTime);
        File.SetLastWriteTimeUtc(path, mtime.ToDateTime().UtcDateTime);
        return PosixResult.Success;
    }

    public PosixResult Write(ReadOnlySpan<byte> fileNamePtr, ReadOnlySpan<byte> buffer, long position, out int writtenLength, ref FuseFileInfo fileInfo)
    {
        if (fileInfo.Context is not Stream stream)
        {
            writtenLength = 0;
            return PosixResult.EBADF;
        }

        stream.Position = position;
        stream.Write(buffer);
        writtenLength = buffer.Length;

        return PosixResult.Success;
    }
}
