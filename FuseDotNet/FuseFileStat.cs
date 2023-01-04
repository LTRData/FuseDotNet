using FuseDotNet.Native;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#pragma warning disable IDE1006 // Naming Styles

namespace FuseDotNet;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public unsafe struct FuseFileStat
{
    public override string ToString() => $"{{ st_ino = {st_ino}, st_mode = [{st_mode}], st_size = {st_size} }}";

    static FuseFileStat()
    {
        if (sizeof(FuseFileStat) != 224)
        {
            throw new PlatformNotSupportedException($"Invalid size of stat structure (running on 32 bit platform?)");
        }
    }

    public long st_dev;               /* inode's device */
    public long st_ino;               /* inode's number */
    public long st_nlink;             /* number of hard links */
    public PosixFileMode st_mode;      /* inode protection mode */
    private readonly short st_padding0;
    public uint st_uid;                /* user ID of the file's owner */
    public uint st_gid;                /* group ID of the file's group */
    private readonly int st_padding1;
    public long st_rdev;              /* device type */
    public TimeSpec st_atim;           /* time of last access */
    public TimeSpec st_mtim;           /* time of last data modification */
    public TimeSpec st_ctim;           /* time of last file status change */
    public TimeSpec st_birthtim;       /* time of file creation */
    public long st_size;               /* file size, in bytes */
    public long st_blocks;             /* blocks allocated for file */
    public int st_blksize;             /* optimal blocksize for I/O */
    public uint st_flags;              /* user defined flags for file */
    public long st_gen;               /* file generation number */
    public fixed long st_spare[10];
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly struct TimeSpec : IEquatable<TimeSpec>, IComparable<TimeSpec>
{
    public readonly IntPtr tv_sec;       /* seconds */
    public readonly long tv_nsec;        /* and nanoseconds */

    public bool IsOmit => tv_nsec == -2;

    public bool IsPseudoNow => tv_nsec == -1;

    public long total_msec => tv_sec.ToInt64() * 1000 + tv_nsec / 1000000;

    public TimeSpec(DateTimeOffset dateTime)
        : this(dateTime.ToUnixTimeMilliseconds())
    {
    }

    public TimeSpec(long msec)
    {
        tv_sec = new(msec / 1000);
        tv_nsec = msec % 1000 * 1000000;
    }

    public static TimeSpec Now(out TimeSpec timespec) => NativeMethods.time(out timespec);

    public static TimeSpec Now() => NativeMethods.time(out _);

    public DateTimeOffset ToDateTime() => IsPseudoNow ? DateTimeOffset.UtcNow : DateTimeOffset.FromUnixTimeMilliseconds(total_msec);

    public override string ToString() => ToDateTime().ToString();

#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP
    public override int GetHashCode() => HashCode.Combine(tv_sec, tv_nsec);
#else
    public override int GetHashCode() => new { tv_sec, tv_nsec }.GetHashCode();
#endif

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

