using System.Runtime.InteropServices;

namespace FuseDotNet;

/// <summary>
/// The stable prefix of libfuse 3's fuse_config, through auto_cache.
/// Fields beyond this prefix have different layouts in different libfuse versions.
/// </summary>
/// <remarks>
/// Integer switches use zero for disabled and nonzero for enabled. The native
/// configuration is owned by libfuse and must not be retained after Init returns.
/// </remarks>
// All doubles start at eight-byte aligned offsets. Pack = 4 ends this view at
// byte 84, without tail padding overlapping the next native configuration field.
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct FuseConfig
{
    public int set_gid;
    public uint gid;
    public int set_uid;
    public uint uid;
    public int set_mode;
    public uint umask;
    public double entry_timeout;
    public double negative_timeout;
    public double attr_timeout;
    public int intr;
    public int intr_signal;
    public int remember;

    /// <summary>
    /// Disables libfuse's hidden-file handling for unlinked open files. Enable only
    /// when the filesystem supports the resulting open-but-unlinked file lifecycle.
    /// </summary>
    public int hard_remove;

    public int use_ino;
    public int readdir_ino;
    public int direct_io;
    public int kernel_cache;
    public int auto_cache;
}
