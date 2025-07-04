using System;

namespace FuseDotNet;

[Flags]
public enum FuseAllocateMode
{
    None = 0x00,
    KeepSize = 0x01,
    PunchHole = 0x02,
    NoHideStale = 0x04,
    CollapseRange = 0x08,
    ZeroRange = 0x10,
    InsertRange = 0x20,
    UnshareRange = 0x40,
}
