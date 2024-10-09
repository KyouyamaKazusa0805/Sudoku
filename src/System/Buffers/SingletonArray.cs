namespace System.Buffers;

/// <summary>
/// Represents an array with only one element.
/// </summary>
/// <typeparam name="T">The type of the only element of array.</typeparam>
/// <param name="value">Indicates the value.</param>
public sealed partial class SingletonArray<T>([Field] T value) : MemoryManager<T>
{
	/// <inheritdoc/>
	public override void Unpin()
	{
	}

	/// <inheritdoc/>
	public override string ToString() => _value?.ToString() ?? "<null>";

	/// <inheritdoc/>
	public override Span<T> GetSpan() => new(ref _value);

	/// <inheritdoc/>
	public override unsafe MemoryHandle Pin(int elementIndex = 0)
	{
		fixed (T* pValue = &_value)
		{
			return new(pValue + elementIndex);
		}
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
	}


	/// <summary>
	/// Creates a <see cref="Span{T}"/> from the current instance without any copy operation.
	/// </summary>
	/// <param name="value">The value to be casted from.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Span<T>(SingletonArray<T> value) => new(ref value._value);

	/// <summary>
	/// Creates a <see cref="ReadOnlySpan{T}"/> from the current instance without any copy operation.
	/// </summary>
	/// <param name="value">The value to be casted from.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ReadOnlySpan<T>(SingletonArray<T> value) => new(in value._value);

	/// <summary>
	/// Creates a <see cref="Memory{T}"/> from the current instance without any copy operation.
	/// </summary>
	/// <param name="value">The value to be casted from.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Memory<T>(SingletonArray<T> value) => value.Memory;

	/// <summary>
	/// Creates a <see cref="ReadOnlyMemory{T}"/> from the current instance without any copy operation.
	/// </summary>
	/// <param name="value">The value to be casted from.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ReadOnlyMemory<T>(SingletonArray<T> value) => value.Memory;
}
