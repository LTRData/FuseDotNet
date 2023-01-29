using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FuseDotNet.Native;

/// <summary>
/// Fuse API callbacks interface
/// 
/// A struct of callbacks that describe all Fuse API operation
/// that will be called when Windows access to the filesystem.
/// 
/// If an error occurs, return -1.
/// 
/// All this callbacks can be set to <c>null</c>
/// if you dont want to support one of them. Be aware that returning such value to important callbacks
/// such <see cref="open"/>/<see cref="read"/>/... would make the filesystem not working or unstable.
/// 
/// Se <see cref="IFuseOperations"/> for more information about the fields.
/// </summary>
/// <remarks>This is the same struct as <c>FUSE_OPERATIONS</c> (fuse.h) in the C version of Fuse.</remarks>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal sealed class FuseOperations
{
	#region Delegates

	public delegate int fuse_f_fill_dir(nint buf, [MarshalAs(NativeMethods.UnmanagedStringType)]string name, in FuseFileStat stat, long off, FuseFillDirFlags flags);

	public delegate int fuse_f_getattr(nint path, nint stat, ref FuseFileInfo fileInfo);
	public delegate int fuse_f_readlink(nint path, nint target, nint size);
	public delegate int fuse_f_mknod(nint path, PosixFileMode mode, int dev);
	public delegate int fuse_f_mkdir(nint path, PosixFileMode mode);
	public delegate int fuse_f_unlink(nint path);
	public delegate int fuse_f_rmdir(nint path);
	public delegate int fuse_f_symlink(nint path, nint target);
	public delegate int fuse_f_rename(nint path, nint target);
	public delegate int fuse_f_link(nint path, nint target);
	public delegate int fuse_f_chmod(nint path, PosixFileMode mode);
	public delegate int fuse_f_chown(nint path, int uid, int gid);
	public delegate int fuse_f_truncate(nint path, long size);
	public delegate int fuse_f_open(nint path, ref FuseFileInfo fileInfo);
	public delegate int fuse_f_read(nint path, nint buffer, nint size, long position, ref FuseFileInfo fileInfo);
	public delegate int fuse_f_write(nint path, nint buffer, nint size, long position, ref FuseFileInfo fileInfo);
	public delegate int fuse_f_statfs(nint path, ref FuseVfsStat statvfs);
	public delegate int fuse_f_flush(nint path, ref FuseFileInfo fileInfo);
	public delegate int fuse_f_release(nint path, ref FuseFileInfo fileInfo);
	public delegate int fuse_f_fsync(nint path, int datasync, ref FuseFileInfo fileInfo);
	public delegate int fuse_f_setxattr(nint path, nint name, nint value, nint size, int flags);
	public delegate int fuse_f_getxattr(nint path, nint name, nint value, nint size);
	public delegate int fuse_f_listxattr(nint path, nint list, nint size);
	public delegate int fuse_f_removexattr(nint path, nint target);
	public delegate int fuse_f_opendir(nint path, ref FuseFileInfo fileInfo);
	public delegate int fuse_f_readdir(nint path, nint buf, nint fuse_fill_dir_t, long offset, ref FuseFileInfo fileInfo, FuseReadDirFlags flags);
	public delegate int fuse_f_releasedir(nint path, ref FuseFileInfo fileInfo);
	public delegate int fuse_f_fsyncdir(nint path, int datasync, ref FuseFileInfo fileInfo);
	public delegate nint fuse_f_init(ref FuseConnInfo fuse_conn_info);
	public delegate void fuse_f_destroy(nint context);
	public delegate int fuse_f_access(nint path, PosixAccessMode mask);
	public delegate int fuse_f_create(nint path, PosixFileMode mode, ref FuseFileInfo fileInfo);
	public delegate int fuse_f_lock(nint path, ref FuseFileInfo fileInfo, int cmd, nint flock);
	public delegate int fuse_f_utimens(nint path, nint timespec, ref FuseFileInfo fileInfo);
	public delegate int fuse_f_bmap(nint path, nint blocksize, out ulong idx);
	public delegate int fuse_f_ioctl(nint path, int cmd, nint arg, ref FuseFileInfo fileInfo, FuseIoctlFlags flags, nint data);
	public delegate int fuse_f_poll(nint path, ref FuseFileInfo fileInfo, nint fuse_pollhandle, ref uint reventsp);
	public delegate int fuse_f_write_buf(nint path, nint fuse_bufvec, long off, ref FuseFileInfo fileInfo);
	public delegate int fuse_f_read_buf(nint path, nint ppbuf, nint size, long off, ref FuseFileInfo fileInfo);
	public delegate int fuse_f_flock(nint path, ref FuseFileInfo fileInfo, int op);
	public delegate int fuse_f_fallocate(nint path, PosixFileMode mode, long offset, long length, ref FuseFileInfo fileInfo);
	public delegate int fuse_f_copy_file_range(nint path_in, ref FuseFileInfo fi_in, long offset_in, nint path_out, ref FuseFileInfo fi_out, long offset_out, nint size, int flags);
	public delegate int fuse_f_lseek(nint path, long offset, int whence, ref FuseFileInfo fileInfo);
	#endregion Delegates

	public fuse_f_getattr? getattr;
	public fuse_f_readlink? readlink;
	public fuse_f_mknod? mknod;
	public fuse_f_mkdir? mkdir;
	public fuse_f_unlink? unlink;
	public fuse_f_rmdir? rmdir;
	public fuse_f_symlink? symlink;
	public fuse_f_rename? rename;
	public fuse_f_link? link;
	public fuse_f_chmod? chmod;
	public fuse_f_chown? chown;
	public fuse_f_truncate? truncate;
	public fuse_f_open? open;
	public fuse_f_read? read;
	public fuse_f_write? write;
	public fuse_f_statfs? statfs;
	public fuse_f_flush? flush;
	public fuse_f_release? release;
	public fuse_f_fsync? fsync;
	public fuse_f_setxattr? setxattr;
	public fuse_f_getxattr? getxattr;
	public fuse_f_listxattr? listxattr;
	public fuse_f_removexattr? removexattr;
	public fuse_f_opendir? opendir;
	public fuse_f_readdir? readdir;
	public fuse_f_releasedir? releasedir;
	public fuse_f_fsyncdir? fsyncdir;
	public fuse_f_init? init;
	public fuse_f_destroy? destroy;
	public fuse_f_access? access;
	public fuse_f_create? create;
	public fuse_f_lock? @lock;
	public fuse_f_utimens? utimens;
	public fuse_f_bmap? bmap;
	public fuse_f_ioctl? ioctl;
	public fuse_f_poll? poll;
	public fuse_f_write_buf? write_buf;
	public fuse_f_read_buf? read_buf;
	public fuse_f_flock? flock;
	public fuse_f_fallocate? fallocate;
	public fuse_f_copy_file_range? copy_file_range;
	public fuse_f_lseek? lseek;
}
