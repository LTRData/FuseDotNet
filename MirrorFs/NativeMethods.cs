using FuseDotNet;
using FuseDotNet.Extensions;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace MirrorFs;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("freebsd")]
#endif
internal static class NativeMethods
{
    [DllImport("libc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, SetLastError = true)]
    private static extern int open([MarshalAs(UnmanagedType.LPUTF8Str)] string path, int flags, uint mode);

    [DllImport("libc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, SetLastError = true)]
    internal static extern unsafe nint write(SafeFileHandle fd, byte* buffer, nuint count);

    internal static FileStream OpenAppend(string path, PosixOpenFlags flags)
    {
        // A native O_APPEND descriptor and write() provide atomic append even
        // across separate handles and writers outside this FUSE mount. FileStream
        // writes may use pwrite(), whose interaction with O_APPEND differs by OS.
        flags &= PosixOpenFlags.AccessModes | PosixOpenFlags.Create | PosixOpenFlags.CreateNew | PosixOpenFlags.Truncate;
        flags |= PosixOpenFlags.Append | PosixOpenFlags.CloseOnExec;

        var fd = open(path, flags, (uint)(PosixFileMode.OwnerReadWrite | PosixFileMode.GroupReadWrite | PosixFileMode.OthersReadWrite));

        if (fd < 0)
        {
            throw new PosixException(Marshal.GetLastWin32Error());
        }

        var handle = new SafeFileHandle((nint)fd, ownsHandle: true);

        try
        {
            // Disable managed buffering because append writes use the same
            // descriptor directly. Reads still use the normal FileStream path.
            return new FileStream(handle, flags.ToFileAccess(), bufferSize: 1, isAsync: false);
        }
        catch
        {
            handle.Dispose();
            throw;
        }
    }
}
