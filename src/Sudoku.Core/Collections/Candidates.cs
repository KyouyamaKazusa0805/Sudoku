using System.ComponentModel;
using static System.Numerics.BitOperations;
using static Sudoku.Constants;
using static Sudoku.Constants.Tables;

namespace Sudoku.Collections;

/// <summary>
/// Encapsulates a map that contains 729 positions to represent a candidate.
/// </summary>
public unsafe struct Candidates :
	IDefaultable<Candidates>,
	IEnumerable<int>,
	IEquatable<Candidates>,
	ISimpleFormattable
#if FEATURE_GENERIC_MATH
	,
	IAdditionOperators<Candidates, int, Candidates>,
	ISubtractionOperators<Candidates, int, Candidates>,
	ISubtractionOperators<Candidates, Candidates, Candidates>,
	IDivisionOperators<Candidates, int, Cells>,
	IModulusOperators<Candidates, Candidates, Candidates>,
	IBitwiseOperators<Candidates, Candidates, Candidates>,
	IEqualityOperators<Candidates, Candidates>
#if FEATURE_GENERIC_MATH_IN_ARG
	,
	IValueAdditionOperators<Candidates, int, Candidates>,
	IValueSubtractionOperators<Candidates, int, Candidates>,
	IValueSubtractionOperators<Candidates, Candidates, Candidates>,
	IValueDivisionOperators<Candidates, int, Cells>,
	IValueModulusOperators<Candidates, Candidates, Candidates>,
	IValueEqualityOperators<Candidates, Candidates>,
	IValueBitwiseNotOperators<Candidates, Candidates>,
	IValueBitwiseAndOperators<Candidates, Candidates, Candidates>,
	IValueBitwiseOrOperators<Candidates, Candidates, Candidates>,
	IValueBitwiseExclusiveOrOperators<Candidates, Candidates, Candidates>,
	IValueLogicalNotOperators<Candidates>,
	IValueGreaterThanOrLessThanOperators<Candidates, Candidates>
#endif
#endif
{
	/// <summary>
	/// Indicates the size of each unit.
	/// </summary>
	private const int Shifting = sizeof(long) * 8;


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
	/// Initializes a <see cref="Candidates"/> instance via the read-only field <see cref="Empty"/>.
	/// </summary>
	/// <remarks>
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp9/feature[@name="parameterless-struct-constructor"]/target[@name="constructor"]' />
	/// </remarks>
	/// <seealso cref="Empty"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Candidates() => this = Empty;

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
		if (binary.Length != 12)
		{
			throw new ArgumentException($"The length of the array should be 12.", nameof(binary));
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
		if (length != 12)
		{
			throw new ArgumentException($"Argument '{nameof(length)}' should be {12}.", nameof(length));
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
	/// Indicates whether the collection is empty.
	/// </summary>
	public readonly bool IsEmpty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Count == 0;
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
			if (IsEmpty)
			{
				return Array.Empty<int>();
			}

			int[] result = new int[Count];
			int count = 0;
			for (int i = 0; i < 729; i++)
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
	readonly bool IDefaultable<Candidates>.IsDefault => IsEmpty;

	/// <inheritdoc/>
	static Candidates IDefaultable<Candidates>.Default => Empty;


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
	/// <exception cref="InvalidOperationException">
	/// Throws when the capacity isn't enough to store all values.
	/// </exception>
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
	public readonly void CopyTo(ref Span<int> span)
	{
		fixed (int* arr = span)
		{
			CopyTo(arr, span.Length);
		}
	}

	/// <summary>
	/// Determine whether the map contains the specified offset.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
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

	/// <summary>
	/// Get all offsets whose bits are set <see langword="true"/>.
	/// </summary>
	/// <returns>An array of offsets.</returns>
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
			var sb = new StringHandler(50);

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
	public void AddRange(in ReadOnlySpan<int> offsets)
	{
		foreach (int candidate in offsets)
		{
			AddAnyway(candidate);
		}
	}

	/// <inheritdoc cref="AddRange(in ReadOnlySpan{int})"/>
	public void AddRange(IEnumerable<int> offsets)
	{
		foreach (int candidate in offsets)
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
	public static Candidates Parse(string str) => Parse(str, (CandidatesParsingOptions)7);

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
		if (options is 0 or > (CandidatesParsingOptions)7)
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
	public static Candidates operator !(in Candidates offsets)
	{
		if (offsets.Count == 0)
		{
			// Empty list can't contain any peer intersections.
			return Empty;
		}

		var result = ~Empty;
		foreach (int candidate in offsets.Offsets)
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
	public static Candidates operator ~(in Candidates offsets)
	{
		const long s = (1 << 729 - Shifting * (12 - 1)) - 1;

		long* result = stackalloc long[12];
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
	/// The syntactic sugar for <c>!(<paramref name="left"/> - <paramref name="right"/>).IsEmpty</c>.
	/// </summary>
	/// <param name="left">The subtrahend.</param>
	/// <param name="right">The subtractor.</param>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	public static bool operator >(in Candidates left, in Candidates right) =>
		!(left - right).IsEmpty;

	/// <summary>
	/// The syntactic sugar for <c>(<paramref name="left"/> - <paramref name="right"/>).IsEmpty</c>.
	/// </summary>
	/// <param name="left">The subtrahend.</param>
	/// <param name="right">The subtractor.</param>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	public static bool operator <(in Candidates left, in Candidates right) =>
		(left - right).IsEmpty;

	/// <summary>
	/// Get the elements that both <paramref name="left"/> and <paramref name="right"/> contain.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidates operator &(in Candidates left, in Candidates right)
	{
		long* result = stackalloc long[12];
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
	public static Candidates operator |(in Candidates left, in Candidates right)
	{
		long* result = stackalloc long[12];
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
	public static Candidates operator ^(in Candidates left, in Candidates right)
	{
		long* result = stackalloc long[12];
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
	public static Candidates operator -(in Candidates left, in Candidates right)
	{
		long* result = stackalloc long[12];
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
	public static Candidates operator -(in Candidates collection, int offset)
	{
		var result = collection;
		if (!result.Contains(offset))
		{
			return result;
		}

		var pThis = &result;
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
	public static Candidates operator +(in Candidates collection, int offset)
	{
		var result = collection;
		if (result.Contains(offset))
		{
			return result;
		}

		var pThis = &result;
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
	public static Candidates operator %(in Candidates @base, in Candidates template) =>
		!(@base & template) & template;

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

#if FEATURE_GENERIC_MATH
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Candidates, Candidates>.operator ==(Candidates left, Candidates right) =>
		left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Candidates, Candidates>.operator !=(Candidates left, Candidates right) =>
		left != right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IAdditionOperators<Candidates, int, Candidates>.operator +(Candidates left, int right) =>
		left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates ISubtractionOperators<Candidates, int, Candidates>.operator -(Candidates left, int right) =>
		left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates ISubtractionOperators<Candidates, Candidates, Candidates>.operator -(Candidates left, Candidates right) =>
		left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IDivisionOperators<Candidates, int, Cells>.operator /(Candidates left, int right) =>
		left / right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IModulusOperators<Candidates, Candidates, Candidates>.operator %(Candidates left, Candidates right) =>
		left % right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IValueAdditionOperators<Candidates, int, Candidates>.operator +(Candidates left, in int right) =>
		left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IValueAdditionOperators<Candidates, int, Candidates>.operator +(in Candidates left, in int right) =>
		left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IValueSubtractionOperators<Candidates, int, Candidates>.operator -(Candidates left, in int right) =>
		left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IValueSubtractionOperators<Candidates, int, Candidates>.operator -(in Candidates left, in int right) =>
		left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IValueSubtractionOperators<Candidates, Candidates, Candidates>.operator -(Candidates left, in Candidates right) =>
		left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IValueSubtractionOperators<Candidates, Candidates, Candidates>.operator -(in Candidates left, Candidates right) =>
		left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueDivisionOperators<Candidates, int, Cells>.operator /(Candidates left, in int right) =>
		left / right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueDivisionOperators<Candidates, int, Cells>.operator /(in Candidates left, in int right) =>
		left / right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IValueModulusOperators<Candidates, Candidates, Candidates>.operator %(Candidates left, in Candidates right) =>
		left % right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IValueModulusOperators<Candidates, Candidates, Candidates>.operator %(in Candidates left, Candidates right) =>
		left % right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IBitwiseOperators<Candidates, Candidates, Candidates>.operator ~(Candidates value) =>
		~value;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IBitwiseOperators<Candidates, Candidates, Candidates>.operator &(Candidates left, Candidates right) =>
		left & right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IBitwiseOperators<Candidates, Candidates, Candidates>.operator |(Candidates left, Candidates right) =>
		left | right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IBitwiseOperators<Candidates, Candidates, Candidates>.operator ^(Candidates left, Candidates right) =>
		left ^ right;

#if FEATURE_GENERIC_MATH_IN_ARG
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<Candidates, Candidates>.operator ==(Candidates left, in Candidates right) =>
		left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<Candidates, Candidates>.operator ==(in Candidates left, Candidates right) =>
		left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<Candidates, Candidates>.operator !=(Candidates left, in Candidates right) =>
		left != right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<Candidates, Candidates>.operator !=(in Candidates left, Candidates right) =>
		left != right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IValueBitwiseAndOperators<Candidates, Candidates, Candidates>.operator &(in Candidates left, Candidates right) =>
		left & right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IValueBitwiseAndOperators<Candidates, Candidates, Candidates>.operator &(Candidates left, in Candidates right) =>
		left & right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IValueBitwiseOrOperators<Candidates, Candidates, Candidates>.operator |(Candidates left, in Candidates right) =>
		left | right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IValueBitwiseOrOperators<Candidates, Candidates, Candidates>.operator |(in Candidates left, Candidates right) =>
		left | right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IValueBitwiseExclusiveOrOperators<Candidates, Candidates, Candidates>.operator ^(Candidates left, in Candidates right) =>
		left ^ right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IValueBitwiseExclusiveOrOperators<Candidates, Candidates, Candidates>.operator ^(in Candidates left, Candidates right) =>
		left ^ right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueGreaterThanOrLessThanOperators<Candidates, Candidates>.operator >(Candidates left, in Candidates right) =>
		left > right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueGreaterThanOrLessThanOperators<Candidates, Candidates>.operator >(in Candidates left, Candidates right) =>
		left > right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueGreaterThanOrLessThanOperators<Candidates, Candidates>.operator <(Candidates left, in Candidates right) =>
		left < right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueGreaterThanOrLessThanOperators<Candidates, Candidates>.operator <(in Candidates left, Candidates right) =>
		left < right;
#endif
#endif


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
	public static implicit operator Candidates(in Span<int> offsets) => new(offsets);

	/// <summary>
	/// Implicit cast from <see cref="ReadOnlySpan{T}"/> to <see cref="Candidates"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Candidates(in ReadOnlySpan<int> offsets) => new(offsets);

	/// <summary>
	/// Explicit cast from <see cref="Candidates"/> to <see cref="int"/>[].
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator int[](in Candidates offsets) => offsets.ToArray();

	/// <summary>
	/// Explicit cast from <see cref="Candidates"/> to <see cref="Span{T}"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Span<int>(in Candidates offsets) => offsets.ToSpan();

	/// <summary>
	/// Explicit cast from <see cref="Candidates"/> to <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator ReadOnlySpan<int>(in Candidates offsets) => offsets.ToReadOnlySpan();
}
