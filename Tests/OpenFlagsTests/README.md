# Open flags and MirrorFs regression checks

Install the .NET 10 SDK, a C compiler, pkg-config and libfuse 3 development headers, then run from the repository root:

```sh
sh Tests/OpenFlagsTests/run.sh
```

The native shim obtains open-flag values and file-info offsets from the installed headers, and writes native flag bits into the managed file-info structure. Checks cover all exposed platform flags, unknown bits, unsupported BSD-only flags on Linux, creation/truncation combinations and the intentional removal of `DirSync`.

MirrorFs checks use disposable backing files and its real callbacks. They cover read/write append, missing files without O_CREAT, explicit/exclusive creation, truncation, external file growth, concurrent writes through separate append handles, release cleanup and the distinct writeback-cache behavior.

Timestamp checks use native `UTIME_NOW` and `UTIME_OMIT` values and exercise every combination of current, omitted and explicit access/modification times. They verify that omitted timestamps remain unchanged and that a request setting both timestamps to now uses one current-time value.

No mount or root privileges are required. The script also works on FreeBSD with a compatible .NET 10 installation and libfuse headers. `DOTNET` may specify the SDK executable, and `FUSE_CFLAGS` may select a different header installation. These checks do not perform a live kernel FUSE mount.
