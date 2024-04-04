using System;
using System.Buffers;
using System.IO;

namespace FuseDotNet;

/// <summary>
/// Represents unmanaged memory managed by Fuse library
/// </summary>
/// <typeparam name="T">Type of elements in the memory</typeparam>
public readonly struct FuseMemory<T>(nint pointer, int length) where T : unmanaged
{

    /// <summary>
    /// Unmanaged pointer to memory.
    /// </summary>
    public nint Address { get; } = pointer;

    /// <summary>
    /// Number of elements at memory address.
    /// </summary>
    public int Length { get; } = length;

    /// <summary>
    /// Return value indicating whether this object represents an unmanaged NULL pointer.
    /// </summary>
    public bool IsNull => Address == 0;

    /// <summary>
    /// Return value indicating whether this object represents an unmanaged NULL pointer
    /// or zero-length memory.
    /// </summary>
    public bool IsEmpty => Length == 0;

    /// <summary>
    /// Gets a <see cref="Span{T}"/> for this memory block.
    /// </summary>
    public unsafe Span<T> Span => new((T*)Address, Length);

    /// <summary>
    /// Gets a disposable <see cref="MemoryManager{T}"/> for this memory block. This can
    /// be used to get a <see cref="Memory{T}"/> that can be sent to asynchronous API or
    /// delegates. Remember though, that the memory is invalid after return to Fuse API
    /// so make sure that no asynchronous operations use the memory after returning from
    /// implementation methods.
    /// </summary>
    public MemoryManager<T> GetMemoryManager()
        => new UnmanagedMemoryManager<T>(Address, Length);

    /// <summary>
    /// Gets a disposable <see cref="UnmanagedMemoryStream"/> for this memory block.
    /// Remember though, that the memory is invalid after return to Dokan API
    /// so make sure that no asynchronous operations use the memory after returning from
    /// implementation methods.
    /// </summary>
    public unsafe UnmanagedMemoryStream GetStream()
        => new((byte*)Address, Length * sizeof(T));

    public override string ToString()
    {
        if (Address == 0)
        {
            return "(null)";
        }

        if (typeof(T) == typeof(char))
        {
            if (Length == 0)
            {
                return "";
            }

            return Span.ToString();
        }

        return $"{typeof(T).Name} 0x{Address:x}[{Length}]";
    }
}

/// <summary>
/// Represents read only unmanaged memory managed by Fuse library
/// </summary>
/// <typeparam name="T">Type of elements in the memory</typeparam>
public readonly struct ReadOnlyFuseMemory<T>(nint pointer, int length) where T : unmanaged
{
    public static implicit operator ReadOnlyFuseMemory<T>(FuseMemory<T> origin)
        => new(origin.Address, origin.Length);

    /// <summary>
    /// Unmanaged pointer to memory.
    /// </summary>
    public nint Address { get; } = pointer;

    /// <summary>
    /// Number of elements at memory address.
    /// </summary>
    public int Length { get; } = length;

    /// <summary>
    /// Return value indicating whether this object represents an unmanaged NULL pointer.
    /// </summary>
    public bool IsNull => Address == 0;

    /// <summary>
    /// Return value indicating whether this object represents an unmanaged NULL pointer
    /// or zero-length memory.
    /// </summary>
    public bool IsEmpty => Length == 0;

    /// <summary>
    /// Gets a <see cref="ReadOnlySpan{T}"/> for this memory block.
    /// </summary>
    public unsafe ReadOnlySpan<T> Span => new((T*)Address, Length);

    /// <summary>
    /// Gets a disposable <see cref="MemoryManager{T}"/> for this memory block. This can
    /// be used to get a <see cref="Memory{T}"/> that can be sent to asynchronous API or
    /// delegates. Remember though, that the memory is invalid after return to Fuse API
    /// so make sure that no asynchronous operations use the memory after returning from
    /// implementation methods.
    /// </summary>
    public MemoryManager<T> GetMemoryManager()
        => new UnmanagedMemoryManager<T>(Address, Length);

    /// <summary>
    /// Gets a disposable <see cref="UnmanagedMemoryStream"/> for this memory block.
    /// Remember though, that the memory is invalid after return to Dokan API
    /// so make sure that no asynchronous operations use the memory after returning from
    /// implementation methods.
    /// </summary>
    public unsafe UnmanagedMemoryStream GetStream()
        => new((byte*)Address, Length * sizeof(T), Length * sizeof(T), FileAccess.Read);

    public override string ToString()
    {
        if (Address == 0)
        {
            return "(null)";
        }

        if (typeof(T) == typeof(char))
        {
            if (Length == 0)
            {
                return "";
            }

            return Span.ToString();
        }

        return $"{typeof(T).Name} 0x{Address:x}[{Length}]";
    }
}

internal sealed class UnmanagedMemoryManager<T>(nint address, int count) : MemoryManager<T> where T : unmanaged
{
    private bool _disposed;

    public override unsafe Span<T> GetSpan()
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(UnmanagedMemoryManager<T>));
        }
#endif

        return new((T*)address, count);
    }

    public override unsafe MemoryHandle Pin(int elementIndex = 0)
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(UnmanagedMemoryManager<T>));
        }
#endif

        if (elementIndex < 0 || elementIndex >= count)
        {
            throw new ArgumentOutOfRangeException(nameof(elementIndex));
        }

        var pointer = address + elementIndex;
        return new MemoryHandle((T*)pointer, default, this);
    }

    public override void Unpin()
    {
        // No need to do anything, since we're dealing with unmanaged memory.
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            address = 0;
            count = 0;
            _disposed = true;
        }
    }

    public override string ToString()
    {
        if (address == 0)
        {
            return "(null)";
        }

        if (typeof(T) == typeof(char))
        {
            if (count == 0)
            {
                return "";
            }

            return GetSpan().ToString();
        }

        return $"{typeof(T).Name} 0x{address:x}[{count}]";
    }
}
