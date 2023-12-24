namespace Sudoku.Analytics;

/// <summary>
/// Represents a list of conclusions. The collection only allows adding conclusions.
/// </summary>
/// <remarks>
/// This type uses <see cref="BitArray"/> to make determining on equality for two collections of <see cref="Conclusion"/> instances.
/// Because the type contains a reference-typed field, the type is also a reference type.
/// </remarks>
/// <seealso cref="BitArray"/>
/// <seealso cref="Conclusion"/>
[Equals]
[EqualityOperators]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public sealed partial class ConclusionBag() :
	IBitwiseOperators<ConclusionBag, ConclusionBag, ConclusionBag>,
	ICoordinateObject<ConclusionBag>,
	ICultureFormattable,
	IEnumerable<Conclusion>,
	IEquatable<ConclusionBag>,
	IEqualityOperators<ConclusionBag, ConclusionBag, bool>,
	ILogicalOperators<ConclusionBag>,
	ISimpleParsable<ConclusionBag>
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
	/// Initializes a <see cref="ConclusionBag"/> instance via the specified conclusions.
	/// </summary>
	/// <param name="conclusions">The conclusions to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ConclusionBag(scoped ReadOnlySpan<Conclusion> conclusions) : this() => AddRange(conclusions);


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


	/// <summary>
	/// An empty instance.
	/// </summary>
	public static ConclusionBag Empty => [];


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

	/// <summary>
	/// Add a new conclusion into the collection.
	/// </summary>
	/// <param name="conclusion">The conclusion to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(Conclusion conclusion)
	{
		var (type, cell, digit) = conclusion;
		_bitArray[(int)type * HalfBitsCount + cell * 9 + digit] = true;
		_conclusionsEntry.Add(conclusion);
	}

	/// <summary>
	/// Add a list of conclusions into the collection.
	/// </summary>
	/// <param name="conclusions">The conclusions to be added.</param>
	public void AddRange(scoped ReadOnlySpan<Conclusion> conclusions)
	{
		foreach (var conclusion in conclusions)
		{
			Add(conclusion);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] ConclusionBag? other)
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
	public override string ToString() => ToString(new RxCyConverter());

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CultureInfo? culture)
		=> culture switch
		{
			{ LCID: 1033 } or { DisplayName: ['E' or 'e', 'N' or 'n', ..] } => ToString(new RxCyConverter(true, true, CurrentCulture: culture)),
			{ LCID: 2052 } or { DisplayName: ['Z' or 'z', 'H' or 'h', ..] } => ToString(new K9Converter(true, CurrentCulture: culture)),
			_ => ToString()
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CoordinateConverter converter) => converter.ConclusionConverter([.. _conclusionsEntry]);

	/// <summary>
	/// Try to get the conclusions.
	/// </summary>
	/// <returns>The conclusions.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion[] ToArray() => [.. _conclusionsEntry];

	/// <summary>
	/// Try to get an enumerator type that iterates on each conclusion.
	/// </summary>
	/// <returns>An enumerator type that iterates on each conclusion.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(this);

	/// <summary>
	/// Slices the collection, from the specified start index and the number of the elements.
	/// </summary>
	/// <param name="start">The start index.</param>
	/// <param name="length">The number of elements you want to get.</param>
	/// <returns>The result <see cref="ConclusionBag"/> instance.</returns>
	public ConclusionBag Slice(int start, int length) => new(CollectionsMarshal.AsSpan(_conclusionsEntry.Slice(start, length)));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Conclusion>)this).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<Conclusion> IEnumerable<Conclusion>.GetEnumerator() => _conclusionsEntry.GetEnumerator();


	/// <inheritdoc/>
	public static bool TryParse(string str, [NotNullWhen(true)] out ConclusionBag? result)
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ConclusionBag Parse(string str) => [.. new RxCyParser().ConclusionParser(str)];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ConclusionBag ParseExact(string str, CoordinateParser parser) => [.. parser.ConclusionParser(str)];


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !(ConclusionBag value) => value.Count == 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator true(ConclusionBag value) => value.Count != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator false(ConclusionBag value) => value.Count == 0;

	/// <inheritdoc/>
	public static ConclusionBag operator ~(ConclusionBag value)
	{
		var result = new ConclusionBag();
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
	public static ConclusionBag operator &(ConclusionBag left, ConclusionBag right)
	{
		var result = new ConclusionBag();
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
	public static ConclusionBag operator |(ConclusionBag left, ConclusionBag right)
	{
		var result = new ConclusionBag();
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
	public static ConclusionBag operator ^(ConclusionBag left, ConclusionBag right)
	{
		var result = new ConclusionBag();
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


	/// <summary>
	/// Implicit cast from <see cref="ReadOnlySpan{T}"/> of <see cref="Conclusion"/> instances to <see cref="ConclusionBag"/>.
	/// </summary>
	/// <param name="conclusions">Conclusions to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ConclusionBag(scoped ReadOnlySpan<Conclusion> conclusions) => [.. conclusions];
}
