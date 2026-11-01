# Open flags in version 2.0

`PosixOpenFlags` is now a four-byte, integer-backed readonly struct, following the library's `PosixResult` design. Its named values match the running operating system and process ABI. Linux x86/x64 and ARM/ARM64 have different values for some flags; FreeBSD x64 has its own mapping. Integer conversion preserves unknown bits without translating the native `FuseFileInfo.flags` storage.

This is a binary-breaking API change. Rebuild consumers. Flag combinations, bitwise operators, comparisons and typed `HasFlag` calls are supported, but enum-specific reflection, serialization, generic constraints and constant switch patterns need adjustment. Runtime-selected named values cannot be used as C# compile-time constants. For example, use `case var flags when flags == PosixOpenFlags.Write` rather than `case PosixOpenFlags.Write`.

`DirSync` has been removed with no compatibility alias. The FreeBSD bit previously given that name is `O_DSYNC`, now named `DataSync`. Review each old use and select `DataSync` only if synchronized data writes were the intended behavior. `SynchronousWrites` represents `O_SYNC`; on Linux its mask contains more than the `O_DSYNC` bit.

FreeBSD-only flags such as `SharedLock`, `ExclusiveLock`, `Execute`, `TtyInit`, `Verify` and `ResolveBeneath` throw `PlatformNotSupportedException` when accessed on Linux. They are never represented as zero or unrelated Linux bits. Access modes still require masking with `AccessModes`; `Read` is zero and must not be detected with `HasFlag(Read)`. `ToString()` reports the raw hexadecimal bits.

## Creation modes and append

`ToFileMode()` now selects only creation/truncation behavior:

| Flags | FileMode |
| --- | --- |
| Create + CreateNew | CreateNew |
| Create + Truncate, without CreateNew | Create |
| Create, without either flag above | OpenOrCreate |
| Truncate, without Create | Truncate |
| Neither Create nor Truncate | Open |

`CreateNew` represents O_EXCL and does not request creation by itself. `Append` does not affect this table. Consumers must implement append at write time; the helper no longer returns `FileMode.Append`, which would create files without O_CREAT and prevent read/write opens.

MirrorFs uses a native O_APPEND descriptor and write() for append when writeback caching is disabled. This lets the backing filesystem select EOF atomically for each write, including when another handle or process grows the file. FileStream buffering is disabled on this descriptor because append writes bypass FileStream. With writeback caching enabled, MirrorFs uses the supplied offsets and allows the kernel's reads on originally write-only handles. As required by libfuse, writeback caching is only reliable when external changes to the backing files are excluded; the sample does not request that capability itself.

MirrorFs remains an illustrative filesystem with other unimplemented operations. This change does not add complete POSIX locking, permission or durability semantics to the sample.

Run `sh Tests/OpenFlagsTests/run.sh` to check the platform values against native headers and exercise the sample's creation, truncation and append behavior.
