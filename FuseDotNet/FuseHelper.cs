using FuseDotNet.Native;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

#pragma warning disable IDE0079 // Remove unnecessary suppression

namespace FuseDotNet.Extensions;

/// <summary>
/// %Fuse functions helpers for user <see cref="IFuseOperations"/> implementation.
/// </summary>
public static class FuseHelper
{
    public static ReadOnlyCollection<FuseDirEntry> DotEntries { get; } = Array.AsReadOnly(new FuseDirEntry[] {
        new(".", 0, 0, new() { st_mode = PosixFileMode.Directory }),
        new("..", 0, 0, new() { st_mode = PosixFileMode.Directory })
    });

#if NET6_0_OR_GREATER

    public static unsafe ReadOnlySpan<byte> SpanFromIntPtr(IntPtr ptr)
        => MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*)ptr.ToPointer());

#else

    public static unsafe ReadOnlySpan<byte> SpanFromIntPtr(IntPtr ptr)
        => ptr == IntPtr.Zero
        ? default
        : SpanFromIntPtr(ptr, NativeMethods.strlen(ptr));

#endif

    public static unsafe Span<byte> SpanFromIntPtr(IntPtr ptr, IntPtr size)
        => new(ptr.ToPointer(), size.ToInt32());

    public static unsafe Span<byte> SpanFromIntPtr(IntPtr ptr, int size)
        => new(ptr.ToPointer(), size);

    public static PosixFileMode ToPosixFileMode(this FileAttributes fileAttributes)
    {
        if (fileAttributes.HasFlag(FileAttributes.Directory))
        {
            if (fileAttributes.HasFlag(FileAttributes.ReadOnly))
            {
                return PosixFileMode.Directory | PosixFileMode.OwnerReadExecute | PosixFileMode.GroupReadExecute | PosixFileMode.OthersReadExecute;
            }
            else
            {
                return PosixFileMode.Directory | PosixFileMode.OwnerAll | PosixFileMode.GroupReadExecute | PosixFileMode.OthersReadExecute;
            }
        }
        else if (fileAttributes.HasFlag(FileAttributes.Device))
        {
            if (fileAttributes.HasFlag(FileAttributes.ReadOnly))
            {
                return PosixFileMode.Block | PosixFileMode.OwnerRead | PosixFileMode.GroupRead;
            }
            else
            {
                return PosixFileMode.Block | PosixFileMode.OwnerReadWrite | PosixFileMode.GroupRead;
            }
        }
        else if (fileAttributes.HasFlag(FileAttributes.ReadOnly))
        {
            return PosixFileMode.Regular | PosixFileMode.OwnerRead | PosixFileMode.GroupRead | PosixFileMode.OthersRead;
        }
        else
        {
            return PosixFileMode.Regular | PosixFileMode.OwnerReadWrite | PosixFileMode.GroupRead | PosixFileMode.OthersRead;
        }
    }

    public static FileMode ToFileMode(this PosixOpenFlags flags)
    {
        FileMode mode;
        if (flags.HasFlag(PosixOpenFlags.CreateNew))
        {
            mode = FileMode.CreateNew;
        }
        else if (flags.HasFlag(PosixOpenFlags.Create))
        {
            mode = FileMode.Create;
        }
        else if (flags.HasFlag(PosixOpenFlags.Truncate))
        {
            mode = FileMode.Truncate;
        }
        else if (flags.HasFlag(PosixOpenFlags.Append))
        {
            mode = FileMode.Append;
        }
        else
        {
            mode = FileMode.Open;
        }
        return mode;
    }

    public static FileAccess ToFileAccess(this PosixOpenFlags flags)
    {
        return (flags & PosixOpenFlags.AccessModes) switch
        {
            PosixOpenFlags.Write => FileAccess.Write,
            PosixOpenFlags.ReadWrite => FileAccess.ReadWrite,
            _ => FileAccess.Read
        };
    }

    public static FileShare ToFileShare(this PosixOpenFlags flags)
    {
        if (flags.HasFlag(PosixOpenFlags.SharedLock))
        {
            return FileShare.Read | FileShare.Delete;
        }
        else if (flags.HasFlag(PosixOpenFlags.ExclusiveLock))
        {
            return FileShare.None | FileShare.Delete;
        }
        else
        {
            return FileShare.ReadWrite | FileShare.Delete;
        }
    }

    public static PosixResult ToPosixResult(this Exception? ex)
    {
        if (ex is TargetInvocationException or AggregateException)
        {
            ex = ex.InnerException;
        }

        return ex switch
        {
            PosixException posixException => posixException.NativeErrorCode,
            FileNotFoundException => PosixResult.ENOENT,
            UnauthorizedAccessException => PosixResult.EPERM,
            DirectoryNotFoundException => PosixResult.ENOTDIR,
            InvalidOperationException => PosixResult.EOPNOTSUPP,
            NotSupportedException or NotImplementedException => PosixResult.ENOSYS,
            PathTooLongException => PosixResult.ENAMETOOLONG,
            OutOfMemoryException => PosixResult.ENOMEM,
            ThreadAbortException or ThreadInterruptedException or OperationCanceledException => PosixResult.EINTR,
            ArgumentException or ArgumentOutOfRangeException or IndexOutOfRangeException or
            ArgumentNullException or NullReferenceException => PosixResult.EINVAL,
            _ => PosixResult.EIO
        };
    }
}
