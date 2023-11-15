using System;
using System.Collections.Generic;

namespace FuseDotNet;

/// <summary>
/// %Fuse API callbacks interface.
/// 
/// A interface of callbacks that describe all %Fuse API operation
/// that will be called when Windows access to the file system.
/// 
/// All this callbacks can return corresponding <see cref="PosixResult"/> values
/// if you dont want to support one of them. Be aware that returning such value to important callbacks
/// such <see cref="Open(ReadOnlyFuseMemory{byte}, ref FuseFileInfo)"/>/<see cref="Read(ReadOnlyFuseMemory{byte}, FuseMemory{byte}, long, out int, ref FuseFileInfo)"/>/... would make the filesystem not working or unstable.
/// </summary>
/// <remarks>This is the same struct as <c>FUSE_OPERATIONS</c> (fuse.h) in the C version of Fuse.</remarks>
public interface IFuseOperations : IDisposable
{
    PosixResult OpenDir(ReadOnlyFuseMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo);
    PosixResult GetAttr(ReadOnlyFuseMemory<byte> fileNamePtr, out FuseFileStat stat, ref FuseFileInfo fileInfo);        
    PosixResult Read(ReadOnlyFuseMemory<byte> fileNamePtr, FuseMemory<byte> buffer, long position, out int readLength, ref FuseFileInfo fileInfo);        
    PosixResult ReadDir(ReadOnlyFuseMemory<byte> fileNamePtr, out IEnumerable<FuseDirEntry> entries, ref FuseFileInfo fileInfo, long offset, FuseReadDirFlags flags);
    PosixResult Open(ReadOnlyFuseMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo);
    void Init(ref FuseConnInfo fuse_conn_info);
    PosixResult Access(ReadOnlyFuseMemory<byte> fileNamePtr, PosixAccessMode mask);
    PosixResult StatFs(ReadOnlyFuseMemory<byte> fileNamePtr, out FuseVfsStat statvfs);
    PosixResult FSyncDir(ReadOnlyFuseMemory<byte> fileNamePtr, bool datasync, ref FuseFileInfo fileInfo);
    PosixResult ReadLink(ReadOnlyFuseMemory<byte> fileNamePtr, FuseMemory<byte> target);
    PosixResult ReleaseDir(ReadOnlyFuseMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo);
    PosixResult Link(ReadOnlyFuseMemory<byte> from, ReadOnlyFuseMemory<byte> to);
    PosixResult MkDir(ReadOnlyFuseMemory<byte> fileNamePtr, PosixFileMode mode);
    PosixResult Release(ReadOnlyFuseMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo);
    PosixResult RmDir(ReadOnlyFuseMemory<byte> fileNamePtr);
    PosixResult FSync(ReadOnlyFuseMemory<byte> fileNamePtr, bool datasync, ref FuseFileInfo fileInfo);
    PosixResult Unlink(ReadOnlyFuseMemory<byte> fileNamePtr);
    PosixResult Write(ReadOnlyFuseMemory<byte> fileNamePtr, ReadOnlyFuseMemory<byte> buffer, long position, out int writtenLength, ref FuseFileInfo fileInfo);
    PosixResult SymLink(ReadOnlyFuseMemory<byte> from, ReadOnlyFuseMemory<byte> to);
    PosixResult Flush(ReadOnlyFuseMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo);
    PosixResult Rename(ReadOnlyFuseMemory<byte> from, ReadOnlyFuseMemory<byte> to);
    PosixResult Truncate(ReadOnlyFuseMemory<byte> fileNamePtr, long size);
    PosixResult UTime(ReadOnlyFuseMemory<byte> fileNamePtr, TimeSpec atime, TimeSpec mtime, ref FuseFileInfo fileInfo);
    PosixResult Create(ReadOnlyFuseMemory<byte> fileNamePtr, PosixFileMode mode, ref FuseFileInfo fileInfo);
    PosixResult IoCtl(ReadOnlyFuseMemory<byte> fileNamePtr, int cmd, nint arg, ref FuseFileInfo fileInfo, FuseIoctlFlags flags, nint data);
}
