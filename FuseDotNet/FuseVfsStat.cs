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
    public ulong f_bsize;  /* Size of blocks counted above */
    public ulong f_frsize; /* Size of fragments */
    public ulong f_blocks;
    public ulong f_bfree;
    public ulong f_bavail;  /* Number of blocks */
    
    public ulong f_files;
    public ulong f_ffree;
    public ulong f_favail;  /* Number of files (e.g., inodes) */
    
    public ulong f_fsid;    /* Not meaningful */
    public ulong f_flag;
    public ulong f_namemax; /* Same as pathconf(_PC_NAME_MAX) */
}
