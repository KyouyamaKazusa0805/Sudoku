namespace System.Linq;

/// <summary>
/// Provides LINQ-based extension methods on <see cref="ReadOnlySpan{T}"/>.
/// </summary>
/// <seealso cref="ReadOnlySpan{T}"/>
public static class SpanEnumerable
{
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

		return (TResult[])[.. result];
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

		return result.AsReadOnlySpan()[..i];
	}

	/// <inheritdoc cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public static ReadOnlySpan<T> OrderBy<T, TKey>(this scoped ReadOnlySpan<T> @this, Func<T, TKey> keySelector) where TKey : IComparable<TKey>
	{
		var copied = new T[@this.Length];
		@this.CopyTo(copied);
		Array.Sort(copied, (a, b) => keySelector(a).CompareTo(keySelector(b)));

		return copied;
	}

	/// <inheritdoc cref="Enumerable.First{TSource}(IEnumerable{TSource})"/>
	public static T First<T>(this scoped ReadOnlySpan<T> @this, Func<T, bool> condition)
	{
		foreach (var element in @this)
		{
			if (condition(element))
			{
				return element;
			}
		}

		throw new InvalidOperationException("The collection does not contain any possible element satisfying the specified condition.");
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

	/// <inheritdoc cref="Enumerable.Aggregate{TSource}(IEnumerable{TSource}, Func{TSource, TSource, TSource})"/>
	public static TSource Aggregate<TSource>(this scoped ReadOnlySpan<TSource> @this, Func<TSource, TSource, TSource> func)
	{
		var result = default(TSource)!;
		foreach (var element in @this)
		{
			result = func(result, element);
		}

		return result;
	}

	/// <inheritdoc cref="Enumerable.Aggregate{TSource, TAccumulate}(IEnumerable{TSource}, TAccumulate, Func{TAccumulate, TSource, TAccumulate})"/>
	public static TAccumulate Aggregate<TSource, TAccumulate>(this scoped ReadOnlySpan<TSource> @this, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
	{
		var result = seed;
		foreach (var element in @this)
		{
			result = func(result, element);
		}

		return result;
	}

	/// <summary>
	/// Get all subsets from the specified number of the values to take.
	/// </summary>
	/// <param name="this">The array.</param>
	/// <param name="count">The number of elements you want to take.</param>
	/// <returns>All subsets.</returns>
	public static T[][] GetSubsets<T>(this scoped ReadOnlySpan<T> @this, int count)
	{
		if (count == 0)
		{
			return [];
		}

		var result = new List<T[]>();
		g(@this.Length, count, count, stackalloc int[count], @this, result);
		return [.. result];


		static void g(int last, int count, int index, scoped Span<int> tempArray, scoped ReadOnlySpan<T> @this, List<T[]> resultList)
		{
			for (var i = last; i >= index; i--)
			{
				tempArray[index - 1] = i - 1;
				if (index > 1)
				{
					g(i - 1, count, index - 1, tempArray, @this, resultList);
				}
				else
				{
					var temp = new T[count];
					for (var j = 0; j < tempArray.Length; j++)
					{
						temp[j] = @this[tempArray[j]];
					}

					resultList.Add(temp);
				}
			}
		}
	}
}
