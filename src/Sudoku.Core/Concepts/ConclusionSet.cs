namespace Sudoku.Concepts;

/// <summary>
/// Represents a list of conclusions.
/// </summary>
[Equals]
[EqualityOperators]
public sealed partial class ConclusionSet :
	IAnyAllMethod<ConclusionSet, Conclusion>,
	IBitwiseOperators<ConclusionSet, ConclusionSet, ConclusionSet>,
	ICollection<Conclusion>,
	IContainsMethod<ConclusionSet, Conclusion>,
	IEnumerable<Conclusion>,
	IEquatable<ConclusionSet>,
	IEqualityOperators<ConclusionSet, ConclusionSet, bool>,
	ILogicalOperators<ConclusionSet>,
	IReadOnlyCollection<Conclusion>,
	IReadOnlySet<Conclusion>,
	ISet<Conclusion>,
	ISliceMethod<ConclusionSet, Conclusion>,
	ISudokuConcept<ConclusionSet>,
	IToArrayMethod<ConclusionSet, Conclusion>
{
	/// <summary>
	/// The total length of bits.
	/// </summary>
	private const int BitsCount = HalfBitsCount << 1;

	/// <summary>
	/// The maximum number of candidates can exist in a grid.
	/// </summary>
	private const int HalfBitsCount = 729;


	/// <summary>
	/// The prime numbers below 100.
	/// </summary>
	private static readonly int[] PrimeNumbers = [2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97];


	/// <summary>
	/// The internal bit array.
	/// </summary>
	private readonly BitArray _bitArray = new(BitsCount);

	/// <summary>
	/// The entry point that can visit conclusions.
	/// </summary>
	private readonly List<Conclusion> _conclusionsEntry = [];


	/// <summary>
	/// Indicates whether the collection contains any assignment conclusions.
	/// </summary>
	public bool ContainsAssignment
	{
		get
		{
			for (var i = 0; i < HalfBitsCount; i++)
			{
				if (_bitArray[i])
				{
					return true;
				}
			}

			return false;
		}
	}

	/// <summary>
	/// Indicates whether the collection contains any elimination conclusions.
	/// </summary>
	public bool ContainsElimination
	{
		get
		{
			for (var i = HalfBitsCount; i < BitsCount; i++)
			{
				if (_bitArray[i])
				{
					return true;
				}
			}

			return false;
		}
	}

	/// <summary>
	/// Indicates the number of bit array elements.
	/// </summary>
	public int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _conclusionsEntry.Count;
	}

	/// <inheritdoc/>
	bool ICollection<Conclusion>.IsReadOnly => false;


	/// <summary>
	/// An empty instance.
	/// </summary>
	public static ConclusionSet Empty => [];

	int ICollection<Conclusion>.Count { get; }


	/// <summary>
	/// Try to get n-th element stored in the collection.
	/// </summary>
	/// <param name="index">The desired index to be checked.</param>
	/// <returns>The found <see cref="Conclusion"/> instance at the specified index.</returns>
	/// <exception cref="IndexOutOfRangeException">Throws when the index is out of range.</exception>
	public Conclusion this[int index] => index >= 0 && index < Count ? _conclusionsEntry[index] : throw new IndexOutOfRangeException();


	/// <summary>
	/// Add a new conclusion, represented as a global index (between 0 and 1458), into the collection.
	/// </summary>
	/// <param name="index">
	/// <para>The global index (between 0 and 1458) to be added.</para>
	/// <para>The global index is equivalent to the result value of this formula <c>conclusionType * 729 + candidate</c>.</para>
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(int index)
	{
		_bitArray[index] = true;
		_conclusionsEntry.Add(new((ConclusionType)(index / HalfBitsCount), index % HalfBitsCount));
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(Conclusion item)
	{
		var (type, cell, digit) = item;
		_bitArray[(int)type * HalfBitsCount + cell * 9 + digit] = true;
		_conclusionsEntry.Add(item);
	}

	/// <summary>
	/// Add a list of conclusions into the collection.
	/// </summary>
	/// <param name="conclusions">The conclusions to be added.</param>
	public void AddRange(ReadOnlySpan<Conclusion> conclusions)
	{
		foreach (var conclusion in conclusions)
		{
			Add(conclusion);
		}
	}

	/// <summary>
	/// Remove a conclusion, represented as a global index (between 0 and 1458), from the collection.
	/// </summary>
	/// <param name="index">
	/// <para>The global index (between 0 and 1458) to be added.</para>
	/// <para>The global index is equivalent to the result value of this formula <c>conclusionType * 729 + candidate</c>.</para>
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove(int index)
	{
		_bitArray[index] = false;
		_conclusionsEntry.Remove(new((ConclusionType)(index / HalfBitsCount), index % HalfBitsCount));
	}

	/// <inheritdoc cref="ICollection{T}.Remove(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove(Conclusion item)
	{
		var (type, cell, digit) = item;
		_bitArray[(int)type * HalfBitsCount + cell * 9 + digit] = false;
		_conclusionsEntry.Remove(item);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear()
	{
		_bitArray.SetAll(false);
		_conclusionsEntry.Clear();
	}

	/// <summary>
	/// Removes all elements in the collection and add all elements from <paramref name="conclusions"/>.
	/// </summary>
	/// <param name="conclusions">The conclusions provider to replace with the current instance.</param>
	public void Replace(ConclusionSet conclusions)
	{
		Clear();
		foreach (var conclusion in conclusions)
		{
			Add(conclusion);
		}
	}

	/// <inheritdoc cref="ICollection{T}.CopyTo(T[], int)"/>
	public void CopyTo(Span<Conclusion> span)
	{
		if (span.Length < Count)
		{
			throw new InvalidOperationException();
		}

		var i = 0;
		foreach (var conclusion in this)
		{
			span[i++] = conclusion;
		}
	}

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] ConclusionSet? other)
	{
		if (other is null)
		{
			return false;
		}

		for (var i = 0; i < BitsCount; i++)
		{
			if (_bitArray[i] != other._bitArray[i])
			{
				return false;
			}
		}

		return true;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(Conclusion value) => _bitArray[value.GetHashCode()];

	/// <summary>
	/// Indicates whether the collection contains the specified cell.
	/// </summary>
	/// <param name="cell">The cell to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool ContainsCell(Cell cell)
	{
		for (var bit = 0; bit < 9; bit++)
		{
			if (_bitArray[cell * 9 + bit] || _bitArray[HalfBitsCount + cell * 9 + bit])
			{
				return true;
			}
		}
		return false;
	}

	/// <inheritdoc cref="IAnyAllMethod{TSelf, TSource}.Any(Func{TSource, bool})"/>
	public bool Exists(Func<Conclusion, bool> predicate)
	{
		foreach (var conclusion in this)
		{
			if (predicate(conclusion))
			{
				return true;
			}
		}
		return false;
	}

	/// <inheritdoc cref="IAnyAllMethod{TSelf, TSource}.All(Func{TSource, bool})"/>
	public bool TrueForAll(Func<Conclusion, bool> predicate)
	{
		foreach (var conclusion in this)
		{
			if (!predicate(conclusion))
			{
				return false;
			}
		}
		return true;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var result = new HashCode();
		var i = 0;
		foreach (bool element in _bitArray)
		{
			if (element)
			{
				result.Add(PrimeNumbers[i % PrimeNumbers.Length] * i);
			}
			i++;
		}
		return result.ToHashCode();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => ToString(GlobalizedConverter.InvariantCultureConverter);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CultureInfo? culture) => ToString(GlobalizedConverter.GetConverter(culture ?? CultureInfo.CurrentUICulture));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString<T>(T converter) where T : CoordinateConverter => converter.ConclusionConverter([.. _conclusionsEntry]);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion[] ToArray() => [.. _conclusionsEntry];

	/// <summary>
	/// Try to get an enumerator type that iterates on each conclusion.
	/// </summary>
	/// <returns>An enumerator type that iterates on each conclusion.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(this);

	/// <inheritdoc cref="ISliceMethod{TSelf, TSource}.Slice(int, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ConclusionSet Slice(int start, int count) => [.. _conclusionsEntry[start..(start + count)]];

	/// <inheritdoc/>
	void ICollection<Conclusion>.CopyTo(Conclusion[] array, int arrayIndex) => CopyTo(array.AsSpan()[arrayIndex..]);

	/// <inheritdoc/>
	void ISet<Conclusion>.ExceptWith(IEnumerable<Conclusion> other)
	{
		foreach (var element in other)
		{
			if (Contains(element))
			{
				Remove(element);
			}
		}
	}

	/// <inheritdoc/>
	void ISet<Conclusion>.IntersectWith(IEnumerable<Conclusion> other)
	{
		foreach (var element in other)
		{
			if (!Contains(element))
			{
				Remove(element);
			}
		}
	}

	/// <inheritdoc/>
	void ISet<Conclusion>.SymmetricExceptWith(IEnumerable<Conclusion> other)
	{
		var p = (ConclusionSet)([.. other]);
		Replace(this & ~p | p & ~this);
	}

	/// <inheritdoc/>
	void ISet<Conclusion>.UnionWith(IEnumerable<Conclusion> other)
	{
		foreach (var element in other)
		{
			if (!Contains(element))
			{
				Add(element);
			}
		}
	}

	/// <inheritdoc/>
	bool IAnyAllMethod<ConclusionSet, Conclusion>.Any() => Count != 0;

	/// <inheritdoc/>
	bool IAnyAllMethod<ConclusionSet, Conclusion>.Any(Func<Conclusion, bool> predicate) => Exists(predicate);

	/// <inheritdoc/>
	bool IAnyAllMethod<ConclusionSet, Conclusion>.All(Func<Conclusion, bool> predicate) => TrueForAll(predicate);

	/// <inheritdoc/>
	bool ICollection<Conclusion>.Remove(Conclusion item)
	{
		if (!Contains(item))
		{
			return false;
		}

		Remove(item);
		return true;
	}

	/// <inheritdoc/>
	bool IReadOnlySet<Conclusion>.IsProperSubsetOf(IEnumerable<Conclusion> other)
	{
		var p = (ConclusionSet)([.. other]);
		return (p & this) == this && p != this;
	}

	/// <inheritdoc/>
	bool IReadOnlySet<Conclusion>.IsProperSupersetOf(IEnumerable<Conclusion> other)
	{
		var p = (ConclusionSet)([.. other]);
		return (this & p) == p && p != this;
	}

	/// <inheritdoc/>
	bool IReadOnlySet<Conclusion>.IsSubsetOf(IEnumerable<Conclusion> other) => ([.. other] & this) == this;

	/// <inheritdoc/>
	bool IReadOnlySet<Conclusion>.IsSupersetOf(IEnumerable<Conclusion> other)
	{
		var p = (ConclusionSet)([.. other]);
		return (this & p) == p;
	}

	/// <inheritdoc/>
	bool IReadOnlySet<Conclusion>.Overlaps(IEnumerable<Conclusion> other) => this & [.. other] ? true : false;

	/// <inheritdoc/>
	bool IReadOnlySet<Conclusion>.SetEquals(IEnumerable<Conclusion> other) => this == [.. other];

	/// <inheritdoc/>
	bool ISet<Conclusion>.Add(Conclusion item)
	{
		if (Contains(item))
		{
			return false;
		}

		Add(item);
		return true;
	}

	/// <inheritdoc/>
	bool ISet<Conclusion>.IsProperSubsetOf(IEnumerable<Conclusion> other) => ((IReadOnlySet<Conclusion>)this).IsProperSubsetOf(other);

	/// <inheritdoc/>
	bool ISet<Conclusion>.IsProperSupersetOf(IEnumerable<Conclusion> other) => ((IReadOnlySet<Conclusion>)this).IsProperSupersetOf(other);

	/// <inheritdoc/>
	bool ISet<Conclusion>.IsSubsetOf(IEnumerable<Conclusion> other) => ((IReadOnlySet<Conclusion>)this).IsSubsetOf(other);

	/// <inheritdoc/>
	bool ISet<Conclusion>.IsSupersetOf(IEnumerable<Conclusion> other) => ((IReadOnlySet<Conclusion>)this).IsSupersetOf(other);

	/// <inheritdoc/>
	bool ISet<Conclusion>.Overlaps(IEnumerable<Conclusion> other) => ((IReadOnlySet<Conclusion>)this).Overlaps(other);

	/// <inheritdoc/>
	bool ISet<Conclusion>.SetEquals(IEnumerable<Conclusion> other) => ((IReadOnlySet<Conclusion>)this).SetEquals(other);

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Conclusion>)this).GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<Conclusion> IEnumerable<Conclusion>.GetEnumerator() => _conclusionsEntry.GetEnumerator();

	/// <inheritdoc/>
	IEnumerable<Conclusion> ISliceMethod<ConclusionSet, Conclusion>.Slice(int start, int count) => Slice(start, count);


	/// <inheritdoc/>
	public static bool TryParse(string str, [NotNullWhen(true)] out ConclusionSet? result)
	{
		try
		{
			result = Parse(str);
			return true;
		}
		catch (FormatException)
		{
			result = null;
			return false;
		}
	}

	/// <inheritdoc/>
	public static bool TryParse<T>(string str, T parser, [NotNullWhen(true)] out ConclusionSet? result) where T : CoordinateParser
	{
		try
		{
			result = parser.ConclusionParser(str);
			return true;
		}
		catch (FormatException)
		{
			result = null;
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ConclusionSet Parse(string str) => [.. new RxCyParser().ConclusionParser(str)];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ConclusionSet Parse<T>(string str, T parser) where T : CoordinateParser => [.. parser.ConclusionParser(str)];


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !(ConclusionSet value) => value.Count == 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator true(ConclusionSet value) => value.Count != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator false(ConclusionSet value) => value.Count == 0;

	/// <inheritdoc/>
	public static ConclusionSet operator ~(ConclusionSet value)
	{
		var result = new ConclusionSet();
		var i = 0;
		foreach (bool bit in ((BitArray)value._bitArray.Clone()).Not())
		{
			if (bit)
			{
				result.Add(i);
			}
			i++;
		}

		return result;
	}

	/// <inheritdoc/>
	public static ConclusionSet operator &(ConclusionSet left, ConclusionSet right)
	{
		var result = new ConclusionSet();
		var i = 0;
		foreach (bool bit in ((BitArray)left._bitArray.Clone()).And(right._bitArray))
		{
			if (bit)
			{
				result.Add(i);
			}
			i++;
		}

		return result;
	}

	/// <inheritdoc/>
	public static ConclusionSet operator |(ConclusionSet left, ConclusionSet right)
	{
		var result = new ConclusionSet();
		var i = 0;
		foreach (bool bit in ((BitArray)left._bitArray.Clone()).Or(right._bitArray))
		{
			if (bit)
			{
				result.Add(i);
			}
			i++;
		}

		return result;
	}

	/// <inheritdoc/>
	public static ConclusionSet operator ^(ConclusionSet left, ConclusionSet right)
	{
		var result = new ConclusionSet();
		var i = 0;
		foreach (bool bit in ((BitArray)left._bitArray.Clone()).Xor(right._bitArray))
		{
			if (bit)
			{
				result.Add(i);
			}
			i++;
		}

		return result;
	}
}
