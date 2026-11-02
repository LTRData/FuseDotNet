using FuseDotNet.Native;
using LTRData.Extensions.Native.Memory;
using System;
using System.Buffers;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;

#pragma warning disable IDE0079 // Remove unnecessary suppression

namespace FuseDotNet.Extensions;

/// <summary>
/// %Fuse functions helpers for user <see cref="IFuseOperations"/> implementation.
/// </summary>
public static class FuseHelper
{
    public static ImmutableArray<FuseDirEntry> DotEntries { get; } = [
        new(Name: ".", Offset: 0, Flags: 0, Stat: new() { st_mode = PosixFileMode.Directory }),
        new(Name: "..", Offset: 0, Flags: 0, Stat: new() { st_mode = PosixFileMode.Directory })
    ];

#if NET6_0_OR_GREATER

    public static unsafe NativeMemory<byte> SpanFromIntPtr(nint ptr)
        => ptr == 0
        ? default
        : new(ptr, MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*)ptr).Length);

#else

    public static unsafe NativeMemory<byte> SpanFromIntPtr(nint ptr)
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

    /// <summary>
    /// Maps creation and truncation flags to a FileMode. Append is a write-time
    /// behavior and must be handled separately; FileMode.Append is not equivalent
    /// to O_APPEND (it creates missing files and disallows read/write access).
    /// </summary>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("freebsd")]
#endif
    public static FileMode ToFileMode(this PosixOpenFlags flags)
    {
        FileMode mode;

        if (flags.HasFlag(PosixOpenFlags.Create | PosixOpenFlags.CreateNew))
        {
            mode = FileMode.CreateNew;
        }
        else if (flags.HasFlag(PosixOpenFlags.Create))
        {
            mode = flags.HasFlag(PosixOpenFlags.Truncate) ? FileMode.Create : FileMode.OpenOrCreate;
        }
        else if (flags.HasFlag(PosixOpenFlags.Truncate))
        {
            mode = FileMode.Truncate;
        }
        else
        {
            mode = FileMode.Open;
        }

        return mode;
    }

#if NET5_0_OR_GREATER
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("freebsd")]
#endif
    public static FileAccess ToFileAccess(this PosixOpenFlags flags)
        => (flags & PosixOpenFlags.AccessModes) switch
        {
            var mode when mode == PosixOpenFlags.Write => FileAccess.Write,
            var mode when mode == PosixOpenFlags.ReadWrite => FileAccess.ReadWrite,
            _ => FileAccess.Read
        };

#if NET5_0_OR_GREATER
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("freebsd")]
#endif
    public static FileShare ToFileShare(this PosixOpenFlags flags)
    {
#if NETCOREAPP
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
#endif
        {
            return FileShare.ReadWrite | FileShare.Delete;
        }

#if NETCOREAPP
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
#endif
    }

#if NET5_0_OR_GREATER
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("freebsd")]
#endif
    public static PosixResult ToPosixResult(this Exception? ex)
    {
        while (ex is TargetInvocationException or AggregateException)
        {
            ex = ex.InnerException;
        }

        return ex switch
        {
            PosixException posixException => posixException.NativeErrorCode,
            FileNotFoundException => PosixResult.ENOENT,
            UnauthorizedAccessException => PosixResult.EPERM,
            DirectoryNotFoundException => PosixResult.ENOTDIR,
            InvalidOperationException => PosixResult.ENOTSUP,
            NotSupportedException or NotImplementedException => PosixResult.ENOSYS,
            PathTooLongException => PosixResult.ENAMETOOLONG,
            OutOfMemoryException or InsufficientExecutionStackException or StackOverflowException => PosixResult.ENOMEM,
            ThreadAbortException or ThreadInterruptedException or OperationCanceledException => PosixResult.EINTR,
            ArgumentException or IndexOutOfRangeException or NullReferenceException => PosixResult.EINVAL,
            _ => PosixResult.EIO
        };
    }

    public static string GetString(ReadOnlyNativeMemory<byte> buffer)
        => GetString(buffer.Span);

    public static unsafe string GetString(ReadOnlySpan<byte> span)
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
            fixed (byte* ptr = span)
            {
                return Encoding.UTF8.GetString(ptr, span.Length);
            }
#endif
        }
    }
}
