namespace System.Linq;

/// <summary>
/// Provides with the LINQ-related methods on type <see cref="Array"/>, especially for the one-dimensional array.
/// </summary>
public static class ArrayEnumerable
{
	/// <summary>
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/summary"/>
	/// </summary>
	/// <param name="this">
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/param[@name='source']"/>
	/// </param>
	/// <param name="selector">
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/param[@name='selector']"/>
	/// </param>
	/// <returns>
	/// A <typeparamref name="T"/>[] whose elements are the result of invoking the transform function on each element of <paramref name="this"/>.
	/// </returns>
	public static TResult[] Select<T, TResult>(this T[] @this, Func<T, TResult> selector)
	{
		var length = @this.Length;
		var result = new TResult[length];
		for (var i = 0; i < length; i++)
		{
			result[i] = selector(@this[i]);
		}

		return result;
	}

	/// <inheritdoc cref="Enumerable.SelectMany{TSource, TCollection, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TCollection}}, Func{TSource, TCollection, TResult})"/>
	public static TResult[] SelectMany<TSource, TCollection, TResult>(
		this TSource[] source,
		Func<TSource, TCollection[]> collectionSelector,
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
	/// Filters a sequence of values based on a predicate.
	/// </summary>
	/// <typeparam name="T">The type of the elements of source.</typeparam>
	/// <param name="this">A (An) <typeparamref name="T"/>[] to filter.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <returns>A (An) <typeparamref name="T"/>[] that contains elements from the input sequence that satisfy the condition.</returns>
	public static T[] Where<T>(this T[] @this, Func<T, bool> predicate)
	{
		var (length, finalIndex) = (@this.Length, 0);
		var result = new T[length];
		for (var i = 0; i < length; i++)
		{
			if (predicate(@this[i]))
			{
				result[finalIndex++] = @this[i];
			}
		}

		return result[..finalIndex];
	}
}
