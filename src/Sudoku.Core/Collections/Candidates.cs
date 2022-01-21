namespace Sudoku.Collections;

/// <summary>
/// Encapsulates a map that contains 729 positions to represent a candidate.
/// </summary>
public unsafe partial struct Candidates : ICellsOrCandidates<Candidates>
{
	/// <summary>
	/// Indicates the size of each unit.
	/// </summary>
	private const int Shifting = sizeof(long) * 8;

	/// <summary>
	/// Indicates the number of all segments.
	/// </summary>
	private const int Len = 12;

	/// <summary>
	/// Indicates the length of the collection that all bits are set <see langword="true"/>.
	/// </summary>
	private const int FullCount = 729;


	/// <summary>
	/// <para>Indicates an empty instance (all bits are 0).</para>
	/// <para>
	/// I strongly recommend you <b>should</b> use this instance instead of default constructor
	/// <see cref="Candidates()"/>.
	/// </para>
	/// </summary>
	/// <seealso cref="Candidates()"/>
	public static readonly Candidates Empty;


	/// <summary>
	/// The inner binary values.
	/// </summary>
	private long _0 = 0, _1 = 0, _2 = 0, _3 = 0, _4 = 0, _5 = 0, _6 = 0, _7 = 0, _8 = 0, _9 = 0, _10 = 0, _11 = 0;


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
	public Candidates(int* candidates, int length) : this()
	{
		for (int i = 0; i < length; i++)
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
	public Candidates(int candidate, bool setItself) : this()
	{
		int cell = candidate / 9, digit = candidate % 9;
		foreach (int c in PeerMaps[cell])
		{
			InternalAdd(c * 9 + digit, true);
		}
		for (int d = 0; d < 9; d++)
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
		foreach (int candidate in candidates)
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
		if (binary.Length != Len)
		{
			throw new ArgumentException($"The length of the array should be {Len}.", nameof(binary));
		}

		int count = 0;

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
	/// <exception cref="ArgumentException">Throws when the length is invalid.</exception>
	public Candidates(long* binary, int length)
	{
		if (length != Len)
		{
			throw new ArgumentException($"Argument '{nameof(length)}' should be {Len}.", nameof(length));
		}

		int count = 0;

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
	/// Initializes an instance with the specified <see cref="Cells"/> and the number
	/// representing.
	/// </summary>
	/// <param name="map">The map.</param>
	/// <param name="digit">The digit.</param>
	public Candidates(in Cells map, int digit) : this()
	{
		foreach (int cell in map)
		{
			InternalAdd(cell * 9 + digit, true);
		}
	}

	/// <summary>
	/// Initializes an instance with the specified candidates.
	/// </summary>
	/// <param name="candidates">The candidates.</param>
	public Candidates(in ReadOnlySpan<int> candidates) : this() => AddRange(candidates);

	/// <summary>
	/// Initializes an instance with the specified candidates.
	/// </summary>
	/// <param name="candidates">The candidates.</param>
	public Candidates(IEnumerable<int> candidates) : this() => AddRange(candidates);

	/// <summary>
	/// Copies the values into the collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Candidates(
		long _0,
		long _1,
		long _2,
		long _3,
		long _4,
		long _5,
		long _6,
		long _7,
		long _8,
		long _9,
		long _10,
		long _11
	)
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


	/// <inheritdoc/>
	public readonly bool IsEmpty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Count == 0;
	}

	/// <inheritdoc/>
	public int Count { get; private set; } = 0;

	/// <inheritdoc/>
	public readonly Candidates PeerIntersection
	{
		get
		{
			if (Count == 0)
			{
				// Empty list can't contain any peer intersections.
				return Empty;
			}

			var result = ~Empty;
			foreach (int candidate in Offsets)
			{
				result &= new Candidates(candidate, false);
			}

			return result;
		}
	}

	/// <summary>
	/// Indicates all indices of set bits.
	/// </summary>
	private readonly int[] Offsets
	{
		get
		{
			if (IsEmpty)
			{
				return Array.Empty<int>();
			}

			int[] result = new int[Count];
			int count = 0;
			for (int i = 0; i < FullCount; i++)
			{
				if (Contains(i))
				{
					result[count++] = i;
				}
			}

			return result;
		}
	}


	/// <inheritdoc/>
	public readonly int this[int index]
	{
		get
		{
			for (int i = 0, count = -1; i < FullCount; i++)
			{
				if (Contains(i) && ++count == index)
				{
					return i;
				}
			}

			return -1;
		}
	}


	/// <inheritdoc/>
	public readonly void CopyTo(int* arr, int length)
	{
		if (IsEmpty)
		{
			return;
		}

		if (length < Count)
		{
			throw new ArgumentException("The capacity is not enough.", nameof(arr));
		}

		for (int i = 0, count = 0; i < FullCount; i++)
		{
			if (Contains(i))
			{
				arr[count++] = i;
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly void CopyTo(ref Span<int> span)
	{
		fixed (int* arr = span)
		{
			CopyTo(arr, span.Length);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(int offset) =>
	(
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
			11 => _11
		} >> offset % Shifting & 1
	) != 0;

	/// <inheritdoc cref="object.Equals(object?)"/>
	public override readonly bool Equals([NotNullWhen(true)] object? obj) => base.Equals(obj);

	/// <summary>
	/// Determine whether the specified <see cref="Candidates"/> instance holds the same
	/// bits as the current instance.
	/// </summary>
	/// <param name="other">The instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(in Candidates other) =>
		_0 == other._0 && _1 == other._1 && _2 == other._2 && _3 == other._3
		&& _4 == other._4 && _5 == other._5 && _6 == other._6 && _7 == other._7
		&& _8 == other._8 && _9 == other._9 && _10 == other._10 && _11 == other._11;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() =>
		(int)(
			RotateLeft((ulong)_0, 3) ^ RotateRight((ulong)_1, 3)
			^ RotateLeft((ulong)_2, 6) ^ RotateRight((ulong)_3, 6)
			^ RotateLeft((ulong)_4, 9) ^ RotateRight((ulong)_5, 9)
			^ RotateLeft((ulong)_6, 12) ^ RotateRight((ulong)_7, 12)
			^ RotateLeft((ulong)_8, 15) ^ RotateRight((ulong)_9, 15)
			^ RotateLeft((ulong)_10, 18) ^ RotateRight((ulong)_11, 18)
		);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int[] ToArray() => Offsets;

	/// <summary>
	/// Get the subview mask of this map.
	/// </summary>
	/// <param name="region">The region.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The mask.</returns>
	public readonly short GetSubviewMask(int region, int digit)
	{
		short p = 0;
		for (int i = 0, length = RegionCells[region].Length; i < length; i++)
		{
			if (Contains(RegionCells[region][i] * 9 + digit))
			{
				p |= (short)(1 << i);
			}
		}

		return p;
	}

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly string ToString() => ToString(null);

	/// <inheritdoc/>
	public readonly string ToString(string? format)
	{
		return this switch
		{
			[] => "{ }",
			[var a] when (a / 9, a % 9) is (var c, var d) => $"r{c / 9 + 1}c{c % 9 + 1}({d + 1})",
			_ => f(Offsets)
		};


		static string f(int[] offsets)
		{
			const string separator = ", ";
			var sb = new StringHandler(initialCapacity: 50);

			foreach (var digitGroup in
				from candidate in offsets
				group candidate by candidate % 9 into digitGroups
				orderby digitGroups.Key
				select digitGroups)
			{
				var cells = Cells.Empty;
				foreach (int candidate in digitGroup)
				{
					cells.AddAnyway(candidate / 9);
				}

				sb.Append(cells);
				sb.Append('(');
				sb.Append(digitGroup.Key + 1);
				sb.Append(')');
				sb.Append(separator);
			}

			sb.RemoveFromEnd(separator.Length);
			return sb.ToStringAndClear();
		}
	}

	/// <summary>
	/// Get the final <see cref="Cells"/> to get all cells that the corresponding indices
	/// are set <see langword="true"/>.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <returns>The map of all cells chosen.</returns>
	public readonly Cells Reduce(int digit)
	{
		var result = Cells.Empty;
		for (int cell = 0; cell < 81; cell++)
		{
			if (Contains(cell * 9 + digit))
			{
				result.AddAnyway(cell);
			}
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Span<int> ToSpan() => Offsets.AsSpan();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ReadOnlySpan<int> ToReadOnlySpan() => Offsets.AsSpan();

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly OneDimensionalArrayEnumerator<int> GetEnumerator() => Offsets.EnumerateImmutable();

	/// <inheritdoc/>
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

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddAnyway(int offset) => InternalAdd(offset, true);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove(int offset) => InternalAdd(offset, false);

	/// <inheritdoc/>
	public void AddRange(in ReadOnlySpan<int> offsets)
	{
		foreach (int candidate in offsets)
		{
			AddAnyway(candidate);
		}
	}

	/// <inheritdoc/>
	public void AddRange(IEnumerable<int> offsets)
	{
		foreach (int candidate in offsets)
		{
			AddAnyway(candidate);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear()
	{
		_0 = _1 = _2 = _3 = _4 = _5 = _6 = _7 = _8 = _9 = _10 = _11 = 0;
		Count = 0;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator<int> IEnumerable<int>.GetEnumerator() => ((IEnumerable<int>)Offsets).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator IEnumerable.GetEnumerator() => Offsets.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InternalAdd(int offset, bool value)
	{
		fixed (Candidates* pThis = &this)
		{
			bool older = Contains(offset);
			long* block = (offset / Shifting) switch
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
	public static Candidates Parse(string str) => Parse(str, CandidatesParsingOptions.All);

	/// <summary>
	/// Parse a <see cref="string"/> and convert to the <see cref="Candidates"/> instance.
	/// </summary>
	/// <param name="str">The string text.</param>
	/// <param name="options">The options to parse.</param>
	/// <returns>The result instance.</returns>
	/// <exception cref="ArgumentException">Throws when <paramref name="options"/> is invalid.</exception>
	/// <exception cref="FormatException">Throws when the specified text is invalid to parse.</exception>
	public static Candidates Parse(string str, CandidatesParsingOptions options)
	{
		if (options is CandidatesParsingOptions.None or > CandidatesParsingOptions.All)
		{
			throw new ArgumentException("The option is invalid.", nameof(options));
		}

		var regex = new Regex(
			RegularExpressions.CandidateOrCandidateList,
			RegexOptions.ExplicitCapture,
			TimeSpan.FromSeconds(5)
		);

		// Check whether the match is successful.
		var matches = regex.Matches(str);
		if (matches.Count == 0)
		{
			throw new FormatException("The specified string can't match any candidate instance.");
		}

		var result = Empty;

		// Iterate on each match item.
		int* bufferDigits = stackalloc int[9];
		foreach (Match match in matches)
		{
			string value = match.Value;
			if (options.Flags(CandidatesParsingOptions.ShortForm)
				&& value.SatisfyPattern(RegularExpressions.CandidateListShortForm)
				&& value is [var a, var b, var c, ..])
			{
				result.AddAnyway((b - '1') * 81 + (c - '1') * 9 + a - '1');
			}
			else if (
				options.Flags(CandidatesParsingOptions.BracketForm)
				&& value.SatisfyPattern(RegularExpressions.CandidateListPrepositionalForm)
			)
			{
				var cells = Cells.Parse(value);
				int digitsCount = 0;
				fixed (char* pValue = value)
				{
					for (char* ptr = pValue; *ptr is not ('{' or 'R' or 'r'); ptr++)
					{
						bufferDigits[digitsCount++] = *ptr - '1';
					}
				}

				foreach (int cell in cells)
				{
					for (int i = 0; i < digitsCount; i++)
					{
						result.AddAnyway(cell * 9 + bufferDigits[i]);
					}
				}
			}
			else if (
				options.Flags(CandidatesParsingOptions.PrepositionalForm)
				&& value.SatisfyPattern(RegularExpressions.CandidateListPostpositionalForm)
			)
			{
				var cells = Cells.Parse(value);
				int digitsCount = 0;
				for (int i = value.IndexOf('(') + 1, length = value.Length; i < length; i++)
				{
					bufferDigits[digitsCount++] = value[i] - '1';
				}

				foreach (int cell in cells)
				{
					for (int i = 0; i < digitsCount; i++)
					{
						result.AddAnyway(cell * 9 + bufferDigits[i]);
					}
				}
			}
		}

		return result;
	}

	/// <inheritdoc/>
	public static bool TryParse(string str, out Candidates result)
	{
		try
		{
			result = Parse(str);
			return true;
		}
		catch (FormatException)
		{
			result = Empty;
			return false;
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidates operator ~(in Candidates map)
	{
		const long s = (1 << FullCount - Shifting * (Len - 1)) - 1;

		long* result = stackalloc long[Len];
		result[0] = ~map._0;
		result[1] = ~map._1;
		result[2] = ~map._2;
		result[3] = ~map._3;
		result[4] = ~map._4;
		result[5] = ~map._5;
		result[6] = ~map._6;
		result[7] = ~map._7;
		result[8] = ~map._8;
		result[9] = ~map._9;
		result[10] = ~map._10;
		result[11] = ~map._11 & s;

		return new(result, Len);
	}

	/// <inheritdoc/>
	public static bool operator >(in Candidates left, in Candidates right) =>
		!(left - right).IsEmpty;

	/// <inheritdoc/>
	public static bool operator <(in Candidates left, in Candidates right) =>
		(left - right).IsEmpty;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidates operator &(in Candidates left, in Candidates right)
	{
		long* result = stackalloc long[Len];
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

		return new(result, Len);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidates operator |(in Candidates left, in Candidates right)
	{
		long* result = stackalloc long[Len];
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

		return new(result, Len);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidates operator ^(in Candidates left, in Candidates right)
	{
		long* result = stackalloc long[Len];
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

		return new(result, Len);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidates operator -(in Candidates left, in Candidates right)
	{
		long* result = stackalloc long[Len];
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

		return new(result, Len);
	}

	/// <inheritdoc/>
	public static Candidates operator -(Candidates collection, int offset)
	{
		if (!collection.Contains(offset))
		{
			return collection;
		}

		var pThis = &collection;
		long* block = (offset / Shifting) switch
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

		*block &= ~(1L << offset % Shifting);
		collection.Count--;

		return collection;
	}

	/// <inheritdoc/>
	public static Candidates operator +(Candidates collection, int offset)
	{
		if (collection.Contains(offset))
		{
			return collection;
		}

		var pThis = &collection;
		long* block = (offset / Shifting) switch
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

		*block |= 1L << offset % Shifting;
		collection.Count++;
		return collection;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidates operator %(in Candidates @base, in Candidates template) =>
		(@base & template).PeerIntersection & template;

	/// <summary>
	/// Simplified calls <see cref="Reduce(int)"/>.
	/// </summary>
	/// <param name="candidates">The candidates.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The cells.</returns>
	/// <seealso cref="Reduce(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cells operator /(in Candidates candidates, int digit) => candidates.Reduce(digit);

	/// <summary>
	/// Determine whether the two <see cref="Candidates"/> instance hold same bits.
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(in Candidates left, in Candidates right) => left.Equals(right);

	/// <summary>
	/// Determine whether the two <see cref="Candidates"/> instance don't hold same bits.
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(in Candidates left, in Candidates right) => !(left == right);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Candidates(int[] offsets) => new(offsets);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Candidates(in Span<int> offsets) => new(offsets);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Candidates(in ReadOnlySpan<int> offsets) => new(offsets);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator int[](in Candidates offsets) => offsets.ToArray();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Span<int>(in Candidates offsets) => offsets.ToSpan();

	///<inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator ReadOnlySpan<int>(in Candidates offsets) => offsets.ToReadOnlySpan();
}
