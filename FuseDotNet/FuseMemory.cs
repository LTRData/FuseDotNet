using System;
using System.Buffers;

namespace FuseDotNet;

/// <summary>
/// Represents unmanaged memory managed by Fuse library
/// </summary>
/// <typeparam name="T">Type of elements in the memory</typeparam>
public readonly struct FuseMemory<T> where T : unmanaged
{
    internal FuseMemory(nint pointer, int length)
    {
        Address = pointer;
        Length = length;
    }

    /// <summary>
    /// Unmanaged pointer to memory.
    /// </summary>
    public nint Address { get; }

    /// <summary>
    /// Number of elements at memory address.
    /// </summary>
    public int Length { get; }

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
public readonly struct ReadOnlyFuseMemory<T> where T : unmanaged
{
    public static implicit operator ReadOnlyFuseMemory<T>(FuseMemory<T> origin)
        => new(origin.Address, origin.Length);

    internal ReadOnlyFuseMemory(nint pointer, int length)
    {
        Address = pointer;
        Length = length;
    }

    /// <summary>
    /// Unmanaged pointer to memory.
    /// </summary>
    public nint Address { get; }

    /// <summary>
    /// Number of elements at memory address.
    /// </summary>
    public int Length { get; }

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

internal sealed class UnmanagedMemoryManager<T> : MemoryManager<T> where T : unmanaged
{
    private nint _pointer;
    private int _count;
    private bool _disposed;

    public UnmanagedMemoryManager(nint ptr, int count)
    {
        _pointer = ptr;
        _count = count;
    }

    public override unsafe Span<T> GetSpan()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(UnmanagedMemoryManager<T>));
        }

        return new((T*)_pointer, _count);
    }

    public override unsafe MemoryHandle Pin(int elementIndex = 0)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(UnmanagedMemoryManager<T>));
        }

        if (elementIndex < 0 || elementIndex >= _count)
        {
            throw new ArgumentOutOfRangeException(nameof(elementIndex));
        }

        var pointer = _pointer + elementIndex;
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
            _pointer = 0;
            _count = 0;
            _disposed = true;
        }
    }

    public override string ToString()
    {
        if (_pointer == 0)
        {
            return "(null)";
        }

        if (typeof(T) == typeof(char))
        {
            if (_count == 0)
            {
                return "";
            }

            return GetSpan().ToString();
        }

        return $"{typeof(T).Name} 0x{_pointer:x}[{_count}]";
    }
}
