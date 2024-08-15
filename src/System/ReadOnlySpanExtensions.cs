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
	public static void ForEach<T>(this Span<T> @this, ActionRef<T> action)
	{
		foreach (ref var element in @this)
		{
			action(ref element);
		}
	}

	/// <inheritdoc cref="FindIndex{T}(ReadOnlySpan{T}, FuncRefReadOnly{T, bool})"/>
	public static int FindIndex<T>(this ReadOnlySpan<T> @this, Func<T, bool> condition)
	{
		for (var i = 0; i < @this.Length; i++)
		{
			if (condition(@this[i]))
			{
				return i;
			}
		}
		return -1;
	}

	/// <inheritdoc cref="List{T}.FindIndex(Predicate{T})"/>
	public static int FindIndex<T>(this ReadOnlySpan<T> @this, FuncRefReadOnly<T, bool> condition)
	{
		for (var i = 0; i < @this.Length; i++)
		{
			if (condition(in @this[i]))
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// Performs the specified action on each element of the <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of elements in the span.</typeparam>
	/// <param name="this">The current collection.</param>
	/// <param name="action">
	/// The <see cref="ActionRefReadOnly{T}"/> delegate to perform on each element of the <see cref="ReadOnlySpan{T}"/>.
	/// </param>
	public static void ForEach<T>(this ReadOnlySpan<T> @this, ActionRefReadOnly<T> action)
	{
		foreach (ref readonly var element in @this)
		{
			action(in element);
		}
	}

	/// <summary>
	/// Returns a new <see cref="ReadOnlySpan{T}"/> instance whose internal elements are all come from the current collection,
	/// with reversed order.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The current collection.</param>
	/// <returns>A new collection whose elements are in reversed order.</returns>
	public static ReadOnlySpan<T> Reverse<T>(this ReadOnlySpan<T> @this)
	{
		var result = new T[@this.Length];
		for (var (i, j) = (@this.Length - 1, 0); i >= 0; i--, j++)
		{
			result[j] = @this[i];
		}
		return result;
	}

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
		=> new(
			(@this.Length & 1) == 0
				? @this
				: throw new ArgumentException(SR.ExceptionMessage("SpecifiedValueMustBeEven"), nameof(@this))
		);
}
