namespace Sudoku.CommandLine;

/// <summary>
/// Represents a symbol list (elements of type <see cref="Option"/> or <see cref="Argument"/>).
/// </summary>
/// <typeparam name="TSymbol">The type of symbol.</typeparam>
/// <seealso cref="Option"/>
/// <seealso cref="Argument"/>
[CollectionBuilder(typeof(SymbolList), nameof(SymbolList.Create))]
public readonly struct SymbolList<TSymbol> :
	IAnyAllMethod<SymbolList<TSymbol>, TSymbol>,
	ICountMethod<SymbolList<TSymbol>, TSymbol>,
	IEnumerable<TSymbol>,
	IElementAtMethod<SymbolList<TSymbol>, TSymbol>,
	IReadOnlyList<TSymbol>,
	IReadOnlyCollection<TSymbol>,
	ISelectMethod<SymbolList<TSymbol>, TSymbol>,
	ISliceMethod<SymbolList<TSymbol>, TSymbol>,
	IWhereMethod<SymbolList<TSymbol>, TSymbol>
	where TSymbol : Symbol
{
	/// <summary>
	/// Indicates the empty instance.
	/// </summary>
	public static readonly SymbolList<TSymbol> Empty = [];


	/// <summary>
	/// Indicates the symbols.
	/// </summary>
	private readonly TSymbol[] _symbols;


	/// <summary>
	/// Indicates the length of the list.
	/// </summary>
	public int Length => _symbols.Length;

	/// <inheritdoc/>
	int IReadOnlyCollection<TSymbol>.Count => ((IReadOnlyCollection<TSymbol>)_symbols).Count;


	/// <inheritdoc/>
	public TSymbol this[int index] => _symbols[index];


	/// <summary>
	/// Initializes a <see cref="SymbolList{TSymbol}"/> instance.
	/// </summary>
	/// <param name="symbols">The symbols.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal SymbolList(ReadOnlySpan<TSymbol> symbols) => _symbols = symbols.ToArray();


	/// <summary>
	/// Determine whether the speciifed condition is satisfied for all elements in the current collection.
	/// </summary>
	/// <param name="predicate">The condition.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool TrueForAll(Predicate<TSymbol> predicate)
	{
		foreach (var element in this)
		{
			if (!predicate(element))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Determine whether at least one element in the current collection satisfies the specified condition.
	/// </summary>
	/// <param name="predicate">The condition.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool Exists(Predicate<TSymbol> predicate)
	{
		foreach (var element in this)
		{
			if (predicate(element))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Finds for the first element satisfying the specified condition, and return the index of the element.
	/// </summary>
	/// <param name="predicate">The condition.</param>
	/// <returns>The index of the element; or -1 if not found.</returns>
	public int FindIndex(Predicate<TSymbol> predicate)
	{
		for (var i = 0; i < _symbols.Length; i++)
		{
			if (predicate(_symbols[i]))
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// Finds for the last element satisfying the specified condition, and return the index of the element.
	/// </summary>
	/// <param name="predicate">The condition.</param>
	/// <returns>The index of the element; or -1 if not found.</returns>
	public int FindLastIndex(Predicate<TSymbol> predicate)
	{
		for (var i = _symbols.Length - 1; i >= 0; i--)
		{
			if (predicate(_symbols[i]))
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// Calculates the number of elements satisfying the specified condition.
	/// </summary>
	/// <param name="predicate">The condition.</param>
	/// <returns>An <see cref="int"/> value indicating that.</returns>
	public int Count(Predicate<TSymbol> predicate)
	{
		var result = 0;
		foreach (var element in this)
		{
			if (predicate(element))
			{
				result++;
			}
		}
		return result;
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public AnonymousSpanEnumerator<TSymbol> GetEnumerator() => new(_symbols);

	/// <summary>
	/// Slices the current instance.
	/// </summary>
	/// <param name="start">The start index.</param>
	/// <param name="length">The desired length.</param>
	/// <returns>The <see cref="SymbolList{TSymbol}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SymbolList<TSymbol> Slice(int start, int length) => new(_symbols.AsSpan().Slice(start, length));

	/// <inheritdoc cref="ISelectMethod{TSelf, TSource}.Select{TResult}(Func{TSource, TResult})"/>
	public ReadOnlySpan<TResult> Select<TResult>(Func<TSymbol, TResult> selector)
	{
		var result = new List<TResult>();
		foreach (var element in this)
		{
			result.Add(selector(element));
		}
		return result.AsSpan();
	}

	/// <inheritdoc cref="IWhereMethod{TSelf, TSource}.Where(Func{TSource, bool})"/>
	public SymbolList<TSymbol> Where(Predicate<TSymbol> predicate)
	{
		var result = new List<TSymbol>();
		foreach (var element in this)
		{
			if (predicate(element))
			{
				result.Add(element);
			}
		}
		return [.. result];
	}

	/// <summary>
	/// Casts the current instance as a <see cref="ReadOnlySpan{T}"/> instance.
	/// </summary>
	/// <returns>A <see cref="ReadOnlySpan{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<TSymbol> AsSpan() => _symbols;

	/// <summary>
	/// Returns an array of <typeparamref name="TSymbol"/> instances.
	/// </summary>
	/// <returns>An array of <typeparamref name="TSymbol"/> instances.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TSymbol[] ToArray() => [.. _symbols];

	/// <inheritdoc/>
	bool IAnyAllMethod<SymbolList<TSymbol>, TSymbol>.Any() => Length != 0;

	/// <inheritdoc/>
	bool IAnyAllMethod<SymbolList<TSymbol>, TSymbol>.Any(Func<TSymbol, bool> predicate) => Exists(predicate.Invoke);

	/// <inheritdoc/>
	bool IAnyAllMethod<SymbolList<TSymbol>, TSymbol>.All(Func<TSymbol, bool> predicate) => TrueForAll(predicate.Invoke);

	/// <inheritdoc/>
	int ICountMethod<SymbolList<TSymbol>, TSymbol>.Count() => Length;

	/// <inheritdoc/>
	int ICountMethod<SymbolList<TSymbol>, TSymbol>.Count(Func<TSymbol, bool> predicate) => Count(predicate.Invoke);

	/// <inheritdoc/>
	long ICountMethod<SymbolList<TSymbol>, TSymbol>.LongCount() => Length;

	/// <inheritdoc/>
	long ICountMethod<SymbolList<TSymbol>, TSymbol>.LongCount(Func<TSymbol, bool> predicate) => Count(predicate.Invoke);

	/// <inheritdoc/>
	TSymbol IElementAtMethod<SymbolList<TSymbol>, TSymbol>.ElementAt(int index) => this[index];

	/// <inheritdoc/>
	TSymbol IElementAtMethod<SymbolList<TSymbol>, TSymbol>.ElementAt(Index index) => this[index];

	/// <inheritdoc/>
	TSymbol? IElementAtMethod<SymbolList<TSymbol>, TSymbol>.ElementAtOrDefault(int index)
		=> index >= 0 && index < Length ? this[index] : default;

	/// <inheritdoc/>
	TSymbol? IElementAtMethod<SymbolList<TSymbol>, TSymbol>.ElementAtOrDefault(Index index)
		=> index.GetOffset(Length) is var i && i >= 0 && i < Length ? this[i] : default;

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => _symbols.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<TSymbol> IEnumerable<TSymbol>.GetEnumerator() => _symbols.AsEnumerable().GetEnumerator();

	/// <inheritdoc/>
	IEnumerable<TSymbol> ISliceMethod<SymbolList<TSymbol>, TSymbol>.Slice(int start, int count) => Slice(start, count);

	/// <inheritdoc/>
	IEnumerable<TResult> ISelectMethod<SymbolList<TSymbol>, TSymbol>.Select<TResult>(Func<TSymbol, TResult> selector)
	{
		foreach (var element in ToArray())
		{
			yield return selector(element);
		}
	}

	/// <inheritdoc/>
	IEnumerable<TResult> ISelectMethod<SymbolList<TSymbol>, TSymbol>.Select<TResult>(Func<TSymbol, int, TResult> selector)
	{
		var array = ToArray();
		for (var i = 0; i < array.Length; i++)
		{
			yield return selector(array[i], i);
		}
	}

	/// <inheritdoc/>
	IEnumerable<TSymbol> IWhereMethod<SymbolList<TSymbol>, TSymbol>.Where(Func<TSymbol, bool> predicate)
	{
		foreach (var element in ToArray())
		{
			if (predicate(element))
			{
				yield return element;
			}
		}
	}

	/// <inheritdoc/>
	IEnumerable<TSymbol> IWhereMethod<SymbolList<TSymbol>, TSymbol>.Where(Func<TSymbol, int, bool> predicate)
	{
		var array = ToArray();
		for (var i = 0; i < array.Length; i++)
		{
			var element = array[i];
			if (predicate(element, i))
			{
				yield return element;
			}
		}
	}


	/// <summary>
	/// Implicit cast from <see cref="SymbolList{TSymbol}"/> instance into <see cref="Symbol"/>[].
	/// </summary>
	/// <param name="symbols">The symbols.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Symbol[](SymbolList<TSymbol> symbols) => symbols._symbols;
}
