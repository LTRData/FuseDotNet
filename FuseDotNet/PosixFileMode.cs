using System;

namespace FuseDotNet;

[Flags]
public enum PosixFileMode : ushort
{
    None = 0x0,
    OthersExecute = 0x1,
    OthersWrite = 0x2,
    OthersRead = 0x4,
    OthersReadExecute = 0x5,
    OthersReadWrite = 0x6,
    OthersAll = 0x7,
    GroupExecute = 0x8,
    GroupWrite = 0x10,
    GroupRead = 0x20,
    GroupReadExecute = 0x28,
    GroupReadWrite = 0x30,
    GroupAll = 0x38,
    OwnerExecute = 0x40,
    OwnerWrite = 0x80,
    OwnerRead = 0x100,
    OwnerReadExecute = 0x140,
    OwnerReadWrite = 0x180,
    OwnerAll = 0x1C0,

    Sticky = 0x200,
    SetGroupId = 0x400,
    SetUserId = 0x800,

    Fifo = 0x1000,
    Character = 0x2000,
    Directory = 0x4000,
    Block = 0x6000,
    Regular = 0x8000,
    SymbolikLink = 0xA000,
    Socket = 0xC000,
    Whiteout = 0xE000
}
