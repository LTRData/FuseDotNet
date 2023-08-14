namespace FuseDotNet;

public enum FuseIoctlFlags : uint
{
    /** 32bit compat ioctl on 64bit machine */
    Compat = 1 << 0,
    /** not restricted to well-formed ioctls, retry allowed */
    Unrestricted = 1 << 1,
    /** retry with new iovecs */
    Retry = 1 << 2,
    /** is a directory */
    Dir = 1 << 4,
}
