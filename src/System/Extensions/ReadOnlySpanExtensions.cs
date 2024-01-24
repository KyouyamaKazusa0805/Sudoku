namespace System;

/// <summary>
/// Provides with extension methods on <see cref="Span{T}"/> and <see cref="ReadOnlySpan{T}"/>.
/// </summary>
/// <seealso cref="Span{T}"/>
/// <seealso cref="ReadOnlySpan{T}"/>
public static class ReadOnlySpanExtensions
{
	/// <summary>
	/// Performs the specified action on each element of the <see cref="Span{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of elements in the span.</typeparam>
	/// <param name="this">The current collection.</param>
	/// <param name="action">The <see cref="ActionRef{T}"/> delegate to perform on each element of the <see cref="Span{T}"/>.</param>
	public static void ForEach<T>(this scoped Span<T> @this, ActionRef<T> action)
	{
		foreach (ref var element in @this)
		{
			action(ref element);
		}
	}

	/// <summary>
	/// Performs the specified action on each element of the <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of elements in the span.</typeparam>
	/// <param name="this">The current collection.</param>
	/// <param name="action">
	/// The <see cref="ActionRefReadOnly{T}"/> delegate to perform on each element of the <see cref="ReadOnlySpan{T}"/>.
	/// </param>
	public static void ForEach<T>(this scoped ReadOnlySpan<T> @this, ActionRefReadOnly<T> action)
	{
		foreach (ref readonly var element in @this)
		{
			action(in element);
		}
	}

	/// <summary>
	/// Select an element from the specified list of elements.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The collection.</param>
	/// <param name="random">The randomizer.</param>
	/// <returns>The chosen element.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly T RandomSelectOne<T>(this ReadOnlySpan<T> @this, Random? random = null)
		=> ref @this[(random ?? Random.Shared).Next(0, @this.Length)];

	/// <summary>
	/// Iterates on each element, in reverse order.
	/// </summary>
	/// <typeparam name="T">The type of each element in the sequence.</typeparam>
	/// <param name="this">The sequence to be iterated.</param>
	/// <returns>An enumerator type that iterates on each element.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReverseEnumerator<T> EnumerateReversely<T>(this ReadOnlySpan<T> @this) => new(@this);

	/// <summary>
	/// Creates a <see cref="PairEnumerator{T}"/> instance that iterates on each element of pair elements.
	/// </summary>
	/// <typeparam name="T">The type of the array elements.</typeparam>
	/// <param name="this">The array.</param>
	/// <returns>An enumerable collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PairEnumerator<T> EnumeratePairly<T>(this ReadOnlySpan<T> @this) where T : notnull
		=> new((@this.Length & 1) != 0 ? throw new ArgumentException("The argument must be of an even length.", nameof(@this)) : @this);
}
