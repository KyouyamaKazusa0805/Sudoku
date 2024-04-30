namespace System.Linq;

/// <summary>
/// Provides LINQ-based extension methods on <see cref="ReadOnlySpan{T}"/>.
/// </summary>
/// <seealso cref="ReadOnlySpan{T}"/>
public static class ReadOnlySpanEnumerable
{
	/// <summary>
	/// Try to get the minimal value appeared in the collection.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">
	/// <para>The collection to be used and checked.</para>
	/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
	/// </param>
	/// <returns>The minimal value.</returns>
	public static T Min<T>(this scoped ReadOnlySpan<T> @this) where T : INumber<T>, IMinMaxValue<T>
	{
		var result = T.MaxValue;
		foreach (var element in @this)
		{
			if (element <= result)
			{
				result = element;
			}
		}
		return result;
	}

	/// <inheritdoc cref="MinBy{TSource, TKey}(ReadOnlySpan{TSource}, FuncRefReadOnly{TSource, TKey})"/>
	public static TKey Min<TSource, TKey>(this scoped ReadOnlySpan<TSource> @this, FuncRefReadOnly<TSource, TKey> keySelector)
		where TKey : IMinMaxValue<TKey>?, IComparisonOperators<TKey, TKey, bool>?
	{
		var resultKey = TKey.MaxValue;
		foreach (ref readonly var element in @this)
		{
			var key = keySelector(in element);
			if (key <= resultKey)
			{
				resultKey = key;
			}
		}

		return resultKey;
	}

	/// <summary>
	/// Returns the minimum value in a generic sequence according to a specified key selector function.
	/// </summary>
	/// <typeparam name="TSource">The type of the elements of source.</typeparam>
	/// <typeparam name="TKey">The type of key to compare elements by.</typeparam>
	/// <param name="this">
	/// <para>The collection to be used and checked.</para>
	/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
	/// </param>
	/// <param name="keySelector">A function to extract the key for each element.</param>
	/// <returns>The value with the minimum key in the sequence.</returns>
	public static TSource? MinBy<TSource, TKey>(this scoped ReadOnlySpan<TSource> @this, FuncRefReadOnly<TSource, TKey> keySelector)
		where TKey : IMinMaxValue<TKey>?, IComparisonOperators<TKey, TKey, bool>?
	{
		var (resultKey, result) = (TKey.MaxValue, default(TSource));
		foreach (ref readonly var element in @this)
		{
			if (keySelector(in element) <= resultKey)
			{
				result = element;
			}
		}

		return result;
	}

	/// <summary>
	/// Try to get the minimal value appeared in the collection.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">
	/// <para>The collection to be used and checked.</para>
	/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
	/// </param>
	/// <returns>The minimal value.</returns>
	public static T Max<T>(this scoped ReadOnlySpan<T> @this) where T : INumber<T>, IMinMaxValue<T>
	{
		var result = T.MinValue;
		foreach (var element in @this)
		{
			if (element >= result)
			{
				result = element;
			}
		}
		return result;
	}

	/// <inheritdoc cref="MaxBy{TSource, TKey}(ReadOnlySpan{TSource}, FuncRefReadOnly{TSource, TKey})"/>
	public static TKey? Max<TSource, TKey>(this scoped ReadOnlySpan<TSource> @this, FuncRefReadOnly<TSource, TKey> keySelector)
		where TKey : IMinMaxValue<TKey>?, IComparisonOperators<TKey, TKey, bool>?
	{
		var resultKey = TKey.MinValue;
		foreach (ref readonly var element in @this)
		{
			var key = keySelector(in element);
			if (key >= resultKey)
			{
				resultKey = key;
			}
		}

		return resultKey;
	}

	/// <summary>
	/// Returns the maximum value in a generic sequence according to a specified key selector function.
	/// </summary>
	/// <typeparam name="TSource">The type of the elements of source.</typeparam>
	/// <typeparam name="TKey">The type of key to compare elements by.</typeparam>
	/// <param name="this">
	/// <para>The collection to be used and checked.</para>
	/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
	/// </param>
	/// <param name="keySelector">A function to extract the key for each element.</param>
	/// <returns>The value with the maximum key in the sequence.</returns>
	public static TSource? MaxBy<TSource, TKey>(this scoped ReadOnlySpan<TSource> @this, FuncRefReadOnly<TSource, TKey> keySelector)
		where TKey : IMinMaxValue<TKey>?, IComparisonOperators<TKey, TKey, bool>?
	{
		var (resultKey, result) = (TKey.MinValue, default(TSource));
		foreach (ref readonly var element in @this)
		{
			if (keySelector(in element) >= resultKey)
			{
				result = element;
			}
		}

		return result;
	}

	/// <summary>
	/// Totals up all elements, and return the result of the sum by the specified property calculated from each element.
	/// </summary>
	/// <typeparam name="TSource">The type of the elements of source.</typeparam>
	/// <typeparam name="TKey">The type of key to add up.</typeparam>
	/// <param name="this">
	/// <para>The collection to be used and checked.</para>
	/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
	/// </param>
	/// <param name="keySelector">A function to extract the key for each element.</param>
	/// <returns>The value with the sum key in the sequence.</returns>
	public static TKey Sum<TSource, TKey>(this scoped ReadOnlySpan<TSource> @this, FuncRefReadOnly<TSource, TKey> keySelector)
		where TKey : IMinMaxValue<TKey>?, IAdditionOperators<TKey, TKey, TKey>?
	{
		var result = TKey.MinValue;
		foreach (ref readonly var element in @this)
		{
			result += keySelector(in element);
		}

		return result;
	}

	/// <summary>
	/// Totals up how many elements stored in the specified sequence satisfy the specified condition.
	/// </summary>
	/// <typeparam name="TSource">The type of each element.</typeparam>
	/// <param name="this">
	/// <para>The collection to be used and checked.</para>
	/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
	/// </param>
	/// <param name="condition">The condition.</param>
	/// <returns>The number of elements satisfying the specified condition.</returns>
	public static int Count<TSource>(this scoped ReadOnlySpan<TSource> @this, FuncRefReadOnly<TSource, bool> condition)
	{
		var result = 0;
		foreach (ref readonly var element in @this)
		{
			if (condition(in element))
			{
				result++;
			}
		}

		return result;
	}

	/// <summary>
	/// Checks whether at least one element are satisfied the specified condition.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">
	/// <para>The collection to be used and checked.</para>
	/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
	/// </param>
	/// <param name="match">The <see cref="FuncRefReadOnly{T, TResult}"/> that defines the conditions of the elements to search for.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool Any<T>(this scoped ReadOnlySpan<T> @this, FuncRefReadOnly<T, bool> match)
	{
		foreach (ref readonly var element in @this)
		{
			if (match(in element))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Checks whether all elements are satisfied the specified condition.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">
	/// <para>The collection to be used and checked.</para>
	/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
	/// </param>
	/// <param name="match">The <see cref="FuncRefReadOnly{T, TResult}"/> that defines the conditions of the elements to search for.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool All<T>(this scoped ReadOnlySpan<T> @this, FuncRefReadOnly<T, bool> match)
	{
		foreach (ref readonly var element in @this)
		{
			if (!match(in element))
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Determines whether all elements are of type <typeparamref name="TDerived"/>.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <typeparam name="TDerived">The derived type to be checked.</typeparam>
	/// <param name="this">
	/// <para>The collection to be used and checked.</para>
	/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
	/// </param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool AllAre<T, TDerived>(this scoped ReadOnlySpan<T> @this) where TDerived : T?
	{
		foreach (ref readonly var element in @this)
		{
			if (element is not TDerived)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Skips the specified number of elements, make a new <see cref="ReadOnlySpan{T}"/> instance points to it.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">
	/// <para>The collection to be used and checked.</para>
	/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
	/// </param>
	/// <param name="count">The number of elements to skip.</param>
	/// <returns>
	/// The new instance that points to the first element that has already skipped the specified number of elements.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<T> Skip<T>(this ReadOnlySpan<T> @this, int count) => new(in @this[count]);

	/// <summary>
	/// Retrieves all the elements that match the conditions defined by the specified predicate.
	/// </summary>
	/// <typeparam name="T">The type of the elements of the span.</typeparam>
	/// <param name="this">
	/// <para>The collection to be used and checked.</para>
	/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
	/// </param>
	/// <param name="match">The <see cref="FuncRefReadOnly{T, TResult}"/> that defines the conditions of the elements to search for.</param>
	/// <returns>
	/// A <see cref="ReadOnlySpan{T}"/> containing all the elements that match the conditions defined
	/// by the specified predicate, if found; otherwise, an empty <see cref="ReadOnlySpan{T}"/>.
	/// </returns>
	public static ReadOnlySpan<T> FindAll<T>(this scoped ReadOnlySpan<T> @this, FuncRefReadOnly<T, bool> match)
	{
		var result = new List<T>(@this.Length);
		foreach (ref readonly var element in @this)
		{
			if (match(in element))
			{
				result.Add(element);
			}
		}

		return result.AsReadOnlySpan();
	}

	/// <summary>
	/// Projects each element in the current instance into the target-typed span of element type <typeparamref name="TResult"/>,
	/// using the specified function to convert.
	/// </summary>
	/// <typeparam name="T">The type of each elements in the span.</typeparam>
	/// <typeparam name="TResult">The type of target value.</typeparam>
	/// <param name="this">
	/// <para>The collection to be used and checked.</para>
	/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
	/// </param>
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

	/// <inheritdoc cref="ArrayEnumerable.SelectMany{TSource, TCollection, TResult}(TSource[], Func{TSource, TCollection[]}, Func{TSource, TCollection, TResult})"/>
	public static ReadOnlySpan<TResult> SelectMany<TSource, TCollection, TResult>(
		this scoped ReadOnlySpan<TSource> source,
		Func<TSource, TCollection[]> collectionSelector,
		Func<TSource, TCollection, TResult> resultSelector
	)
	{
		var length = source.Length;
		var result = new List<TResult>(length << 1);
		for (var i = 0; i < length; i++)
		{
			var element = source[i];
			foreach (ref readonly var subElement in collectionSelector(element).AsReadOnlySpan())
			{
				result.Add(resultSelector(element, subElement));
			}
		}

		return (TResult[])[.. result];
	}

	/// <summary>
	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})" path="/summary"/>
	/// </summary>
	/// <param name="this">
	/// <para>The collection to be used and checked.</para>
	/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
	/// </param>
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

	/// <inheritdoc cref="Enumerable.ThenBy{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SpanOrderedEnumerable<T> OrderBy<T, TKey>(this ReadOnlySpan<T> @this, Func<T, TKey> selector)
		=> new(
			@this,
			(Func<T, T, int>[])[
				(l, r) => (selector(l), selector(r)) switch
				{
					(IComparable<TKey> left, var right) => left.CompareTo(right),
					var (a, b) => Comparer<TKey>.Default.Compare(a, b)
				}
			]
		);

	/// <inheritdoc cref="Enumerable.ThenByDescending{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SpanOrderedEnumerable<T> OrderByDescending<T, TKey>(this ReadOnlySpan<T> @this, Func<T, TKey> selector)
		=> new(
			@this,
			(Func<T, T, int>[])[
				(l, r) => (selector(l), selector(r)) switch
				{
					(IComparable<TKey> left, var right) => -left.CompareTo(right),
					var (a, b) => -Comparer<TKey>.Default.Compare(a, b)
				}
			]
		);

	/// <inheritdoc cref="Enumerable.GroupBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public static ReadOnlySpan<SpanGrouping<TSource, TKey>> GroupBy<TSource, TKey>(
		this scoped ReadOnlySpan<TSource> values,
		Func<TSource, TKey> keySelector
	) where TKey : notnull
	{
		var tempDictionary = new Dictionary<TKey, List<TSource>>(values.Length >> 2);
		foreach (var element in values)
		{
			var key = keySelector(element);
			if (!tempDictionary.TryAdd(key, [element]))
			{
				tempDictionary[key].Add(element);
			}
		}

		var result = new List<SpanGrouping<TSource, TKey>>(tempDictionary.Count);
		foreach (var key in tempDictionary.Keys)
		{
			unsafe
			{
				var tempValues = tempDictionary[key];
				result.Add(new(Ref.ToPointer(in tempValues.AsReadOnlySpan()[0]), tempValues.Count, key));
			}
		}
		return result.AsReadOnlySpan();
	}

	/// <inheritdoc cref="Enumerable.First{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public static T First<T>(this scoped ReadOnlySpan<T> @this, Func<T, bool> predicate)
	{
		foreach (var element in @this)
		{
			if (predicate(element))
			{
				return element;
			}
		}

		throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("NoSuchElementSatisfyingCondition"));
	}

	/// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public static T? FirstOrDefault<T>(this scoped ReadOnlySpan<T> @this, Func<T, bool> predicate)
	{
		foreach (var element in @this)
		{
			if (predicate(element))
			{
				return element;
			}
		}

		return default;
	}

	/// <inheritdoc cref="Enumerable.Last{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public static T Last<T>(this scoped ReadOnlySpan<T> @this, Func<T, bool> predicate)
	{
		foreach (var element in @this.EnumerateReversely())
		{
			if (predicate(element))
			{
				return element;
			}
		}

		throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("NoSuchElementSatisfyingCondition"));
	}

	/// <inheritdoc cref="Enumerable.LastOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public static T? LastOrDefault<T>(this scoped ReadOnlySpan<T> @this, Func<T, bool> predicate)
	{
		foreach (var element in @this.EnumerateReversely())
		{
			if (predicate(element))
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
}
