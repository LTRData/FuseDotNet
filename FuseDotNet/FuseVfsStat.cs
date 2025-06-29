using System;
using System.Runtime.InteropServices;

#pragma warning disable IDE1006 // Naming Styles

namespace FuseDotNet;

public struct FuseVfsStat
{
    unsafe static FuseVfsStat()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && RuntimeInformation.OSArchitecture is Architecture.X64 or Architecture.Arm64)
        {
            pMarshalToNative = (nint pNative, in FuseVfsStat stat) => ((LinuxX64VfsStat*)pNative)->Initialize(stat);
            NativeStructSize = sizeof(LinuxX64VfsStat);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && RuntimeInformation.OSArchitecture is Architecture.X86 or Architecture.Arm)
        {
            pMarshalToNative = (nint pNative, in FuseVfsStat stat) => ((LinuxX86VfsStat*)pNative)->Initialize(stat);
            NativeStructSize = sizeof(LinuxX86VfsStat);
        }
#if NETCOREAPP
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD) && RuntimeInformation.OSArchitecture == Architecture.X64)
        {
            pMarshalToNative = (nint pNative, in FuseVfsStat stat) => ((FreeBSDX64VfsStat*)pNative)->Initialize(stat);
            NativeStructSize = sizeof(FreeBSDX64VfsStat);
        }
#endif
        else
        {
            throw new PlatformNotSupportedException($"Current platform {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture} not supported by FuseDotNet library");
        }
    }

    private unsafe delegate void fMarshalToNative(nint pNative, in FuseVfsStat stat);

    private static readonly fMarshalToNative pMarshalToNative;

    public static int NativeStructSize { get; }

    public readonly unsafe void MarshalToNative(nint pNative) => pMarshalToNative(pNative, this);

    public ulong f_bsize { get; set; }

    public ulong f_frsize { get; set; }

    public ulong f_blocks { get; set; }

    public ulong f_bfree { get; set; }

    public ulong f_bavail { get; set; }

    public ulong f_files { get; set; }

    public ulong f_ffree { get; set; }

    public ulong f_favail { get; set; }

    public ulong f_fsid { get; set; }

    public ulong f_flag { get; set; }

    public ulong f_namemax { get; set; }

    public override readonly string ToString() => $"{{ f_bsize = {f_bsize}, f_blocks = {f_blocks}, f_files = {f_files}, f_bfree = {f_bfree}, f_bavail = {f_bavail}, f_fsid = {f_fsid}, f_flag = {f_flag}, f_fsid = {f_fsid}, f_namemax = {f_namemax} }}";
}

/*
 * The difference between `avail' and `free' is that `avail' represents
 * space available to unprivileged processes, whereas `free' includes all
 * unallocated space, including that reserved for privileged processes.
 * Or at least, that's the most useful interpretation.  (According to
 * the letter of the standard, this entire interface is completely
 * unspecified!)
 */

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct FreeBSDX64VfsStat
{
    public ulong f_bavail;       /* Number of blocks */
    public ulong f_bfree;
    public ulong f_blocks;
    public ulong f_favail;       /* Number of files (e.g., inodes) */
    public ulong f_ffree;
    public ulong f_files;
    public ulong f_bsize;        /* Size of blocks counted above */
    public ulong f_flag;
    public ulong f_frsize;  /* Size of fragments */
    public ulong f_fsid;    /* Not meaningful */
    public ulong f_namemax; /* Same as pathconf(_PC_NAME_MAX) */

    public void Initialize(in FuseVfsStat stat)
    {
        f_bsize = stat.f_bsize;
        f_frsize = stat.f_frsize;
        f_blocks = stat.f_blocks;
        f_bfree = stat.f_bfree;
        f_bavail = stat.f_bavail;
        f_files = stat.f_files;
        f_ffree = stat.f_ffree;
        f_favail = stat.f_favail;
        f_fsid = stat.f_fsid;
        f_flag = stat.f_flag;
        f_namemax = stat.f_namemax;
    }

    public override readonly string ToString() => $"{{ f_bsize = {f_bsize}, f_blocks = {f_blocks}, f_files = {f_files}, f_bfree = {f_bfree}, f_bavail = {f_bavail}, f_fsid = {f_fsid}, f_flag = {f_flag}, f_fsid = {f_fsid}, f_namemax = {f_namemax} }}";

    unsafe static FreeBSDX64VfsStat()
    {
        if (sizeof(FreeBSDX64VfsStat) != 88)
        {
            throw new PlatformNotSupportedException($"Invalid size {sizeof(FreeBSDX64VfsStat)} of statvfs structure, expected 88 (wrong structure pack settings?)");
        }
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public unsafe struct LinuxX64VfsStat
{
    public ulong f_bsize;   /* Size of data blocks on the filesystem. */
    public ulong f_frsize;  /* Preferred size of data fragments on the filesystem. This does not necessarily need to match f_bsize. */
    public ulong f_blocks;  /* Total count of data blocks on the filesystem. */
    public ulong f_bfree;   /* Free data blocks on the filesystem for all users. */
    public ulong f_bavail;  /* Free data blocks on the filesystem for non-privledged users. */

    public ulong f_files;  /* Total count of inodes on the filesystem. */
    public ulong f_ffree;  /* Free inodes on the filesystem for all users. */
    public ulong f_favail; /* Free inodes on the filesystem for non-privledged users. */

    public ulong f_fsid;    /* Filesystem ID. Optional, but useful for identification. */
    public ulong f_flag;    /* Mounting flags. */
    public ulong f_namemax; /* Max length in bytes of inode names. */

    public fixed int f_spare[6];

    public void Initialize(in FuseVfsStat stat)
    {
        f_bsize = stat.f_bsize;
        f_frsize = stat.f_frsize;
        f_blocks = stat.f_blocks;
        f_bfree = stat.f_bfree;
        f_bavail = stat.f_bavail;
        f_files = stat.f_files;
        f_ffree = stat.f_ffree;
        f_favail = stat.f_favail;
        f_fsid = stat.f_fsid;
        f_flag = stat.f_flag;
        f_namemax = stat.f_namemax;
    }

    public override readonly string ToString() => $"{{ f_bsize = {f_bsize}, f_blocks = {f_blocks}, f_files = {f_files}, f_bfree = {f_bfree}, f_bavail = {f_bavail}, f_fsid = {f_fsid}, f_flag = {f_flag}, f_fsid = {f_fsid}, f_namemax = {f_namemax} }}";

    unsafe static LinuxX64VfsStat()
    {
        if (sizeof(LinuxX64VfsStat) != 112)
        {
            throw new PlatformNotSupportedException($"Invalid size {sizeof(LinuxX64VfsStat)} of statvfs structure, expected 112 (wrong structure pack settings?)");
        }
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public unsafe struct LinuxX86VfsStat
{
    public uint f_bsize;

    public uint f_frsize;

    public ulong f_blocks;

    public ulong f_bfree;

    public ulong f_bavail;

    public ulong f_files;

    public ulong f_ffree;

    public ulong f_favail;

    public uint f_fsid;

    public int f_unused;

    public uint f_flag;

    public uint f_namemax;

    public unsafe fixed int f_spare[6];

    public void Initialize(in FuseVfsStat stat)
    {
        checked
        {
            f_bsize = (uint)stat.f_bsize;
            f_frsize = (uint)stat.f_frsize;
            f_blocks = stat.f_blocks;
            f_bfree = stat.f_bfree;
            f_bavail = stat.f_bavail;
            f_files = stat.f_files;
            f_ffree = stat.f_ffree;
            f_favail = stat.f_favail;
            f_fsid = (uint)stat.f_fsid;
            f_flag = (uint)stat.f_flag;
            f_namemax = (uint)stat.f_namemax;
        }
    }

    public override readonly string ToString() => $"{{ f_bsize = {f_bsize}, f_blocks = {f_blocks}, f_files = {f_files}, f_bfree = {f_bfree}, f_bavail = {f_bavail}, f_fsid = {f_fsid}, f_flag = {f_flag}, f_fsid = {f_fsid}, f_namemax = {f_namemax} }}";

    unsafe static LinuxX86VfsStat()
    {
        if (sizeof(LinuxX86VfsStat) != 96)
        {
            throw new PlatformNotSupportedException($"Invalid size {sizeof(LinuxX86VfsStat)} of statvfs structure, expected 96 (wrong structure pack settings?)");
        }
    }
}

