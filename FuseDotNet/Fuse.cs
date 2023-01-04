#define CONSOLE_LOGGER

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using FuseDotNet.Extensions;
using FuseDotNet.Logging;
using FuseDotNet.Native;

#pragma warning disable IDE0079 // Remove unnecessary suppression

namespace FuseDotNet;

/// <summary>
/// Helper and extension methods to %Fuse.
/// </summary>
public static class Fuse
{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public static IntPtr StringToCoTaskMemUTF8(string? arg)
        => Marshal.StringToCoTaskMemUTF8(arg);
#else
    public static IntPtr StringToCoTaskMemUTF8(string? arg)
    {
        if (arg == null)
        {
            return IntPtr.Zero;
        }

        var array = Encoding.UTF8.GetBytes(arg);
        var ptr = Marshal.AllocCoTaskMem(array.Length + 1);
        var bytes = FuseHelper.SpanFromIntPtr(ptr, array.Length + 1);
        array.CopyTo(bytes);
        bytes[array.Length] = 0;
        return ptr;
    }
#endif

    /// <summary>
    /// Call %Fuse without file system operations. Useful for example to list available command line
    /// switches, version etc.
    /// </summary>
    /// <param name="args">Command line arguments to pass to fuse_main().</param>
    /// <returns><see cref="PosixResult"/> from fuse_main()</returns>
    public static PosixResult CallMain(IEnumerable<string> args)
    {
        var utf8args = args.Select(StringToCoTaskMemUTF8).ToArray();

        var status = NativeMethods.fuse_main_real(utf8args.Length, utf8args, null, IntPtr.Zero, IntPtr.Zero);

        Array.ForEach(utf8args, Marshal.FreeCoTaskMem);

        return status;
    }

    /// <summary>
    /// Mount a new %Fuse Volume.
    /// This function block until the device is unmounted.
    /// </summary>
    /// <param name="operations">Instance of <see cref="IFuseOperations"/> that will be called for each request made by the kernel.</param>
    /// <param name="args">Command line arguments to pass to fuse_main().</param>
    /// <param name="logger"><see cref="ILogger"/> that will log all FuseDotNet debug information.</param>
    /// <exception cref="PosixException">If the mount fails.</exception>
    public static void Mount(this IFuseOperations operations, IEnumerable<string> args, ILogger? logger = null)
    {
        logger ??= new NullLogger();

        var fuseOperationProxy = new FuseOperationProxy(operations, logger);

        var fuseOperations = new FuseOperations
        {
            getattr = fuseOperationProxy.getattr,
            readlink = fuseOperationProxy.readlink,
            mknod = null, // fuseOperationProxy.mknod,
            mkdir = fuseOperationProxy.mkdir,
            unlink = fuseOperationProxy.unlink,
            rmdir = fuseOperationProxy.rmdir,
            symlink = fuseOperationProxy.symlink,
            rename = fuseOperationProxy.rename,
            link = fuseOperationProxy.link,
            chmod = null, // fuseOperationProxy.chmod,
            chown = null, // fuseOperationProxy.chown,
            truncate = fuseOperationProxy.truncate,
            open = fuseOperationProxy.open,
            read = fuseOperationProxy.read,
            write = fuseOperationProxy.write,
            statfs = fuseOperationProxy.statfs,
            flush = fuseOperationProxy.flush,
            release = fuseOperationProxy.release,
            fsync = fuseOperationProxy.fsync,
            setxattr = null, // fuseOperationProxy.setxattr,
            getxattr = null, // fuseOperationProxy.getxattr,
            listxattr = null, // fuseOperationProxy.listxattr,
            removexattr = null, // fuseOperationProxy.removexattr,
            opendir = fuseOperationProxy.opendir,
            readdir = fuseOperationProxy.readdir,
            releasedir = fuseOperationProxy.releasedir,
            fsyncdir = fuseOperationProxy.fsyncdir,
            init = fuseOperationProxy.init,
            destroy = fuseOperationProxy.destroy,
            access = fuseOperationProxy.access,
            create = fuseOperationProxy.create,
            @lock = null, // fuseOperationProxy.@lock,
            utimens = fuseOperationProxy.utimens,
            bmap = null, // fuseOperationProxy.bmap,
            ioctl = fuseOperationProxy.ioctl,
            poll = null, // fuseOperationProxy.poll,
            write_buf = null, // fuseOperationProxy.write_buf,
            read_buf = null, // fuseOperationProxy.read_buf,
            flock = null, // fuseOperationProxy.flock,
            fallocate = null, // fuseOperationProxy.fallocate
            copy_file_range = null, // fuseOperationProxy.copy_file_range
            lseek = null, // fuseOperationProxy.lseek
        };

        var utf8args = args.Select(StringToCoTaskMemUTF8).ToArray();

        var status = NativeMethods.fuse_main_real(utf8args.Length, utf8args, fuseOperations, new IntPtr(Marshal.SizeOf(fuseOperations)), IntPtr.Zero);

        Array.ForEach(utf8args, Marshal.FreeCoTaskMem);

        GC.KeepAlive(fuseOperations);

        if (status != PosixResult.Success)
        {
            throw new PosixException(status);
        }
    }
}

