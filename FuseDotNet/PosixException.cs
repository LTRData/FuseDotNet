using FuseDotNet.Native;
using System;

namespace FuseDotNet;

public class PosixException : Exception
{
    public PosixResult NativeErrorCode { get; }

    public PosixException(PosixResult errno)
        : base(NativeMethods.strerror(errno))
    {
        NativeErrorCode = errno;
    }

    public PosixException(PosixResult errno, Exception? innerException)
        : base(NativeMethods.strerror(errno), innerException)
    {
        NativeErrorCode = errno;
    }

}
