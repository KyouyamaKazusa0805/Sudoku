#pragma warning disable IDE0032, IDE0064
namespace Sudoku.Concepts;

/// <summary>
/// Encapsulates a binary series of candidate status table.
/// </summary>
/// <remarks>
/// This type holds a <see langword="static readonly"/> field called <see cref="Empty"/>,
/// it is the only field provided to be used as the entry to create or update collection.
/// If you want to add elements into it, you can use <see cref="Add(int)"/>, <see cref="AddRange(IEnumerable{int})"/>
/// or just <see cref="op_Addition(in CandidateMap, int)"/> or <see cref="op_Addition(in CandidateMap, IEnumerable{int})"/>:
/// <code><![CDATA[
/// var candidateMap = CandidateMap.Empty;
/// candidateMap += 0; // Adds 'r1c1(1)' into the collection.
/// candidateMap.Add(1); // Adds 'r1c1(2)' into the collection.
/// candidateMap.AddRange(stackalloc[] { 2, 3, 4 }); // Adds 'r1c1(345)' into the collection.
/// candidateMap |= anotherMap; // Adds a list of another instance of type 'CandidateMap' into the current collection.
/// ]]></code>
/// </remarks>
[IsLargeStruct]
[GeneratedOverloadingOperator(GeneratedOperator.EqualityOperators | GeneratedOperator.Boolean)]
public unsafe partial struct CandidateMap :
	IAdditionOperators<CandidateMap, int, CandidateMap>,
	IAdditionOperators<CandidateMap, IEnumerable<int>, CandidateMap>,
	IDivisionOperators<CandidateMap, int, CellMap>,
	ISubtractionOperators<CandidateMap, int, CandidateMap>,
	ISubtractionOperators<CandidateMap, IEnumerable<int>, CandidateMap>,
	IStatusMapBase<CandidateMap>
{
	/// <inheritdoc cref="IStatusMapBase{T}.Empty"/>
	public static readonly CandidateMap Empty;

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MaxValue"/>
	public static readonly CandidateMap MaxValue = ~default(CandidateMap);

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MinValue"/>
	/// <remarks>
	/// This value is equivalent to <see cref="Empty"/>.
	/// </remarks>
	public static readonly CandidateMap MinValue;


	/// <summary>
	/// The background field of the property <see cref="Count"/>.
	/// </summary>
	/// <remarks><b><i>This field is explicitly declared on purpose. Please don't use auto property.</i></b></remarks>
	/// <seealso cref="Count"/>
	private int _count;

	/// <summary>
	/// Indicates the internal bits. 12 is for floor(729 / <see langword="sizeof"/>(<see cref="long"/>) <![CDATA[<<]]> 6).
	/// </summary>
	/// <seealso cref="IStatusMapBase{TSelf}.Shifting"/>
	private fixed long _bits[12];


	/// <inheritdoc/>
	public readonly int Count => _count;

	/// <inheritdoc/>
	readonly int IStatusMapBase<CandidateMap>.Shifting => sizeof(long) << 3;

	/// <summary>
	/// Indicates the cell offsets in this collection.
	/// </summary>
	private readonly int[] Offsets
	{
		get
		{
			if (!this)
			{
				return Array.Empty<int>();
			}

			var arr = new int[_count];
			var pos = 0;
			for (var i = 0; i < 729; i++)
			{
				if ((_bits[i >> 6] >> (i & 63) & 1) != 0)
				{
					arr[pos++] = i;
				}
			}

			return arr;
		}
	}

	/// <inheritdoc/>
	static CandidateMap IStatusMapBase<CandidateMap>.Empty => Empty;

	/// <inheritdoc/>
	static CandidateMap IMinMaxValue<CandidateMap>.MaxValue => MaxValue;

	/// <inheritdoc/>
	static CandidateMap IMinMaxValue<CandidateMap>.MinValue => MinValue;


	/// <inheritdoc/>
	public int this[int index]
	{
		get
		{
			if (!this)
			{
				return -1;
			}

			var pos = 0;
			for (var i = 0; i < 729; i++)
			{
				if ((_bits[i >> 6] >> (i & 63) & 1) != 0)
				{
					if (pos++ == index)
					{
						return i;
					}
				}
			}

			return -1;
		}
	}


	/// <inheritdoc/>
	public readonly unsafe void CopyTo(int* arr, int length)
	{
		if (length < 729)
		{
			return;
		}

		var target = Offsets;
		fixed (int* pTarget = target)
		{
			CopyBlock(arr, pTarget, (uint)(sizeof(int) * length));
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(int offset) => (_bits[offset >> 6] >> (offset & 63) & 1) != 0;

	[GeneratedOverriddingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	public readonly bool Equals(scoped in CandidateMap other)
	{
		for (var i = 0; i < 12; i++)
		{
			if (_bits[i] != other._bits[i])
			{
				return false;
			}
		}

		return true;
	}

	/// <inheritdoc/>
	public readonly void ForEach(Action<int> action)
	{
		foreach (var element in this)
		{
			action(element);
		}
	}

	/// <inheritdoc cref="object.GetHashCode"/>
	public override readonly int GetHashCode()
	{
		var hashCode = new HashCode();
		for (var i = 0; i < 12; i++)
		{
			if ((i & 1) != 0)
			{
				hashCode.Add(_bits[i]);
			}
			else
			{
				var bitBlock = _bits[i];
				hashCode.Add(bitBlock >> 32 | bitBlock & int.MaxValue << 32);
			}
		}

		return hashCode.ToHashCode();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int[] ToArray() => Offsets;

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly string ToString() => RxCyNotation.ToCandidatesString((Candidates)this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly OneDimensionalArrayEnumerator<int> GetEnumerator() => Offsets.EnumerateImmutable();

	/// <summary>
	/// Slices the current instance, and get the new instance with some of elements between two indices.
	/// </summary>
	/// <param name="start">The start index.</param>
	/// <param name="count">The number of elements.</param>
	/// <returns>The target instance.</returns>
	public readonly CandidateMap Slice(int start, int count)
	{
		var result = Empty;
		var offsets = Offsets;
		for (int i = start, end = start + count; i < end; i++)
		{
			result.Add(offsets[i]);
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(int offset)
	{
		scoped ref var v = ref _bits[offset >> 6];
		var older = Contains(offset);
		v |= checked(1L << offset & 63);
		if (!older)
		{
			_count++;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddChecked(int offset)
	{
		if (offset is not (>= 0 and < 729))
		{
			throw new ArgumentOutOfRangeException(nameof(offset), "The cell offset is invalid.");
		}

		scoped ref var v = ref _bits[offset >> 6];
		var older = Contains(offset);
		v |= checked(1L << offset & 63);
		if (!older)
		{
			_count++;
		}
	}

	/// <inheritdoc/>
	public void AddRange(IEnumerable<int> offsets)
	{
		foreach (var element in offsets)
		{
			Add(element);
		}
	}

	/// <inheritdoc cref="IStatusMapBase{TSelf}.AddRange(ReadOnlySpan{int})"/>
	/// <remarks>
	/// Different with the method <see cref="AddRange(IEnumerable{int})"/>, this method
	/// also checks for the validity of each cell offsets. If the value is below 0 or greater than 80,
	/// this method will throw an exception to report about this.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	/// Throws when found at least one cell offset invalid.
	/// </exception>
	public void AddRangeChecked(IEnumerable<int> offsets)
	{
		foreach (var cell in offsets)
		{
			AddChecked(cell);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear() => this = default;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove(int offset)
	{
		scoped ref var v = ref _bits[offset >> 6];
		var older = Contains(offset);
		v &= ~(1L << offset & 63);
		if (older)
		{
			_count--;
		}
	}

	/// <inheritdoc/>
	/// <exception cref="NotImplementedException">Throws always due to too complex.</exception>
	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly int IStatusMapBase<CandidateMap>.CompareTo(scoped in CandidateMap other)
		=> throw new NotImplementedException("Instances of this type contains too many bits, which is hard to be used as comparison.");

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly string ISimpleFormattable.ToString(string? format) => ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IStatusMapBase<CandidateMap>.ExceptWith(IEnumerable<int> other) => this -= other;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IStatusMapBase<CandidateMap>.IntersectWith(IEnumerable<int> other) => this &= Empty + other;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IStatusMapBase<CandidateMap>.SymmetricExceptWith(IEnumerable<int> other) => this = (this - other) | (Empty + other - this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IStatusMapBase<CandidateMap>.UnionWith(IEnumerable<int> other) => this += other;


	/// <inheritdoc/>
	public static bool TryParse(string str, out CandidateMap result)
	{
		try
		{
			result = Parse(str);
			return true;
		}
		catch (FormatException)
		{
			SkipInit(out result);
			return false;
		}
	}

	/// <inheritdoc/>
	public static CandidateMap Parse(string str) => (CandidateMap)RxCyNotation.ParseCandidates(str);

	/// <inheritdoc/>
	static bool IParsable<CandidateMap>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out CandidateMap result)
	{
		try
		{
			if (s is null)
			{
				goto ReturnFalse;
			}

			return TryParse(s, out result);
		}
		catch
		{
		}

	ReturnFalse:
		SkipInit(out result);
		return false;
	}


	/// <inheritdoc/>
	public static bool operator !(scoped in CandidateMap offsets) => offsets ? false : true;

	/// <inheritdoc cref="IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"/>
	public static CellMap operator /(scoped in CandidateMap offsets, int digit)
	{
		var result = CellMap.Empty;
		foreach (var element in offsets)
		{
			if (element % 9 == digit)
			{
				result.Add(element / 9);
			}
		}

		return result;
	}

	/// <inheritdoc/>
	public static CandidateMap operator +(scoped in CandidateMap collection, int offset)
	{
		if (collection.Contains(offset))
		{
			return collection;
		}

		var copied = collection;
		copied.Add(offset);

		return copied;
	}

	/// <inheritdoc/>
	public static CandidateMap operator +(scoped in CandidateMap collection, IEnumerable<int> offsets)
	{
		var copied = collection;
		copied.AddRange(offsets);

		return copied;
	}

	/// <inheritdoc/>
	public static CandidateMap operator -(scoped in CandidateMap collection, int offset)
	{
		if (collection.Contains(offset))
		{
			return collection;
		}

		var copied = collection;
		copied.Remove(offset);

		return copied;
	}

	/// <inheritdoc/>
	public static CandidateMap operator -(scoped in CandidateMap collection, IEnumerable<int> offsets)
	{
		var copied = collection;
		foreach (var element in offsets)
		{
			copied.Remove(element);
		}

		return copied;
	}

	/// <inheritdoc/>
	public static CandidateMap operator ~(scoped in CandidateMap offsets)
	{
		var result = offsets;
		result._bits[11] = ~result._bits[11] & 0x1FFFFFF;
		for (var i = 0; i < 11; i++)
		{
			result._bits[i] = ~result._bits[i];
		}

		return result;
	}

	/// <inheritdoc/>
	public static CandidateMap operator &(scoped in CandidateMap left, scoped in CandidateMap right)
	{
		var copied = left;
		foreach (var pair in new LocalRefEnumerator(ref copied._bits[0], right._bits[0]))
		{
			pair.First &= pair.Second;
		}

		return copied;
	}

	/// <inheritdoc/>
	public static CandidateMap operator |(scoped in CandidateMap left, scoped in CandidateMap right)
	{
		var copied = left;
		foreach (var pair in new LocalRefEnumerator(ref copied._bits[0], right._bits[0]))
		{
			pair.First |= pair.Second;
		}

		return copied;
	}

	/// <inheritdoc/>
	public static CandidateMap operator ^(scoped in CandidateMap left, scoped in CandidateMap right)
	{
		var copied = left;
		foreach (var pair in new LocalRefEnumerator(ref copied._bits[0], right._bits[0]))
		{
			pair.First ^= pair.Second;
		}

		return copied;
	}

	/// <inheritdoc/>
	public static CandidateMap operator -(scoped in CandidateMap left, scoped in CandidateMap right)
	{
		var copied = left;
		foreach (var pair in new LocalRefEnumerator(ref copied._bits[0], right._bits[0]))
		{
			pair.First &= ~pair.Second;
		}

		return copied;
	}

	/// <inheritdoc/>
	/// <exception cref="NotImplementedException">Throws always.</exception>
	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap[] operator &(scoped in CandidateMap offsets, int subsetSize)
		=> throw new NotImplementedException("This method is too complex to be implemented. In the future I'll consider on it.");

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap IAdditionOperators<CandidateMap, int, CandidateMap>.operator +(CandidateMap left, int right)
		=> left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap IAdditionOperators<CandidateMap, IEnumerable<int>, CandidateMap>.operator +(CandidateMap left, IEnumerable<int> right)
		=> left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap ISubtractionOperators<CandidateMap, int, CandidateMap>.operator -(CandidateMap left, int right)
		=> left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap ISubtractionOperators<CandidateMap, IEnumerable<int>, CandidateMap>.operator -(CandidateMap left, IEnumerable<int> right)
		=> left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IDivisionOperators<CandidateMap, int, CellMap>.operator /(CandidateMap left, int right) => left / right;


#if DEBUG
	/// <summary>
	/// Implicit cast. This method will be removed in the future.
	/// </summary>
	public static explicit operator Candidates(CandidateMap collection)
	{
		var result = Candidates.Empty;
		foreach (var element in collection.Offsets)
		{
			result.Add(element);
		}

		return result;
	}

	/// <summary>
	/// Implicit cast. This method will be removed in the future.
	/// </summary>
	public static explicit operator CandidateMap(Candidates collection)
	{
		var result = Empty;
		foreach (var element in collection)
		{
			result.Add(element);
		}

		return result;
	}
#endif
}

/// <summary>
/// A reference pair.
/// </summary>
file readonly ref struct RefPair
{
	/// <summary>
	/// The first reference.
	/// </summary>
	public readonly ref long First;

	/// <summary>
	/// The second reference.
	/// </summary>
	public readonly ref readonly long Second;


	/// <summary>
	/// Initializes a <see cref="RefPair"/> instance.
	/// </summary>
	public RefPair(ref long first, in long second)
	{
		First = ref first;
		Second = ref second;
	}
}

/// <summary>
/// Defines an enumerator that iterates the one-dimensional array.
/// </summary>
file ref struct LocalRefEnumerator
{
	/// <summary>
	/// Indicates the first reference.
	/// </summary>
	private readonly ref long _refFirst;

	/// <summary>
	/// Indicates the second reference.
	/// </summary>
	private readonly ref readonly long _refSecond;

	/// <summary>
	/// Indicates the current index being iterated.
	/// </summary>
	private int _index = -1;


	/// <summary>
	/// Initializes a <see cref="LocalRefEnumerator"/> instance via the specified two references to iterate.
	/// </summary>
	/// <param name="first">The first reference.</param>
	/// <param name="second">The second reference.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal LocalRefEnumerator(ref long first, in long second)
	{
		_refFirst = ref first;
		_refSecond = ref second;
	}


	/// <summary>
	/// Indicates the current instance being iterated. Please note that the value is returned by reference.
	/// </summary>
	public readonly RefPair Current
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new(ref AddByteOffset(ref _refFirst, _index), AddByteOffset(ref AsRef(_refSecond), _index));
	}


	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public readonly LocalRefEnumerator GetEnumerator() => this;

	/// <summary>
	/// Retrieve the iterator to make it points to the next element.
	/// </summary>
	/// <returns>
	/// A <see cref="bool"/> value indicating whether the moving operation is successful.
	/// Returns <see langword="false"/> when the last iteration is for the last element,
	/// and now there's no elements to be iterated.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool MoveNext() => ++_index < 12;
}
