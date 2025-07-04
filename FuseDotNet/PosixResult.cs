using FuseDotNet.Native;
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace FuseDotNet;

/// <summary>
/// Posix-style error codes as a semantic type. Some fields are the same across platforms
/// and others have different values depending on platform.
/// </summary>
/// <param name="value">Integer value of error code</param>
public readonly struct PosixResult(int value) : IEquatable<PosixResult>
{
    private readonly int value = value;

    public static implicit operator int(PosixResult source) => source.value;

    public static implicit operator PosixResult(int source) => new(source);

    public static bool operator ==(PosixResult left, PosixResult right) => left.value == right.value;

    public static bool operator !=(PosixResult left, PosixResult right) => left.value != right.value;

    public static bool operator ==(PosixResult left, int right) => left.value == right;

    public static bool operator !=(PosixResult left, int right) => left.value != right;

    public static bool operator ==(int left, PosixResult right) => left == right.value;

    public static bool operator !=(int left, PosixResult right) => left != right.value;

    public static int operator -(PosixResult source) => -source.value;

    public override bool Equals(object? obj)
        => (obj is PosixResult other && value == other.value)
        || (obj is int otherValue && value == otherValue);

    public bool Equals(PosixResult other) => value == other.value;

    public bool Equals(int otherValue) => value == otherValue;

    public override int GetHashCode() => value;

    private static readonly ConcurrentDictionary<int, string> systemMessages = new();

    public override string ToString() => systemMessages.GetOrAdd(value, value => $"{NativeMethods.strerror(value)} ({value})");

    static PosixResult()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            ENOTSUP = LinuxResult.ENOTSUP;
            ENOSYS = LinuxResult.ENOSYS;
            ENAMETOOLONG = LinuxResult.ENAMETOOLONG;
        }
#if NETCOREAPP
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            ENOTSUP = FreeBSDResult.ENOTSUP;
            ENOSYS = FreeBSDResult.ENOSYS;
            ENAMETOOLONG = FreeBSDResult.ENAMETOOLONG;
        }
#endif
        else
        {
            throw new PlatformNotSupportedException($"Current platform {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture} not supported by FuseDotNet library");
        }
    }

    public static readonly PosixResult Success = 0;

    public static readonly PosixResult EPERM = 1;
    public static readonly PosixResult ENOENT = 2;
    public static readonly PosixResult ESRCH = 3;
    public static readonly PosixResult EINTR = 4;
    public static readonly PosixResult EIO = 5;
    public static readonly PosixResult ENXIO = 6;
    public static readonly PosixResult E2BIG = 7;
    public static readonly PosixResult ENOEXEC = 8;
    public static readonly PosixResult EBADF = 9;
    public static readonly PosixResult ECHILD = 10;
    public static readonly PosixResult ENOMEM = 12;
    public static readonly PosixResult EACCES = 13;
    public static readonly PosixResult EFAULT = 14;
    public static readonly PosixResult ENOTBLK = 15;
    public static readonly PosixResult EBUSY = 16;
    public static readonly PosixResult EEXIST = 17;
    public static readonly PosixResult EXDEV = 18;
    public static readonly PosixResult ENODEV = 19;
    public static readonly PosixResult ENOTDIR = 20;
    public static readonly PosixResult EISDIR = 21;
    public static readonly PosixResult EINVAL = 22;
    public static readonly PosixResult ENFILE = 23;
    public static readonly PosixResult EMFILE = 24;
    public static readonly PosixResult ENOTTY = 25;
    public static readonly PosixResult ETXTBSY = 26;
    public static readonly PosixResult EFBIG = 27;
    public static readonly PosixResult ENOSPC = 28;
    public static readonly PosixResult ESPIPE = 29;
    public static readonly PosixResult EROFS = 30;
    public static readonly PosixResult EMLINK = 31;
    public static readonly PosixResult EPIPE = 32;
    public static readonly PosixResult EDOM = 33;
    public static readonly PosixResult ERANGE = 34;

    public static readonly PosixResult ENOTSUP;
    public static readonly PosixResult ENOSYS;
    public static readonly PosixResult ENAMETOOLONG;
}

/// <summary>
/// Posix errno values.
/// </summary>
public static class FreeBSDResult
{
    public static readonly PosixResult Success = 0;   /* No error */

    public static readonly PosixResult EPERM = 1;   /* Operation not permitted */
    public static readonly PosixResult ENOENT = 2;   /* No such file or directory */
    public static readonly PosixResult ESRCH = 3;   /* No such process */
    public static readonly PosixResult EINTR = 4;   /* Interrupted system call */
    public static readonly PosixResult EIO = 5;   /* Input/output error */
    public static readonly PosixResult ENXIO = 6;   /* Device not configured */
    public static readonly PosixResult E2BIG = 7;   /* Argument list too long */
    public static readonly PosixResult ENOEXEC = 8;   /* Exec format error */
    public static readonly PosixResult EBADF = 9;   /* Bad file descriptor */
    public static readonly PosixResult ECHILD = 10;   /* No child processes */
    public static readonly PosixResult EDEADLK = 11;   /* Resource deadlock avoided */
    public static readonly PosixResult ENOMEM = 12;   /* Cannot allocate memory */
    public static readonly PosixResult EACCES = 13;   /* Permission denied */
    public static readonly PosixResult EFAULT = 14;   /* Bad address */
    public static readonly PosixResult ENOTBLK = 15;   /* Block device required */
    public static readonly PosixResult EBUSY = 16;   /* Device busy */
    public static readonly PosixResult EEXIST = 17;   /* File exists */
    public static readonly PosixResult EXDEV = 18;   /* Cross-device link */
    public static readonly PosixResult ENODEV = 19;   /* Operation not supported by device */
    public static readonly PosixResult ENOTDIR = 20;   /* Not a directory */
    public static readonly PosixResult EISDIR = 21;   /* Is a directory */
    public static readonly PosixResult EINVAL = 22;   /* Invalid argument */
    public static readonly PosixResult ENFILE = 23;   /* Too many open files in system */
    public static readonly PosixResult EMFILE = 24;   /* Too many open files */
    public static readonly PosixResult ENOTTY = 25;   /* Inappropriate ioctl for device */
    public static readonly PosixResult ETXTBSY = 26;   /* Text file busy */
    public static readonly PosixResult EFBIG = 27;   /* File too large */
    public static readonly PosixResult ENOSPC = 28;   /* No space left on device */
    public static readonly PosixResult ESPIPE = 29;   /* Illegal seek */
    public static readonly PosixResult EROFS = 30;   /* Read-only filesystem */
    public static readonly PosixResult EMLINK = 31;   /* Too many links */
    public static readonly PosixResult EPIPE = 32;   /* Broken pipe */

    /* math software */
    public static readonly PosixResult EDOM = 33;   /* Numerical argument out of domain */
    public static readonly PosixResult ERANGE = 34;   /* Result too large */

    /* non-blocking and interrupt i/o */
    public static readonly PosixResult EAGAIN = 35;   /* Resource temporarily unavailable */
    public static readonly PosixResult EINPROGRESS = 36;   /* Operation now in progress */

    public static readonly PosixResult EALREADY = 37;   /* Operation already in progress */

    /* ipc/network software -- argument errors */
    public static readonly PosixResult ENOTSOCK = 38;   /* Socket operation on non-socket */
    public static readonly PosixResult EDESTADDRREQ = 39;   /* Destination address required */
    public static readonly PosixResult EMSGSIZE = 40;   /* Message too long */
    public static readonly PosixResult EPROTOTYPE = 41;   /* Protocol wrong type for socket */
    public static readonly PosixResult ENOPROTOOPT = 42;   /* Protocol not available */
    public static readonly PosixResult EPROTONOSUPPORT = 43;   /* Protocol not supported */
    public static readonly PosixResult ESOCKTNOSUPPORT = 44;   /* Socket type not supported */
    public static readonly PosixResult EOPNOTSUPP = 45;   /* Operation not supported */
    public static readonly PosixResult ENOTSUP = EOPNOTSUPP; /* Operation not supported */
    public static readonly PosixResult EPFNOSUPPORT = 46;   /* Protocol family not supported */
    public static readonly PosixResult EAFNOSUPPORT = 47;   /* Address family not supported by protocol family */
    public static readonly PosixResult EADDRINUSE = 48;   /* Address already in use */
    public static readonly PosixResult EADDRNOTAVAIL = 49;   /* Can't assign requested address */

    /* ipc/network software -- operational errors */
    public static readonly PosixResult ENETDOWN = 50;   /* Network is down */
    public static readonly PosixResult ENETUNREACH = 51;   /* Network is unreachable */
    public static readonly PosixResult ENETRESET = 52;   /* Network dropped connection on reset */
    public static readonly PosixResult ECONNABORTED = 53;   /* Software caused connection abort */
    public static readonly PosixResult ECONNRESET = 54;   /* Connection reset by peer */
    public static readonly PosixResult ENOBUFS = 55;   /* No buffer space available */
    public static readonly PosixResult EISCONN = 56;   /* Socket is already connected */
    public static readonly PosixResult ENOTCONN = 57;   /* Socket is not connected */
    public static readonly PosixResult ESHUTDOWN = 58;   /* Can't send after socket shutdown */
    public static readonly PosixResult ETOOMANYREFS = 59;   /* Too many references: can't splice */
    public static readonly PosixResult ETIMEDOUT = 60;   /* Operation timed out */
    public static readonly PosixResult ECONNREFUSED = 61;   /* Connection refused */

    public static readonly PosixResult ELOOP = 62;   /* Too many levels of symbolic links */
    public static readonly PosixResult ENAMETOOLONG = 63;   /* File name too long */

    /* should be rearranged */
    public static readonly PosixResult EHOSTDOWN = 64;   /* Host is down */
    public static readonly PosixResult EHOSTUNREACH = 65;   /* No route to host */
    public static readonly PosixResult ENOTEMPTY = 66;   /* Directory not empty */

    /* quotas & mush */
    public static readonly PosixResult EPROCLIM = 67;   /* Too many processes */
    public static readonly PosixResult EUSERS = 68;   /* Too many users */
    public static readonly PosixResult EDQUOT = 69;   /* Disc quota exceeded */

    /* Network File System */
    public static readonly PosixResult ESTALE = 70;   /* Stale NFS file handle */
    public static readonly PosixResult EREMOTE = 71;   /* Too many levels of remote in path */
    public static readonly PosixResult EBADRPC = 72;   /* RPC struct is bad */
    public static readonly PosixResult ERPCMISMATCH = 73;   /* RPC version wrong */
    public static readonly PosixResult EPROGUNAVAIL = 74;   /* RPC prog. not avail */
    public static readonly PosixResult EPROGMISMATCH = 75;   /* Program version wrong */
    public static readonly PosixResult EPROCUNAVAIL = 76;   /* Bad procedure for program */

    public static readonly PosixResult ENOLCK = 77;   /* No locks available */
    public static readonly PosixResult ENOSYS = 78;   /* Function not implemented */

    public static readonly PosixResult EFTYPE = 79;   /* Inappropriate file type or format */
    public static readonly PosixResult EAUTH = 80;   /* Authentication error */
    public static readonly PosixResult ENEEDAUTH = 81;   /* Need authenticator */
    public static readonly PosixResult EIDRM = 82;   /* Identifier removed */
    public static readonly PosixResult ENOMSG = 83;   /* No message of desired type */
    public static readonly PosixResult EOVERFLOW = 84;   /* Value too large to be stored in data type */
    public static readonly PosixResult ECANCELED = 85;   /* Operation canceled */
    public static readonly PosixResult EILSEQ = 86;   /* Illegal byte sequence */
    public static readonly PosixResult ENOATTR = 87;   /* Attribute not found */

    public static readonly PosixResult EDOOFUS = 88;   /* Programming error */

    public static readonly PosixResult EBADMSG = 89;   /* Bad message */
    public static readonly PosixResult EMULTIHOP = 90;   /* Multihop attempted */
    public static readonly PosixResult ENOLINK = 91;   /* Link has been severed */
    public static readonly PosixResult EPROTO = 92;   /* Protocol error */

    public static readonly PosixResult ENOTCAPABLE = 93;   /* Capabilities insufficient */
    public static readonly PosixResult ECAPMODE = 94;   /* Not permitted in capability mode */
    public static readonly PosixResult ENOTRECOVERABLE = 95;   /* State not recoverable */
    public static readonly PosixResult EOWNERDEAD = 96;   /* Previous owner died */
    public static readonly PosixResult EINTEGRITY = 97;   /* Integrity check failed */
}

public static class LinuxResult
{
    public static readonly PosixResult Success = 0;   /* No error */

    public static readonly PosixResult EPERM = 1;  /* Operation not permitted */
    public static readonly PosixResult ENOENT = 2; /* No such file or directory */
    public static readonly PosixResult ESRCH = 3;  /* No such process */
    public static readonly PosixResult EINTR = 4;  /* Interrupted system call */
    public static readonly PosixResult EIO = 5;    /* I/O error */
    public static readonly PosixResult ENXIO = 6;  /* No such device or address */
    public static readonly PosixResult E2BIG = 7;  /* Argument list too long */
    public static readonly PosixResult ENOEXEC = 8;    /* Exec format error */
    public static readonly PosixResult EBADF = 9;  /* Bad file number */
    public static readonly PosixResult ECHILD = 10;    /* No child processes */
    public static readonly PosixResult EAGAIN = 11;    /* Try again */
    public static readonly PosixResult ENOMEM = 12;    /* Out of memory */
    public static readonly PosixResult EACCES = 13;    /* Permission denied */
    public static readonly PosixResult EFAULT = 14;    /* Bad address */
    public static readonly PosixResult ENOTBLK = 15;   /* Block device required */
    public static readonly PosixResult EBUSY = 16; /* Device or resource busy */
    public static readonly PosixResult EEXIST = 17;    /* File exists */
    public static readonly PosixResult EXDEV = 18; /* Cross-device link */
    public static readonly PosixResult ENODEV = 19;    /* No such device */
    public static readonly PosixResult ENOTDIR = 20;   /* Not a directory */
    public static readonly PosixResult EISDIR = 21;    /* Is a directory */
    public static readonly PosixResult EINVAL = 22;    /* Invalid argument */
    public static readonly PosixResult ENFILE = 23;    /* File table overflow */
    public static readonly PosixResult EMFILE = 24;    /* Too many open files */
    public static readonly PosixResult ENOTTY = 25;    /* Not a typewriter */
    public static readonly PosixResult ETXTBSY = 26;   /* Text file busy */
    public static readonly PosixResult EFBIG = 27; /* File too large */
    public static readonly PosixResult ENOSPC = 28;    /* No space left on device */
    public static readonly PosixResult ESPIPE = 29;    /* Illegal seek */
    public static readonly PosixResult EROFS = 30; /* Read-only file system */
    public static readonly PosixResult EMLINK = 31;    /* Too many links */
    public static readonly PosixResult EPIPE = 32; /* Broken pipe */
    public static readonly PosixResult EDOM = 33;  /* Math argument out of domain of func */
    public static readonly PosixResult ERANGE = 34;    /* Math result not representable */
    public static readonly PosixResult EDEADLK = 35;   /* Resource deadlock would occur */
    public static readonly PosixResult ENAMETOOLONG = 36;  /* File name too long */
    public static readonly PosixResult ENOLCK = 37;    /* No record locks available */
    public static readonly PosixResult ENOSYS = 38;    /* Invalid system call number */
    public static readonly PosixResult ENOTEMPTY = 39; /* Directory not empty */
    public static readonly PosixResult ELOOP = 40; /* Too many symbolic links encountered */
    public static readonly PosixResult EWOULDBLOCK = EAGAIN; /* Operation would block */
    public static readonly PosixResult ENOMSG = 42;    /* No message of desired type */
    public static readonly PosixResult EIDRM = 43; /* Identifier removed */
    public static readonly PosixResult ECHRNG = 44;    /* Channel number out of range */
    public static readonly PosixResult EL2NSYNC = 45;  /* Level 2 not synchronized */
    public static readonly PosixResult EL3HLT = 46;    /* Level 3 halted */
    public static readonly PosixResult EL3RST = 47;    /* Level 3 reset */
    public static readonly PosixResult ELNRNG = 48;    /* Link number out of range */
    public static readonly PosixResult EUNATCH = 49;   /* Protocol driver not attached */
    public static readonly PosixResult ENOCSI = 50;    /* No CSI structure available */
    public static readonly PosixResult EL2HLT = 51;    /* Level 2 halted */
    public static readonly PosixResult EBADE = 52; /* Invalid exchange */
    public static readonly PosixResult EBADR = 53; /* Invalid request descriptor */
    public static readonly PosixResult EXFULL = 54;    /* Exchange full */
    public static readonly PosixResult ENOANO = 55;    /* No anode */
    public static readonly PosixResult EBADRQC = 56;   /* Invalid request code */
    public static readonly PosixResult EBADSLT = 57;   /* Invalid slot */
    public static readonly PosixResult EDEADLOCK = EDEADLK;
    public static readonly PosixResult EBFONT = 59;    /* Bad font file format */
    public static readonly PosixResult ENOSTR = 60;    /* Device not a stream */
    public static readonly PosixResult ENODATA = 61;   /* No data available */
    public static readonly PosixResult ETIME = 62; /* Timer expired */
    public static readonly PosixResult ENOSR = 63; /* Out of streams resources */
    public static readonly PosixResult ENONET = 64;    /* Machine is not on the network */
    public static readonly PosixResult ENOPKG = 65;    /* Package not installed */
    public static readonly PosixResult EREMOTE = 66;   /* Object is remote */
    public static readonly PosixResult ENOLINK = 67;   /* Link has been severed */
    public static readonly PosixResult EADV = 68;  /* Advertise error */
    public static readonly PosixResult ESRMNT = 69;    /* Srmount error */
    public static readonly PosixResult ECOMM = 70; /* Communication error on send */
    public static readonly PosixResult EPROTO = 71;    /* Protocol error */
    public static readonly PosixResult EMULTIHOP = 72; /* Multihop attempted */
    public static readonly PosixResult EDOTDOT = 73;   /* RFS specific error */
    public static readonly PosixResult EBADMSG = 74;   /* Not a data message */
    public static readonly PosixResult EOVERFLOW = 75; /* Value too large for defined data type */
    public static readonly PosixResult ENOTUNIQ = 76;  /* Name not unique on network */
    public static readonly PosixResult EBADFD = 77;    /* File descriptor in bad state */
    public static readonly PosixResult EREMCHG = 78;   /* Remote address changed */
    public static readonly PosixResult ELIBACC = 79;   /* Can not access a needed shared library */
    public static readonly PosixResult ELIBBAD = 80;   /* Accessing a corrupted shared library */
    public static readonly PosixResult ELIBSCN = 81;   /* .lib section in a.out corrupted */
    public static readonly PosixResult ELIBMAX = 82;   /* Attempting to link in too many shared libraries */
    public static readonly PosixResult ELIBEXEC = 83;  /* Cannot exec a shared library directly */
    public static readonly PosixResult EILSEQ = 84;    /* Illegal byte sequence */
    public static readonly PosixResult ERESTART = 85;  /* Interrupted system call should be restarted */
    public static readonly PosixResult ESTRPIPE = 86;  /* Streams pipe error */
    public static readonly PosixResult EUSERS = 87;    /* Too many users */
    public static readonly PosixResult ENOTSOCK = 88;  /* Socket operation on non-socket */
    public static readonly PosixResult EDESTADDRREQ = 89;  /* Destination address required */
    public static readonly PosixResult EMSGSIZE = 90;  /* Message too long */
    public static readonly PosixResult EPROTOTYPE = 91;    /* Protocol wrong type for socket */
    public static readonly PosixResult ENOPROTOOPT = 92;   /* Protocol not available */
    public static readonly PosixResult EPROTONOSUPPORT = 93;   /* Protocol not supported */
    public static readonly PosixResult ESOCKTNOSUPPORT = 94;   /* Socket type not supported */
    public static readonly PosixResult EOPNOTSUPP = 95;    /* Operation not supported on transport endpoint */
    public static readonly PosixResult ENOTSUP = EOPNOTSUPP;
    public static readonly PosixResult EPFNOSUPPORT = 96;  /* Protocol family not supported */
    public static readonly PosixResult EAFNOSUPPORT = 97;  /* Address family not supported by protocol */
    public static readonly PosixResult EADDRINUSE = 98;    /* Address already in use */
    public static readonly PosixResult EADDRNOTAVAIL = 99; /* Cannot assign requested address */
    public static readonly PosixResult ENETDOWN = 100; /* Network is down */
    public static readonly PosixResult ENETUNREACH = 101;  /* Network is unreachable */
    public static readonly PosixResult ENETRESET = 102;    /* Network dropped connection because of reset */
    public static readonly PosixResult ECONNABORTED = 103; /* Software caused connection abort */
    public static readonly PosixResult ECONNRESET = 104;   /* Connection reset by peer */
    public static readonly PosixResult ENOBUFS = 105;  /* No buffer space available */
    public static readonly PosixResult EISCONN = 106;  /* Transport endpoint is already connected */
    public static readonly PosixResult ENOTCONN = 107; /* Transport endpoint is not connected */
    public static readonly PosixResult ESHUTDOWN = 108;    /* Cannot send after transport endpoint shutdown */
    public static readonly PosixResult ETOOMANYREFS = 109; /* Too many references: cannot splice */
    public static readonly PosixResult ETIMEDOUT = 110;    /* Connection timed out */
    public static readonly PosixResult ECONNREFUSED = 111; /* Connection refused */
    public static readonly PosixResult EHOSTDOWN = 112;    /* Host is down */
    public static readonly PosixResult EHOSTUNREACH = 113; /* No route to host */
    public static readonly PosixResult EALREADY = 114; /* Operation already in progress */
    public static readonly PosixResult EINPROGRESS = 115;  /* Operation now in progress */
    public static readonly PosixResult ESTALE = 116;   /* Stale file handle */
    public static readonly PosixResult EUCLEAN = 117;  /* Structure needs cleaning */
    public static readonly PosixResult ENOTNAM = 118;  /* Not a XENIX named type file */
    public static readonly PosixResult ENAVAIL = 119;  /* No XENIX semaphores available */
    public static readonly PosixResult EISNAM = 120;   /* Is a named type file */
    public static readonly PosixResult EREMOTEIO = 121;    /* Remote I/O error */
    public static readonly PosixResult EDQUOT = 122;   /* Quota exceeded */
    public static readonly PosixResult ENOMEDIUM = 123;    /* No medium found */
    public static readonly PosixResult EMEDIUMTYPE = 124;  /* Wrong medium type */
    public static readonly PosixResult ECANCELED = 125;    /* Operation Canceled */
    public static readonly PosixResult ENOKEY = 126;   /* Required key not available */
    public static readonly PosixResult EKEYEXPIRED = 127;  /* Key has expired */
    public static readonly PosixResult EKEYREVOKED = 128;  /* Key has been revoked */
    public static readonly PosixResult EKEYREJECTED = 129; /* Key was rejected by service */
    public static readonly PosixResult EOWNERDEAD = 130;   /* Owner died */
    public static readonly PosixResult ENOTRECOVERABLE = 131;  /* State not recoverable */
    public static readonly PosixResult ERFKILL = 132;  /* Operation not possible due to RF-kill */
    public static readonly PosixResult EHWPOISON = 133;	/* Memory page has hardware error */
}
