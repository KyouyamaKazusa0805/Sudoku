namespace System;

/// <summary>
/// Provides with extension methods on <see cref="ReadOnlyMemory{T}"/>.
/// </summary>
/// <seealso cref="ReadOnlyMemory{T}"/>
public static class ReadOnlyMemoryExtensions
{
	/// <summary>
	/// Fetch the element at the specified index inside a <see cref="ReadOnlyMemory{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The <see cref="ReadOnlyMemory{T}"/> instance.</param>
	/// <param name="index">The desired index.</param>
	/// <returns>The reference to the element at the specified index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly T ElementAt<T>(this ReadOnlyMemory<T> @this, int index) => ref @this.Span[index];

	/// <inheritdoc cref="ElementAt{T}(ReadOnlyMemory{T}, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly T ElementAt<T>(this ReadOnlyMemory<T> @this, Index index) => ref @this.Span[index];

	/// <summary>
	/// Creates a <see cref="ReadOnlyMemoryEnumerator{T}"/> instance that can be consumed by a <see langword="foreach"/> loop.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The sequence of <see cref="ReadOnlyMemory{T}"/> instance.</param>
	/// <returns>A <see cref="ReadOnlyMemoryEnumerator{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlyMemoryEnumerator<T> GetEnumerator<T>(this ReadOnlyMemory<T> @this) => new(@this);
}
