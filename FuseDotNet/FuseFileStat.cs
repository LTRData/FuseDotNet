using FuseDotNet.Native;
using System;
using System.Runtime.InteropServices;

#pragma warning disable IDE1006 // Naming Styles

namespace FuseDotNet;

public struct FuseFileStat
{
    unsafe static FuseFileStat()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && RuntimeInformation.OSArchitecture == Architecture.X64)
        {
            pMarshalToNative = (nint pNative, in FuseFileStat stat) => ((LinuxX64FileStat*)pNative)->Initialize(stat);
            NativeStructSize = sizeof(LinuxX64FileStat);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && RuntimeInformation.OSArchitecture == Architecture.Arm64)
        {
            pMarshalToNative = (nint pNative, in FuseFileStat stat) => ((LinuxArm64FileStat*)pNative)->Initialize(stat);
            NativeStructSize = sizeof(LinuxArm64FileStat);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && RuntimeInformation.OSArchitecture == Architecture.X86)
        {
            pMarshalToNative = (nint pNative, in FuseFileStat stat) => ((LinuxX86FileStat*)pNative)->Initialize(stat);
            NativeStructSize = sizeof(LinuxX86FileStat);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && RuntimeInformation.OSArchitecture == Architecture.Arm)
        {
            pMarshalToNative = (nint pNative, in FuseFileStat stat) => ((LinuxArm32FileStat*)pNative)->Initialize(stat);
            NativeStructSize = sizeof(LinuxArm32FileStat);
        }
#if NETCOREAPP
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD) && RuntimeInformation.OSArchitecture == Architecture.X64)
        {
            pMarshalToNative = (nint pNative, in FuseFileStat stat) => ((FreeBSDX64FileStat*)pNative)->Initialize(stat);
            NativeStructSize = sizeof(FreeBSDX64FileStat);
        }
#endif
        else
        {
            throw new PlatformNotSupportedException($"Current platform {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture} not supported by FuseDotNet library");
        }
    }

    private unsafe delegate void fMarshalToNative(nint pNative, in FuseFileStat stat);

    private static readonly fMarshalToNative pMarshalToNative;

    public static int NativeStructSize { get; }

    public readonly unsafe void MarshalToNative(nint pNative) => pMarshalToNative(pNative, this);

    public long st_size { get; set; }
    public long st_nlink { get; set; }
    public PosixFileMode st_mode { get; set; }
    public long st_gen { get; set; }
    public TimeSpec st_birthtim { get; set; }
    public TimeSpec st_atim { get; set; }
    public TimeSpec st_ctim { get; set; }
    public TimeSpec st_mtim { get; set; }
    public long st_ino { get; set; }
    public long st_dev { get; set; }
    public long st_rdev { get; set; }
    public uint st_uid { get; set; }
    public uint st_gid { get; set; }
    public int st_blksize { get; set; }
    public long st_blocks { get; set; }

    public override readonly string ToString() => $"{{ Size = {st_size}, Mode = {st_mode}, Inode = {st_ino}, Uid = {st_uid}, Gid = {st_gid} }}";
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct FreeBSDX64FileStat
{
    public void Initialize(in FuseFileStat stat)
    {
        st_dev = stat.st_dev;
        st_ino = stat.st_ino;
        st_nlink = stat.st_nlink;
        st_mode = stat.st_mode;
        st_uid = stat.st_uid;
        st_gid = stat.st_gid;
        st_rdev = stat.st_rdev;
        st_atim = stat.st_atim;
        st_mtim = stat.st_mtim;
        st_ctim = stat.st_ctim;
        st_birthtim = stat.st_birthtim;
        st_size = stat.st_size;
        st_blocks = stat.st_blocks;
        st_blksize = stat.st_blksize;
        st_gen = stat.st_gen;
    }

    public override readonly string ToString() => $"{{ st_ino = {st_ino}, st_mode = [{st_mode}], st_size = {st_size} }}";

    unsafe static FreeBSDX64FileStat()
    {
        if (sizeof(FreeBSDX64FileStat) != 224)
        {
            throw new PlatformNotSupportedException($"Invalid size {sizeof(FreeBSDX64FileStat)} of stat structure, expected 224 (wrong structure pack settings?)");
        }
    }

    public long st_dev { get; set; }            /* inode's device */
    public long st_ino { get; set; }            /* inode's number */
    public long st_nlink { get; set; }          /* number of hard links */
    public PosixFileMode st_mode { get; set; }  /* inode protection mode */
    private readonly short st_padding0;
    public uint st_uid { get; set; }            /* user ID of the file's owner */
    public uint st_gid { get; set; }            /* group ID of the file's group */
    private readonly int st_padding1;
    public long st_rdev { get; set; }           /* device type */
    public TimeSpec st_atim { get; set; }       /* time of last access */
    public TimeSpec st_mtim { get; set; }       /* time of last data modification */
    public TimeSpec st_ctim { get; set; }       /* time of last file status change */
    public TimeSpec st_birthtim { get; set; }   /* time of file creation */
    public long st_size { get; set; }           /* file size, in bytes */
    public long st_blocks { get; set; }         /* blocks allocated for file */
    public int st_blksize { get; set; }         /* optimal blocksize for I/O */
    public uint st_flags;                       /* user defined flags for file */
    public long st_gen { get; set; }            /* file generation number */
    private unsafe fixed long st_spare[10];
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct LinuxX64FileStat
{
    public void Initialize(in FuseFileStat stat)
    {
        st_dev = stat.st_dev;
        st_ino = stat.st_ino;
        st_nlink = stat.st_nlink;
        st_mode = stat.st_mode;
        st_uid = stat.st_uid;
        st_gid = stat.st_gid;
        st_rdev = stat.st_rdev;
        st_atim = stat.st_atim;
        st_mtim = stat.st_mtim;
        st_ctim = stat.st_ctim;
        st_size = stat.st_size;
        st_blocks = stat.st_blocks;
        st_blksize = stat.st_blksize;
    }

    public override readonly string ToString() => $"{{ st_ino = {st_ino}, st_mode = [{st_mode}], st_size = {st_size} }}";

    unsafe static LinuxX64FileStat()
    {
        if (sizeof(LinuxX64FileStat) != 144)
        {
            throw new PlatformNotSupportedException($"Invalid size {sizeof(LinuxX64FileStat)} of stat structure, expected 144 (wrong structure pack settings?)");
        }
    }

    public long st_dev;

    public long st_ino;

    public long st_nlink;

    public PosixFileMode st_mode;

    public uint st_uid;

    public uint st_gid;

    private readonly int pad0;

    public long st_rdev;

    public long st_size;

    public int st_blksize;

    public long st_blocks;

    public TimeSpec st_atim;

    public TimeSpec st_mtim;

    public TimeSpec st_ctim;

    private unsafe fixed long __glibc_reserved[3];
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct LinuxArm64FileStat
{
    public void Initialize(in FuseFileStat stat)
    {
        checked
        {
            st_dev = stat.st_dev;
            st_ino = stat.st_ino;
            st_nlink = (uint)stat.st_nlink;
            st_mode = stat.st_mode;
            st_uid = stat.st_uid;
            st_gid = stat.st_gid;
            st_rdev = stat.st_rdev;
            st_atim = stat.st_atim;
            st_mtim = stat.st_mtim;
            st_ctim = stat.st_ctim;
            st_size = stat.st_size;
            st_blocks = stat.st_blocks;
            st_blksize = stat.st_blksize;
        }
    }

    public override readonly string ToString() => $"{{ st_ino = {st_ino}, st_mode = [{st_mode}], st_size = {st_size} }}";

    unsafe static LinuxArm64FileStat()
    {
        if (sizeof(LinuxArm64FileStat) != 128)
        {
            throw new PlatformNotSupportedException($"Invalid size {sizeof(LinuxArm64FileStat)} of stat structure, expected 128 (wrong structure pack settings?)");
        }
    }

    public long st_dev;

    public long st_ino;

    public PosixFileMode st_mode;

    public uint st_nlink;

    public uint st_uid;

    public uint st_gid;

    public long st_rdev;

    private readonly long pad1;

    public long st_size;

    public int st_blksize;

    private readonly int pad2;

    public long st_blocks;

    public TimeSpec st_atim;

    public TimeSpec st_mtim;

    public TimeSpec st_ctim;

    private unsafe fixed int __glibc_reserved[2];
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct LinuxX86FileStat
{
    public void Initialize(in FuseFileStat stat)
    {
        checked
        {
            st_dev = stat.st_dev;
            st_ino = stat.st_ino;
            st_ino = stat.st_ino;
            st_nlink = (int)stat.st_nlink;
            st_mode = stat.st_mode;
            st_uid = stat.st_uid;
            st_gid = stat.st_gid;
            st_rdev = stat.st_rdev;
            st_atim = stat.st_atim;
            st_mtim = stat.st_mtim;
            st_ctim = stat.st_ctim;
            st_size = stat.st_size;
            st_blocks = stat.st_blocks;
            st_blksize = stat.st_blksize;
        }
    }

    public override readonly string ToString() => $"{{ st_ino = {st_ino}, st_mode = [{st_mode}], st_size = {st_size} }}";

    unsafe static LinuxX86FileStat()
    {
        if (sizeof(LinuxX86FileStat) != 96)
        {
            throw new PlatformNotSupportedException($"Invalid size {sizeof(LinuxX86FileStat)} of stat structure, expected 96 (wrong structure pack settings?)");
        }
    }

    public long st_dev;

    private readonly long pad1;

    public PosixFileMode st_mode;

    public int st_nlink;

    public uint st_uid;

    public uint st_gid;

    public long st_rdev;

    private readonly int pad2;

    public long st_size;

    public int st_blksize;

    public long st_blocks;

    public TimeSpec st_atim;

    public TimeSpec st_mtim;

    public TimeSpec st_ctim;

    public long st_ino;
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct LinuxArm32FileStat
{
    public void Initialize(in FuseFileStat stat)
    {
        checked
        {
            st_dev = stat.st_dev;
            st_ino = stat.st_ino;
            st_nlink = (int)stat.st_nlink;
            st_mode = stat.st_mode;
            st_uid = stat.st_uid;
            st_gid = stat.st_gid;
            st_rdev = stat.st_rdev;
            st_atim = stat.st_atim;
            st_mtim = stat.st_mtim;
            st_ctim = stat.st_ctim;
            st_size = (int)stat.st_size;
            st_blocks = (int)stat.st_blocks;
            st_blksize = stat.st_blksize;
        }
    }

    public override readonly string ToString() => $"{{ st_ino = {st_ino}, st_mode = [{st_mode}], st_size = {st_size} }}";

    unsafe static LinuxArm32FileStat()
    {
        if (sizeof(LinuxArm32FileStat) != 104)
        {
            throw new PlatformNotSupportedException($"Invalid size {sizeof(LinuxArm32FileStat)} of stat structure, expected 104 (wrong structure pack settings?)");
        }
    }

    public long st_dev;

    private readonly long pad1;

    public PosixFileMode st_mode;

    private readonly short pad2;

    public int st_nlink;

    public uint st_uid;

    public uint st_gid;

    public long st_rdev;

    private readonly long pad3;

    public long st_size;

    public int st_blksize;

    private readonly int pad4;

    public long st_blocks;

    public TimeSpec st_atim;

    public TimeSpec st_mtim;

    public TimeSpec st_ctim;

    public long st_ino;
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct TimeSpec(long msec) : IEquatable<TimeSpec>, IComparable<TimeSpec>
{
    public readonly nint tv_sec = (nint)(msec / 1000);       /* seconds */
    public readonly nint tv_nsec = (nint)(msec % 1000 * 1000000);        /* and nanoseconds */

    public bool IsOmit => tv_nsec == -2;

    public bool IsPseudoNow => tv_nsec == -1;

    public long total_msec => (long)tv_sec * 1000 + tv_nsec / 1000000;

    public TimeSpec(DateTimeOffset dateTime)
        : this(dateTime.ToUnixTimeMilliseconds())
    {
    }

    public static TimeSpec Now(out TimeSpec timespec) => NativeMethods.time(out timespec);

    public static TimeSpec Now() => NativeMethods.time(out _);

    public DateTimeOffset ToDateTime() => IsPseudoNow ? DateTimeOffset.UtcNow : DateTimeOffset.FromUnixTimeMilliseconds(total_msec);

    public override string ToString() => ToDateTime().ToString();

    public override int GetHashCode() => HashCode.Combine(tv_sec, tv_nsec);

    public int CompareTo(TimeSpec other) => total_msec.CompareTo(other.total_msec);

    public bool Equals(TimeSpec other) => other.tv_sec == tv_sec && other.tv_nsec == tv_nsec;

    public override bool Equals(object? obj) => obj is TimeSpec other && Equals(other);

    public static bool operator ==(TimeSpec left, TimeSpec right) => left.Equals(right);

    public static bool operator !=(TimeSpec left, TimeSpec right) => !(left == right);

    public static bool operator <(TimeSpec left, TimeSpec right) => left.CompareTo(right) < 0;

    public static bool operator <=(TimeSpec left, TimeSpec right) => left.CompareTo(right) <= 0;

    public static bool operator >(TimeSpec left, TimeSpec right) => left.CompareTo(right) > 0;

    public static bool operator >=(TimeSpec left, TimeSpec right) => left.CompareTo(right) >= 0;

    public static implicit operator TimeSpec(DateTimeOffset dateTime) => new(dateTime);

    public static implicit operator TimeSpec(DateTime dateTime) => new(dateTime);
}

