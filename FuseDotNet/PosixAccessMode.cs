using System;

namespace FuseDotNet;

[Flags]
public enum PosixAccessMode
{
    Exists = 0x00,
    Execute = 0x01,
    Write = 0x02,
    Read = 0x04
}