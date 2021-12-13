namespace Sudoku.Data;

/// <summary>
/// Represents a sudoku grid that uses the mask list to construct the data structure.
/// </summary>
/// <remarks>
/// The data structure uses the mask table of length 81 to indicate the status and all possible candidates
/// holding for each cell. Each mask uses a <see cref="short"/> value, but only uses 11 of 16 bits.
/// <code>
/// |xxx|--|--------|
/// |-------|-------|
/// 16      8       0
/// </code>
/// Here the first-nine bits indicate whether the digit 1-9 is possible candidate in the current cell respectively,
/// and the higher 3 bits indicate the cell status. The possible cell status are:
/// <list type="table">
/// <listheader>
/// <term>Status name (Value corresponding to <see cref="CellStatus"/> enumeration)</term>
/// <description>Description</description>
/// </listheader>
/// <item>
/// <term>Empty cell (i.e. <see cref="CellStatus.Empty"/>)</term>
/// <description>The cell is currently empty, and wait for being filled.</description>
/// </item>
/// <item>
/// <term>Modifiable cell (i.e. <see cref="CellStatus.Modifiable"/>)</term>
/// <description>The cell is filled by a digit, but the digit isn't the given by the initial grid.</description>
/// </item>
/// <item>
/// <term>Given cell (i.e. <see cref="CellStatus.Given"/>)</term>
/// <description>The cell is filled by a digit, which is given by the initial grid and can't be modified.</description>
/// </item>
/// </list>
/// </remarks>
/// <seealso cref="CellStatus"/>
#if DEBUG
#if USE_TO_MASK_STRING_METHOD
[DebuggerDisplay($@"{{{nameof(ToMaskString)}("".+:""),nq}}")]
#else
[DebuggerDisplay($@"{{{nameof(ToString)}("".+:""),nq}}")]	
#endif // !USE_TO_MASK_STRING_METHOD
#endif // !DEBUG
[AutoDeconstruct(nameof(EmptyCells), nameof(BivalueCells), nameof(CandidatesMap), nameof(DigitsMap), nameof(ValuesMap))]
[AutoGetEnumerator(nameof(EnumerateCandidates), MemberConversion = "@()", ReturnType = typeof(CandidateCollectionEnumerator))]
public unsafe partial struct Grid
: IGrid<Grid>
, ISimpleFormattable
, IJsonSerializable<Grid, Grid.JsonConverter>
, ISimpleParseable<Grid>
, IValueEquatable<Grid>
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
	public static readonly void* ValueChanged;

	/// <inheritdoc cref="IGrid{TGrid}.RefreshingCandidates"/>
	public static readonly void* RefreshingCandidates;

	/// <inheritdoc cref="IGrid{TGrid}.Undefined"/>
	public static readonly Grid Undefined;

	/// <inheritdoc cref="IGrid{TGrid}.Empty"/>
	public static readonly Grid Empty;

	/// <summary>
	/// The lookup table.
	/// </summary>
	private static readonly IReadOnlyDictionary<char, int> Lookup;


	/// <summary>
	/// Indicates the inner array that stores the masks of the sudoku grid, which
	/// stores the in-time sudoku grid inner information.
	/// </summary>
	private fixed short _values[Length];


	/// <summary>
	/// Creates an instance using grid values.
	/// </summary>
	/// <param name="gridValues">The array of grid values.</param>
	public Grid(int[] gridValues) : this(gridValues, GridCreatingOption.None)
	{
	}

	/// <summary>
	/// Creates an instance using grid values.
	/// </summary>
	/// <param name="gridValues">The array of grid values.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	public Grid(int[] gridValues, GridCreatingOption creatingOption)
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
	public Grid(string token)
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
	/// <remarks>
	/// In order to decrease the memory allocation, you can use the system buffer,
	/// whose corresponding code will be implemented like this:
	/// <code><![CDATA[
	/// // Rents the buffer memory.
	/// short[] buffer = ArrayPool<short>.Shared.Rent(81);
	/// 
	/// // Initialize the memory in order to be used later.
	/// fixed (short* pBuffer = buffer, pGrid = this)
	/// {
	///     Unsafe.CopyBlock(pBuffer, pGrid, sizeof(short) * 81);
	/// }
	///
	/// // Gets the result sudoku grid instance.
	/// try
	/// {
	///     var targetGrid = new Grid(buffer); // Now the result grid is created here.
	///
	///     // Do something to use 'targetGrid'.
	/// }
	/// finally
	/// {
	///     // Returns the buffer memory to system.
	///     ArrayPool<short>.Shared.Return(buffer, false);
	/// }
	/// ]]></code>
	/// In this way we can get the sudoku grid without any allocations.
	/// </remarks>
	/// <exception cref="ArgumentException">Throws when <see cref="Array.Length"/> is not 81.</exception>
	public Grid(short[] masks)
	{
		if (masks.Length != Length)
		{
			throw new ArgumentException($"The length of the array argument should be {Length}.", nameof(masks));
		}

		fixed (short* pArray = masks, pValues = _values)
		{
			Unsafe.CopyBlock(pValues, pArray, sizeof(short) * Length);
		}
	}


	/// <summary>
	/// Indicates the <see langword="static"/> constructor of type <see cref="Grid"/>. This construtcor will
	/// initialize some <see langword="static readonly"/> data members of this type that can't use
	/// a simple expression to describe the initial value.
	/// </summary>
	static Grid()
	{
		// Initializes the empty grid.
		Empty = default;
		fixed (short* p = Empty._values)
		{
			int i = 0;
			for (short* ptrP = p; i < Length; *ptrP++ = DefaultMask, i++) ;
		}

		// Lookup table.
		Lookup = new Dictionary<char, int>(
			from i in Enumerable.Range(0, Base64Length)
			select new KeyValuePair<char, int>(Base64List[i], i)
		);

		// Initializes events.
		ValueChanged = (delegate*<ref Grid, int, short, short, int, void>)&onValueChanged;
		RefreshingCandidates = (delegate*<ref Grid, void>)&onRefreshingCandidates;


		static void onValueChanged(ref Grid @this, int cell, short oldMask, short newMask, int setValue)
		{
			if (setValue != -1)
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

		static void onRefreshingCandidates(ref Grid @this)
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
	public readonly int GivensCount
	{
		get
		{
			int result = 0;
			for (int i = 0; i < Length; i++)
			{
				if (GetStatus(i) == CellStatus.Given)
				{
					result++;
				}
			}

			return result;
		}
	}

	/// <inheritdoc/>
	public readonly int ModifiablesCount
	{
		get
		{
			int result = 0;
			for (int i = 0; i < Length; i++)
			{
				if (GetStatus(i) == CellStatus.Modifiable)
				{
					result++;
				}
			}

			return result;
		}
	}

	/// <inheritdoc/>
	public readonly int EmptiesCount
	{
		get
		{
			int result = 0;
			for (int i = 0; i < Length; i++)
			{
				if (GetStatus(i) == CellStatus.Empty)
				{
					result++;
				}
			}

			return result;
		}
	}

	/// <summary>
	/// <para>Indicates which regions are null regions.</para>
	/// <para>A <b>Null Region</b> is a region whose hold cells are all empty cells.</para>
	/// <para>
	/// The property returns an <see cref="int"/> value as a mask that contains all possible regions.
	/// For example, if the row 5, column 5 and block 5 (1-9) are null regions, the property will return
	/// the result <see cref="int"/> value, <c>000010000_000010000_000010000</c> as binary.
	/// </para>
	/// </summary>
	public readonly int NullRegions
	{
		get
		{
			int maskResult = 0;
			for (int region = 0; region < 27; region++)
			{
				if ((EmptyCells & RegionMaps[region]).Count == 9)
				{
					maskResult |= 1 << region;
				}
			}

			return maskResult;
		}
	}

	/// <inheritdoc/>
	public readonly string Token
	{
		get
		{
			// The maximum grid as the base 64 is of length 45.
			var sb = new StringHandler(initialCapacity: 45);
			for (var temp = BigInteger.Parse(EigenString); temp > 0; temp /= Base64Length)
			{
				sb.Append(Base64List[(int)(temp % Base64Length)]);
			}

			return sb.ToStringAndClear();
		}
	}

	/// <inheritdoc/>
	public readonly string EigenString
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ToString("0").TrimStart('0');
	}

	/// <summary>
	/// Gets the pattern of the current sudoku grid.
	/// </summary>
	/// <remarks>
	/// A <b>pattern</b> is a template that holds the values (i.e. givens and modifiables).
	/// This property is equivalent to the expression <c>~EmptyCells</c>,
	/// but useful in some cases you want to create a puzzle with the specified pattern.
	/// </remarks>
	public readonly Cells Pattern
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ~EmptyCells;
	}

	/// <inheritdoc/>
	public readonly Cells EmptyCells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetCells(&p);


			static bool p(in Grid g, int cell) => g.GetStatus(cell) == CellStatus.Empty;
		}
	}

	/// <inheritdoc/>
	public readonly Cells BivalueCells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetCells(&p);


			static bool p(in Grid g, int cell) => PopCount((uint)g.GetCandidates(cell)) == 2;
		}
	}

	/// <inheritdoc/>
	public readonly Cells[] CandidatesMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetMap(&p);


			static bool p(in Grid g, int cell, int digit) => g.Exists(cell, digit) is true;
		}
	}

	/// <inheritdoc/>
	public readonly Cells[] DigitsMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetMap(&p);


			static bool p(in Grid g, int cell, int digit) => (g.GetCandidates(cell) >> digit & 1) != 0;
		}
	}

	/// <inheritdoc/>
	public readonly Cells[] ValuesMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetMap(&p);


			static bool p(in Grid g, int cell, int digit) => g[cell] == digit;
		}
	}

	/// <summary>
	/// Gets the grid where all modifiable cells are empty cells (i.e. the initial one).
	/// </summary>
	public readonly Grid ResetGrid
	{
		get
		{
			var result = this; // Copy an instance.
			result.Reset();

			return result;
		}
	}

	/// <inheritdoc/>
	static short IGrid<Grid>.DefaultMask => DefaultMask;

	/// <inheritdoc/>
	static short IGrid<Grid>.MaxCandidatesMask => MaxCandidatesMask;

	/// <inheritdoc/>
	static short IGrid<Grid>.EmptyMask => EmptyMask;

	/// <inheritdoc/>
	static short IGrid<Grid>.ModifiableMask => ModifiableMask;

	/// <inheritdoc/>
	static short IGrid<Grid>.GivenMask => GivenMask;

	/// <inheritdoc/>
	static string IGrid<Grid>.EmptyString => EmptyString;

	/// <inheritdoc/>
	static void* IGrid<Grid>.ValueChanged => ValueChanged;

	/// <inheritdoc/>
	static void* IGrid<Grid>.RefreshingCandidates => RefreshingCandidates;

	/// <inheritdoc/>
	static Grid IGrid<Grid>.Undefined => Undefined;

	/// <inheritdoc/>
	static Grid IGrid<Grid>.Empty => Empty;


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

					((delegate*<ref Grid, void>)RefreshingCandidates)(ref this);

					break;
				}
				case >= 0 and < RegionCellsCount:
				{
					ref short result = ref _values[cell];
					short copied = result;

					// Set cell status to 'CellStatus.Modifiable'.
					result = (short)(ModifiableMask | 1 << value);

					// To trigger the event, which is used for eliminate all same candidates in peer cells.
					var f = (delegate*<ref Grid, int, short, short, int, void>)ValueChanged;
					f(ref this, cell, copied, result, value);

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
			if (cell is >= 0 and < Length && digit is >= 0 and < RegionCellsCount)
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
				var f = (delegate*<ref Grid, int, short, short, int, void>)ValueChanged;
				f(ref this, cell, copied, _values[cell], -1);
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

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode() => this switch
	{
		{ IsUndefined: true } => 0,
#if DEBUG
		{ IsDebuggerUndefined: true } => 0,
#endif
		{ IsEmpty: true } => 1,
		_ => $"{this:#}".GetHashCode()
	};

	/// <inheritdoc/>
	public readonly int[] ToArray()
	{
		int[] result = new int[Length];
		for (int i = 0; i < Length; i++)
		{
			// 'this[i]' is always between -1 and 8 (-1 is empty, and 0 to 8 is 1 to 9 for human representation).
			result[i] = this[i] + 1;
		}

		return result;
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

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly override string ToString() => ToString(null);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format) =>
		this switch
		{
			{ IsEmpty: true } => "<Empty>",
			{ IsUndefined: true } => "<Undefined>",
#if DEBUG
			{ IsDebuggerUndefined: true } => "<Debugger can't recognize the fixed buffer>",
#endif
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

	/// <summary>
	/// Try to enumerate all possible candidates in the current grid.
	/// </summary>
	/// <returns>
	/// An enumerator that allows us using <see langword="foreach"/> statement
	/// to iterate all possible candidates in the current grid.
	/// </returns>
	public readonly CandidateCollectionEnumerator EnumerateCandidates()
	{
		fixed (short* arr = _values)
		{
			return new(arr);
		}
	}

	/// <summary>
	/// Try to enumerate the mask table of the current grid.
	/// </summary>
	/// <returns>
	/// An enumerator that allows us using <see langword="foreach"/> statement
	/// to iterate all masks in the current grid. The mask list must contain 81 masks.
	/// </returns>
	/// <remarks>
	/// Please note that the iterator will iterate all masks by reference, which means
	/// you can use <see langword="ref"/> and <see langword="ref readonly"/> modifier
	/// onto the iteration variable:
	/// <code>
	/// foreach (ref readonly short mask in grid)
	/// {
	///     // Do something.
	/// }
	/// </code>
	/// </remarks>
	public readonly MaskCollectionEnumerator EnumerateMasks()
	{
		fixed (short* arr = _values)
		{
			return new(arr);
		}
	}

	/// <summary>
	/// Convertes the current instance to a <see cref="SudokuGrid"/>.
	/// </summary>
	/// <returns>The sudoku grid result.</returns>
	[Obsolete($"The method is deprecated due to the usage of type '{nameof(SudokuGrid)}'.", false)]
	public readonly SudokuGrid ToSudokuGrid()
	{
		short[] arr = ArrayPool<short>.Shared.Rent(Length);
		fixed (short* pArr = arr, pGrid = this)
		{
			Unsafe.CopyBlock(pArr, pGrid, sizeof(short) * Length);
		}

		try
		{
			return new(arr);
		}
		finally
		{
			ArrayPool<short>.Shared.Return(arr);
		}
	}

	/// <summary>
	/// Reset the sudoku grid, to set all modifiable values to empty ones.
	/// </summary>
	public void Reset()
	{
		for (int i = 0; i < Length; i++)
		{
			if (GetStatus(i) == CellStatus.Modifiable)
			{
				this[i] = -1; // Reset the cell, and then re-compute all candidates.
			}
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

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetStatus(int cell, CellStatus status)
	{
		ref short mask = ref _values[cell];
		short copied = mask;
		mask = (short)((int)status << RegionCellsCount | mask & MaxCandidatesMask);

		var f = (delegate*<ref Grid, int, short, short, int, void>)ValueChanged;
		f(ref this, cell, copied, mask, -1);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMask(int cell, short mask)
	{
		ref short m = ref _values[cell];
		short copied = m;
		m = mask;

		var f = (delegate*<ref Grid, int, short, short, int, void>)ValueChanged;
		f(ref this, cell, copied, m, -1);
	}

	/// <summary>
	/// Called by properties <see cref="CandidatesMap"/>, <see cref="DigitsMap"/> and <see cref="ValuesMap"/>.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The map of digits.</returns>
	/// <seealso cref="CandidatesMap"/>
	/// <seealso cref="DigitsMap"/>
	/// <seealso cref="ValuesMap"/>
	private readonly Cells[] GetMap(delegate*<in Grid, int, int, bool> predicate)
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
	private readonly Cells GetCells(delegate*<in Grid, int, bool> predicate)
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
	public static bool Equals(in Grid left, in Grid right)
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
	public static Grid Parse(in ReadOnlySpan<char> str) => new Parser(str.ToString()).Parse();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(char* ptrStr) => Parse(new string(ptrStr));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string? str)
	{
		Nullability.ThrowIfNull(str);

		return new Parser(str).Parse();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string? str, bool compatibleFirst)
	{
		Nullability.ThrowIfNull(str);

		return new Parser(str, compatibleFirst).Parse();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string? str, GridParsingOption gridParsingOption)
	{
		Nullability.ThrowIfNull(str);

		return new Parser(str).Parse(gridParsingOption);
	}

	/// <inheritdoc/>
	public static bool TryParse([NotNullWhen(true)] string? str, out Grid result)
	{
		try
		{
			result = Parse(str);
			return !result.IsUndefined;
		}
		catch (Exception ex) when (ex is FormatException or ArgumentNullException)
		{
			result = Undefined;
			return false;
		}
	}

	/// <inheritdoc/>
	public static bool TryParse([NotNullWhen(true)] string? str, GridParsingOption option, out Grid result)
	{
		try
		{
			result = Parse(str, option);
			return true;
		}
		catch (Exception ex) when (ex is FormatException or ArgumentNullException)
		{
			result = Undefined;
			return false;
		}
	}
}
