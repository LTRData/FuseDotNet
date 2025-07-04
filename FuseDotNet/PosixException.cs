using System;

namespace FuseDotNet;

public class PosixException : Exception
{
    public PosixResult NativeErrorCode { get; }

    public PosixException(PosixResult errno)
        : base(errno.ToString())
    {
        NativeErrorCode = errno;
    }

    public PosixException(PosixResult errno, Exception? innerException)
        : base(errno.ToString(), innerException)
    {
        NativeErrorCode = errno;
    }
}
