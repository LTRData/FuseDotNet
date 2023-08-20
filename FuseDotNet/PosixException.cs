using FuseDotNet.Native;
using System;
using System.Collections.Concurrent;

namespace FuseDotNet;

public class PosixException : Exception
{
    public PosixResult NativeErrorCode { get; }

    public PosixException(PosixResult errno)
        : base(GetSystemErrorMessage(errno))
    {
        NativeErrorCode = errno;
    }

    public PosixException(PosixResult errno, Exception? innerException)
        : base(GetSystemErrorMessage(errno), innerException)
    {
        NativeErrorCode = errno;
    }

    public static string GetSystemErrorMessage(PosixResult errno)
        => systemMessages.GetOrAdd(errno, NativeMethods.strerror);

    private static readonly ConcurrentDictionary<PosixResult, string> systemMessages = new();
}
