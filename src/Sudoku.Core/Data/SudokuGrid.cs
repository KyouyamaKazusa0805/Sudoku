extern alias extended;

namespace Sudoku.Data;

/// <summary>
/// Encapsulates a sudoku grid using value type instead of reference type.
/// </summary>
#if DEBUG
[DebuggerDisplay($@"{{{nameof(ToString)}("".+:""),nq}}")]
#endif
[AutoDeconstruct(nameof(EmptyCells), nameof(BivalueCells), nameof(CandidateMap), nameof(DigitsMap), nameof(ValuesMap))]
[AutoFormattable]
[Obsolete($"Please use the type '{nameof(Grid)}' instead.", false)]
public unsafe partial struct SudokuGrid : IGrid<SudokuGrid>, IValueEquatable<SudokuGrid>, IFormattable, IJsonSerializable<SudokuGrid, SudokuGrid.JsonConverter>, extended::System.IParseable<SudokuGrid>
{
	/// <inheritdoc cref="IGrid{TGrid}.DefaultMask"/>
	public const short DefaultMask = EmptyMask | MaxCandidatesMask;

	/// <inheritdoc cref="IGrid{TGrid}.MaxCandidatesMask"/>
	public const short MaxCandidatesMask = (1 << RegionCellsCount) - 1;

	/// <inheritdoc cref="IGrid{TGrid}.EmptyMask"/>
	public const short EmptyMask = (int)CellStatus.Empty << RegionCellsCount;

	/// <inheritdoc cref="IGrid{TGrid}.ModifiableMask"/>
	public const short ModifiableMask = (int)CellStatus.Modifiable << RegionCellsCount;

	/// <inheritdoc cref="IGrid{TGrid}.GivenMask"/>
	public const short GivenMask = (int)CellStatus.Given << RegionCellsCount;

	/// <summary>
	/// The list of 64-based characters.
	/// </summary>
	private const string Base64List = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,.";

	/// <summary>
	/// Indicates the size of each region.
	/// </summary>
	private const byte RegionCellsCount = 9;

	/// <summary>
	/// Indicates the size of each grid.
	/// </summary>
	private const byte Length = 81;

	/// <summary>
	/// Indicates the length of the string <see cref="Base64List"/>.
	/// </summary>
	/// <seealso cref="Base64List"/>
	private const byte Base64Length = 64;


	/// <inheritdoc cref="IGrid{TGrid}.EmptyString"/>
	public static readonly string EmptyString = new('0', Length);

	/// <inheritdoc cref="IGrid{TGrid}.ValueChanged"/>
	public static readonly delegate*<ref SudokuGrid, in ValueChangedArgs, void> ValueChanged;

	/// <inheritdoc cref="IGrid{TGrid}.RefreshingCandidates"/>
	public static readonly delegate*<ref SudokuGrid, void> RefreshingCandidates;

	/// <inheritdoc cref="IGrid{TGrid}.Undefined"/>
	public static readonly SudokuGrid Undefined;

	/// <inheritdoc cref="IGrid{TGrid}.Empty"/>
	public static readonly SudokuGrid Empty;

	/// <summary>
	/// The lookup table.
	/// </summary>
	private static readonly IReadOnlyDictionary<char, int> Lookup;


	/// <summary>
	/// Indicates the inner array that stores the masks of the sudoku grid, where:
	/// <list type="table">
	/// <item>
	/// <term><c>_values</c></term>
	/// <description>Stores the in-time sudoku grid inner information.</description>
	/// </item>
	/// <item>
	/// <term><c>_initialValues</c></term>
	/// <description>Stores the initial information of a sudoku grid.</description>
	/// </item>
	/// </list>
	/// </summary>
	private fixed short _values[Length], _initialValues[Length];


	/// <summary>
	/// Creates an instance using grid values.
	/// </summary>
	/// <param name="gridValues">The array of grid values.</param>
	public SudokuGrid(int[] gridValues) : this(gridValues, GridCreatingOption.None)
	{
	}

	/// <summary>
	/// Creates an instance using grid values.
	/// </summary>
	/// <param name="gridValues">The array of grid values.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	public SudokuGrid(int[] gridValues, GridCreatingOption creatingOption)
	{
		this = Empty;
		for (int i = 0; i < Length; i++)
		{
			if (gridValues[i] is var value and not 0)
			{
				// Calls the indexer to trigger the event
				// (Clear the candidates in peer cells).
				this[i] = creatingOption == GridCreatingOption.MinusOne ? value - 1 : value;

				// Set the status to 'CellStatus.Given'.
				SetStatus(i, CellStatus.Given);
			}
		}
	}

	/// <summary>
	/// Try to parse a token, and converts the token to the sudoku grid instance.
	/// </summary>
	/// <param name="token">The token.</param>
	public SudokuGrid(string token)
	{
		var bi = BigInteger.Zero;
		for (int i = 0, length = token.Length; i < length; i++)
		{
			bi += Lookup[token[i]] * BigInteger.Pow(Base64Length, i);
		}

		this = Parse(bi.ToString().PadLeft(Length, '0'));
	}

	/// <summary>
	/// Initializes an instance with the specified mask array.
	/// </summary>
	/// <param name="masks">The masks.</param>
	/// <exception cref="ArgumentException">Throws when <see cref="Array.Length"/> is not 81.</exception>
	internal SudokuGrid(short[] masks)
	{
#if DEBUG
		Debug.Assert(masks.Length == Length, $"The length of the array argument should be {Length}.");
#endif

		fixed (short* pArray = masks, pValues = _values, pInitialValues = _initialValues)
		{
			Unsafe.CopyBlock(pValues, pArray, sizeof(short) * Length);
			Unsafe.CopyBlock(pInitialValues, pArray, sizeof(short) * Length);
		}
	}


	[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")]
	static SudokuGrid()
	{
		// Initializes the empty grid.
#pragma warning disable SD0303
		Empty = default;
#pragma warning restore SD0303
		fixed (short* p = Empty._values, q = Empty._initialValues)
		{
			int i = 0;
			for (short* ptrP = p, ptrQ = q; i < Length; *ptrP++ = *ptrQ++ = DefaultMask, i++) ;
		}

		// Lookup table.
		Lookup = new Dictionary<char, int>(
			from i in Enumerable.Range(0, Base64Length)
			select new KeyValuePair<char, int>(Base64List[i], i)
		);

		// Initializes events.
		ValueChanged = &onValueChanged;
		RefreshingCandidates = &onRefreshingCandidates;


		static void onValueChanged(ref SudokuGrid @this, in ValueChangedArgs e)
		{
			if (e is { Cell: var cell, SetValue: var setValue and not -1 })
			{
				foreach (int peerCell in PeerMaps[cell])
				{
					if (@this.GetStatus(peerCell) == CellStatus.Empty)
					{
						// You can't do this because of being invoked recursively.
						//@this[peerCell, setValue] = false;

						@this._values[peerCell] &= (short)~(1 << setValue);
					}
				}
			}
		}

		static void onRefreshingCandidates(ref SudokuGrid @this)
		{
			for (int i = 0; i < Length; i++)
			{
				if (@this.GetStatus(i) == CellStatus.Empty)
				{
					// Remove all appeared digits.
					short mask = MaxCandidatesMask;
					foreach (int cell in PeerMaps[i])
					{
						if (@this[cell] is var digit and not -1)
						{
							mask &= (short)~(1 << digit);
						}
					}

					@this._values[i] = (short)(EmptyMask | mask);
				}
			}
		}
	}


	/// <inheritdoc/>
	public readonly bool IsSolved
	{
		get
		{
			for (int i = 0; i < Length; i++)
			{
				if (GetStatus(i) == CellStatus.Empty)
				{
					return false;
				}
			}

			return SimplyValidate();
		}
	}

	/// <inheritdoc/>
	public readonly bool IsUndefined
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this == Undefined;
	}

	/// <inheritdoc/>
	public readonly bool IsEmpty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this == Empty;
	}

#if DEBUG
	/// <inheritdoc/>
	public readonly bool IsDebuggerUndefined
	{
		get
		{
			fixed (short* pGrid = _values)
			{
				int i = 1;
				short* p = pGrid + 1;
				while (i < Length)
				{
					if (p++[i++] != 0)
					{
						return false;
					}
				}

				return true;
			}
		}
	}
#endif

	/// <inheritdoc/>
	public readonly int CandidatesCount
	{
		get
		{
			int count = 0;
			for (int i = 0; i < Length; i++)
			{
				if (GetStatus(i) == CellStatus.Empty)
				{
					count += PopCount((uint)GetCandidates(i));
				}
			}

			return count;
		}
	}

	/// <inheritdoc/>
	public readonly int GivensCount => Triplet.C;

	/// <inheritdoc/>
	public readonly int ModifiablesCount => Triplet.B;

	/// <inheritdoc/>
	public readonly int EmptiesCount => Triplet.A;

	/// <inheritdoc/>
	public readonly string Token
	{
		get
		{
			// The maximum grid as the base 64 is of length 45.
			var sb = new StringHandler(initialCapacity: 45);
			for (var temp = BigInteger.Parse(EigenString); temp > 0; temp /= Base64Length)
			{
				sb.AppendChar(Base64List[(int)(temp % Base64Length)]);
			}

			return sb.ToStringAndClear();
		}
	}

	/// <inheritdoc/>
	public readonly string EigenString => ToString("0").TrimStart('0');

	/// <inheritdoc/>
	public readonly Cells EmptyCells
	{
		get
		{
			return GetCells(&p);

			static bool p(in SudokuGrid g, int cell) => g.GetStatus(cell) == CellStatus.Empty;
		}
	}

	/// <inheritdoc/>
	public readonly Cells BivalueCells
	{
		get
		{
			return GetCells(&p);

			static bool p(in SudokuGrid g, int cell) => PopCount((uint)g.GetCandidates(cell)) == 2;
		}
	}

	/// <inheritdoc/>
	public readonly Cells[] CandidateMap
	{
		get
		{
			return GetMap(&p);

			static bool p(in SudokuGrid g, int cell, int digit) => g.Exists(cell, digit) is true;
		}
	}

	/// <inheritdoc/>
	public readonly Cells[] DigitsMap
	{
		get
		{
			return GetMap(&p);

			static bool p(in SudokuGrid g, int cell, int digit) => (g.GetCandidates(cell) >> digit & 1) != 0;
		}
	}

	/// <inheritdoc/>
	public readonly Cells[] ValuesMap
	{
		get
		{
			return GetMap(&p);

			static bool p(in SudokuGrid g, int cell, int digit) => g[cell] == digit;
		}
	}

	/// <summary>
	/// The triplet of three main information.
	/// </summary>
	private readonly (int A, int B, int C) Triplet
	{
		get
		{
			int a = 0, b = 0, c = 0;
			for (int i = 0; i < Length; i++)
			{
				var s = GetStatus(i);
				(s == CellStatus.Empty ? ref a : ref s == CellStatus.Modifiable ? ref b : ref c)++;
			}

			return (a, b, c);
		}
	}

	/// <inheritdoc/>
	Cells[] IGrid<SudokuGrid>.CandidatesMap => CandidateMap;

	/// <inheritdoc/>
	static short IGrid<SudokuGrid>.DefaultMask => DefaultMask;

	/// <inheritdoc/>
	static short IGrid<SudokuGrid>.MaxCandidatesMask => MaxCandidatesMask;

	/// <inheritdoc/>
	static short IGrid<SudokuGrid>.EmptyMask => EmptyMask;

	/// <inheritdoc/>
	static short IGrid<SudokuGrid>.ModifiableMask => ModifiableMask;

	/// <inheritdoc/>
	static short IGrid<SudokuGrid>.GivenMask => GivenMask;

	/// <inheritdoc/>
	static string IGrid<SudokuGrid>.EmptyString => EmptyString;

	/// <inheritdoc/>
	static void* IGrid<SudokuGrid>.ValueChanged => ValueChanged;

	/// <inheritdoc/>
	static void* IGrid<SudokuGrid>.RefreshingCandidates => RefreshingCandidates;

	/// <inheritdoc/>
	static SudokuGrid IGrid<SudokuGrid>.Undefined => Undefined;

	/// <inheritdoc/>
	static SudokuGrid IGrid<SudokuGrid>.Empty => Empty;


	/// <inheritdoc/>
	public int this[int cell]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get => GetStatus(cell) switch
		{
			CellStatus.Undefined => -2,
			CellStatus.Empty => -1,
			CellStatus.Modifiable or CellStatus.Given => TrailingZeroCount(_values[cell])
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			switch (value)
			{
				case -1 when GetStatus(cell) == CellStatus.Modifiable:
				{
					// If 'value' is -1, we should reset the grid.
					// Note that reset candidates may not trigger the event.
					_values[cell] = DefaultMask;

					RefreshingCandidates(ref this);

					break;
				}
				case >= 0 and < RegionCellsCount:
				{
					ref short result = ref _values[cell];
					short copy = result;

					// Set cell status to 'CellStatus.Modifiable'.
					result = (short)(ModifiableMask | 1 << value);

					// To trigger the event, which is used for eliminate
					// all same candidates in peer cells.
					ValueChanged(ref this, new(cell, copy, result, value));

					break;
				}
			}
		}
	}

	/// <inheritdoc/>
	public bool this[int cell, int digit]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get => (_values[cell] >> digit & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if ((Cell: cell, Digit: digit) is (Cell: >= 0 and < Length, Digit: >= 0 and < RegionCellsCount))
			{
				short copied = _values[cell];
				if (value)
				{
					_values[cell] |= (short)(1 << digit);
				}
				else
				{
					_values[cell] &= (short)~(1 << digit);
				}

				// To trigger the event.
				ValueChanged(ref this, new(cell, copied, _values[cell], -1));
			}
		}
	}


	/// <inheritdoc/>
	public readonly bool SimplyValidate()
	{
		for (int i = 0; i < Length; i++)
		{
			switch (GetStatus(i))
			{
				case CellStatus.Given:
				case CellStatus.Modifiable:
				{
					int curDigit = this[i];
					foreach (int cell in PeerMaps[i])
					{
						if (curDigit == this[cell])
						{
							return false;
						}
					}

					break;
				}
				case CellStatus.Empty:
				{
					continue;
				}
				default:
				{
					return false;
				}
			}
		}

		return true;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool? Exists(int candidate) => Exists(candidate / 9, candidate % 9);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool? Exists(int cell, int digit) =>
		GetStatus(cell) == CellStatus.Empty ? this[cell, digit] : null;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode() => this switch
	{
		{ IsUndefined: true } => 0,
#if DEBUG
		{ IsDebuggerUndefined: true } => 0,
#endif
		{ IsEmpty: true } => 1,
		_ => ToString("#").GetHashCode()
	};

	/// <inheritdoc/>
	public readonly int[] ToArray()
	{
		var span = (stackalloc int[Length]);
		for (int i = 0; i < Length; i++)
		{
			// 'this[i]' is always in range -1 to 8 (-1 is empty, and 0 to 8 is 1 to 9 for
			// mankind representation).
			span[i] = this[i] + 1;
		}

		return span.ToArray();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly short GetMask(int offset) => _values[offset];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly short GetCandidates(int cell) => (short)(_values[cell] & MaxCandidatesMask);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public readonly ref readonly short GetPinnableReference() => ref _values[0];

	/// <summary>
	/// Returns a reference to the element of the <see cref="SudokuGrid"/> at index zero.
	/// </summary>
	/// <param name="pinnedItem">
	/// The item you want to fix. If the value is
	/// <list type="table">
	/// <item>
	/// <term><see cref="PinnedItem.CurrentGrid"/></term>
	/// <description>The current grid mask list of pointer value will be returned.</description>
	/// </item>
	/// <item>
	/// <term><see cref="PinnedItem.InitialGrid"/></term>
	/// <description>The initial grid mask list of pointer value will be returned.</description>
	/// </item>
	/// <item>
	/// <term>Otherwise</term>
	/// <description>The reference of <see langword="null"/>.</description>
	/// </item>
	/// </list>
	/// </param>
	/// <returns>A reference to the element of the <see cref="SudokuGrid"/> at index zero.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ref readonly short GetPinnableReference(PinnedItem pinnedItem) =>
		ref pinnedItem == PinnedItem.CurrentGrid
			? ref GetPinnableReference()
			: ref pinnedItem == PinnedItem.InitialGrid
				? ref _initialValues[0]
				: ref *(short*)null;

	/// <inheritdoc/>
	public readonly string ToMaskString()
	{
		const string separator = ", ";
		fixed (short* pArr = _values)
		{
			var sb = new StringHandler(initialCapacity: 400);
			sb.AppendRangeWithSeparatorUnsafe(pArr, Length, static v => v.ToString(), separator);
			return sb.ToStringAndClear();
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format, IFormatProvider? formatProvider) => this switch
	{
		{ IsEmpty: true } => "<Empty>",
		{ IsUndefined: true } => "<Undefined>",
#if DEBUG
		{ IsDebuggerUndefined: true } => "<Debugger can't recognize the fixed buffer>",
#endif
		_ when formatProvider.HasFormatted(this, format, out string? result) => result,
		_ when Formatter.Create(format) is var f => format switch
		{
			":" => f.ToString(this).Match(RegularExpressions.ExtendedSusserEliminations) ?? string.Empty,
			"!" => f.ToString(this).Replace("+", string.Empty),
			".!" or "!." or "0!" or "!0" => f.ToString(this).Replace("+", string.Empty),
			".!:" or "!.:" or "0!:" => f.ToString(this).Replace("+", string.Empty),
			_ => f.ToString(this)
		}
	};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CellStatus GetStatus(int cell) => _values[cell].MaskToStatus();

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Enumerator GetEnumerator()
	{
		fixed (short* arr = _values)
		{
			return new Enumerator(arr);
		}
	}

	/// <inheritdoc/>
	public void Fix()
	{
		for (int i = 0; i < Length; i++)
		{
			if (GetStatus(i) == CellStatus.Modifiable)
			{
				SetStatus(i, CellStatus.Given);
			}
		}

		UpdateInitialMasks();
	}

	/// <inheritdoc/>
	public void Unfix()
	{
		for (int i = 0; i < Length; i++)
		{
			if (GetStatus(i) == CellStatus.Given)
			{
				SetStatus(i, CellStatus.Modifiable);
			}
		}
	}

	/// <summary>
	/// To reset the grid to initial status.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Reset()
	{
		fixed (short* pValues = _values, pInitialValues = _initialValues)
		{
			Unsafe.CopyBlock(pValues, pInitialValues, sizeof(short) * Length);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetStatus(int cell, CellStatus status)
	{
		ref short mask = ref _values[cell];
		short copy = mask;
		mask = (short)((int)status << RegionCellsCount | mask & MaxCandidatesMask);

		ValueChanged(ref this, new(cell, copy, mask, -1));
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMask(int cell, short mask)
	{
		ref short m = ref _values[cell];
		short copy = m;
		m = mask;

		ValueChanged(ref this, new(cell, copy, m, -1));
	}

	/// <summary>
	/// To update initial masks.
	/// </summary>
	internal void UpdateInitialMasks()
	{
		fixed (short* pValues = _values, pInitialValues = _initialValues)
		{
			Unsafe.CopyBlock(pInitialValues, pValues, sizeof(short) * Length);
		}
	}

	/// <summary>
	/// Called by properties <see cref="CandidateMap"/>, <see cref="DigitsMap"/>
	/// and <see cref="ValuesMap"/>.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The map of digits.</returns>
	/// <seealso cref="CandidateMap"/>
	/// <seealso cref="DigitsMap"/>
	/// <seealso cref="ValuesMap"/>
	private readonly Cells[] GetMap(delegate*<in SudokuGrid, int, int, bool> predicate)
	{
		var result = new Cells[RegionCellsCount];
		for (int digit = 0; digit < RegionCellsCount; digit++)
		{
			ref var map = ref result[digit];
			for (int cell = 0; cell < Length; cell++)
			{
				if (predicate(this, cell, digit))
				{
					map.AddAnyway(cell);
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Called by properties <see cref="EmptyCells"/> and <see cref="BivalueCells"/>.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The cells.</returns>
	/// <seealso cref="EmptyCells"/>
	/// <seealso cref="BivalueCells"/>
	private readonly Cells GetCells(delegate*<in SudokuGrid, int, bool> predicate)
	{
		var result = Cells.Empty;
		for (int cell = 0; cell < Length; cell++)
		{
			if (predicate(this, cell))
			{
				result.AddAnyway(cell);
			}
		}

		return result;
	}


	/// <inheritdoc/>
	[ProxyEquality]
	public static bool Equals(in SudokuGrid left, in SudokuGrid right)
	{
		fixed (short* pThis = left, pOther = right)
		{
			int i = 0;
			for (short* l = pThis, r = pOther; i < Length; i++, l++, r++)
			{
				if (*l != *r)
				{
					return false;
				}
			}

			return true;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SudokuGrid Parse(in ReadOnlySpan<char> str) => new Parser(str.ToString()).Parse();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SudokuGrid Parse([NotNull, DisallowNull] char* ptrStr) => Parse(new string(ptrStr));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SudokuGrid Parse(string? str)
	{
		Nullability.ThrowIfNull(str);

		return new Parser(str).Parse();
	}

#nullable disable
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SudokuGrid Parse(string str, bool compatibleFirst) =>
		new Parser(str, compatibleFirst).Parse();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SudokuGrid Parse(string str, GridParsingOption gridParsingOption) =>
		new Parser(str).Parse(gridParsingOption);
#nullable restore

	/// <inheritdoc/>
	public static bool TryParse([NotNullWhen(true)] string? str, [DiscardWhen(false)] out SudokuGrid result)
	{
		try
		{
			result = Parse(str);
			return !result.IsUndefined;
		}
		catch (Exception ex) when (ex is ArgumentNullException or FormatException)
		{
			result = Undefined;
			return false;
		}
	}

#nullable disable
	/// <inheritdoc/>
	public static bool TryParse(string str, GridParsingOption option, [DiscardWhen(false)] out SudokuGrid result)
	{
		try
		{
			result = Parse(str, option);
			return true;
		}
		catch (FormatException)
		{
			result = Undefined;
			return false;
		}
	}
#nullable restore


	/// <summary>
	/// Returns the segment via the specified region and the sudoku grid to filter.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="region">The region.</param>
	/// <returns>The segment.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SudokuGridSegment operator /(in SudokuGrid grid, int region) => new(grid, region);
}
