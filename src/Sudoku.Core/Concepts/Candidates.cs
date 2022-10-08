namespace Sudoku.Concepts;

/// <summary>
/// Encapsulates a map that contains 729 positions to represent a candidate.
/// </summary>
/// <remarks><i>
/// This type is being deprecated. You can also use this type but we don't recommend you use it.
/// In the future, I'll re-consider about the design of this type.
/// </i></remarks>
public unsafe struct Candidates :
	IEnumerable<int>,
	IEquatable<Candidates>,
	ISimpleFormattable,
	IAdditionOperators<Candidates, int, Candidates>,
	ISubtractionOperators<Candidates, int, Candidates>,
	ISubtractionOperators<Candidates, Candidates, Candidates>,
	IDivisionOperators<Candidates, int, CellMap>,
	IModulusOperators<Candidates, Candidates, Candidates>,
	IBitwiseOperators<Candidates, Candidates, Candidates>,
	IEqualityOperators<Candidates, Candidates, bool>
{
	/// <summary>
	/// Indicates the size of each unit.
	/// </summary>
	private const int Shifting = sizeof(long) * 8;


	/// <summary>
	/// Indicates an empty instance (all bits are 0).
	/// </summary>
	public static readonly Candidates Empty;


	/// <summary>
	/// The inner binary values.
	/// </summary>
	private long _0 = 0, _1 = 0, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0;


	/// <summary>
	/// Throws a <see cref="NotSupportedException"/>.
	/// </summary>
	/// <exception cref="NotSupportedException">
	/// The exception will always be thrown.
	/// </exception>
	/// <remarks>
	/// The main idea of the parameterless constructor is to create a new instance
	/// without any extra information, but the current type is special:
	/// the author wants to make you use another member instead of it to get a better experience.
	/// Therefore, the parameterless constructor is disallowed to be invoked
	/// no matter what kind of invocation, reflection or strongly reference.
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete($"Please use the member '{nameof(Empty)}' instead.", true)]
	public Candidates() => throw new NotSupportedException();

	/// <summary>
	/// Initializes an instance with the specified candidate and its peers.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Candidates(int candidate) : this(candidate, true)
	{
	}

	/// <summary>
	/// Initializes an instance with the candidate list specified as a pointer.
	/// </summary>
	/// <param name="candidates">The pointer points to an array of elements.</param>
	/// <param name="length">The length of the array.</param>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="candidates"/> is <see langword="null"/>.
	/// </exception>
	public Candidates(int* candidates, int length)
	{
		ArgumentNullException.ThrowIfNull(candidates);

		this = default;
		for (var i = 0; i < length; i++)
		{
			InternalAdd(candidates[i], true);
		}
	}

	/// <summary>
	/// Indicates an instance with the peer candidates of the specified candidate and a <see cref="bool"/>
	/// value indicating whether the map will process itself with <see langword="true"/> value.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	/// <param name="setItself">
	/// Indicates whether the map will process itself with <see langword="true"/> value.
	/// </param>
	public Candidates(int candidate, bool setItself)
	{
		this = default;
		int cell = candidate / 9, digit = candidate % 9;
		foreach (var c in PeersMap[cell])
		{
			InternalAdd(c * 9 + digit, true);
		}
		for (var d = 0; d < 9; d++)
		{
			if (d != digit || d == digit && setItself)
			{
				InternalAdd(cell * 9 + d, true);
			}
		}
	}

	/// <summary>
	/// Initializes an instance with the specified candidates.
	/// </summary>
	/// <param name="candidates">The candidates.</param>
	public Candidates(int[] candidates)
	{
		foreach (var candidate in candidates)
		{
			InternalAdd(candidate, true);
		}
	}

	/// <summary>
	/// Initializes an instance with the binary array.
	/// </summary>
	/// <param name="binary">The array.</param>
	/// <exception cref="ArgumentException">Throws when the length is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Candidates(long[] binary)
	{
		Argument.ThrowIfNotEqual(binary.Length, 12, nameof(binary));

		var count = 0;

		_0 = binary[0]; count += PopCount((ulong)binary[0]);
		_1 = binary[1]; count += PopCount((ulong)binary[1]);
		_2 = binary[2]; count += PopCount((ulong)binary[2]);
		_3 = binary[3]; count += PopCount((ulong)binary[3]);
		_4 = binary[4]; count += PopCount((ulong)binary[4]);
		_5 = binary[5]; count += PopCount((ulong)binary[5]);
		_6 = binary[6]; count += PopCount((ulong)binary[6]);
		_7 = binary[7]; count += PopCount((ulong)binary[7]);
		_8 = binary[8]; count += PopCount((ulong)binary[8]);
		_9 = binary[9]; count += PopCount((ulong)binary[9]);
		_10 = binary[10]; count += PopCount((ulong)binary[10]);
		_11 = binary[11]; count += PopCount((ulong)binary[11]);

		Count = count;
	}

	/// <summary>
	/// Initializes an instance with the pointer to the binary array and the length.
	/// </summary>
	/// <param name="binary">The pointer to the binary array.</param>
	/// <param name="length">The length.</param>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="binary"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="ArgumentException">Throws when the length is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Candidates(long* binary, int length)
	{
		ArgumentNullException.ThrowIfNull(binary);
		Argument.ThrowIfNotEqual(length, 12);

		var count = 0;

		_0 = binary[0]; count += PopCount((ulong)binary[0]);
		_1 = binary[1]; count += PopCount((ulong)binary[1]);
		_2 = binary[2]; count += PopCount((ulong)binary[2]);
		_3 = binary[3]; count += PopCount((ulong)binary[3]);
		_4 = binary[4]; count += PopCount((ulong)binary[4]);
		_5 = binary[5]; count += PopCount((ulong)binary[5]);
		_6 = binary[6]; count += PopCount((ulong)binary[6]);
		_7 = binary[7]; count += PopCount((ulong)binary[7]);
		_8 = binary[8]; count += PopCount((ulong)binary[8]);
		_9 = binary[9]; count += PopCount((ulong)binary[9]);
		_10 = binary[10]; count += PopCount((ulong)binary[10]);
		_11 = binary[11]; count += PopCount((ulong)binary[11]);

		Count = count;
	}

	/// <summary>
	/// Initializes an instance with the specified <see cref="CellMap"/> and the number
	/// representing.
	/// </summary>
	/// <param name="map">The map.</param>
	/// <param name="digit">The digit.</param>
	public Candidates(scoped in CellMap map, int digit)
	{
		this = default;
		foreach (var cell in map)
		{
			InternalAdd(cell * 9 + digit, true);
		}
	}

	/// <summary>
	/// Initializes an instance with the specified candidates.
	/// </summary>
	/// <param name="candidates">The candidates.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Candidates(scoped in ReadOnlySpan<int> candidates)
	{
		this = default;
		AddRange(candidates);
	}

	/// <summary>
	/// Initializes an instance with the specified candidates.
	/// </summary>
	/// <param name="candidates">The candidates.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Candidates(IEnumerable<int> candidates)
	{
		this = default;
		AddRange(candidates);
	}

	/// <summary>
	/// Copies the values into the collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Candidates(
		long _0, long _1, long _2, long _3, long _4, long _5,
		long _6, long _7, long _8, long _9, long _10, long _11)
	{
		this._0 = _0;
		this._1 = _1;
		this._2 = _2;
		this._3 = _3;
		this._4 = _4;
		this._5 = _5;
		this._6 = _6;
		this._7 = _7;
		this._8 = _8;
		this._9 = _9;
		this._10 = _10;
		this._11 = _11;
		Count = PopCount((ulong)_0) + PopCount((ulong)_1) + PopCount((ulong)_2) + PopCount((ulong)_3)
			+ PopCount((ulong)_4) + PopCount((ulong)_5) + PopCount((ulong)_6) + PopCount((ulong)_7)
			+ PopCount((ulong)_8) + PopCount((ulong)_9) + PopCount((ulong)_10) + PopCount((ulong)_11);
	}


	/// <summary>
	/// Indicates the number of the values stored in this collection.
	/// </summary>
	public int Count { get; private set; } = 0;

	/// <summary>
	/// Indicates all indices of set bits.
	/// </summary>
	private readonly int[] Offsets
	{
		get
		{
			if (Count == 0)
			{
				return Array.Empty<int>();
			}

			var result = new int[Count];
			var count = 0;
			for (var i = 0; i < 729; i++)
			{
				if (Contains(i))
				{
					result[count++] = i;
				}
			}

			return result;
		}
	}


	/// <summary>
	/// Get the offset at the specified position index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>
	/// The offset at the specified position index. If the value is invalid, the return value will be <c>-1</c>.
	/// </returns>
	public readonly int this[int index]
	{
		get
		{
			for (int i = 0, count = -1; i < 729; i++)
			{
				if (Contains(i) && ++count == index)
				{
					return i;
				}
			}

			return -1;
		}
	}


	/// <summary>
	/// Copies the current instance to the target array specified as an <see cref="int"/>*.
	/// </summary>
	/// <param name="arr">The pointer that points to an array of type <see cref="int"/>.</param>
	/// <param name="length">The length of that array.</param>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="arr"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// Throws when the capacity isn't enough to store all values.
	/// </exception>
	public readonly void CopyTo(int* arr, int length)
	{
		ArgumentNullException.ThrowIfNull(arr);

		if (Count == 0)
		{
			return;
		}

		Argument.ThrowIfFalse(length >= Count, "The capacity is not enough.");

		for (int i = 0, count = 0; i < 729; i++)
		{
			if (Contains(i))
			{
				arr[count++] = i;
			}
		}
	}

	/// <summary>
	/// Copies the current instance to the target <see cref="Span{T}"/> instance.
	/// </summary>
	/// <param name="span">
	/// The target <see cref="Span{T}"/> instance.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly void CopyTo(scoped Span<int> span)
	{
		fixed (int* arr = span)
		{
			CopyTo(arr, span.Length);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is Candidates comparer && Equals(comparer);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(scoped in Candidates other)
		=> _0 == other._0 && _1 == other._1 && _2 == other._2 && _3 == other._3
		&& _4 == other._4 && _5 == other._5 && _6 == other._6 && _7 == other._7
		&& _8 == other._8 && _9 == other._9 && _10 == other._10 && _11 == other._11;

	/// <summary>
	/// Determine whether the map contains the specified offset.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="offset"/> is out of range.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(int offset)
		=> (
			(offset / Shifting) switch
			{
				0 => _0,
				1 => _1,
				2 => _2,
				3 => _3,
				4 => _4,
				5 => _5,
				6 => _6,
				7 => _7,
				8 => _8,
				9 => _9,
				10 => _10,
				11 => _11,
				_ => throw new ArgumentOutOfRangeException(nameof(offset))
			} >> offset % Shifting & 1
		) != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		var result = new HashCode();
		result.Add(_0);
		result.Add(_1);
		result.Add(_2);
		result.Add(_3);
		result.Add(_4);
		result.Add(_5);
		result.Add(_6);
		result.Add(_7);
		result.Add(_8);
		result.Add(_9);
		result.Add(_10);
		result.Add(_11);
		return result.ToHashCode();
	}

	/// <summary>
	/// Get all offsets whose bits are set <see langword="true"/>.
	/// </summary>
	/// <returns>An array of offsets.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int[] ToArray() => Offsets;

	/// <summary>
	/// Get the sub-view mask of this map.
	/// </summary>
	/// <param name="hosue">The house index.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The mask.</returns>
	public readonly short GetSubviewMask(int hosue, int digit)
	{
		short p = 0;
		for (int i = 0, length = HouseCells[hosue].Length; i < length; i++)
		{
			if (Contains(HouseCells[hosue][i] * 9 + digit))
			{
				p |= (short)(1 << i);
			}
		}

		return p;
	}

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly string ToString() => RxCyNotation.ToCandidatesString(this);

	/// <inheritdoc/>
	/// <remarks>
	/// <b><i>This method does not implement any formats. This method or even the whole type will be reconsidered.</i></b>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete($"This method does not implement any formats. Therefore we suggest you use '{nameof(ToString)}()' instead.", false)]
	public readonly string ToString(string? format) => RxCyNotation.ToCandidatesString(this);

	/// <summary>
	/// Get the final <see cref="CellMap"/> to get all cells that the corresponding indices
	/// are set <see langword="true"/>.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <returns>The map of all cells chosen.</returns>
	public readonly CellMap Reduce(int digit)
	{
		var result = CellMap.Empty;
		for (var cell = 0; cell < 81; cell++)
		{
			if (Contains(cell * 9 + digit))
			{
				result.Add(cell);
			}
		}

		return result;
	}

	/// <summary>
	/// Converts the current instance to a <see cref="Span{T}"/> of type <see cref="int"/>.
	/// </summary>
	/// <returns>The <see cref="Span{T}"/> of <see cref="int"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Span<int> ToSpan() => Offsets.AsSpan();

	/// <summary>
	/// Converts the current instance to a <see cref="ReadOnlySpan{T}"/> of type <see cref="int"/>.
	/// </summary>
	/// <returns>The <see cref="ReadOnlySpan{T}"/> of <see cref="int"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ReadOnlySpan<int> ToReadOnlySpan() => Offsets.AsSpan();

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly OneDimensionalArrayEnumerator<int> GetEnumerator() => Offsets.EnumerateImmutable();

	/// <summary>
	/// Set the specified offset as <see langword="true"/> or <see langword="false"/> value.
	/// </summary>
	/// <param name="offset">
	/// The offset. This value can be positive and negative. If 
	/// negative, the offset will be assigned <see langword="false"/>
	/// into the corresponding bit position of its absolute value.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Add(int offset)
	{
		switch (offset)
		{
			case >= 0 when !Contains(offset):
			{
				InternalAdd(offset, true);
				break;
			}
			case < 0 when Contains(~offset):
			{
				InternalAdd(~offset, false);
				break;
			}
		}
	}

	/// <summary>
	/// Set the specified offset as <see langword="true"/> value.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <remarks>
	/// Different with <see cref="Add(int)"/>, the method will process negative values,
	/// but this won't.
	/// </remarks>
	/// <seealso cref="Add(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddAnyway(int offset) => InternalAdd(offset, true);

	/// <summary>
	/// Set the specified offset as <see langword="false"/> value.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <remarks>
	/// Different with <see cref="Add(int)"/>, this method <b>can't</b> receive the negative value as the parameter.
	/// </remarks>
	/// <seealso cref="Add(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove(int offset) => InternalAdd(offset, false);

	/// <summary>
	/// Set the specified offsets as <see langword="true"/> value.
	/// </summary>
	/// <param name="offsets">The offsets to add.</param>
	public void AddRange(scoped in ReadOnlySpan<int> offsets)
	{
		foreach (var candidate in offsets)
		{
			AddAnyway(candidate);
		}
	}

	/// <inheritdoc cref="AddRange(in ReadOnlySpan{int})"/>
	public void AddRange(IEnumerable<int> offsets)
	{
		foreach (var candidate in offsets)
		{
			AddAnyway(candidate);
		}
	}

	/// <summary>
	/// Clear all bits.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear()
	{
		_0 = _1 = _2 = _3 = _4 = _5 = _6 = _7 = _8 = _9 = _10 = _11 = 0;
		Count = 0;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IEquatable<Candidates>.Equals(Candidates other) => Equals(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator IEnumerable.GetEnumerator() => Offsets.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator<int> IEnumerable<int>.GetEnumerator() => ((IEnumerable<int>)Offsets).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InternalAdd(int offset, bool value)
	{
		fixed (Candidates* pThis = &this)
		{
			var older = Contains(offset);
			var block = (offset / Shifting) switch
			{
				0 => &pThis->_0,
				1 => &pThis->_1,
				2 => &pThis->_2,
				3 => &pThis->_3,
				4 => &pThis->_4,
				5 => &pThis->_5,
				6 => &pThis->_6,
				7 => &pThis->_7,
				8 => &pThis->_8,
				9 => &pThis->_9,
				10 => &pThis->_10,
				11 => &pThis->_11
			};
			if (value)
			{
				*block |= 1L << offset % Shifting;
				if (!older)
				{
					Count++;
				}
			}
			else
			{
				*block &= ~(1L << offset % Shifting);
				if (older)
				{
					Count--;
				}
			}
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidates Parse(string str) => RxCyNotation.ParseCandidates(str);

	/// <summary>
	/// Parse a <see cref="string"/> and convert to the <see cref="Candidates"/> instance.
	/// </summary>
	/// <param name="str">The string text.</param>
	/// <param name="options">The options to parse.</param>
	/// <returns>The result instance.</returns>
	/// <exception cref="NotSupportedException">Always throws.</exception>
	[Obsolete("This method or even the whole type will be re-considered.", false)]
	public static Candidates Parse(string str, CandidatesParsingOptions options) => throw new NotSupportedException();

	/// <inheritdoc/>
	public static bool TryParse(string str, out Candidates result) => RxCyNotation.TryParseCandidates(str, out result);


	/// <summary>
	/// Gets the peer intersection of the current instance.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	/// <returns>The result list that is the peer intersection of the current instance.</returns>
	/// <remarks>
	/// A <b>Peer Intersection</b> is a set of candidates that all candidates
	/// from the base collection can be seen.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidates operator !(scoped in Candidates offsets)
	{
		if (offsets.Count == 0)
		{
			// Empty list can't contain any peer intersections.
			return Empty;
		}

		var result = ~Empty;
		foreach (var candidate in offsets.Offsets)
		{
			result &= new Candidates(candidate, false);
		}

		return result;
	}

	/// <summary>
	/// Reverse status for all offsets, which means all <see langword="true"/> bits
	/// will be set <see langword="false"/>, and all <see langword="false"/> bits
	/// will be set <see langword="true"/>.
	/// </summary>
	/// <param name="offsets">The instance to negate.</param>
	/// <returns>The negative result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidates operator ~(scoped in Candidates offsets)
	{
		const long s = (1 << 729 - Shifting * (12 - 1)) - 1;

		var result = stackalloc long[12];
		result[0] = ~offsets._0;
		result[1] = ~offsets._1;
		result[2] = ~offsets._2;
		result[3] = ~offsets._3;
		result[4] = ~offsets._4;
		result[5] = ~offsets._5;
		result[6] = ~offsets._6;
		result[7] = ~offsets._7;
		result[8] = ~offsets._8;
		result[9] = ~offsets._9;
		result[10] = ~offsets._10;
		result[11] = ~offsets._11 & s;

		return new(result, 12);
	}

	/// <summary>
	/// The syntactic sugar for <c>(<paramref name="left"/> - <paramref name="right"/>).Count != 0</c>.
	/// </summary>
	/// <param name="left">The subtrahend.</param>
	/// <param name="right">The subtractor.</param>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	public static bool operator >(scoped in Candidates left, scoped in Candidates right) => (left - right).Count != 0;

	/// <summary>
	/// The syntactic sugar for <c>(<paramref name="left"/> - <paramref name="right"/>).Count == 0</c>.
	/// </summary>
	/// <param name="left">The subtrahend.</param>
	/// <param name="right">The subtractor.</param>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	public static bool operator <(scoped in Candidates left, scoped in Candidates right) => (left - right).Count == 0;

	/// <summary>
	/// Get the elements that both <paramref name="left"/> and <paramref name="right"/> contain.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidates operator &(scoped in Candidates left, scoped in Candidates right)
	{
		var result = stackalloc long[12];
		result[0] = left._0 & right._0;
		result[1] = left._1 & right._1;
		result[2] = left._2 & right._2;
		result[3] = left._3 & right._3;
		result[4] = left._4 & right._4;
		result[5] = left._5 & right._5;
		result[6] = left._6 & right._6;
		result[7] = left._7 & right._7;
		result[8] = left._8 & right._8;
		result[9] = left._9 & right._9;
		result[10] = left._10 & right._10;
		result[11] = left._11 & right._11;

		return new(result, 12);
	}

	/// <summary>
	/// Combine the elements from <paramref name="left"/> and <paramref name="right"/>,
	/// and return the merged result.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidates operator |(scoped in Candidates left, scoped in Candidates right)
	{
		var result = stackalloc long[12];
		result[0] = left._0 | right._0;
		result[1] = left._1 | right._1;
		result[2] = left._2 | right._2;
		result[3] = left._3 | right._3;
		result[4] = left._4 | right._4;
		result[5] = left._5 | right._5;
		result[6] = left._6 | right._6;
		result[7] = left._7 | right._7;
		result[8] = left._8 | right._8;
		result[9] = left._9 | right._9;
		result[10] = left._10 | right._10;
		result[11] = left._11 | right._11;

		return new(result, 12);
	}

	/// <summary>
	/// Get the elements that either <paramref name="left"/> or <paramref name="right"/> contains.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidates operator ^(scoped in Candidates left, scoped in Candidates right)
	{
		var result = stackalloc long[12];
		result[0] = left._0 ^ right._0;
		result[1] = left._1 ^ right._1;
		result[2] = left._2 ^ right._2;
		result[3] = left._3 ^ right._3;
		result[4] = left._4 ^ right._4;
		result[5] = left._5 ^ right._5;
		result[6] = left._6 ^ right._6;
		result[7] = left._7 ^ right._7;
		result[8] = left._8 ^ right._8;
		result[9] = left._9 ^ right._9;
		result[10] = left._10 ^ right._10;
		result[11] = left._11 ^ right._11;

		return new(result, 12);
	}

	/// <summary>
	/// Get a <see cref="Candidates"/> that contains all <paramref name="left"/> instance
	/// but not in <paramref name="right"/> instance.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidates operator -(scoped in Candidates left, scoped in Candidates right)
	{
		var result = stackalloc long[12];
		result[0] = left._0 & ~right._0;
		result[1] = left._1 & ~right._1;
		result[2] = left._2 & ~right._2;
		result[3] = left._3 & ~right._3;
		result[4] = left._4 & ~right._4;
		result[5] = left._5 & ~right._5;
		result[6] = left._6 & ~right._6;
		result[7] = left._7 & ~right._7;
		result[8] = left._8 & ~right._8;
		result[9] = left._9 & ~right._9;
		result[10] = left._10 & ~right._10;
		result[11] = left._11 & ~right._11;

		return new(result, 12);
	}

	/// <summary>
	/// Removes the specified <paramref name="offset"/> from the <paramref name="collection"/>,
	/// and returns the removed result.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be removed.</param>
	/// <returns>The result collection.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="offset"/> is out of range.
	/// </exception>
	public static Candidates operator -(scoped in Candidates collection, int offset)
	{
		var result = collection;
		if (!result.Contains(offset))
		{
			return result;
		}

		var pThis = &result;
		var block = (offset / Shifting) switch
		{
			0 => &pThis->_0,
			1 => &pThis->_1,
			2 => &pThis->_2,
			3 => &pThis->_3,
			4 => &pThis->_4,
			5 => &pThis->_5,
			6 => &pThis->_6,
			7 => &pThis->_7,
			8 => &pThis->_8,
			9 => &pThis->_9,
			10 => &pThis->_10,
			11 => &pThis->_11,
			_ => throw new ArgumentOutOfRangeException(nameof(offset))
		};

		*block &= ~(1L << offset % Shifting);
		result.Count--;
		return result;
	}

	/// <summary>
	/// Adds the specified <paramref name="offset"/> to the <paramref name="collection"/>,
	/// and returns the added result.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be removed.</param>
	/// <returns>The result collection.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="offset"/> is out of range.
	/// </exception>
	public static Candidates operator +(scoped in Candidates collection, int offset)
	{
		var result = collection;
		if (result.Contains(offset))
		{
			return result;
		}

		var pThis = &result;
		var block = (offset / Shifting) switch
		{
			0 => &pThis->_0,
			1 => &pThis->_1,
			2 => &pThis->_2,
			3 => &pThis->_3,
			4 => &pThis->_4,
			5 => &pThis->_5,
			6 => &pThis->_6,
			7 => &pThis->_7,
			8 => &pThis->_8,
			9 => &pThis->_9,
			10 => &pThis->_10,
			11 => &pThis->_11,
			_ => throw new ArgumentOutOfRangeException(nameof(offset))
		};

		*block |= 1L << offset % Shifting;
		result.Count++;
		return result;
	}

	/// <summary>
	/// <para>Expands the operator to <c><![CDATA[!(a & b) & b]]></c>.</para>
	/// <para>The operator is used for searching for and checking eliminations.</para>
	/// </summary>
	/// <param name="base">The base map.</param>
	/// <param name="template">The template map that the base map to check and cover.</param>
	/// <returns>The result map.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidates operator %(scoped in Candidates @base, scoped in Candidates template)
		=> !(@base & template) & template;

	/// <summary>
	/// Simplified calls <see cref="Reduce(int)"/>.
	/// </summary>
	/// <param name="candidates">The candidates.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The cells.</returns>
	/// <seealso cref="Reduce(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator /(scoped in Candidates candidates, int digit) => candidates.Reduce(digit);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(scoped in Candidates left, scoped in Candidates right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(scoped in Candidates left, scoped in Candidates right) => !(left == right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Candidates, Candidates, bool>.operator ==(Candidates left, Candidates right)
		=> left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Candidates, Candidates, bool>.operator !=(Candidates left, Candidates right)
		=> left != right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IAdditionOperators<Candidates, int, Candidates>.operator +(Candidates left, int right)
		=> left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates ISubtractionOperators<Candidates, int, Candidates>.operator -(Candidates left, int right)
		=> left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates ISubtractionOperators<Candidates, Candidates, Candidates>.operator -(Candidates left, Candidates right)
		=> left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IDivisionOperators<Candidates, int, CellMap>.operator /(Candidates left, int right) => left / right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IModulusOperators<Candidates, Candidates, Candidates>.operator %(Candidates left, Candidates right)
		=> left % right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IBitwiseOperators<Candidates, Candidates, Candidates>.operator ~(Candidates value) => ~value;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IBitwiseOperators<Candidates, Candidates, Candidates>.operator &(Candidates left, Candidates right)
		=> left & right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IBitwiseOperators<Candidates, Candidates, Candidates>.operator |(Candidates left, Candidates right)
		=> left | right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IBitwiseOperators<Candidates, Candidates, Candidates>.operator ^(Candidates left, Candidates right)
		=> left ^ right;


	/// <summary>
	/// Implicit cast from <see cref="int"/>[] to <see cref="Candidates"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Candidates(int[] offsets) => new(offsets);

	/// <summary>
	/// Implicit cast from <see cref="Span{T}"/> to <see cref="Candidates"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Candidates(scoped in Span<int> offsets) => new(offsets);

	/// <summary>
	/// Implicit cast from <see cref="ReadOnlySpan{T}"/> to <see cref="Candidates"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Candidates(scoped in ReadOnlySpan<int> offsets) => new(offsets);

	/// <summary>
	/// Explicit cast from <see cref="Candidates"/> to <see cref="int"/>[].
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator int[](scoped in Candidates offsets) => offsets.ToArray();

	/// <summary>
	/// Explicit cast from <see cref="Candidates"/> to <see cref="Span{T}"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Span<int>(scoped in Candidates offsets) => offsets.ToSpan();

	/// <summary>
	/// Explicit cast from <see cref="Candidates"/> to <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator ReadOnlySpan<int>(scoped in Candidates offsets) => offsets.ToReadOnlySpan();
}
