namespace System.Linq;

/// <summary>
/// Provides LINQ-based extension methods on <see cref="Span{T}"/> and <see cref="ReadOnlySpan{T}"/>.
/// </summary>
/// <seealso cref="Span{T}"/>
/// <seealso cref="ReadOnlySpan{T}"/>
public static class SpanEnumerable
{
	/// <inheritdoc cref="Select{T, TResult}(ReadOnlySpan{T}, Func{T, TResult})"/>
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
	/// Projects each element in the current instance into the target-typed span of element type <typeparamref name="TResult"/>,
	/// using the specified function to convert.
	/// </summary>
	/// <typeparam name="T">The type of each elements in the span.</typeparam>
	/// <typeparam name="TResult">The type of target value.</typeparam>
	/// <param name="this">The source elements.</param>
	/// <param name="selector">The selector.</param>
	/// <returns>An array of <typeparamref name="TResult"/> elements.</returns>
	public static ReadOnlySpan<TResult> Select<T, TResult>(this scoped ReadOnlySpan<T> @this, Func<T, TResult> selector)
	{
		var result = new TResult[@this.Length];
		var i = 0;
		foreach (var element in @this)
		{
			result[i++] = selector(element);
		}

		return result;
	}

	/// <inheritdoc cref="ArrayEnumerable.SelectMany{TSource, TCollection, TResult}(TSource[], Func{TSource, IEnumerable{TCollection}}, Func{TSource, TCollection, TResult})"/>
	public static ReadOnlySpan<TResult> SelectMany<TSource, TCollection, TResult>(
		this scoped ReadOnlySpan<TSource> source,
		Func<TSource, IEnumerable<TCollection>> collectionSelector,
		Func<TSource, TCollection, TResult> resultSelector
	)
	{
		var length = source.Length;
		var result = new List<TResult>(length << 1);
		for (var i = 0; i < length; i++)
		{
			var element = source[i];
			foreach (var subElement in collectionSelector(element))
			{
				result.Add(resultSelector(element, subElement));
			}
		}

		return [.. result];
	}

	/// <summary>
	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})" path="/summary"/>
	/// </summary>
	/// <param name="this">A <see cref="ReadOnlySpan{T}"/> to filter.</param>
	/// <param name="predicate">
	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})" path="/param[@name='predicate']"/>
	/// </param>
	/// <returns>A <typeparamref name="T"/>[] that contains elements form the input sequence that satisfy the condition.</returns>
	public static ReadOnlySpan<T> Where<T>(this scoped ReadOnlySpan<T> @this, Func<T, bool> predicate)
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

		return result.AsSpan()[..i];
	}

	/// <inheritdoc cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public static ReadOnlySpan<T> OrderBy<T, TKey>(this scoped ReadOnlySpan<T> @this, Func<T, TKey> keySelector) where TKey : IComparable<TKey>
	{
		var copied = new T[@this.Length];
		@this.CopyTo(copied);
		Array.Sort(copied, (a, b) => keySelector(a).CompareTo(keySelector(b)));

		return copied;
	}

	/// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public static T? FirstOrDefault<T>(this scoped ReadOnlySpan<T> @this, Func<T, bool> condition)
	{
		foreach (var element in @this)
		{
			if (condition(element))
			{
				return element;
			}
		}

		return default;
	}
}
