using System;
using System.IO;
using System.Runtime.InteropServices;

#pragma warning disable 649,169

namespace FuseDotNet;

[Flags]
public enum FuseFileInfoOptions : long
{
    none = 0x0,

    /** In case of a write operation indicates if this was caused by a
        writepage */
    write_page = 0x1,

    /** Can be filled in by open, to use direct I/O on this file.
           Introduced in version 2.4 */
    direct_io = 0x2,

    /** Can be filled in by open, to indicate, that cached file data
        need not be invalidated.  Introduced in version 2.4 */
    keep_cache = 0x4,

    /** Indicates a flush operation.  Set in flush operation, also
        maybe set in highlevel lock operation and lowlevel release
        operation.  Introduced in version 2.6 */
    flush = 0x8,

    /** Can be filled in by open, to indicate that the file is not
    seekable.  Introduced in version 2.8 */
    nonseekable = 0x10,

    /** Indicates that flock locks for this file should be
       released.  If set, lock_owner shall contain a valid value.
       May only be set in ->release().  Introduced in version
       2.9 */
    flock_release = 0x20,

    /** Can be filled in by opendir. It signals the kernel to
        enable caching of entries returned by readdir().  Has no
        effect when set in other contexts (in particular it does
        nothing when set by open()). */
    cache_readdir = 0x40,

    /** Can be filled in by open, to indicate that flush is not needed
	    on close. */
    noflush = 0x80,
}

/// <summary>
/// %Fuse file information on the current operation.
/// </summary>
/// <remarks>
/// This class cannot be instantiated in C#, it is created by the kernel %Fuse driver.
/// This is the same structure as <c>fileInfo</c> (fuse_common.h) in the C version of Fuse.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public struct FuseFileInfo
{
    static unsafe FuseFileInfo()
    {
        if (sizeof(FuseFileInfo) != 40)
        {
            throw new PlatformNotSupportedException($"Invalid packing of structure FuseFileInfo. Should be 40 bytes, is {sizeof(FuseFileInfo)} bytes");
        }
    }

    /** Open flags.  Available in open() and release() */
    public readonly PosixOpenFlags flags;

    public FuseFileInfoOptions options;

    /** File handle.  May be filled in by filesystem in open().
        Available in all other file operations */
    private long fh;

    /** Lock owner id.  Available in locking operations and flush */
    public ulong lock_owner;

    /** Requested poll events.  Available in ->poll.  Only set on kernels
    which support it.  If unsupported, this field is set to zero. */
    public readonly uint poll_events;

    /// <summary>
    /// Gets or sets context that can be used to carry information between operation.
    /// The Context can carry whatever type like <c><see cref="FileStream"/></c>, <c>struct</c>, <c>int</c>,
    /// or internal reference that will help the implementation understand the request context of the event.
    /// </summary>
    public object? Context
    {
        get
        {
            if (fh != 0)
            {
                var gch = GCHandle.FromIntPtr(new(fh));
                if (gch.IsAllocated)
                {
                    return gch.Target;
                }
            }

            return null;
        }

        set
        {
            if (fh != 0)
            {
                ((GCHandle)(nint)fh).Free();
                fh = 0;
            }

            if (value != null)
            {
                fh = (nint)GCHandle.Alloc(value);
            }
        }
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
        => FormatProviders.FuseFormat($"Context = {{{Context}, Options = {options}, Flags = {flags}, FileHandle = 0x{fh:X}}}");
}

