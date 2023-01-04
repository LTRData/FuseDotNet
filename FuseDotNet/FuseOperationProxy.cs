using FuseDotNet.Extensions;
using FuseDotNet.Logging;
using FuseDotNet.Native;
using System;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE1006 // Naming Styles

namespace FuseDotNet;

/// <summary>
/// The fuse operation proxy.
/// </summary>
internal sealed class FuseOperationProxy
{
    private readonly IFuseOperations operations;

    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FuseOperationProxy"/> class.
    /// </summary>
    /// <param name="operations">
    /// A <see cref="IFuseOperations"/> that contains the custom implementation of the driver.
    /// </param>
    /// <param name="logger">
    /// A <see cref="ILogger"/> that handle all logging.
    /// </param>
    public FuseOperationProxy(IFuseOperations operations, ILogger logger)
    {
        this.operations = operations;
        this.logger = logger;
    }

    internal int readlink(IntPtr path, IntPtr target, IntPtr size)
    {
        try
        {
            var result = operations.ReadLink(FuseHelper.SpanFromIntPtr(path), FuseHelper.SpanFromIntPtr(target, size));
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"readlink(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int link(IntPtr path, IntPtr target)
    {
        try
        {
            var result = operations.Link(from: FuseHelper.SpanFromIntPtr(path), to: FuseHelper.SpanFromIntPtr(path));
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"link(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int mkdir(IntPtr path, PosixFileMode mode)
    {
        try
        {
            var result = operations.MkDir(FuseHelper.SpanFromIntPtr(path), mode);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"mkdir(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int flush(IntPtr path, ref FuseFileInfo fileInfo)
    {
        try
        {
            var result = operations.Flush(FuseHelper.SpanFromIntPtr(path), ref fileInfo);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"flush(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int write(IntPtr path, IntPtr buffer, IntPtr size, long position, ref FuseFileInfo fileInfo)
    {
        try
        {
            var result = operations.Write(FuseHelper.SpanFromIntPtr(path), FuseHelper.SpanFromIntPtr(buffer, size), position, out var writtenLength, ref fileInfo);
            if (result == PosixResult.Success)
            {
                return writtenLength;
            }

            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"write(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int unlink(IntPtr path)
    {
        try
        {
            var result = operations.Unlink(FuseHelper.SpanFromIntPtr(path));
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"unlink(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int ioctl(IntPtr path, int cmd, IntPtr arg, ref FuseFileInfo fileInfo, FuseIoctlFlags flags, IntPtr data)
    {
        try
        {
            var result = operations.IoCtl(FuseHelper.SpanFromIntPtr(path), cmd, arg, ref fileInfo, flags, data);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"open(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal unsafe int utimens(IntPtr path, IntPtr timespec, ref FuseFileInfo fileInfo)
    {
        try
        {
            var ts = (TimeSpec*)timespec.ToPointer();
            var result = operations.UTime(FuseHelper.SpanFromIntPtr(path), atime: ts[0], mtime: ts[1], ref fileInfo);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"utimens(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int create(IntPtr path, PosixFileMode mode, ref FuseFileInfo fileInfo)
    {
        try
        {
            var result = operations.Create(FuseHelper.SpanFromIntPtr(path), mode, ref fileInfo);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"create(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int symlink(IntPtr path, IntPtr target)
    {
        try
        {
            var result = operations.SymLink(FuseHelper.SpanFromIntPtr(path), FuseHelper.SpanFromIntPtr(target));
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"symlink(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int rename(IntPtr path, IntPtr target)
    {
        try
        {
            var result = operations.Rename(FuseHelper.SpanFromIntPtr(path), FuseHelper.SpanFromIntPtr(target));
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"rename(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int rmdir(IntPtr path)
    {
        try
        {
            var result = operations.RmDir(FuseHelper.SpanFromIntPtr(path));
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"rmdir(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int truncate(IntPtr path, long size)
    {
        try
        {
            var result = operations.Truncate(FuseHelper.SpanFromIntPtr(path), size);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"truncate(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int open(IntPtr path, ref FuseFileInfo fileInfo)
    {
        try
        {
            var result = operations.Open(FuseHelper.SpanFromIntPtr(path), ref fileInfo);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"open(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int opendir(IntPtr path, ref FuseFileInfo fileInfo)
    {
        try
        {
            var result = operations.OpenDir(FuseHelper.SpanFromIntPtr(path), ref fileInfo);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"opendir(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int fsync(IntPtr path, int datasync, ref FuseFileInfo fileInfo)
    {
        try
        {
            var result = operations.FSync(FuseHelper.SpanFromIntPtr(path), datasync != 0, ref fileInfo);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"fsync(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int release(IntPtr path, ref FuseFileInfo fileInfo)
    {
        try
        {
            var result = operations.Release(FuseHelper.SpanFromIntPtr(path), ref fileInfo);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"release(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int releasedir(IntPtr path, ref FuseFileInfo fileInfo)
    {
        try
        {
            var result = operations.ReleaseDir(FuseHelper.SpanFromIntPtr(path), ref fileInfo);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"releasedir(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal IntPtr init(ref FuseConnInfo fuse_conn_info)
    {
        try
        {
            operations.Init(ref fuse_conn_info);
            return IntPtr.Zero;
        }
        catch (Exception ex)
        {
            logger.Error($"init(): {ex}");
            return IntPtr.Zero;
        }
    }

    internal int access(IntPtr path, PosixAccessMode mask)
    {
        try
        {
            var result = operations.Access(FuseHelper.SpanFromIntPtr(path), mask);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"access(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal void destroy(IntPtr context)
    {
        try
        {
            operations.Dispose();
        }
        catch (Exception ex)
        {
            logger.Error($"destroy(): {ex}");
        }
    }

    internal int fsyncdir(IntPtr path, int datasync, ref FuseFileInfo fileInfo)
    {
        try
        {
            var result = operations.FSyncDir(FuseHelper.SpanFromIntPtr(path), datasync != 0, ref fileInfo);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"fsyncdir(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int statfs(IntPtr path, ref FuseVfsStat statvfs)
    {
        try
        {
            var result = operations.StatFs(FuseHelper.SpanFromIntPtr(path), out statvfs);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"statfs(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int getattr(IntPtr path, out FuseFileStat stat)
    {
        stat = default;
        try
        {
            var result = operations.GetAttr(FuseHelper.SpanFromIntPtr(path), out stat);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"getattr(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int readdir(IntPtr path, IntPtr buf, IntPtr fuse_fill_dir_t, long offset, ref FuseFileInfo fileInfo, FuseReadDirFlags flags)
    {
        try
        {
            var pathPtr = FuseHelper.SpanFromIntPtr(path);

            var result = operations.ReadDir(pathPtr, out var entries, ref fileInfo, offset, flags);

            if (result != PosixResult.Success)
            {
                return -(int)result;
            }

            var fuse_fill_dir = Marshal.GetDelegateForFunctionPointer<FuseOperations.fuse_f_fill_dir>(fuse_fill_dir_t);

            if (logger.DebugEnabled)
            {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
                var pathstr = Encoding.UTF8.GetString(pathPtr);
#else
                var pathstr = Encoding.UTF8.GetString(pathPtr.ToArray());
#endif

                logger.Debug($"Filling files for directory '{pathstr}'");
            }

            foreach (var file in entries)
            {
                if (logger.DebugEnabled)
                {
                    logger.Debug($"{file}");
                }

                var fill_result = fuse_fill_dir(buf, file.Name, file.Stat, file.Offset, file.Flags);

                if (fill_result != 0)
                {
                    break;
                }
            }

            if (logger.DebugEnabled)
            {
                logger.Debug($"Finished filling files for directory");
            }

            return 0;
        }
        catch (Exception ex)
        {
            logger.Error($"readdir(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int read(IntPtr path, IntPtr buffer, IntPtr size, long position, ref FuseFileInfo fileInfo)
    {
        try
        {
            var result = operations.Read(FuseHelper.SpanFromIntPtr(path), FuseHelper.SpanFromIntPtr(buffer, size), position, out var readLength, ref fileInfo);
            if (result == PosixResult.Success)
            {
                return readLength;
            }

            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"read(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }
}

