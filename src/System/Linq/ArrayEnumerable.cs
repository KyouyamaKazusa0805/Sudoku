namespace System.Linq;

/// <summary>
/// Provides with the LINQ-related methods on type <see cref="Array"/>, especially for the one-dimensional array.
/// </summary>
public static class ArrayEnumerable
{
	/// <summary>
	/// Totals up the number of elements that satisfy the specified condition.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array.</param>
	/// <param name="predicate">The condition.</param>
	/// <returns>The number of elements satisfying the specified condition.</returns>
	public static int Count<T>(this T[] @this, Func<T, bool> predicate)
	{
		var result = 0;
		foreach (var element in @this)
		{
			if (predicate(element))
			{
				result++;
			}
		}

		return result;
	}

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
	/// An array of <typeparamref name="TResult"/> instances being the result of invoking the transform function on each element of <paramref name="this"/>.
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

	/// <inheritdoc cref="Enumerable.Cast{TResult}(IEnumerable)"/>
	public static TResult[] Cast<TResult>(this object[] @this)
	{
		var result = new TResult[@this.Length];
		for (var i = 0; i < @this.Length; i++)
		{
			result[i] = (TResult)@this[i];
		}

		return result;
	}

#if false
	/// <summary>
	/// <inheritdoc cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})" path="/summary"/>
	/// </summary>
	/// <typeparam name="T">
	/// <inheritdoc cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})" path="/typeparam[@name='T']"/>
	/// </typeparam>
	/// <typeparam name="TKey">
	/// <inheritdoc cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})" path="/typeparam[@name='TKey']"/>
	/// </typeparam>
	/// <param name="this">
	/// <inheritdoc cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})" path="/param[@name='source']"/>
	/// </param>
	/// <param name="keySelector">
	/// <inheritdoc cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})" path="/param[@name='keySelector']"/>
	/// </param>
	/// <returns>
	/// An array of <typeparamref name="T"/> whose elements are sorted according to a key.
	/// </returns>
	public static T[] OrderBy<T, TKey>(this T[] @this, Func<T, TKey> keySelector)
	{
		var copied = new T[@this.Length];
		Array.Copy(@this, copied, @this.Length);
		Array.Sort(copied, (a, b) => Comparer<TKey>.Default.Compare(keySelector(a), keySelector(b)));
		return copied;
	}

	/// <inheritdoc cref="OrderBy{T, TKey}(T[], Func{T, TKey})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] ThenBy<T, TKey>(this T[] @this, Func<T, TKey> keySelector) => @this.OrderBy(keySelector);
#endif

	/// <summary>
	/// <inheritdoc cref="Enumerable.Aggregate{TSource}(IEnumerable{TSource}, Func{TSource, TSource, TSource})" path="/summary"/>
	/// </summary>
	/// <typeparam name="TSource">
	/// <inheritdoc
	///     cref="Enumerable.Aggregate{TSource}(IEnumerable{TSource}, Func{TSource, TSource, TSource})"
	///     path="/typeparam[@name='TSource']"
	/// />
	/// </typeparam>
	/// <param name="this">An array of <typeparamref name="TSource"/> elementsto aggregate over.</param>
	/// <param name="func">
	/// <inheritdoc cref="Enumerable.Aggregate{TSource}(IEnumerable{TSource}, Func{TSource, TSource, TSource})" path="/param[@name='func']"/>
	/// </param>
	/// <returns>
	/// <inheritdoc cref="Enumerable.Aggregate{TSource}(IEnumerable{TSource}, Func{TSource, TSource, TSource})" path="/returns"/>
	/// </returns>
	public static TSource? Aggregate<TSource>(this TSource[] @this, Func<TSource?, TSource?, TSource> func)
	{
		var result = default(TSource);
		foreach (var element in @this)
		{
			result = func(result, element);
		}

		return result;
	}

	/// <inheritdoc cref="Enumerable.Zip{TFirst, TSecond}(IEnumerable{TFirst}, IEnumerable{TSecond})"/>
	/// <param name="this">
	/// <inheritdoc cref="Enumerable.Zip{TFirst, TSecond}(IEnumerable{TFirst}, IEnumerable{TSecond})" path="/param[@name='first']"/>
	/// </param>
	/// <param name="other">
	/// <inheritdoc cref="Enumerable.Zip{TFirst, TSecond}(IEnumerable{TFirst}, IEnumerable{TSecond})" path="/param[@name='second']"/>
	/// </param>
	public static (TFirst, TSecond)[] Zip<TFirst, TSecond>(this TFirst[] @this, TSecond[] other)
	{
		if (@this.Length != other.Length)
		{
			throw new InvalidOperationException("Two arrays should be of same length.");
		}

		var result = new (TFirst, TSecond)[@this.Length];
		for (var i = 0; i < @this.Length; i++)
		{
			result[i] = (@this[i], other[i]);
		}

		return result;
	}

	/// <inheritdoc cref="SequenceEquals{T}(T[], T[], Func{T, T, bool})"/>
	public static bool SequenceEquals<T>(this T[] @this, T[] other) where T : IEqualityOperators<T, T, bool>
	{
		if (@this.Length != other.Length)
		{
			return false;
		}

		for (var i = 0; i < @this.Length; i++)
		{
			if (@this[i] != other[i])
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Compares elements from two arrays one by one respetively.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array to be compared.</param>
	/// <param name="other">The other array to be compared.</param>
	/// <param name="equalityComparer">
	/// A method that compares two <typeparamref name="T"/> elements, and returns a <see cref="bool"/> result
	/// indicating whether two elements are considered equal.
	/// </param>
	/// <returns>A <see cref="bool"/> result indicating whether two arrays are considered equal.</returns>
	public static bool SequenceEquals<T>(this T[] @this, T[] other, Func<T, T, bool> equalityComparer)
	{
		if (@this.Length != other.Length)
		{
			return false;
		}

		for (var i = 0; i < @this.Length; i++)
		{
			if (!equalityComparer(@this[i], other[i]))
			{
				return false;
			}
		}

		return true;
	}
}
