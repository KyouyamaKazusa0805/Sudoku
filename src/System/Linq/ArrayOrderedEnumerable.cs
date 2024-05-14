namespace System.Linq;

/// <summary>
/// Represents an enumerable instance that is based on an array of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Indicates the type of each element.</typeparam>
/// <param name="values">Indicates the values.</param>
/// <param name="selectors">
/// <para>Indicates the selector functions that return <typeparamref name="T"/> instances, to be used as comparison.</para>
/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
/// <include file="../../global-doc-comments.xml" path="//g/csharp12/feature[@name='params-collections']/target[@name='parameter']"/>
/// </param>
public sealed partial class ArrayOrderedEnumerable<T>(
	[PrimaryConstructorParameter(MemberKinds.Field)] T[] values,
	[PrimaryConstructorParameter(MemberKinds.Field)] params Func<T, T, int>[] selectors
)
{
	/// <summary>
	/// Indicates the number of elements stored in the collection.
	/// </summary>
	public int Length => _values.Length;

	/// <summary>
	/// Creates an ordered <typeparamref name="T"/>[] instance.
	/// </summary>
	/// <returns>An ordered <typeparamref name="T"/>[] instance, whose value is from the current enumerable instance.</returns>
	public T[] ArrayOrdered
	{
		get
		{
			var result = _values[..];
			Array.Sort(
				result,
				(l, r) =>
				{
					foreach (var selector in _selectors)
					{
						if (selector(l, r) is var tempResult and not 0)
						{
							return tempResult;
						}
					}
					return 0;
				}
			);
			return result;
		}
	}


	/// <inheritdoc cref="ReadOnlySpan{T}.this[int]"/>
	public ref readonly T this[int index] => ref _values[index];


	/// <summary>
	/// Projects each element into a new transform.
	/// </summary>
	/// <typeparam name="TResult">The type of the result values.</typeparam>
	/// <param name="selector">The selector to be used by transforming the <typeparamref name="T"/> instances.</param>
	/// <returns>A span of <typeparamref name="TResult"/> values.</returns>
	public TResult[] Select<TResult>(Func<T, TResult> selector)
	{
		var result = new List<TResult>(_values.Length);
		foreach (var element in ArrayOrdered)
		{
			result.Add(selector(element));
		}
		return [.. result];
	}

	/// <summary>
	/// Filters the collection using the specified condition.
	/// </summary>
	/// <param name="condition">The condition to be used.</param>
	/// <returns>A span of <typeparamref name="T"/> instances.</returns>
	public T[] Where(Func<T, bool> condition)
	{
		var result = new List<T>(_values.Length);
		foreach (var element in ArrayOrdered)
		{
			if (condition(element))
			{
				result.Add(element);
			}
		}
		return [.. result];
	}

	/// <inheritdoc cref="Enumerable.ThenBy{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ArrayOrderedEnumerable<T> ThenBy<TKey>(Func<T, TKey> selector)
		=> new(
			_values,
			[
				.. _selectors,
				(l, r) => (selector(l), selector(r)) switch
				{
					(IComparable<TKey> left, var right) => left.CompareTo(right),
					var (a, b) => Comparer<TKey>.Default.Compare(a, b)
				}
			]
		);

	/// <inheritdoc cref="Enumerable.ThenByDescending{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ArrayOrderedEnumerable<T> ThenByDescending<TKey>(Func<T, TKey> selector)
		=> new(
			_values,
			[
				.. _selectors,
				(l, r) => (selector(l), selector(r)) switch
				{
					(IComparable<TKey> left, var right) => -left.CompareTo(right),
					var (a, b) => -Comparer<TKey>.Default.Compare(a, b)
				}
			]
		);

	/// <inheritdoc cref="ReadOnlySpanEnumerable.GroupBy{TSource, TKey}(ReadOnlySpan{TSource}, Func{TSource, TKey})"/>
	public ArrayGrouping<T, TKey>[] GroupBy<TKey>(Func<T, TKey> keySelector) where TKey : notnull
	{
		var tempDictionary = new Dictionary<TKey, List<T>>(_values.Length >> 2);
		foreach (var element in ArrayOrdered)
		{
			var key = keySelector(element);
			if (!tempDictionary.TryAdd(key, [element]))
			{
				tempDictionary[key].Add(element);
			}
		}

		var result = new List<ArrayGrouping<T, TKey>>(tempDictionary.Count);
		foreach (var key in tempDictionary.Keys)
		{
			var tempValues = tempDictionary[key];
			result.Add(new([.. tempValues], key));
		}
		return [.. result];
	}

	/// <inheritdoc cref="Enumerable.GroupBy{TSource, TKey, TElement}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement})"/>
	public ArrayGrouping<TElement, TKey>[] GroupBy<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector)
		where TKey : notnull
	{
		var tempDictionary = new Dictionary<TKey, List<T>>(_values.Length >> 2);
		foreach (var element in ArrayOrdered)
		{
			var key = keySelector(element);
			if (!tempDictionary.TryAdd(key, [element]))
			{
				tempDictionary[key].Add(element);
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

	/// <summary>
	/// Accumulates the values by the specified rule.
	/// </summary>
	/// <param name="accumulator">The accumulator that creates a new value via original value and the next value.</param>
	/// <returns>The final result.</returns>
	public T? Aggregate(Func<T?, T?, T> accumulator)
	{
		var result = default(T);
		foreach (var element in this)
		{
			result = accumulator(result, element);
		}
		return result;
	}

	/// <inheritdoc cref="ReadOnlySpan{T}.Slice(int, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T[] Slice(int start, int length) => ArrayOrdered[start..(start + length)];

	/// <summary>
	/// Sorts the span and return the array representation.
	/// </summary>
	/// <returns>The array of values sorted.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T[] ToArray() => ArrayOrdered;

	/// <inheritdoc cref="ReadOnlySpan{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<T>.Enumerator GetEnumerator() => ArrayOrdered.AsReadOnlySpan().GetEnumerator();
}
