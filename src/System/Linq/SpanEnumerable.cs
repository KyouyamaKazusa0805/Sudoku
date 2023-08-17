namespace System.Linq;

/// <summary>
/// Provides LINQ-based extension methods on <see cref="Span{T}"/> and <see cref="ReadOnlySpan{T}"/>.
/// </summary>
/// <seealso cref="Span{T}"/>
/// <seealso cref="ReadOnlySpan{T}"/>
public static class SpanEnumerable
{
	/// <summary>
	/// The select method used in <see langword="from"/>-<see langword="in"/>-<see langword="select"/>
	/// clause.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	/// <typeparam name="TResult">The result type.</typeparam>
	/// <param name="this">The list.</param>
	/// <param name="selector">The selector that is used for conversion.</param>
	/// <returns>The array of target result elements.</returns>
	public static TResult[] Select<T, TResult>(this scoped Span<T> @this, Func<T, TResult> selector)
	{
		var result = new TResult[@this.Length];
		var i = 0;
		foreach (var element in @this)
		{
			result[i++] = selector(element);
		}

		return result;
	}

	/// <summary>
	/// The select method used in <see langword="from"/>-<see langword="in"/>-<see langword="select"/>
	/// clause.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	/// <typeparam name="TResult">The result type.</typeparam>
	/// <param name="this">The list.</param>
	/// <param name="selector">The selector that is used for conversion.</param>
	/// <returns>The array of target result elements.</returns>
	public static TResult[] Select<T, TResult>(this scoped ReadOnlySpan<T> @this, Func<T, TResult> selector)
	{
		var result = new TResult[@this.Length];
		var i = 0;
		foreach (var element in @this)
		{
			result[i++] = selector(element);
		}

		return result;
	}

	/// <summary>
	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})" path="/summary"/>
	/// </summary>
	/// <param name="this">A <see cref="ReadOnlySpan{T}"/> to filter.</param>
	/// <param name="predicate">
	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})" path="/param[@name='predicate']"/>
	/// </param>
	/// <returns>A <typeparamref name="T"/>[] that contains elements form the input sequence that satisfy the condition.</returns>
	public static T[] Where<T>(this scoped ReadOnlySpan<T> @this, Func<T, bool> predicate)
	{
		var result = new T[@this.Length];
		var i = 0;
		foreach (var element in @this)
		{
			if (predicate(element))
			{
				result[i++] = element;
			}
		}

		return result[..i];
	}

	/// <inheritdoc cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public static ReadOnlySpan<T> OrderBy<T, TKey>(this scoped ReadOnlySpan<T> @this, Func<T, TKey> keySelector) where TKey : IComparable<TKey>
	{
		var copied = new T[@this.Length];
		@this.CopyTo(copied);
		Array.Sort(copied, (a, b) => keySelector(a).CompareTo(keySelector(b)));

		return copied;
	}
}
