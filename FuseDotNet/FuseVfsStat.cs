namespace FuseDotNet;

/*
 * The difference between `avail' and `free' is that `avail' represents
 * space available to unprivileged processes, whereas `free' includes all
 * unallocated space, including that reserved for privileged processes.
 * Or at least, that's the most useful interpretation.  (According to
 * the letter of the standard, this entire interface is completely
 * unspecified!)
 */
public struct FuseVfsStat
{
    public long f_bavail;       /* Number of blocks */
    public long f_bfree;
    public long f_blocks;
    public long f_favail;       /* Number of files (e.g., inodes) */
    public long f_ffree;
    public long f_files;
    public uint f_bsize;        /* Size of blocks counted above */
    public uint f_flag;
    public uint f_frsize;       /* Size of fragments */
    public uint f_fsid;         /* Not meaningful */
    public uint f_namemax;      /* Same as pathconf(_PC_NAME_MAX) */
}
