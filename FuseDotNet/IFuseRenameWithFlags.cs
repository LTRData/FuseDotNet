using LTRData.Extensions.Native.Memory;

namespace FuseDotNet;

/// <summary>
/// Optional extension to <see cref="IFuseOperations"/> for FUSE 3 rename flags.
/// </summary>
public interface IFuseRenameWithFlags
{
    /// <summary>
    /// Renames an entry. NoReplace must not overwrite an existing destination;
    /// Exchange must atomically exchange two existing entries. Return an error
    /// without modifying either entry if the requested operation is unsupported.
    /// </summary>
    PosixResult Rename(ReadOnlyNativeMemory<byte> from, ReadOnlyNativeMemory<byte> to, FuseRenameFlags flags);
}
