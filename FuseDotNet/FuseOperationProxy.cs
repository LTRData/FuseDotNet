using FuseDotNet.Extensions;
using FuseDotNet.Logging;
using System;
using System.Buffers;
using System.Text;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE1006 // Naming Styles

namespace FuseDotNet;

/// <summary>
/// The fuse operation proxy.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FuseOperationProxy"/> class.
/// </remarks>
/// <param name="operations">
/// A <see cref="IFuseOperations"/> that contains the custom implementation of the driver.
/// </param>
/// <param name="logger">
/// A <see cref="ILogger"/> that handle all logging.
/// </param>
internal sealed class FuseOperationProxy(IFuseOperations operations, ILogger logger)
{
    private readonly IFuseOperations operations = operations;

    private readonly ILogger logger = logger;

    internal int readlink(nint path, nint target, nint size)
    {
        try
        {
            var result = operations.ReadLink(FuseHelper.SpanFromIntPtr(path), new(target, (int)size));
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"readlink(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int link(nint path, nint target)
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

    internal int mkdir(nint path, PosixFileMode mode)
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

    internal int flush(nint path, ref FuseFileInfo fileInfo)
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

    internal int write(nint path, nint buffer, nint size, long position, ref FuseFileInfo fileInfo)
    {
        try
        {
            var result = operations.Write(FuseHelper.SpanFromIntPtr(path), new(buffer, (int)size), position, out var writtenLength, ref fileInfo);
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

    internal int unlink(nint path)
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

    internal int ioctl(nint path, int cmd, nint arg, ref FuseFileInfo fileInfo, FuseIoctlFlags flags, nint data)
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

    internal unsafe int utimens(nint path, nint timespec, ref FuseFileInfo fileInfo)
    {
        try
        {
            var ts = (TimeSpec*)timespec;
            var result = operations.UTime(FuseHelper.SpanFromIntPtr(path), atime: ts[0], mtime: ts[1], ref fileInfo);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"utimens(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int create(nint path, PosixFileMode mode, ref FuseFileInfo fileInfo)
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

    internal int symlink(nint path, nint target)
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

    internal int rename(nint path, nint target)
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

    internal int rmdir(nint path)
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

    internal int truncate(nint path, long size)
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

    internal int open(nint path, ref FuseFileInfo fileInfo)
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

    internal int opendir(nint path, ref FuseFileInfo fileInfo)
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

    internal int fsync(nint path, int datasync, ref FuseFileInfo fileInfo)
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

    internal int release(nint path, ref FuseFileInfo fileInfo)
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

    internal int releasedir(nint path, ref FuseFileInfo fileInfo)
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

    internal nint init(ref FuseConnInfo fuse_conn_info)
    {
        try
        {
            operations.Init(ref fuse_conn_info);
            return 0;
        }
        catch (Exception ex)
        {
            logger.Error($"init(): {ex}");
            return 0;
        }
    }

    internal int access(nint path, PosixAccessMode mask)
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

    internal void destroy(nint context)
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

    internal int fsyncdir(nint path, int datasync, ref FuseFileInfo fileInfo)
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

    internal int statfs(nint path, nint statptr)
    {
        try
        {
            var result = operations.StatFs(FuseHelper.SpanFromIntPtr(path), out var statvfs);
            statvfs.MarshalToNative(statptr);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"statfs(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal int getattr(nint path, nint statptr, ref FuseFileInfo fileInfo)
    {
        try
        {
            var result = operations.GetAttr(FuseHelper.SpanFromIntPtr(path), out var stat, ref fileInfo);
            stat.MarshalToNative(statptr);
            return -(int)result;
        }
        catch (Exception ex)
        {
            logger.Error($"getattr(): {ex}");
            return -(int)ex.ToPosixResult();
        }
    }

    internal unsafe int readdir(nint path, nint buf, nint fuse_fill_dir_t, long offset, ref FuseFileInfo fileInfo, FuseReadDirFlags flags)
    {
        try
        {
            var pathPtr = FuseHelper.SpanFromIntPtr(path);

            var result = operations.ReadDir(pathPtr, out var entries, ref fileInfo, offset, flags);

            if (result != PosixResult.Success)
            {
                return -(int)result;
            }

            var fuse_fill_dir = (delegate* unmanaged[Cdecl]<nint, in byte, void*, long, FuseFillDirFlags, int>)fuse_fill_dir_t;

            if (logger.DebugEnabled)
            {
                var pathstr = FuseHelper.GetString(pathPtr);

                logger.Debug($"Filling files for directory '{pathstr}'");
            }

            var stat = stackalloc byte[FuseFileStat.NativeStructSize];

            var name = ArrayPool<byte>.Shared.Rent(512);

            try
            {
                foreach (var file in entries)
                {
                    if (logger.DebugEnabled)
                    {
                        logger.Debug($"Directory entry: {file}");
                    }

                    file.Stat.MarshalToNative((nint)stat);

                    var length = Encoding.UTF8.GetByteCount(file.Name) + 1;  // Extra byte for null terminator

                    if (name.Length < length)
                    {
                        ArrayPool<byte>.Shared.Return(name);
                        name = null;
                        name = ArrayPool<byte>.Shared.Rent(length);
                    }

                    length = Encoding.UTF8.GetBytes(file.Name, 0, file.Name.Length, name, 0);
                    
                    name[length] = 0; // Add null terminator

                    var fill_result = fuse_fill_dir(buf, name[0], stat, file.Offset, file.Flags);

                    if (fill_result != 0)
                    {
                        break;
                    }
                }
            }
            finally
            {
                if (name is not null)
                {
                    ArrayPool<byte>.Shared.Return(name);
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

    internal int read(nint path, nint buffer, nint size, long position, ref FuseFileInfo fileInfo)
    {
        try
        {
            var result = operations.Read(FuseHelper.SpanFromIntPtr(path), new(buffer, (int)size), position, out var readLength, ref fileInfo);
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

