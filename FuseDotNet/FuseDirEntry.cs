using System.Collections.Generic;

namespace FuseDotNet;

/// <summary>
/// Directory entry information returned by <see cref="IFuseOperations.ReadDir(ReadOnlyFuseMemory{byte}, out IEnumerable{FuseDirEntry}, ref FuseFileInfo, long, FuseReadDirFlags)"/> implementations.
/// </summary>
/// <param name="Name">Name of found file, without path.</param>
/// <param name="Offset">Unique sequence number of file entry following
/// this file within the directory. Default is zero
/// which indicates that the directory does not support
/// to continue file listing at a specific offset.</param>
/// <param name="Flags">Set Flags to FillDirPlus if the entire Stat
/// staructure is valid. Otherwise, only inode number
/// and file type bits of mode fields are used.</param>
/// <param name="Stat">If Flags field indicate FillDirPlus, this entire
/// staructure is valid. Otherwise, only inode number
/// and file type bits of mode fields are used.</param>
public record struct FuseDirEntry(string Name, long Offset, FuseFillDirFlags Flags, FuseFileStat Stat);
