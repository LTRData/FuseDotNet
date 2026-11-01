using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace FuseDotNet;

/// <summary>
/// Native open flags. Named values follow the current operating system and process ABI.
/// Integer conversions preserve all bits, including flags not named by this library.
/// </summary>
/// <param name="value">Native integer flag value</param>
#if NET5_0_OR_GREATER
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("freebsd")]
#endif
[StructLayout(LayoutKind.Sequential)]
public readonly struct PosixOpenFlags(int value) : IEquatable<PosixOpenFlags>
{
    private readonly int value = value;

    public static implicit operator int(PosixOpenFlags source) => source.value;

    public static implicit operator PosixOpenFlags(int source) => new(source);

    public static bool operator ==(PosixOpenFlags left, PosixOpenFlags right) => left.value == right.value;

    public static bool operator !=(PosixOpenFlags left, PosixOpenFlags right) => left.value != right.value;

    public static bool operator ==(PosixOpenFlags left, int right) => left.value == right;

    public static bool operator !=(PosixOpenFlags left, int right) => left.value != right;

    public static bool operator ==(int left, PosixOpenFlags right) => left == right.value;

    public static bool operator !=(int left, PosixOpenFlags right) => left != right.value;

    public static PosixOpenFlags operator &(PosixOpenFlags left, PosixOpenFlags right) => new(left.value & right.value);

    public static PosixOpenFlags operator |(PosixOpenFlags left, PosixOpenFlags right) => new(left.value | right.value);

    public static PosixOpenFlags operator ^(PosixOpenFlags left, PosixOpenFlags right) => new(left.value ^ right.value);

    public static PosixOpenFlags operator ~(PosixOpenFlags source) => new(~source.value);

    public static PosixOpenFlags operator &(PosixOpenFlags left, int right) => new(left.value & right);

    public static PosixOpenFlags operator |(PosixOpenFlags left, int right) => new(left.value | right);

    public static PosixOpenFlags operator ^(PosixOpenFlags left, int right) => new(left.value ^ right);

    public static PosixOpenFlags operator &(int left, PosixOpenFlags right) => new(left & right.value);

    public static PosixOpenFlags operator |(int left, PosixOpenFlags right) => new(left | right.value);

    public static PosixOpenFlags operator ^(int left, PosixOpenFlags right) => new(left ^ right.value);

    public bool HasFlag(PosixOpenFlags flag) => (value & flag.value) == flag.value;

    public bool Equals(PosixOpenFlags other) => value == other.value;

    public bool Equals(int otherValue) => value == otherValue;

    public override bool Equals(object? obj)
        => (obj is PosixOpenFlags other && value == other.value)
        || (obj is int otherValue && value == otherValue);

    public override int GetHashCode() => value;

    public override string ToString() => $"0x{value:X8}";

    static PosixOpenFlags()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            NonBlocking = 0x00000800;
            Append = 0x00000400;
            Async = 0x00002000;
            SynchronousWrites = 0x00101000;
            Create = 0x00000040;
            Truncate = 0x00000200;
            CreateNew = 0x00000080;
            NoControllingTerminal = 0x00000100;
            CloseOnExec = 0x00080000;
            DataSync = 0x00001000;

            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X86 or Architecture.X64:
                    Direct = 0x00004000;
                    Directory = 0x00010000;
                    NoFollowLinks = 0x00020000;
                    break;

                case Architecture.Arm or Architecture.Arm64:
                    Direct = 0x00010000;
                    Directory = 0x00004000;
                    NoFollowLinks = 0x00008000;
                    break;

                default:
                    throw new PlatformNotSupportedException($"Current process architecture {RuntimeInformation.ProcessArchitecture} not supported by FuseDotNet library");
            }
        }
#if NETCOREAPP
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD) && RuntimeInformation.ProcessArchitecture == Architecture.X64)
        {
            NonBlocking = 0x00000004;
            Append = 0x00000008;
            Async = 0x00000040;
            SynchronousWrites = 0x00000080;
            NoFollowLinks = 0x00000100;
            Create = 0x00000200;
            Truncate = 0x00000400;
            CreateNew = 0x00000800;
            NoControllingTerminal = 0x00008000;
            Direct = 0x00010000;
            Directory = 0x00020000;
            CloseOnExec = 0x00100000;
            DataSync = 0x01000000;
        }
#endif
        else
        {
            throw new PlatformNotSupportedException($"Current platform {RuntimeInformation.OSDescription} {RuntimeInformation.ProcessArchitecture} not supported by FuseDotNet library");
        }
    }

    public static readonly PosixOpenFlags Read = 0;
    public static readonly PosixOpenFlags Write = 1;
    public static readonly PosixOpenFlags ReadWrite = 2;
    public static readonly PosixOpenFlags AccessModes = 3;
    public static readonly PosixOpenFlags NonBlocking;
    public static readonly PosixOpenFlags Append;
    public static readonly PosixOpenFlags Async;
    public static readonly PosixOpenFlags SynchronousWrites;
    public static readonly PosixOpenFlags NoFollowLinks;
    public static readonly PosixOpenFlags Create;
    public static readonly PosixOpenFlags Truncate;
    /// <summary>O_EXCL. Use with Create for exclusive file creation.</summary>
    public static readonly PosixOpenFlags CreateNew;
    public static readonly PosixOpenFlags NoControllingTerminal;
    public static readonly PosixOpenFlags Direct;
    public static readonly PosixOpenFlags Directory;
    public static readonly PosixOpenFlags CloseOnExec;
    /// <summary>O_DSYNC: synchronized data writes, not directory synchronization.</summary>
    public static readonly PosixOpenFlags DataSync;

    // These open flags have no Linux equivalent. Do not alias them to unrelated
    // bits or zero (HasFlag(0) is always true).
    public static PosixOpenFlags SharedLock => FreeBSDFlag(0x00000010);
    public static PosixOpenFlags ExclusiveLock => FreeBSDFlag(0x00000020);
    public static PosixOpenFlags Execute => FreeBSDFlag(0x00040000);
    public static PosixOpenFlags Search => Execute;
    public static PosixOpenFlags TtyInit => FreeBSDFlag(0x00080000);
    public static PosixOpenFlags Verify => FreeBSDFlag(0x00200000);
    public static PosixOpenFlags ResolveBeneath => FreeBSDFlag(0x00800000);

    private static PosixOpenFlags FreeBSDFlag(int value)
    {
#if NETCOREAPP
        if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            return new(value);
        }
#endif
        throw new PlatformNotSupportedException("This open flag is available only on FreeBSD");
    }
}
