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

The library targets `net11.0`, `net10.0`, `net9.0`, `net8.0`, `netstandard2.1`, `netstandard2.0` and `net48`. The .NET Standard and .NET Framework builds provide compatibility options for older runtimes, including Mono.

The current native platform mappings are:

| Operating system | Architectures | Notes |
| --- | --- | --- |
| Linux | x86, x64, ARM32, ARM64 | Requires a compatible runtime and native FUSE 3 installation. |
| FreeBSD | x64 | Use a `net8.0`, `net9.0`, `net10.0` or `net11.0` build; FreeBSD stat marshalling and unmount handling are excluded from the .NET Standard and .NET Framework builds. |

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

## Rename flags and initialization configuration

Filesystems can optionally implement `IFuseRenameWithFlags` and/or `IFuseInitWithConfig` in addition to `IFuseOperations`. The proxy calls the optional method instead of the corresponding legacy method, including for an ordinary rename with no flags. Without `IFuseRenameWithFlags`, a rename with nonzero flags returns `EINVAL` without calling the legacy implementation. Unknown flags and the invalid combination of `NoReplace | Exchange` also return `EINVAL`.

`NoReplace` must preserve an existing destination and `Exchange` must atomically swap two existing entries. A filesystem must return an error if it cannot implement the requested semantics. `IFuseInitWithConfig.Init` receives libfuse's initialized configuration by reference; modify only the required fields and do not retain the reference. `FuseConfig` exposes the stable prefix through `auto_cache`, leaving later, version-dependent fields untouched. Enabling `hard_remove` opts out of libfuse's hidden-file handling and requires the filesystem to support open-but-unlinked files; it is not enabled by the wrapper.

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

Use the .NET 11 SDK for the current source tree. For a single-target library build:

```sh
dotnet build FuseDotNet/FuseDotNet.csproj -c Debug -f net10.0
```

The library's Release configuration also generates a NuGet package. A full build targets all configured frameworks.

The samples currently target `net10.0` and `net11.0`, as well as `netstandard2.1`; choose a runnable .NET target with a matching installed runtime. From the repository root, with an existing empty mount directory:

```sh
dotnet run --project TempFs/TempFs.csproj -f net11.0 -- -f -s /path/to/mountpoint
```

For the mirror sample, supply the source directory before the FUSE options and use a separate mount directory:

```sh
dotnet run --project MirrorFs/MirrorFs.csproj -f net11.0 -- /path/to/source -f -s /path/to/mountpoint
```

These are illustrative implementations with incomplete file-system semantics; review their callbacks before adapting them.

## Continuous integration and NuGet publishing

The native FUSE callback tests and MirrorFs open-flag tests run on Linux x64
and ARM64 using both .NET 10 and .NET 11. The tests compile native FUSE 3 test
libraries and require the `libfuse3-dev` package. Run either test suite
locally with `TEST_TARGET_FRAMEWORK=net10.0` (default) or
`TEST_TARGET_FRAMEWORK=net11.0`, for example:

```sh
TEST_TARGET_FRAMEWORK=net11.0 sh Tests/CallbackTests/run.sh
TEST_TARGET_FRAMEWORK=net11.0 sh Tests/OpenFlagsTests/run.sh
```

NuGet publishing is a separate, tag-triggered operation. After merging
changes to `master`, create an annotated publication tag:

```sh
git switch master
git pull --ff-only
git tag -a publish/2026.11.12.1 -m "NuGet publication 2026.11.12.1"
git push origin publish/2026.11.12.1
```

The [publishing workflow](.github/workflows/publish-nuget.yml) accepts only
`publish/*` tags whose commit is reachable from `master`. It checks all
library targets, builds samples and exercises .NET 10/11 native regression
checks on Linux x64 and ARM64 before packaging `LTRData.FuseDotNet`.

**A publication tag does not determine the NuGet package version.** The
`Version` and `PackageVersion` in `Directory.Build.props` remain the
authority. Release builds generate `.nupkg` and `.snupkg` files, and the
publishing workflow uploads those files without changing their versions.
Previously published versions are skipped; increase `Version` when a new
package version is required.

The first publication requires these one-time settings:

- Configure the NuGet.org Trusted Publishing policy for `LTRData/FuseDotNet`
  with GitHub workflow filename `publish-nuget.yml`, and environment
  `nuget-publish` if the policy specifies an environment.
- Set the GitHub Actions repository variable `NUGET_USER` to the NuGet.org
  account username associated with the policy, not the GitHub organization name.
- Create the `nuget-publish` GitHub Actions environment, optionally requiring
  approval and restricting deployments to `publish/*` tags.

The build-and-test jobs have no NuGet publishing credentials. Only the final
environment-protected job obtains a temporary credential via Trusted
Publishing, after the package artifact is built and verified. No long-lived
NuGet API key is stored in GitHub.

## License

FuseDotNet is distributed under the [MIT License](https://github.com/LTRData/FuseDotNet/blob/master/LICENSE.txt).
