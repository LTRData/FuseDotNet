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
which is a BSD-like license. See the 'license.mit.txt' file for details.

## Environment
64 bit Linux or FreeBSD.

Targets .NET Framework 4.6 and 4.8 and .NET Standard 2.0 can be used with Mono
and targets .NET Standard 2.1 and .NET 6.0 can be used with .NET Core and .NET
6.0 applications and libraries.

## How to write a file system
To make a file system, an application needs to implement IFuseOperations
interface. Once implemented, you can invoke Mount function on your driver
instance to mount a drive. The function blocks until the file system is
unmounted. Semantics and parameters are just like Fuse library. See sample
codes under 'sample' directory. Administrator privileges are required to run
file system applications.

## Unmounting
Just run the below command to unmount a file system.

   > umount /mountpoint

