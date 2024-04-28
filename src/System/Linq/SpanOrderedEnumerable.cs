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
[StructLayout(LayoutKind.Auto)]
[DebuggerStepThrough]
[Equals]
[GetHashCode]
[ToString]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly ref partial struct SpanOrderedEnumerable<T>(
	[PrimaryConstructorParameter(MemberKinds.Field)] ReadOnlySpan<T> values,
	[PrimaryConstructorParameter(MemberKinds.Field), UnscopedRef] params ReadOnlySpan<Func<T, T, int>> selectors
)
{
	/// <summary>
	/// Indicates the number of elements stored in the collection.
	/// </summary>
	public int Length => _values.Length;

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
			result.Add(selector(element));
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
				result.Add(element);
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

	/// <inheritdoc cref="ReadOnlySpanEnumerable.GroupBy{TSource, TKey}(ReadOnlySpan{TSource}, Func{TSource, TKey})"/>
	public ReadOnlySpan<SpanGrouping<T, TKey>> GroupBy<TKey>(Func<T, TKey> keySelector) where TKey : notnull
	{
		var tempDictionary = new Dictionary<TKey, List<T>>(_values.Length >> 2);
		foreach (var element in _values)
		{
			var key = keySelector(element);
			if (!tempDictionary.TryAdd(key, [element]))
			{
				tempDictionary[key].Add(element);
			}
		}

		var result = new List<SpanGrouping<T, TKey>>(tempDictionary.Count);
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

	/// <inheritdoc cref="ReadOnlySpan{T}.Slice(int, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<T> Slice(int start, int length) => Span.Slice(start, length);

	/// <inheritdoc cref="ReadOnlySpan{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<T>.Enumerator GetEnumerator() => Span.GetEnumerator();

	/// <summary>
	/// Sorts the span and return the array representation.
	/// </summary>
	/// <returns>The array of values sorted.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T[] ToArray() => Span.ToArray();
}
