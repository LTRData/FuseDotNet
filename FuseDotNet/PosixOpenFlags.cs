using System;

namespace FuseDotNet;

[Flags]
public enum PosixOpenFlags : int
{
    Read = 0x0000,   /*open for reading only */
    Write = 0x0001,   /*open for writing only */
    ReadWrite = 0x0002,   /*open for reading and writing */
    AccessModes = 0x0003,   /*mask for selecting access modes */
    NonBlocking = 0x0004,   /*no delay */
    Append = 0x0008,   /*set append mode */
    SharedLock = 0x0010,   /*open with shared file lock */
    ExclusiveLock = 0x0020,   /*open with exclusive file lock */
    Async = 0x0040,   /*signal pgrp when data ready */
    SynchronousWrites = 0x0080,   /*synchronous writes */
    NoFollowLinks = 0x0100,   /*don't follow symlinks */
    Create = 0x0200,   /*create if nonexistent */
    Truncate = 0x0400,   /*truncate to zero length */
    CreateNew = 0x0800,   /*error if already exists */
    NoControllingTerminal = 0x8000,   /*don't assign controlling terminal */
    Direct = 0x00010000,
    Directory = 0x00020000,   /*Fail if not directory */
    Execute = 0x00040000,   /*Open for execute only */
    Search = Execute,
    TtyInit = 0x00080000,   /*Restore default termios attributes */
    Verify = 0x00200000,   /*open only after verification */
    ResolveBeneath = 0x00800000,
    DirSync = 0x01000000
}
