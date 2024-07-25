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
) :
	IAggregateMethod<ArrayOrderedEnumerable<T>, T>,
	IEnumerable<T>,
	IGroupByMethod<ArrayOrderedEnumerable<T>, T>,
	IOrderedEnumerable<T>,
	IReadOnlyCollection<T>,
	ISelectMethod<ArrayOrderedEnumerable<T>, T>,
	ISliceMethod<ArrayOrderedEnumerable<T>, T>,
	IThenByMethod<ArrayOrderedEnumerable<T>, T>,
	IToArrayMethod<ArrayOrderedEnumerable<T>, T>,
	IWhereMethod<ArrayOrderedEnumerable<T>, T>
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

	/// <inheritdoc/>
	int IReadOnlyCollection<T>.Count => Length;


	/// <inheritdoc cref="ReadOnlySpan{T}.this[int]"/>
	public ref readonly T this[int index] => ref _values[index];


	/// <inheritdoc cref="ISelectMethod{TSelf, TSource}.Select{TResult}(Func{TSource, TResult})"/>
	public TResult[] Select<TResult>(Func<T, TResult> selector)
	{
		var result = new List<TResult>(_values.Length);
		foreach (var element in ArrayOrdered)
		{
			result.AddRef(selector(element));
		}
		return [.. result];
	}

	/// <inheritdoc cref="IWhereMethod{TSelf, TSource}.Where(Func{TSource, bool})"/>
	public T[] Where(Func<T, bool> predicate)
	{
		var result = new List<T>(_values.Length);
		foreach (var element in ArrayOrdered)
		{
			if (predicate(element))
			{
				result.AddRef(in element);
			}
		}
		return [.. result];
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ArrayOrderedEnumerable<T> ThenBy<TKey>(Func<T, TKey> keySelector) where TKey : notnull
		=> new(
			_values,
			[
				.. _selectors,
				(l, r) => (keySelector(l), keySelector(r)) switch
				{
					(IComparable<TKey> left, var right) => left.CompareTo(right),
					var (a, b) => Comparer<TKey>.Default.Compare(a, b)
				}
			]
		);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ArrayOrderedEnumerable<T> ThenByDescending<TKey>(Func<T, TKey> keySelector) where TKey : notnull
		=> new(
			_values,
			[
				.. _selectors,
				(l, r) => (keySelector(l), keySelector(r)) switch
				{
					(IComparable<TKey> left, var right) => -left.CompareTo(right),
					var (a, b) => -Comparer<TKey>.Default.Compare(a, b)
				}
			]
		);

	/// <inheritdoc cref="IGroupByMethod{TSelf, TSource}.GroupBy{TKey}(Func{TSource, TKey})"/>
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

	/// <inheritdoc cref="IGroupByMethod{TSelf, TSource}.GroupBy{TKey, TElement}(Func{TSource, TKey}, Func{TSource, TElement})"/>
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

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T? Aggregate(Func<T?, T?, T> func) => Aggregate(default, func, @delegate.Self);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func)
		where TAccumulate : allows ref struct
		=> Aggregate(seed, func, @delegate.Self);

	/// <inheritdoc/>
	public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
		where TAccumulate : allows ref struct
		where TResult : allows ref struct
	{
		var result = seed;
		foreach (var element in this)
		{
			result = func(result, element);
		}
		return resultSelector(result);
	}

	/// <inheritdoc cref="ISliceMethod{TSelf, TSource}.Slice(int, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T[] Slice(int start, int length) => ArrayOrdered[start..(start + length)];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T[] ToDigitsArray() => ArrayOrdered;

	/// <inheritdoc cref="ReadOnlySpan{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<T>.Enumerator GetEnumerator() => ArrayOrdered.AsReadOnlySpan().GetEnumerator();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)ArrayOrdered).GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)ArrayOrdered).GetEnumerator();

	/// <inheritdoc/>
	IEnumerable<T> ISliceMethod<ArrayOrderedEnumerable<T>, T>.Slice(int start, int count) => Slice(start, count);

	/// <inheritdoc/>
	IEnumerable<T> IWhereMethod<ArrayOrderedEnumerable<T>, T>.Where(Func<T, bool> predicate) => Where(predicate);

	/// <inheritdoc/>
	IEnumerable<T> IThenByMethod<ArrayOrderedEnumerable<T>, T>.ThenBy<TKey>(Func<T, TKey> keySelector) => ThenBy(keySelector);

	/// <inheritdoc/>
	IEnumerable<T> IThenByMethod<ArrayOrderedEnumerable<T>, T>.ThenByDescending<TKey>(Func<T, TKey> keySelector)
		=> ThenByDescending(keySelector);

	/// <inheritdoc/>
	IEnumerable<TResult> ISelectMethod<ArrayOrderedEnumerable<T>, T>.Select<TResult>(Func<T, TResult> selector) => Select(selector);

	/// <inheritdoc/>
	IEnumerable<IGrouping<TKey, T>> IGroupByMethod<ArrayOrderedEnumerable<T>, T>.GroupBy<TKey>(Func<T, TKey> keySelector)
		=> GroupBy(keySelector);

	/// <inheritdoc/>
	IEnumerable<IGrouping<TKey, TElement>> IGroupByMethod<ArrayOrderedEnumerable<T>, T>.GroupBy<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector)
		=> GroupBy(keySelector, elementSelector);

	/// <inheritdoc/>
	IOrderedEnumerable<T> IOrderedEnumerable<T>.CreateOrderedEnumerable<TKey>(Func<T, TKey> keySelector, IComparer<TKey>? comparer, bool descending) => Create(_values, keySelector, comparer, descending);


	/// <summary>
	/// Creates an <see cref="ArrayOrderedEnumerable{T}"/> instance via the specified values.
	/// </summary>
	/// <typeparam name="TKey">The type of the key to be compared.</typeparam>
	/// <param name="values">The values to be used.</param>
	/// <param name="keySelector">
	/// The selector method that calculates a <typeparamref name="TKey"/> from each <typeparamref name="T"/> instance.
	/// </param>
	/// <param name="comparer">
	/// A comparable instance that temporarily checks the comparing result of two <typeparamref name="TKey"/> values.
	/// </param>
	/// <param name="descending">A <see cref="bool"/> value indicating whether the creation is for descending comparison rule.</param>
	/// <returns>An <see cref="ArrayOrderedEnumerable{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ArrayOrderedEnumerable<T> Create<TKey>(T[] values, Func<T, TKey> keySelector, IComparer<TKey>? comparer, bool descending)
	{
		comparer ??= Comparer<TKey>.Default;
		return new(values, descending ? descendingComparer : ascendingComparer);


		int ascendingComparer(T left, T right) => comparer.Compare(keySelector(left), keySelector(right));

		int descendingComparer(T left, T right) => -comparer.Compare(keySelector(left), keySelector(right));
	}
}
