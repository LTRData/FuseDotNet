namespace FuseDotNet;

/*
 * The difference between `avail' and `free' is that `avail' represents
 * space available to unprivileged processes, whereas `free' includes all
 * unallocated space, including that reserved for privileged processes.
 * Or at least, that's the most useful interpretation.  (According to
 * the letter of the standard, this entire interface is completely
 * unspecified!)
 */

public struct BSDVfsStat
{
    public long f_bavail;       /* Number of blocks */
    public long f_bfree;
    public long f_blocks;
    public long f_favail;       /* Number of files (e.g., inodes) */
    public long f_ffree;
    public long f_files;
    public uint f_bsize;        /* Size of blocks counted above */
    public uint f_flag;
    public uint f_frsize;  /* Size of fragments */
    public uint f_fsid;    /* Not meaningful */
    public uint f_namemax; /* Same as pathconf(_PC_NAME_MAX) */
    
    public FuseVfsStat ToFuseVfsStat()
    {
        FuseVfsStat stats = new FuseVfsStat
        {
            f_bsize   = f_bsize,
            f_frsize  = f_frsize,
            f_blocks  = (ulong)f_blocks,
            f_bfree   = (ulong)f_bfree,
            f_bavail  = (ulong)f_bavail,
            f_files   = (ulong)f_files,
            f_ffree   = (ulong)f_ffree,
            f_favail  = (ulong)f_favail,
            f_fsid    = f_fsid,
            f_flag    = f_flag,
            f_namemax = f_namemax
        };

        return stats;
    }
}

public struct LinuxVfsStat
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

    public FuseVfsStat ToFuseVfsStat()
    {
        FuseVfsStat stats = new FuseVfsStat
        {
            f_bsize   = f_bsize,
            f_frsize  = f_frsize,
            f_blocks  = f_blocks,
            f_bfree   = f_bfree,
            f_bavail  = f_bavail,
            f_files   = f_files,
            f_ffree   = f_ffree,
            f_favail  = f_favail,
            f_fsid    = f_fsid,
            f_flag    = f_flag,
            f_namemax = f_namemax
        };

        return stats;
    }
}

public struct FuseVfsStat
{
    public ulong f_bsize;   /* Size of data blocks on the filesystem. */
    public ulong f_frsize;  /* Preferred size of data fragments on the filesystem. This does not necessarily need to match f_bsize. */
    public ulong f_blocks;  /* Total count of data blocks on the filesystem. */
    public ulong f_bfree;   /* Free data blocks on the filesystem for all users. */
    public ulong f_bavail;  /* Free data blocks on the filesystem for non-privledged users. */
    
    public ulong f_files;   /* Total count of inodes on the filesystem. */
    public ulong f_ffree;   /* Free inodes on the filesystem for all users. */
    public ulong f_favail;  /* Free inodes on the filesystem for non-privledged users. */
    
    public ulong f_fsid;    /* Filesystem ID. Optional, but useful for identification. */
    public ulong f_flag;    /* Mounting flags. */
    public ulong f_namemax; /* Max length in bytes of inode names. */
    
    public void ToLinuxVfsStat(ref LinuxVfsStat stats)
    {
        stats.f_bsize   = f_bsize;
        stats.f_frsize  = f_frsize;
        stats.f_blocks  = f_blocks;
        stats.f_bfree   = f_bfree;
        stats.f_bavail  = f_bavail;
        stats.f_files   = f_files;
        stats.f_ffree   = f_ffree;
        stats.f_favail  = f_favail;
        stats.f_fsid    = f_fsid;
        stats.f_flag    = f_flag; 
        stats.f_namemax = f_namemax;
    }
    
    public void ToBSDVfsStat(ref BSDVfsStat stats)
    {
        stats.f_bsize   = (uint)f_bsize;
        stats.f_frsize  = (uint)f_frsize;
        stats.f_blocks  = (long)f_blocks;
        stats.f_bfree   = (long)f_bfree;
        stats.f_bavail  = (long)f_bavail;
        stats.f_files   = (long)f_files;
        stats.f_ffree   = (long)f_ffree;
        stats.f_favail  = (long)f_favail;
        stats.f_fsid    = (uint)f_fsid;
        stats.f_flag    = (uint)f_flag;
        stats.f_namemax = (uint)f_namemax;
    }
}