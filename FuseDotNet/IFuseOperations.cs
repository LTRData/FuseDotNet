using LTRData.Extensions.Native.Memory;
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
/// such <see cref="Open(ReadOnlyNativeMemory{byte}, ref FuseFileInfo)"/>/<see cref="Read(ReadOnlyNativeMemory{byte}, NativeMemory{byte}, long, out int, ref FuseFileInfo)"/>/... would make the filesystem not working or unstable.
/// </summary>
/// <remarks>This is the same struct as <c>FUSE_OPERATIONS</c> (fuse.h) in the C version of Fuse.</remarks>
public interface IFuseOperations : IDisposable
{
    PosixResult OpenDir(ReadOnlyNativeMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo);
    PosixResult GetAttr(ReadOnlyNativeMemory<byte> fileNamePtr, out FuseFileStat stat, ref FuseFileInfo fileInfo);        
    PosixResult Read(ReadOnlyNativeMemory<byte> fileNamePtr, NativeMemory<byte> buffer, long position, out int readLength, ref FuseFileInfo fileInfo);        
    PosixResult ReadDir(ReadOnlyNativeMemory<byte> fileNamePtr, out IEnumerable<FuseDirEntry> entries, ref FuseFileInfo fileInfo, long offset, FuseReadDirFlags flags);
    PosixResult Open(ReadOnlyNativeMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo);
    void Init(ref FuseConnInfo fuse_conn_info);
    PosixResult Access(ReadOnlyNativeMemory<byte> fileNamePtr, PosixAccessMode mask);
    PosixResult StatFs(ReadOnlyNativeMemory<byte> fileNamePtr, out FuseVfsStat statvfs);
    PosixResult FSyncDir(ReadOnlyNativeMemory<byte> fileNamePtr, bool datasync, ref FuseFileInfo fileInfo);
    PosixResult ReadLink(ReadOnlyNativeMemory<byte> fileNamePtr, NativeMemory<byte> target);
    PosixResult ReleaseDir(ReadOnlyNativeMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo);
    PosixResult Link(ReadOnlyNativeMemory<byte> from, ReadOnlyNativeMemory<byte> to);
    PosixResult MkDir(ReadOnlyNativeMemory<byte> fileNamePtr, PosixFileMode mode);
    PosixResult Release(ReadOnlyNativeMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo);
    PosixResult RmDir(ReadOnlyNativeMemory<byte> fileNamePtr);
    PosixResult FSync(ReadOnlyNativeMemory<byte> fileNamePtr, bool datasync, ref FuseFileInfo fileInfo);
    PosixResult Unlink(ReadOnlyNativeMemory<byte> fileNamePtr);
    PosixResult Write(ReadOnlyNativeMemory<byte> fileNamePtr, ReadOnlyNativeMemory<byte> buffer, long position, out int writtenLength, ref FuseFileInfo fileInfo);
    PosixResult SymLink(ReadOnlyNativeMemory<byte> from, ReadOnlyNativeMemory<byte> to);
    PosixResult Flush(ReadOnlyNativeMemory<byte> fileNamePtr, ref FuseFileInfo fileInfo);
    PosixResult Rename(ReadOnlyNativeMemory<byte> from, ReadOnlyNativeMemory<byte> to);
    PosixResult Truncate(ReadOnlyNativeMemory<byte> fileNamePtr, long size);
    PosixResult UTime(ReadOnlyNativeMemory<byte> fileNamePtr, TimeSpec atime, TimeSpec mtime, ref FuseFileInfo fileInfo);
    PosixResult Create(ReadOnlyNativeMemory<byte> fileNamePtr, int mode, ref FuseFileInfo fileInfo);
    PosixResult IoCtl(ReadOnlyNativeMemory<byte> fileNamePtr, int cmd, nint arg, ref FuseFileInfo fileInfo, FuseIoctlFlags flags, nint data);
    PosixResult ChMod(NativeMemory<byte> fileNamePtr, PosixFileMode mode);
    PosixResult ChOwn(NativeMemory<byte> fileNamePtr, int uid, int gid);
    PosixResult FAllocate(NativeMemory<byte> fileNamePtr, FuseAllocateMode mode, long offset, long length, ref FuseFileInfo fileInfo);
}
