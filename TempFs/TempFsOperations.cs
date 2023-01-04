using DiscUtils;
using FuseDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestFs;

internal class TempFsOperations : IFuseOperations
{
    public PosixResult GetAttr(ReadOnlySpan<byte> fileNamePtr, out FuseFileStat stat)
    {
        var fileName = Encoding.UTF8.GetString(fileNamePtr);

        stat = default;

        Console.WriteLine($"Getting attributes for file '{fileName}'");

        if (fileName == "/")
        {
            stat.st_mode = _fileModeDirs;
            stat.st_nlink = 2;
            stat.st_atim =
                stat.st_mtim =
                stat.st_ctim =
                stat.st_birthtim = TimeSpec.Now();
        }
        else
        {
            stat.st_mode = _fileModeReg;
            stat.st_nlink = 1;
            stat.st_size = _fileContents.LongLength;
            stat.st_atim =
                stat.st_mtim =
                stat.st_ctim =
                stat.st_birthtim = TimeSpec.Now();
        }

        return PosixResult.Success;
    }

    private readonly PosixFileMode _fileModeDirs = PosixFileMode.Directory
        | PosixFileMode.OwnerAll
        | PosixFileMode.GroupReadExecute
        | PosixFileMode.OthersReadExecute;
    
    private readonly PosixFileMode _fileModeReg = PosixFileMode.Regular
        | PosixFileMode.OwnerRead
        | PosixFileMode.OwnerWrite
        | PosixFileMode.GroupRead
        | PosixFileMode.OthersRead;

    public PosixResult OpenDir(ReadOnlySpan<byte> fileNamePtr, ref FuseFileInfo fileInfo)
    {
        var fileName = Encoding.UTF8.GetString(fileNamePtr);

        Console.WriteLine($"Opening directory '{fileName}'");

        return PosixResult.Success;
    }

    private static readonly byte[] _fileContents = Encoding.UTF8.GetBytes("Hello world!\n");

    public PosixResult Read(ReadOnlySpan<byte> fileNamePtr, Span<byte> buffer, long position, out int readLength, ref FuseFileInfo fileInfo)
    {
        var fileName = Encoding.UTF8.GetString(fileNamePtr);

        Console.WriteLine($"Reading file '{fileName}', {buffer.Length} bytes from position {position}");

        _fileContents.CopyTo(buffer);
        
        readLength = _fileContents.Length;

        return PosixResult.Success;
    }

    public PosixResult ReadDir(ReadOnlySpan<byte> fileNamePtr, out IEnumerable<FuseDirEntry> entries, ref FuseFileInfo fileInfo, long offset, FuseReadDirFlags flags)
    {
        var fileName = Encoding.UTF8.GetString(fileNamePtr);

        entries = EnumerateEntries(fileName);

        return PosixResult.Success;
    }

    private static IEnumerable<FuseDirEntry> EnumerateEntries(string fileName)
    {
        Console.WriteLine($"Reading directory '{fileName}'");

        yield return new(".", 0, 0, new() { st_mode = PosixFileMode.Directory });

        yield return new("..", 0, 0, new() { st_mode = PosixFileMode.Directory });

        yield return new("test.txt", 0, 0, new() { st_mode = PosixFileMode.Regular });
    }

    public PosixResult Open(ReadOnlySpan<byte> fileNamePtr, ref FuseFileInfo fileInfo) => PosixResult.Success;

    public void Init(ref FuseConnInfo fuse_conn_info)
        => Console.WriteLine($"Initializing file system, driver capabilities: {fuse_conn_info.capable}, requested: {fuse_conn_info.want}");

    public PosixResult Access(ReadOnlySpan<byte> fileNamePtr, PosixAccessMode mask) => PosixResult.Success;

    public PosixResult StatFs(ReadOnlySpan<byte> fileNamePtr, out FuseVfsStat statvfs)
    {
        statvfs = default;
        return PosixResult.Success;
    }

    public PosixResult FSyncDir(ReadOnlySpan<byte> fileNamePtr, bool datasync, ref FuseFileInfo fileInfo) => PosixResult.Success;

    public PosixResult ReadLink(ReadOnlySpan<byte> fileNamePtr, Span<byte> target) => PosixResult.ENOSYS;

    public PosixResult ReleaseDir(ReadOnlySpan<byte> fileNamePtr, ref FuseFileInfo fileInfo) => PosixResult.Success;
    
    public PosixResult MkDir(ReadOnlySpan<byte> fileNamePtr, PosixFileMode mode) => PosixResult.Success;
    
    public PosixResult Release(ReadOnlySpan<byte> fileNamePtr, ref FuseFileInfo fileInfo) => PosixResult.Success;
    
    public PosixResult RmDir(ReadOnlySpan<byte> fileNamePtr) => PosixResult.Success;
    
    public PosixResult FSync(ReadOnlySpan<byte> fileNamePtr, bool datasync, ref FuseFileInfo fileInfo) => PosixResult.Success;
    
    public PosixResult Unlink(ReadOnlySpan<byte> fileNamePtr) => PosixResult.Success;
    
    public PosixResult SymLink(ReadOnlySpan<byte> from, ReadOnlySpan<byte> to) => PosixResult.Success;
    
    public PosixResult Rename(ReadOnlySpan<byte> from, ReadOnlySpan<byte> to) => PosixResult.Success;
    
    public PosixResult Truncate(ReadOnlySpan<byte> fileNamePtr, long size) => PosixResult.Success;
    
    public void Dispose() => Console.WriteLine("Disposing file system");
    
    public PosixResult Write(ReadOnlySpan<byte> fileNamePtr, ReadOnlySpan<byte> buffer, long position, out int writtenLength, ref FuseFileInfo fileInfo)
    {
        var fileName = Encoding.UTF8.GetString(fileNamePtr);

        Console.WriteLine($"Writing file '{fileName}', {buffer.Length} bytes at position {position}");

        writtenLength = buffer.Length;
        return PosixResult.Success;
    }

    public PosixResult Link(ReadOnlySpan<byte> from, ReadOnlySpan<byte> to) => PosixResult.Success;
    
    public PosixResult Flush(ReadOnlySpan<byte> readOnlySpan, ref FuseFileInfo fileInfo) => PosixResult.Success;
    
    public PosixResult UTime(ReadOnlySpan<byte> fileNamePtr, TimeSpec atime, TimeSpec mtime, ref FuseFileInfo fileInfo) => PosixResult.Success;
    
    public PosixResult Create(ReadOnlySpan<byte> fileNamePtr, PosixFileMode mode, ref FuseFileInfo fileInfo) => PosixResult.Success;
    
    public PosixResult IoCtl(ReadOnlySpan<byte> readOnlySpan, int cmd, IntPtr arg, ref FuseFileInfo fileInfo, FuseIoctlFlags flags, IntPtr data) => PosixResult.ENOSYS;
}
