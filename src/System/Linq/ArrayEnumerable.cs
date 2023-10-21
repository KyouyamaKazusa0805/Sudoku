using System.Collections;
using System.Numerics;

namespace System.Linq;

/// <summary>
/// Provides with the LINQ-related methods on type <see cref="Array"/>, especially for the one-dimensional array.
/// </summary>
public static class ArrayEnumerable
{
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

	/// <summary>
	/// Totals up the number of elements that satisfy the specified condition.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array.</param>
	/// <param name="predicate">The condition.</param>
	/// <returns>The number of elements satisfying the specified condition.</returns>
	public static Count Count<T>(this T[] @this, Func<T, bool> predicate)
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

	/// <summary>
	/// Projects each element of a sequence of a collection, flattens the resulting sequence into one sequence,
	/// and invokes a result selector function on each element therein.
	/// </summary>
	/// <typeparam name="TSource">
	/// <inheritdoc
	///     cref="Enumerable.SelectMany{TSource, TCollection, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TCollection}}, Func{TSource, TCollection, TResult})"
	///     path="/typeparam[@name='TSource']"/>
	/// </typeparam>
	/// <typeparam name="TCollection">
	/// <inheritdoc
	///     cref="Enumerable.SelectMany{TSource, TCollection, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TCollection}}, Func{TSource, TCollection, TResult})"
	///     path="/typeparam[@name='TCollection']"/>
	/// </typeparam>
	/// <typeparam name="TResult">
	/// <inheritdoc
	///     cref="Enumerable.SelectMany{TSource, TCollection, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TCollection}}, Func{TSource, TCollection, TResult})"
	///     path="/typeparam[@name='TResult']"/>
	/// </typeparam>
	/// <param name="source">
	/// <inheritdoc
	///     cref="Enumerable.SelectMany{TSource, TCollection, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TCollection}}, Func{TSource, TCollection, TResult})"
	///     path="/param[@name='source']"/>
	/// </param>
	/// <param name="collectionSelector">
	/// <inheritdoc
	///     cref="Enumerable.SelectMany{TSource, TCollection, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TCollection}}, Func{TSource, TCollection, TResult})"
	///     path="/param[@name='collectionSelector']"/>
	/// </param>
	/// <param name="resultSelector">
	/// <inheritdoc
	///     cref="Enumerable.SelectMany{TSource, TCollection, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TCollection}}, Func{TSource, TCollection, TResult})"
	///     path="/param[@name='resultSelector']"/>
	/// </param>
	/// <returns>
	/// A same type of collection whose elements are the result of invoking the one-to-many transform function
	/// <paramref name="collectionSelector"/> on each element of <paramref name="source"/> and then mapping each of those sequence elements
	/// and their corresponding source element to a result element.
	/// </returns>
	public static TResult[] SelectMany<TSource, TCollection, TResult>(
		this TSource[] source,
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
	/// Filters a sequence of values based on a predicate.
	/// </summary>
	/// <typeparam name="T">The type of the elements of source.</typeparam>
	/// <param name="this">An array of <typeparamref name="T"/> instances to filter.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <returns>
	/// An array of <typeparamref name="T"/> instances that contains elements from the input sequence that satisfy the condition.
	/// </returns>
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

	/// <summary>
	/// Computes the sum of the sequence of <typeparamref name="TInterim"/> values that are obtained by invoking a transform function
	/// on each element of the input sequence.
	/// </summary>
	/// <typeparam name="T">The type of element of <paramref name="this"/>.</typeparam>
	/// <typeparam name="TInterim">The type of interim variables.</typeparam>
	/// <param name="this">
	/// <inheritdoc cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, int})" path="/param[@name='source']"/>
	/// </param>
	/// <param name="selector">
	/// <inheritdoc cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, int})" path="/param[@name='selector']"/>
	/// </param>
	/// <returns>
	/// <inheritdoc cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, int})" path="/returns"/>
	/// </returns>
	public static TInterim Sum<T, TInterim>(this T[] @this, Func<T, TInterim> selector)
		where TInterim : IAdditionOperators<TInterim, TInterim, TInterim>, IAdditiveIdentity<TInterim, TInterim>
	{
		var result = TInterim.AdditiveIdentity;
		foreach (var element in @this)
		{
			result += selector(element);
		}

		return result;
	}

	/// <summary>
	/// Invokes a transform function on each element of a sequence and returns the minimum <typeparamref name="TInterim"/> value.
	/// </summary>
	/// <typeparam name="T">The type of the elements of <paramref name="this"/>.</typeparam>
	/// <typeparam name="TInterim">The type of projected values after the transform function invoked.</typeparam>
	/// <param name="this">A sequence of values to determine the minimum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <returns>The value of type <typeparamref name="TInterim"/> that corresponds to the minimum value in the sequence.</returns>
	public static TInterim Min<T, TInterim>(this T[] @this, Func<T, TInterim> selector)
		where TInterim : IMinMaxValue<TInterim>, IComparisonOperators<TInterim, TInterim, bool>
	{
		var result = TInterim.MaxValue;
		foreach (var element in @this)
		{
			var projectedElement = selector(element);
			if (projectedElement <= result)
			{
				result = projectedElement;
			}
		}

		return result;
	}

	/// <summary>
	/// Invokes a transform function on each element of a sequence and returns the maximum <typeparamref name="TInterim"/> value.
	/// </summary>
	/// <typeparam name="T">The type of the elements of <paramref name="this"/>.</typeparam>
	/// <typeparam name="TInterim">The type of projected values after the transform function invoked.</typeparam>
	/// <param name="this">A sequence of values to determine the maximum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <returns>The value of type <typeparamref name="TInterim"/> that corresponds to the maximum value in the sequence.</returns>
	public static TInterim Max<T, TInterim>(this T[] @this, Func<T, TInterim> selector)
		where TInterim : IMinMaxValue<TInterim>, IComparisonOperators<TInterim, TInterim, bool>
	{
		var result = TInterim.MinValue;
		foreach (var element in @this)
		{
			var projectedElement = selector(element);
			if (projectedElement >= result)
			{
				result = projectedElement;
			}
		}

		return result;
	}

	/// <summary>
	/// <inheritdoc cref="Enumerable.Aggregate{TSource}(IEnumerable{TSource}, Func{TSource, TSource, TSource})" path="/summary"/>
	/// </summary>
	/// <typeparam name="TSource">
	/// <inheritdoc
	///     cref="Enumerable.Aggregate{TSource}(IEnumerable{TSource}, Func{TSource, TSource, TSource})"
	///     path="/typeparam[@name='TSource']"/>
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

	/// <summary>
	/// Get all subsets from the specified number of the values to take.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array.</param>
	/// <param name="count">The number of elements you want to take.</param>
	/// <returns>
	/// The subsets of the list.
	/// For example, if the input array is <c>[1, 2, 3]</c> and the argument <paramref name="count"/> is 2, the result will be
	/// <code>
	/// [[1, 2], [1, 3], [2, 3]]
	/// </code>
	/// 3 cases.
	/// </returns>
	public static T[][] GetSubsets<T>(this T[] @this, Count count)
	{
		if (count == 0)
		{
			return [];
		}

		var result = new List<T[]>();
		g(@this.Length, count, count, stackalloc Offset[count], @this, result);
		return [.. result];


		static void g(Offset last, Count count, Offset index, scoped Span<Offset> tempArray, T[] @this, List<T[]> resultList)
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

	/// <summary>
	/// Get all combinations that each sub-array only choose one.
	/// </summary>
	/// <param name="this">The jigsaw array.</param>
	/// <returns>
	/// All combinations that each sub-array choose one.
	/// For example, if one array is <c>[[1, 2, 3], [1, 3], [1, 4, 7, 10]]</c>, the final combinations will be
	/// <code>
	/// [
	///     [1, 1, 1], [1, 1, 4], [1, 1, 7], [1, 1, 10],
	///     [1, 3, 1], [1, 3, 4], [1, 3, 7], [1, 3, 10],
	///     [2, 1, 1], [2, 1, 4], [2, 1, 7], [2, 1, 10],
	///     [2, 3, 1], [2, 3, 4], [2, 3, 7], [2, 3, 10],
	///     [3, 1, 1], [3, 1, 4], [3, 1, 7], [3, 1, 10],
	///     [3, 3, 1], [3, 3, 4], [3, 3, 7], [3, 3, 10]
	/// ]
	/// </code>
	/// 24 cases.
	/// </returns>
	public static unsafe T[][] GetExtractedCombinations<T>(this T[][] @this)
	{
		var length = @this.Length;
		var resultCount = 1;
		var tempArray = stackalloc Offset[length];
		for (var i = 0; i < length; i++)
		{
			tempArray[i] = -1;
			resultCount *= @this[i].Length;
		}

		var result = new T[resultCount][];
		var m = -1;
		var n = -1;
		do
		{
			if (m < length - 1)
			{
				m++;
			}

			var value = tempArray + m;
			(*value)++;
			if (*value > @this[m].Length - 1)
			{
				*value = -1;
				m -= 2; // Backtrack.
			}

			if (m == length - 1)
			{
				n++;
				result[n] = new T[m + 1];
				for (var i = 0; i <= m; i++)
				{
					result[n][i] = @this[i][tempArray[i]];
				}
			}
		} while (m >= -1);

		return result;
	}
}
