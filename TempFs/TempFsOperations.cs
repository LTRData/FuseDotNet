using FuseDotNet;
using FuseDotNet.Extensions;
using LTRData.Extensions.Native.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace TempFs;

internal class TempFsOperations : IFuseOperations
{
    public PosixResult GetAttr(ReadOnlyNativeMemory<byte> fileNamePtr, out FuseFileStat stat, ref FuseFileInfo fileInfo)
    {
        var fileName = FuseHelper.GetString(fileNamePtr);

        Console.WriteLine($"Getting attributes for file '{fileName}'");

        stat = default;

        if (fileName == "/")
        {
            stat.st_mode = fileModeDirs;
            stat.st_nlink = 2;
            stat.st_atim =
                stat.st_mtim =
                stat.st_ctim =
                stat.st_birthtim = TimeSpec.Now();
        }
        else
        {
            stat.st_mode = fileModeReg;
            stat.st_nlink = 1;
            stat.st_size = FileContents.LongLength;
            stat.st_atim =
                stat.st_mtim =
                stat.st_ctim =
                stat.st_birthtim = TimeSpec.Now();
        }

        return PosixResult.Success;
    }

    private readonly PosixFileMode fileModeDirs = PosixFileMode.Directory
        | PosixFileMode.OwnerAll
        | PosixFileMode.GroupReadExecute
        | PosixFileMode.OthersReadExecute;
    
    private readonly PosixFileMode fileModeReg = PosixFileMode.Regular
        | PosixFileMode.OwnerRead
        | PosixFileMode.OwnerWrite
        | PosixFileMode.GroupRead
        | PosixFileMode.OthersRead;

    public PosixResult OpenDir(ReadOnlyNativeMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo)
    {
        var fileName = FuseHelper.GetString(fileNamePtr);

        Console.WriteLine($"Opening directory '{fileName}'");

        return PosixResult.Success;
    }

    private static readonly byte[] FileContents = Encoding.UTF8.GetBytes("Hello world!\n");

    public PosixResult Read(ReadOnlyNativeMemory<byte> fileNamePtr, NativeMemory<byte> buffer, long position, out int readLength, ref FuseFileInfo fileInfo)
    {
        var fileName = FuseHelper.GetString(fileNamePtr);

        Console.WriteLine($"Reading file '{fileName}', {buffer.Length} bytes from position {position}");

        FileContents.CopyTo(buffer.Span);
        
        readLength = FileContents.Length;

        return PosixResult.Success;
    }

    public PosixResult ReadDir(ReadOnlyNativeMemory<byte> fileNamePtr, out IEnumerable<FuseDirEntry> entries, ref FuseFileInfo fileInfo, long offset, FuseReadDirFlags flags)
    {
        var fileName = FuseHelper.GetString(fileNamePtr);

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

    public PosixResult Open(ReadOnlyNativeMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo) => PosixResult.Success;

    public void Init(ref FuseConnInfo fuse_conn_info)
        => Console.WriteLine($"Initializing file system, driver capabilities: {fuse_conn_info.capable}, requested: {fuse_conn_info.want}");

    public PosixResult Access(ReadOnlyNativeMemory<byte> fileNamePtr, PosixAccessMode mask) => PosixResult.Success;

    public PosixResult StatFs(ReadOnlyNativeMemory<byte> fileNamePtr, out FuseVfsStat statvfs)
    {
        statvfs = default;
        return PosixResult.Success;
    }

    public PosixResult FSyncDir(ReadOnlyNativeMemory<byte> fileNamePtr, bool datasync, ref FuseFileInfo fileInfo) => PosixResult.Success;

    public PosixResult ReadLink(ReadOnlyNativeMemory<byte> fileNamePtr, NativeMemory<byte> target) => PosixResult.ENOSYS;

    public PosixResult ReleaseDir(ReadOnlyNativeMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo) => PosixResult.Success;
    
    public PosixResult MkDir(ReadOnlyNativeMemory<byte> fileNamePtr, PosixFileMode mode) => PosixResult.Success;
    
    public PosixResult Release(ReadOnlyNativeMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo) => PosixResult.Success;
    
    public PosixResult RmDir(ReadOnlyNativeMemory<byte> fileNamePtr) => PosixResult.Success;
    
    public PosixResult FSync(ReadOnlyNativeMemory<byte> fileNamePtr, bool datasync, ref FuseFileInfo fileInfo) => PosixResult.Success;
    
    public PosixResult Unlink(ReadOnlyNativeMemory<byte> fileNamePtr) => PosixResult.Success;
    
    public PosixResult SymLink(ReadOnlyNativeMemory<byte> from, ReadOnlyNativeMemory<byte> to) => PosixResult.Success;
    
    public PosixResult Rename(ReadOnlyNativeMemory<byte> from, ReadOnlyNativeMemory<byte> to) => PosixResult.Success;
    
    public PosixResult Truncate(ReadOnlyNativeMemory<byte> fileNamePtr, long size) => PosixResult.Success;
    
    public void Dispose() => Console.WriteLine("Disposing file system");
    
    public PosixResult Write(ReadOnlyNativeMemory<byte> fileNamePtr, ReadOnlyNativeMemory<byte> buffer, long position, out int writtenLength, ref FuseFileInfo fileInfo)
    {
        var fileName = FuseHelper.GetString(fileNamePtr);

        Console.WriteLine($"Writing file '{fileName}', {buffer.Length} bytes at position {position}");

        writtenLength = buffer.Length;
        return PosixResult.Success;
    }

    public PosixResult Link(ReadOnlyNativeMemory<byte> from, ReadOnlyNativeMemory<byte> to) => PosixResult.Success;
    
    public PosixResult Flush(ReadOnlyNativeMemory<byte> readOnlySpan, ref FuseFileInfo fileInfo) => PosixResult.Success;
    
    public PosixResult UTime(ReadOnlyNativeMemory<byte> fileNamePtr, TimeSpec atime, TimeSpec mtime, ref FuseFileInfo fileInfo) => PosixResult.Success;
    
    public PosixResult Create(ReadOnlyNativeMemory<byte> fileNamePtr, PosixFileMode mode, ref FuseFileInfo fileInfo) => PosixResult.Success;
    
    public PosixResult IoCtl(ReadOnlyNativeMemory<byte> readOnlySpan, int cmd, nint arg, ref FuseFileInfo fileInfo, FuseIoctlFlags flags, nint data) => PosixResult.ENOSYS;
}
