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
	/// Filters the array, removing elements not of type <typeparamref name="TResult"/>.
	/// </summary>
	/// <typeparam name="TResult">The type of the target elements.</typeparam>
	/// <param name="this">The array to be filtered.</param>
	/// <returns>A list of <typeparamref name="TResult"/> elements.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TResult[] OfType<TResult>(this object[] @this)
		=> from element in @this where element is TResult select (TResult)element;

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

	/// <inheritdoc cref="Count{T}(T[], Func{T, bool})"/>
	public static unsafe int CountUnsafe<T>(this T[] @this, delegate*<T, bool> predicate)
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
	/// Sum all elements up and return the result.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array that contains a list of elements to be calculated.</param>
	/// <returns>A <typeparamref name="T"/> instance as the result.</returns>
	public static T Sum<T>(this T[] @this) where T : IAdditiveIdentity<T, T>, IAdditionOperators<T, T, T>
	{
		var result = T.AdditiveIdentity;
		foreach (ref readonly var element in @this.AsReadOnlySpan())
		{
			result += element;
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public static TSource? MinBy<TSource, TKey>(this TSource[] @this, Func<TSource, TKey> keySelector)
		where TKey : IComparable<TKey>, allows ref struct
	{
		var result = default(TSource);
		var minValue = default(TKey);
		foreach (ref readonly var element in @this.AsReadOnlySpan())
		{
			var elementKey = keySelector(element);
			if (elementKey.CompareTo(minValue) <= 0)
			{
				result = element;
				minValue = elementKey;
			}
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public static TSource? MinBy<TSource, TKey, TComparer>(this TSource[] @this, Func<TSource, TKey> keySelector, TComparer? comparer)
		where TKey : allows ref struct
		where TComparer : IComparer<TKey>, new(), allows ref struct
	{
		comparer ??= new();

		var result = default(TSource);
		var minValue = default(TKey);
		foreach (ref readonly var element in @this.AsReadOnlySpan())
		{
			var elementKey = keySelector(element);
			if (comparer.Compare(elementKey, minValue) <= 0)
			{
				result = element;
				minValue = elementKey;
			}
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public static TSource? MaxBy<TSource, TKey>(this TSource[] @this, Func<TSource, TKey> keySelector)
		where TKey : IComparable<TKey>, allows ref struct
	{
		var result = default(TSource);
		var maxValue = default(TKey);
		foreach (ref readonly var element in @this.AsReadOnlySpan())
		{
			var elementKey = keySelector(element);
			if (elementKey.CompareTo(maxValue) >= 0)
			{
				result = element;
				maxValue = elementKey;
			}
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public static TSource? MaxBy<TSource, TKey, TComparer>(this TSource[] @this, Func<TSource, TKey> keySelector, TComparer? comparer)
		where TKey : allows ref struct
		where TComparer : IComparer<TKey>, new(), allows ref struct
	{
		comparer ??= new();

		var result = default(TSource);
		var maxValue = default(TKey);
		foreach (ref readonly var element in @this.AsReadOnlySpan())
		{
			var elementKey = keySelector(element);
			if (comparer.Compare(elementKey, maxValue) >= 0)
			{
				result = element;
				maxValue = elementKey;
			}
		}
		return result;
	}

	/// <returns>
	/// An array of <typeparamref name="TResult"/> instances being the result
	/// of invoking the transform function on each element of <paramref name="source"/>.
	/// </returns>
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/>
	public static TResult[] Select<T, TResult>(this T[] source, Func<T, TResult> selector)
	{
		var length = source.Length;
		var result = new TResult[length];
		for (var i = 0; i < length; i++)
		{
			result[i] = selector(source[i]);
		}
		return result;
	}

	/// <summary>
	/// An array of <typeparamref name="TResult"/> instances being the result
	/// of invoking the transform function on each element of <paramref name="source"/>, and its indices.
	/// </summary>
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, int, TResult})"/>
	public static TResult[] Select<T, TResult>(this T[] source, Func<T, int, TResult> selector)
	{
		var length = source.Length;
		var result = new TResult[length];
		for (var i = 0; i < length; i++)
		{
			result[i] = selector(source[i], i);
		}
		return result;
	}

	/// <summary>
	/// Projects each element of a sequence of a collection, flattens the resulting sequence into one sequence,
	/// and invokes a result selector function on each element therein.
	/// </summary>
	/// <returns>
	/// A same type of collection whose elements are the result of invoking the one-to-many transform function
	/// <paramref name="collectionSelector"/> on each element of <paramref name="source"/> and then mapping each of those sequence elements
	/// and their corresponding source element to a result element.
	/// </returns>
	/// <inheritdoc cref="Enumerable.SelectMany{TSource, TCollection, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TCollection}}, Func{TSource, TCollection, TResult})"/>
	public static TResult[] SelectMany<TSource, TCollection, TResult>(
		this TSource[] source,
		Func<TSource, ReadOnlySpan<TCollection>> collectionSelector,
		Func<TSource, TCollection, TResult> resultSelector
	)
	{
		var length = source.Length;
		var result = new List<TResult>(length << 1);
		for (var i = 0; i < length; i++)
		{
			var element = source[i];
			foreach (ref readonly var subElement in collectionSelector(element))
			{
				result.AddRef(resultSelector(element, subElement));
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
	/// Computes the sum of the sequence of <typeparamref name="TResult"/> values that are obtained by invoking a transform function
	/// on each element of the input sequence.
	/// </summary>
	/// <typeparam name="T">The type of element of <paramref name="source"/>.</typeparam>
	/// <typeparam name="TResult">The type of the return value.</typeparam>
	/// <param name="source">Indicates the source values.</param>
	/// <param name="selector">The method that projects the value into an instance of type <typeparamref name="TResult"/>.</param>
	/// <returns>The result value.</returns>
	public static TResult Sum<T, TResult>(this T[] source, Func<T, TResult> selector)
		where TResult : IAdditionOperators<TResult, TResult, TResult>, IAdditiveIdentity<TResult, TResult>
	{
		var result = TResult.AdditiveIdentity;
		foreach (var element in source)
		{
			result += selector(element);
		}
		return result;
	}

	/// <inheritdoc cref="Sum{T, TResult}(T[], Func{T, TResult})"/>
	public static unsafe TResult SumUnsafe<T, TResult>(this T[] source, delegate*<T, TResult> selector)
		where TResult : IAdditionOperators<TResult, TResult, TResult>, IAdditiveIdentity<TResult, TResult>
	{
		var result = TResult.AdditiveIdentity;
		foreach (var element in source)
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
			var elementCasted = selector(element);
			if (elementCasted <= result)
			{
				result = elementCasted;
			}
		}
		return result;
	}

	/// <inheritdoc cref="Min{T, TInterim}(T[], Func{T, TInterim})"/>
	public static unsafe TInterim MinUnsafe<T, TInterim>(this T[] @this, delegate*<T, TInterim> selector)
		where TInterim : IMinMaxValue<TInterim>, IComparisonOperators<TInterim, TInterim, bool>
	{
		var result = TInterim.MaxValue;
		foreach (var element in @this)
		{
			var elementCasted = selector(element);
			if (elementCasted <= result)
			{
				result = elementCasted;
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
			var elementCasted = selector(element);
			if (elementCasted >= result)
			{
				result = elementCasted;
			}
		}
		return result;
	}

	/// <inheritdoc cref="Max{T, TInterim}(T[], Func{T, TInterim})"/>
	public static unsafe TInterim MaxUnsafe<T, TInterim>(this T[] @this, delegate*<T, TInterim> selector)
		where TInterim : IMinMaxValue<TInterim>, IComparisonOperators<TInterim, TInterim, bool>
	{
		var result = TInterim.MinValue;
		foreach (var element in @this)
		{
			var elementCasted = selector(element);
			if (elementCasted >= result)
			{
				result = elementCasted;
			}
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.ThenBy{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ArrayOrderedEnumerable<T> OrderBy<T, TKey>(this T[] @this, Func<T, TKey> selector)
		=> new(
			@this,
			(l, r) => (selector(l), selector(r)) switch
			{
				(IComparable<TKey> left, var right) => left.CompareTo(right),
				var (a, b) => Comparer<TKey>.Default.Compare(a, b)
			}
		);

	/// <inheritdoc cref="Enumerable.ThenByDescending{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ArrayOrderedEnumerable<T> OrderByDescending<T, TKey>(this T[] @this, Func<T, TKey> selector)
		=> new(
			@this,
			(l, r) => (selector(l), selector(r)) switch
			{
				(IComparable<TKey> left, var right) => -left.CompareTo(right),
				var (a, b) => -Comparer<TKey>.Default.Compare(a, b)
			}
		);

	/// <inheritdoc cref="Enumerable.GroupBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public static ArrayGrouping<TSource, TKey>[] GroupBy<TSource, TKey>(this TSource[] values, Func<TSource, TKey> keySelector)
		where TKey : notnull
	{
		var tempDictionary = new Dictionary<TKey, List<TSource>>(values.Length >> 2);
		foreach (var element in values)
		{
			var key = keySelector(element);
			if (!tempDictionary.TryAdd(key, [element]))
			{
				tempDictionary[key].AddRef(in element);
			}
		}

		var result = new List<ArrayGrouping<TSource, TKey>>(tempDictionary.Count);
		foreach (var key in tempDictionary.Keys)
		{
			var tempValues = tempDictionary[key];
			result.Add(new([.. tempValues], key));
		}
		return [.. result];
	}

	/// <inheritdoc cref="Enumerable.GroupBy{TSource, TKey, TElement}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement})"/>
	public static ArrayGrouping<TElement, TKey>[] GroupBy<TSource, TKey, TElement>(
		this TSource[] values,
		Func<TSource, TKey> keySelector,
		Func<TSource, TElement> elementSelector
	) where TKey : notnull
	{
		var tempDictionary = new Dictionary<TKey, List<TSource>>(values.Length >> 2);
		foreach (var element in values)
		{
			var key = keySelector(element);
			if (!tempDictionary.TryAdd(key, [element]))
			{
				tempDictionary[key].AddRef(in element);
			}
		}

		var result = new List<ArrayGrouping<TElement, TKey>>(tempDictionary.Count);
		foreach (var key in tempDictionary.Keys)
		{
			var tempValues = tempDictionary[key];
			var valuesConverted = from value in tempValues select elementSelector(value);
			result.Add(new([.. valuesConverted], key));
		}
		return [.. result];
	}

	/// <inheritdoc cref="Enumerable.Join{TOuter, TInner, TKey, TResult}(IEnumerable{TOuter}, IEnumerable{TInner}, Func{TOuter, TKey}, Func{TInner, TKey}, Func{TOuter, TInner, TResult})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TResult[] Join<TOuter, TInner, TKey, TResult>(
		this TOuter[] outer,
		TInner[] inner,
		Func<TOuter, TKey> outerKeySelector,
		Func<TInner, TKey> innerKeySelector,
		Func<TOuter, TInner, TResult> resultSelector
	) where TKey : notnull => Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);

	/// <inheritdoc cref="Enumerable.Join{TOuter, TInner, TKey, TResult}(IEnumerable{TOuter}, IEnumerable{TInner}, Func{TOuter, TKey}, Func{TInner, TKey}, Func{TOuter, TInner, TResult}, IEqualityComparer{TKey}?)"/>
	public static TResult[] Join<TOuter, TInner, TKey, TResult>(
		this TOuter[] outer,
		TInner[] inner,
		Func<TOuter, TKey> outerKeySelector,
		Func<TInner, TKey> innerKeySelector,
		Func<TOuter, TInner, TResult> resultSelector,
		IEqualityComparer<TKey>? comparer
	) where TKey : notnull
	{
		comparer ??= EqualityComparer<TKey>.Default;

		var result = new List<TResult>(outer.Length * inner.Length);
		foreach (var outerItem in outer)
		{
			var outerKey = outerKeySelector(outerItem);
			var outerKeyHash = comparer.GetHashCode(outerKey);
			foreach (var innerItem in inner)
			{
				var innerKey = innerKeySelector(innerItem);
				var innerKeyHash = comparer.GetHashCode(innerKey);
				if (outerKeyHash != innerKeyHash)
				{
					// They are not same due to hash code difference.
					continue;
				}

				if (!comparer.Equals(outerKey, innerKey))
				{
					// They are not same due to inequality.
					continue;
				}

				result.AddRef(resultSelector(outerItem, innerItem));
			}
		}
		return [.. result];
	}

	/// <inheritdoc cref="Enumerable.GroupJoin{TOuter, TInner, TKey, TResult}(IEnumerable{TOuter}, IEnumerable{TInner}, Func{TOuter, TKey}, Func{TInner, TKey}, Func{TOuter, IEnumerable{TInner}, TResult})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TResult[] GroupJoin<TOuter, TInner, TKey, TResult>(
		this TOuter[] outer,
		TInner[] inner,
		Func<TOuter, TKey> outerKeySelector,
		Func<TInner, TKey> innerKeySelector,
		Func<TOuter, TInner[], TResult> resultSelector
	) where TKey : notnull => GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);

	/// <inheritdoc cref="Enumerable.GroupJoin{TOuter, TInner, TKey, TResult}(IEnumerable{TOuter}, IEnumerable{TInner}, Func{TOuter, TKey}, Func{TInner, TKey}, Func{TOuter, IEnumerable{TInner}, TResult}, IEqualityComparer{TKey}?)"/>
	public static TResult[] GroupJoin<TOuter, TInner, TKey, TResult>(
		this TOuter[] outer,
		TInner[] inner,
		Func<TOuter, TKey> outerKeySelector,
		Func<TInner, TKey> innerKeySelector,
		Func<TOuter, TInner[], TResult> resultSelector,
		IEqualityComparer<TKey>? comparer
	) where TKey : notnull
	{
		comparer ??= EqualityComparer<TKey>.Default;

		var innerKvps = from element in inner select new KeyValuePair<TKey, TInner>(innerKeySelector(element), element);
		var result = new List<TResult>();
		foreach (var outerItem in outer)
		{
			var outerKey = outerKeySelector(outerItem);
			var outerKeyHash = comparer.GetHashCode(outerKey);
			var satisfiedInnerKvps = new List<TInner>(innerKvps.Length);
			foreach (var kvp in innerKvps)
			{
				ref readonly var innerKey = ref kvp.KeyRef();
				ref readonly var innerItem = ref kvp.ValueRef();
				var innerKeyHash = comparer.GetHashCode(innerKey);
				if (outerKeyHash != innerKeyHash)
				{
					// They are not same due to hash code difference.
					continue;
				}

				if (!comparer.Equals(outerKey, innerKey))
				{
					// They are not same due to inequality.
					continue;
				}

				satisfiedInnerKvps.AddRef(in innerItem);
			}

			result.AddRef(resultSelector(outerItem, [.. satisfiedInnerKvps]));
		}
		return [.. result];
	}

	/// <summary>
	/// Applies an accumulator function over a sequence.
	/// </summary>
	/// <typeparam name="TSource">The type of each element.</typeparam>
	/// <param name="this">An array of elements to be aggregated over.</param>
	/// <param name="func">The function that aggregates the values.</param>
	/// <returns>An element accumulated, of type <typeparamref name="TSource"/>.</returns>
	public static TSource? Aggregate<TSource>(this TSource[] @this, Func<TSource?, TSource?, TSource> func)
	{
		var result = default(TSource);
		foreach (var element in @this)
		{
			result = func(result, element);
		}
		return result;
	}

	/// <summary>
	/// Applies an accumulator function over a sequence. The initial value can be set in this method.
	/// </summary>
	/// <typeparam name="TSource">The type of each element.</typeparam>
	/// <typeparam name="TAccumulate">The type of the accumulated values.</typeparam>
	/// <param name="this">An array of elements to be aggregated over.</param>
	/// <param name="seed">The initial value.</param>
	/// <param name="func">The function that aggregates the values.</param>
	/// <returns>An element accumulated, of type <typeparamref name="TSource"/>.</returns>
	public static TAccumulate Aggregate<TSource, TAccumulate>(this TSource[] @this, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
		where TAccumulate : allows ref struct
	{
		var result = seed;
		foreach (var element in @this)
		{
			result = func(result, element);
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.Zip{TFirst, TSecond}(IEnumerable{TFirst}, IEnumerable{TSecond})"/>
	public static (TFirst Left, TSecond Right)[] Zip<TFirst, TSecond>(this TFirst[] first, TSecond[] second)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(first.Length, second.Length);

		var result = new (TFirst, TSecond)[first.Length];
		for (var i = 0; i < first.Length; i++)
		{
			result[i] = (first[i], second[i]);
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
	/// Compares elements from two arrays one by one respectively.
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
	/// Filters duplicate items from an array.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array to be filtered.</param>
	/// <returns>A new array of elements that doesn't contain any duplicate items.</returns>
	public static T[] Distinct<T>(this T[] @this)
	{
		if (@this.Length == 0 || ReferenceEquals(@this, Array.Empty<T>()))
		{
			return [];
		}

		var tempSet = new HashSet<T>(@this.Length, EqualityComparer<T>.Default);
		var result = new T[@this.Length];
		var i = 0;
		foreach (var element in @this)
		{
			if (tempSet.Add(element))
			{
				result[i++] = element;
			}
		}
		return result[..i];
	}

	/// <inheritdoc cref="Enumerable.DistinctBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public static T[] DistinctBy<T, TKey>(this T[] @this, Func<T, TKey> keySelector)
		where TKey : notnull, IEqualityOperators<TKey, TKey, bool>
	{
		var result = new T[@this.Length];
		var i = 0;
		foreach (var element in @this)
		{
			if (i == 0)
			{
				result[i++] = element;
			}
			else
			{
				var elementKey = keySelector(element);
				var contains = false;
				foreach (var recordedElement in result)
				{
					var recordedElementKey = keySelector(recordedElement);
					if (elementKey == recordedElementKey)
					{
						contains = true;
						break;
					}
				}
				if (!contains)
				{
					result[i++] = element;
				}
			}
		}
		return result[..i];
	}

	/// <inheritdoc cref="Enumerable.DistinctBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IEqualityComparer{TKey})"/>
	public static T[] DistinctBy<T, TKey>(this T[] @this, Func<T, TKey> keySelector, IEqualityComparer<TKey> equalityComparer)
		where TKey : notnull
	{
		var result = new T[@this.Length];
		var i = 0;
		foreach (var element in @this)
		{
			if (i == 0)
			{
				result[i++] = element;
			}
			else
			{
				var elementKey = keySelector(element);
				var hashCodeThis = equalityComparer.GetHashCode(elementKey);

				var contains = false;
				foreach (ref readonly var recordedElement in result.AsReadOnlySpan()[..i])
				{
					var recordedElementKey = keySelector(recordedElement);
					var hashCodeOther = equalityComparer.GetHashCode(recordedElementKey);
					if (hashCodeThis == hashCodeOther && equalityComparer.Equals(elementKey, recordedElementKey))
					{
						contains = true;
						break;
					}
				}
				if (!contains)
				{
					result[i++] = element;
				}
			}
		}
		return result[..i];
	}

	/// <inheritdoc cref="Enumerable.Append{TSource}(IEnumerable{TSource}, TSource)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ArrayAppendIterator<T> Append<T>(this T[] @this, T value) => new(@this, value);

	/// <inheritdoc cref="Enumerable.Prepend{TSource}(IEnumerable{TSource}, TSource)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ArrayPrependIterator<T> Prepend<T>(this T[] @this, T value) => new(@this, value);
}
