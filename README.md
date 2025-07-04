## FuseDotNet

.NET wrapper for Fuse libraries for creating file systems in user mode.

By using Fuse library, you can create your own file systems very easily
without writing a kernel level file system driver. FuseDotNet is a library
that allows you to make a file system on .NET environment.

## Alternatives

In comparison to other libraries like FuseSharp, the main goal of this library
is optimized performance by less garbage collector and heap allocation
pressure. It could be a bit more difficult to use, because path strings and
I/O buffers are not directly available to handling routines as strings and
byte arrays, but the implemenations should probably be fairly straight-forward
anyway.

## Licensing
FuseDotNet is distributed under a version of the "MIT License",
which is a BSD-like license. See the 'LICENSE.txt' file for details.

## Environment
* Linux: x86, x64, arm32 or arm64
* FreeBSD: x64
* Fuse: Fuse3

Targets .NET Framework 4.8 and .NET Standard 2.0 can be used with Mono and targets
.NET Standard 2.1 can be used with .NET Core 3.1 - .NET 5.0 applications and
libraries. There are also .NET 6.0 and .NET 7.0 builds with certain optimizations
available in those versions.

## How to write a file system
To make a file system, an application needs to implement IFuseOperations
interface. Once implemented, you can invoke `Mount` method on your driver
instance to mount a drive. By default, the Fuse native library forks the process
and continues work in a child process. The parent process returns. This can be an
issue when for example debugging, in which case "-f" or "-d" options can be passed
to Fuse to instead do work in current process and block until file system is
dismounted. Parameters to `Mount` method are just like Fuse library. See sample
codes under 'sample' directory. Administrator privileges are required to run
file system applications.

#### Sample code

There are sample implementations, "mirror" and "tempfs", that show basic
implementation of the library. There is also a "DiscUtils.MountFuse" package that
implements a FuseDotNet file system using `IFileSystem` implementations in DiscUtils
library. Source code for that library is in https://github.com/LTRData/DiscUtils
repository.

## Unmounting
Just run the below command to unmount a file system.

   > umount /mountpoint

