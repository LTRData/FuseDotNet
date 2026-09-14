# FuseDotNet

.NET wrapper for FUSE 3 for implementing file systems in user space on Linux and FreeBSD.

FuseDotNet focuses on reducing garbage collection and heap allocation pressure. Its callbacks expose native-memory views of path names and I/O buffers instead of automatically allocating strings and byte arrays for every request.

## Package and repository contents

Install the [LTRData.FuseDotNet](https://www.nuget.org/packages/LTRData.FuseDotNet) NuGet package:

```sh
dotnet add package LTRData.FuseDotNet
```

The C# namespace is `FuseDotNet`.

| Project | Purpose |
| --- | --- |
| [FuseDotNet](https://github.com/LTRData/FuseDotNet/tree/master/FuseDotNet) | Library, callback interface, mount/unmount helpers and service lifecycle. |
| [MirrorFs](https://github.com/LTRData/FuseDotNet/tree/master/MirrorFs) | Sample that exposes an existing directory through FUSE. It can modify the underlying files; several operations are unimplemented. |
| [TempFs](https://github.com/LTRData/FuseDotNet/tree/master/TempFs) | Minimal callback demonstration exposing `test.txt` with fixed content. Writes are acknowledged but not stored. |

For DiscUtils file systems, use the separate [LTRData.DiscUtils.MountFuse](https://github.com/LTRData/DiscUtils/tree/LTRData.DiscUtils-initial/Integrations/DiscUtils.MountFuse) adapter.

## Frameworks and native requirements

The library targets `net10.0`, `net9.0`, `net8.0`, `netstandard2.1`, `netstandard2.0` and `net48`. The .NET Standard and .NET Framework builds provide compatibility options for older runtimes, including Mono.

The current native platform mappings are:

| Operating system | Architectures | Notes |
| --- | --- | --- |
| Linux | x86, x64, ARM32, ARM64 | Requires a compatible runtime and native FUSE 3 installation. |
| FreeBSD | x64 | Use a `net8.0`, `net9.0` or `net10.0` build; FreeBSD stat marshalling and unmount handling are excluded from the .NET Standard and .NET Framework builds. |

Install FUSE 3 separately and ensure the native library can be resolved as `fuse3`. The host also needs kernel FUSE support and appropriate device and mount-point permissions. Framework compatibility alone does not add Windows or macOS support.

Mount privileges depend on the host configuration. On Linux, [libfuse's `fusermount3` helper](https://github.com/libfuse/libfuse#security-implications) can allow ordinary users to mount their own file systems.

## Implementing a file system

Implement [IFuseOperations](https://github.com/LTRData/FuseDotNet/blob/master/FuseDotNet/IFuseOperations.cs), then call the `Mount` extension method. Path arguments contain UTF-8 bytes; `FuseHelper.GetString` can decode them when needed. Native-memory arguments refer to callback-owned buffers and must not be retained after the callback returns.

Given an existing `IFuseOperations operations` instance:

```csharp
using FuseDotNet;

operations.Mount(new[] { "MyFileSystem", "-f", "-s", "/path/to/mountpoint" });
```

Arguments follow the native FUSE command line, including a program name as the first argument. `-f` keeps FUSE in the foreground and the call blocks until unmounted; `-d` also enables native debug output. Without foreground mode, native FUSE may daemonize by forking the process. Use foreground mode when hosting FUSE in a managed application. `-s` selects single-threaded request handling; omit it only when the implementation handles concurrent callbacks.

Keep the operations object and its backing resources alive until the mount call returns after unmounting. Callbacks return `PosixResult` values; the wrapper translates these to native FUSE results. The wrapper does not expose every FUSE 3 operation: extended attributes, file locking and several other callbacks are currently left unset in [Fuse.Mount](https://github.com/LTRData/FuseDotNet/blob/master/FuseDotNet/Fuse.cs).

## Service lifecycle and unmounting

[FuseService](https://github.com/LTRData/FuseDotNet/blob/master/FuseDotNet/FuseService.cs) hosts an operations instance on a long-running task. Pass foreground arguments with the mount point last.

- `Start()` launches the task; it does not wait for the mount to become ready.
- `WaitForExit()` and `WaitForExitAsync()` wait for completion. Their timeout overloads return whether the task finished; a timeout does not request unmounting.
- Subscribe to `Error` for failures from the service procedure and `Stopped` for a normal return from the mount call. The service disposes its operations object.
- `Dispose()` requests unmounting when it detects an active FUSE mount and waits for the task to stop. It can block if unmounting fails.

`Fuse.Unmount(path)` throws `PosixException` on a native unmount error. `Fuse.TryUnmount(path, out result)` returns a Boolean and the native error result. These call the OS unmount API directly, so their permissions can differ from those of a mount helper.

To unmount from another terminal on Linux using the FUSE helper:

```sh
fusermount3 -u /path/to/mountpoint
```

On FreeBSD, or where the caller has the required OS privileges:

```sh
umount /path/to/mountpoint
```

## Building and running samples

Use the .NET 10 SDK for the current source tree. For a single-target library build:

```sh
dotnet build FuseDotNet/FuseDotNet.csproj -c Debug -f net10.0
```

The library's Release configuration also generates a NuGet package. A full build targets all configured frameworks.

The samples currently target `net8.0` and `net9.0`, as well as `netstandard2.1`; choose a runnable .NET target with a matching installed runtime. From the repository root, with an existing empty mount directory:

```sh
dotnet run --project TempFs/TempFs.csproj -f net8.0 -- -f -s /path/to/mountpoint
```

For the mirror sample, supply the source directory before the FUSE options and use a separate mount directory:

```sh
dotnet run --project MirrorFs/MirrorFs.csproj -f net8.0 -- /path/to/source -f -s /path/to/mountpoint
```

These are illustrative implementations with incomplete file-system semantics; review their callbacks before adapting them.

## License

FuseDotNet is distributed under the [MIT License](https://github.com/LTRData/FuseDotNet/blob/master/LICENSE.txt).
