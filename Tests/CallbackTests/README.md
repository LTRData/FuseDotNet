# Callback regression checks

Install the .NET 10 SDK, a C compiler, pkg-config and libfuse 3 development headers, then run from the repository root:

```sh
sh Tests/CallbackTests/run.sh
```

The native shim calls the actual managed delegates using FUSE 3 signatures and obtains configuration offsets from the installed headers. Checks cover legacy rejection without touching either file, optional dispatch including zero flags, invalid flags, exception translation, initialization exactly once, configuration/connection write-back and preservation of bytes beyond the represented configuration prefix.

Timestamp checks obtain `UTIME_NOW` and `UTIME_OMIT` from the native headers and pass all combinations with explicit timestamps through the `utimens` callback. They verify `timespec` layout, recognition of both signals regardless of `tv_sec`, current-time conversion and unchanged millisecond conversion of explicit timestamps.

No mount or root privileges are required. These checks exercise the native callback boundary; they do not replace mounted filesystem qualification. The script also works on FreeBSD with a compatible .NET 10 installation and libfuse headers. `DOTNET` may specify the SDK executable, and `FUSE_CFLAGS` may select a different header installation.

Native helper libraries are built in temporary subdirectories of `Tests/Debug` and removed when the script exits. This avoids loading executable code from `/tmp` on systems where that filesystem is mounted with `noexec`.
