using System;

namespace FuseDotNet;

/// <summary>
/// Flags passed to the FUSE 3 rename callback.
/// </summary>
[Flags]
public enum FuseRenameFlags : uint
{
    None = 0,
    NoReplace = 1,
    Exchange = 2
}
