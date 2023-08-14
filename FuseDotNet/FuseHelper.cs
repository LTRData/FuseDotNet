using FuseDotNet.Native;
using System;
using System.Buffers;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
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

    public static unsafe FuseMemory<byte> SpanFromIntPtr(nint ptr)
        => new(ptr, MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*)ptr).Length);

#else

    public static unsafe FuseMemory<byte> SpanFromIntPtr(nint ptr)
        => ptr == 0
        ? default
        : new(ptr, (int)NativeMethods.strlen(ptr));

#endif

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
        => (flags & PosixOpenFlags.AccessModes) switch
        {
            PosixOpenFlags.Write => FileAccess.Write,
            PosixOpenFlags.ReadWrite => FileAccess.ReadWrite,
            _ => FileAccess.Read
        };

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

    public static string GetStringFromSpan(ReadOnlySpan<byte> span)
    {
        if (span.IsEmpty)
        {
            return "";
        }
        else if (span.SequenceEqual("/"u8))
        {
            return "/";
        }
        else if (span.SequenceEqual(@"\"u8))
        {
            return @"\";
        }
        else if (span.SequenceEqual("*"u8))
        {
            return "*";
        }
        else if (span.SequenceEqual("*.*"u8))
        {
            return "*.*";
        }
        else if (span.SequenceEqual("?"u8))
        {
            return "?";
        }
        else
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
            return Encoding.UTF8.GetString(span);
#else
            var buffer = ArrayPool<byte>.Shared.Rent(span.Length);
            try
            {
                span.CopyTo(buffer);
                return Encoding.UTF8.GetString(buffer, 0, span.Length);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
#endif
        }
    }
}
