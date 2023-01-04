using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;

/// <summary>
/// Base namespace for %Fuse.
/// </summary>
namespace FuseDotNet
{
    /// <summary>
    /// %Fuse API callbacks interface.
    /// 
    /// A interface of callbacks that describe all %Fuse API operation
    /// that will be called when Windows access to the file system.
    /// 
    /// All this callbacks can return <see cref="PosixResult.NotImplemented"/>
    /// if you dont want to support one of them. Be aware that returning such value to important callbacks
    /// such <see cref="open"/>/<see cref="read"/>/... would make the filesystem not working or unstable.
    /// </summary>
    /// <remarks>This is the same struct as <c>FUSE_OPERATIONS</c> (fuse.h) in the C version of Fuse.</remarks>
    public interface IFuseOperations : IDisposable
    {
        PosixResult OpenDir(ReadOnlySpan<byte> fileNamePtr, ref FuseFileInfo fileInfo);
        PosixResult GetAttr(ReadOnlySpan<byte> fileNamePtr, out FuseFileStat stat);        
        PosixResult Read(ReadOnlySpan<byte> fileNamePtr, Span<byte> buffer, long position, out int readLength, ref FuseFileInfo fileInfo);        
        PosixResult ReadDir(ReadOnlySpan<byte> fileNamePtr, out IEnumerable<FuseDirEntry> entries, ref FuseFileInfo fileInfo, long offset, FuseReadDirFlags flags);
        PosixResult Open(ReadOnlySpan<byte> fileNamePtr, ref FuseFileInfo fileInfo);
        void Init(ref FuseConnInfo fuse_conn_info);
        PosixResult Access(ReadOnlySpan<byte> fileNamePtr, PosixAccessMode mask);
        PosixResult StatFs(ReadOnlySpan<byte> fileNamePtr, out FuseVfsStat statvfs);
        PosixResult FSyncDir(ReadOnlySpan<byte> fileNamePtr, bool datasync, ref FuseFileInfo fileInfo);
        PosixResult ReadLink(ReadOnlySpan<byte> fileNamePtr, Span<byte> target);
        PosixResult ReleaseDir(ReadOnlySpan<byte> fileNamePtr, ref FuseFileInfo fileInfo);
        PosixResult Link(ReadOnlySpan<byte> from, ReadOnlySpan<byte> to);
        PosixResult MkDir(ReadOnlySpan<byte> fileNamePtr, PosixFileMode mode);
        PosixResult Release(ReadOnlySpan<byte> fileNamePtr, ref FuseFileInfo fileInfo);
        PosixResult RmDir(ReadOnlySpan<byte> fileNamePtr);
        PosixResult FSync(ReadOnlySpan<byte> fileNamePtr, bool datasync, ref FuseFileInfo fileInfo);
        PosixResult Unlink(ReadOnlySpan<byte> fileNamePtr);
        PosixResult Write(ReadOnlySpan<byte> fileNamePtr, ReadOnlySpan<byte> buffer, long position, out int writtenLength, ref FuseFileInfo fileInfo);
        PosixResult SymLink(ReadOnlySpan<byte> from, ReadOnlySpan<byte> to);
        PosixResult Flush(ReadOnlySpan<byte> fileNamePtr, ref FuseFileInfo fileInfo);
        PosixResult Rename(ReadOnlySpan<byte> from, ReadOnlySpan<byte> to);
        PosixResult Truncate(ReadOnlySpan<byte> fileNamePtr, long size);
        PosixResult UTime(ReadOnlySpan<byte> fileNamePtr, TimeSpec atime, TimeSpec mtime, ref FuseFileInfo fileInfo);
        PosixResult Create(ReadOnlySpan<byte> fileNamePtr, PosixFileMode mode, ref FuseFileInfo fileInfo);
        PosixResult IoCtl(ReadOnlySpan<byte> fileNamePtr, int cmd, IntPtr arg, ref FuseFileInfo fileInfo, FuseIoctlFlags flags, IntPtr data);
    }
}

/// <summary>
/// Namespace for AssemblyInfo and resource strings
/// </summary>
namespace FuseDotNet.Properties
{
    // This is only for documentation of the FuseDotNet.Properties namespace.
}
