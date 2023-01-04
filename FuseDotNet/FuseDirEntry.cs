namespace FuseDotNet;

public record struct FuseDirEntry
(
    /// Name of found file, without path.
    string Name,
    
    /// Unique sequence number of file entry following
    /// this file within the directory. Default is zero
    /// which indicates that the directory does not support
    /// to continue file listing at a specific offset.
    long Offset,

    /// Set Flags to FillDirPlus if the entire Stat
    /// staructure is valid. Otherwise, only inode number
    /// and file type bits of mode fields are used.
    FuseFillDirFlags Flags,

    /// If Flags field indicate FillDirPlus, this entire
    /// staructure is valid. Otherwise, only inode number
    /// and file type bits of mode fields are used.
    FuseFileStat Stat
);