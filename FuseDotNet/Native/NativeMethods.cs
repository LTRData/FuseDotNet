using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments

/// <summary>
/// Namespace for structures and classes related to native API.
/// </summary>
namespace FuseDotNet.Native;

/// <summary>
/// Native API to the kernel Fuse driver.
/// </summary>
internal static class NativeMethods
{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public const UnmanagedType UnmanagedStringType = UnmanagedType.LPUTF8Str;
#else
    public const UnmanagedType UnmanagedStringType = UnmanagedType.LPStr;
#endif

    private const string LIB_FUSE = "fuse3";

    private const string LIB_C = "c";

    /// <summary>
    /// Mount a new Fuse Volume.
    /// This function block until the device is unmounted.
    /// If the mount fails, it will directly return an error.
    /// </summary>
    /// <param name="argv">Array of pointers to UTF8 encoded arguments that describe the mount.</param>
    /// <param name="operations">Instance of <see cref="FuseOperations"/> that will be called for each request made by the kernel.</param>
    /// <returns><see cref="PosixResult"/></returns>
    [DllImport(LIB_FUSE, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern PosixResult fuse_main_real(int argc,
        [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] argv,
        [In] FuseOperations? operations, IntPtr operationsSize, IntPtr userData);

    [DllImport(LIB_C, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern PosixResult unmount([In, MarshalAs(UnmanagedStringType)] string dir, int flags);

    [DllImport(LIB_C, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern IntPtr strlen(IntPtr ptr);

    [DllImport(LIB_C, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern TimeSpec time(out TimeSpec timespec);

    [DllImport(LIB_C, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedStringType)]
    internal static extern string strerror(PosixResult errno);

}
