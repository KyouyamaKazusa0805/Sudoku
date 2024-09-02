namespace System.Linq;

/// <summary>
/// Represents an enumerable instance that is based on a <see cref="ReadOnlySpan{T}"/>.
/// </summary>
/// <typeparam name="T">Indicates the type of each element.</typeparam>
/// <param name="values">Indicates the values.</param>
/// <param name="selectors">
/// <para>Indicates the selector functions that return <typeparamref name="T"/> instances, to be used as comparison.</para>
/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
/// <include file="../../global-doc-comments.xml" path="//g/csharp12/feature[@name='params-collections']/target[@name='parameter']"/>
/// </param>
[DebuggerStepThrough]
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlag.AllObjectMethods)]
public readonly ref partial struct SpanOrderedEnumerable<T>(
	[PrimaryConstructorParameter(MemberKinds.Field)] ReadOnlySpan<T> values,
	[PrimaryConstructorParameter(MemberKinds.Field), UnscopedRef] params ReadOnlySpan<Func<T, T, int>> selectors
) :
	IEnumerable<T>,
	IGroupByMethod<SpanOrderedEnumerable<T>, T>,
	IOrderedEnumerable<T>,
	IReadOnlyCollection<T>,
	ISelectMethod<SpanOrderedEnumerable<T>, T>,
	ISliceMethod<SpanOrderedEnumerable<T>, T>,
	IThenByMethod<SpanOrderedEnumerable<T>, T>,
	IToArrayMethod<SpanOrderedEnumerable<T>, T>,
	IWhereMethod<SpanOrderedEnumerable<T>, T>
{
	/// <summary>
	/// Indicates the number of elements stored in the collection.
	/// </summary>
	public int Length => _values.Length;

	/// <inheritdoc/>
	int IReadOnlyCollection<T>.Count => Length;

	/// <summary>
	/// Creates an ordered <see cref="Span{T}"/> instance.
	/// </summary>
	/// <returns>An ordered <see cref="Span{T}"/> instance, whose value is from the current enumerable instance.</returns>
	private Span<T> Span
	{
		get
		{
			// Copy field in order to make the variable can be used inside lambda.
			var selectors = _selectors.ToArray();

			// Sort the span of values.
			var result = new T[_values.Length].AsSpan();
			_values.CopyTo(result);
			result.Sort(
				(l, r) =>
				{
					foreach (var selector in selectors)
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
	public ReadOnlySpan<TResult> Select<TResult>(Func<T, TResult> selector)
	{
		var result = new List<TResult>(_values.Length);
		foreach (var element in Span)
		{
			result.AddRef(selector(element));
		}
		return result.AsReadOnlySpan();
	}

	/// <summary>
	/// Filters the collection using the specified condition.
	/// </summary>
	/// <param name="condition">The condition to be used.</param>
	/// <returns>A span of <typeparamref name="T"/> instances.</returns>
	public ReadOnlySpan<T> Where(Func<T, bool> condition)
	{
		var result = new List<T>(_values.Length);
		foreach (var element in Span)
		{
			if (condition(element))
			{
				result.AddRef(in element);
			}
		}
		return result.AsReadOnlySpan();
	}

	/// <inheritdoc cref="Enumerable.ThenBy{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SpanOrderedEnumerable<T> ThenBy<TKey>(Func<T, TKey> selector)
		=> new(
			_values,
			(Func<T, T, int>[])[
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
	public SpanOrderedEnumerable<T> ThenByDescending<TKey>(Func<T, TKey> selector)
		=> new(
			_values,
			(Func<T, T, int>[])[
				.. _selectors,
				(l, r) => (selector(l), selector(r)) switch
				{
					(IComparable<TKey> left, var right) => -left.CompareTo(right),
					var (a, b) => -Comparer<TKey>.Default.Compare(a, b)
				}
			]
		);

	/// <inheritdoc cref="SpanEnumerable.GroupBy{TSource, TKey}(ReadOnlySpan{TSource}, Func{TSource, TKey})"/>
	public ReadOnlySpan<SpanGrouping<T, TKey>> GroupBy<TKey>(Func<T, TKey> keySelector) where TKey : notnull
	{
		var tempDictionary = new Dictionary<TKey, List<T>>(_values.Length >> 2);
		foreach (var element in Span)
		{
			var key = keySelector(element);
			if (!tempDictionary.TryAdd(key, [element]))
			{
				tempDictionary[key].AddRef(in element);
			}
		}

		var result = new List<SpanGrouping<T, TKey>>(tempDictionary.Count);
		foreach (var key in tempDictionary.Keys)
		{
			var tempValues = tempDictionary[key];
			result.AddRef(new([.. tempValues], key));
		}
		return result.AsReadOnlySpan();
	}

	/// <inheritdoc cref="SpanEnumerable.GroupBy{TSource, TKey, TElement}(ReadOnlySpan{TSource}, Func{TSource, TKey}, Func{TSource, TElement})"/>
	public ReadOnlySpan<SpanGrouping<TElement, TKey>> GroupBy<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector)
		where TKey : notnull
	{
		var tempDictionary = new Dictionary<TKey, List<T>>(_values.Length >> 2);
		foreach (var element in Span)
		{
			var key = keySelector(element);
			if (!tempDictionary.TryAdd(key, [element]))
			{
				tempDictionary[key].AddRef(in element);
			}
		}

		var result = new List<SpanGrouping<TElement, TKey>>(tempDictionary.Count);
		foreach (var key in tempDictionary.Keys)
		{
			var tempValues = tempDictionary[key];
			var valuesConverted = from value in tempValues select elementSelector(value);
			result.AddRef(new(valuesConverted.ToArray(), key));
		}
		return result.AsReadOnlySpan();
	}

	/// <inheritdoc cref="ISliceMethod{TSelf, TSource}.Slice(int, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<T> Slice(int start, int length) => Span.Slice(start, length);

	/// <inheritdoc cref="ReadOnlySpan{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<T>.Enumerator GetEnumerator() => Span.GetEnumerator();

	/// <summary>
	/// Casts the current instance into equivalent-comparison <see cref="ArrayOrderedEnumerable{T}"/> instance.
	/// </summary>
	/// <returns>An <see cref="ArrayOrderedEnumerable{T}"/> instance.</returns>
	/// <seealso cref="ArrayOrderedEnumerable{T}"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ArrayOrderedEnumerable<T> AsArrayOrderedEnumerable() => new(_values.ToArray(), _selectors.ToArray());

	/// <inheritdoc cref="IToArrayMethod{TSelf, TSource}.ToArray"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T[] ToArray() => Span.ToArray();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => Span.ToArray().GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<T> IEnumerable<T>.GetEnumerator() => Span.ToArray().AsEnumerable().GetEnumerator();

	/// <inheritdoc/>
	IOrderedEnumerable<T> IOrderedEnumerable<T>.CreateOrderedEnumerable<TKey>(Func<T, TKey> keySelector, IComparer<TKey>? comparer, bool descending)
		=> Create(_values, keySelector, comparer, descending).AsArrayOrderedEnumerable();

	/// <inheritdoc/>
	IEnumerable<IGrouping<TKey, T>> IGroupByMethod<SpanOrderedEnumerable<T>, T>.GroupBy<TKey>(Func<T, TKey> keySelector)
		=> GroupBy(keySelector).ToArray().Select(element => (IGrouping<TKey, T>)element);

	/// <inheritdoc/>
	IEnumerable<IGrouping<TKey, TElement>> IGroupByMethod<SpanOrderedEnumerable<T>, T>.GroupBy<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector)
		=> GroupBy(keySelector, elementSelector).ToArray().Select(element => (IGrouping<TKey, TElement>)element);

	/// <inheritdoc cref="Enumerable.GroupBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IEqualityComparer{TKey}?)"/>
	IEnumerable<IGrouping<TKey, T>> IGroupByMethod<SpanOrderedEnumerable<T>, T>.GroupBy<TKey>(Func<T, TKey> keySelector, IEqualityComparer<TKey>? comparer)
		=> ToArray().ToLookup(keySelector, comparer ?? EqualityComparer<TKey>.Default);

	/// <inheritdoc cref="Enumerable.GroupBy{TSource, TKey, TElement}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement}, IEqualityComparer{TKey}?)"/>
	IEnumerable<IGrouping<TKey, TElement>> IGroupByMethod<SpanOrderedEnumerable<T>, T>.GroupBy<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector, IEqualityComparer<TKey>? comparer)
		=> ToArray().ToLookup(keySelector, elementSelector);

	/// <inheritdoc cref="Enumerable.GroupBy{TSource, TKey, TElement, TResult}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement}, Func{TKey, IEnumerable{TElement}, TResult})"/>
	IEnumerable<TResult> IGroupByMethod<SpanOrderedEnumerable<T>, T>.GroupBy<TKey, TElement, TResult>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
		=> ToArray().GroupBy(keySelector, elementSelector, resultSelector, null);

	/// <inheritdoc cref="Enumerable.GroupBy{TSource, TKey, TElement, TResult}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement}, Func{TKey, IEnumerable{TElement}, TResult}, IEqualityComparer{TKey}?)"/>
	IEnumerable<TResult> IGroupByMethod<SpanOrderedEnumerable<T>, T>.GroupBy<TKey, TElement, TResult>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
		=> ToArray().ToLookup(keySelector, elementSelector, comparer).Select(p => resultSelector(p.Key, p));

	/// <inheritdoc/>
	IEnumerable<TResult> ISelectMethod<SpanOrderedEnumerable<T>, T>.Select<TResult>(Func<T, TResult> selector) => Select(selector).ToArray();

	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, int, TResult})"/>
	IEnumerable<TResult> ISelectMethod<SpanOrderedEnumerable<T>, T>.Select<TResult>(Func<T, int, TResult> selector)
	{
		var result = new List<TResult>();
		var i = 0;
		foreach (var element in this)
		{
			result.Add(selector(element, i++));
		}
		return result;
	}

	/// <inheritdoc/>
	IEnumerable<T> ISliceMethod<SpanOrderedEnumerable<T>, T>.Slice(int start, int count) => Slice(start, count).ToArray();

	/// <inheritdoc/>
	IEnumerable<T> IWhereMethod<SpanOrderedEnumerable<T>, T>.Where(Func<T, bool> predicate) => Where(predicate).ToArray();

	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, int, bool})"/>
	IEnumerable<T> IWhereMethod<SpanOrderedEnumerable<T>, T>.Where(Func<T, int, bool> predicate)
	{
		var result = new List<T>();
		var i = 0;
		foreach (var element in this)
		{
			if (predicate(element, i++))
			{
				result.Add(element);
			}
		}
		return result;
	}

	/// <inheritdoc/>
	IEnumerable<T> IThenByMethod<SpanOrderedEnumerable<T>, T>.ThenBy<TKey>(Func<T, TKey> keySelector)
		=> ThenBy(keySelector).ToArray();

	/// <inheritdoc/>
	IEnumerable<T> IThenByMethod<SpanOrderedEnumerable<T>, T>.ThenByDescending<TKey>(Func<T, TKey> keySelector)
		=> ThenByDescending(keySelector).ToArray();

	/// <inheritdoc cref="Enumerable.ThenBy{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	IEnumerable<T> IThenByMethod<SpanOrderedEnumerable<T>, T>.ThenBy<TKey>(Func<T, TKey> keySelector, IComparer<TKey>? comparer)
	{
		comparer ??= Comparer<TKey>.Default;
		var result = new SortedList<TKey, T>(comparer);
		foreach (var element in this)
		{
			result.Add(keySelector(element), element);
		}
		return result.Values;
	}

	/// <inheritdoc cref="Enumerable.ThenByDescending{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	IEnumerable<T> IThenByMethod<SpanOrderedEnumerable<T>, T>.ThenByDescending<TKey>(Func<T, TKey> keySelector, IComparer<TKey>? comparer)
	{
		comparer ??= Comparer<TKey>.Default;
		var result = new SortedList<TKey, T>(Comparer<TKey>.Create((l, r) => -comparer.Compare(l, r)));
		foreach (var element in this)
		{
			result.Add(keySelector(element), element);
		}
		return result.Values;
	}


	/// <summary>
	/// Creates an <see cref="SpanOrderedEnumerable{T}"/> instance via the specified values.
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
	/// <returns>An <see cref="SpanOrderedEnumerable{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SpanOrderedEnumerable<T> Create<TKey>(ReadOnlySpan<T> values, Func<T, TKey> keySelector, IComparer<TKey>? comparer, bool descending)
	{
		comparer ??= Comparer<TKey>.Default;
		return new(values, (Func<T, T, int>[])[descending ? descendingComparer : ascendingComparer]);


		int ascendingComparer(T left, T right) => comparer.Compare(keySelector(left), keySelector(right));

		int descendingComparer(T left, T right) => -comparer.Compare(keySelector(left), keySelector(right));
	}
}
