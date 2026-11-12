#!/bin/sh
set -eu
cd "$(dirname "$0")/../.."
native_dir=$(mktemp -d)
trap 'rm -rf "$native_dir"' EXIT HUP INT TERM

# FUSE_CFLAGS can select headers from a non-system libfuse installation.
cc -Wall -Wextra -Werror -D_FILE_OFFSET_BITS=64 -fPIC -shared \
    ${FUSE_CFLAGS:-$(pkg-config --cflags fuse3)} \
    Tests/CallbackTests/native.c -o "$native_dir/libcallback_tests.so"
target_framework=${TEST_TARGET_FRAMEWORK:-net10.0}
LD_LIBRARY_PATH="$native_dir${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}" \
    "${DOTNET:-dotnet}" run --project Tests/CallbackTests/FuseDotNet.CallbackTests.csproj \
    -c Debug -f "$target_framework" -p:TargetFrameworks="$target_framework"
