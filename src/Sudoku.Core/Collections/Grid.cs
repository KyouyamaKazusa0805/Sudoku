using System.ComponentModel;
using Sudoku.Solving;
using static System.Numerics.BitOperations;
using static Sudoku.Constants;
using static Sudoku.Constants.Tables;

namespace Sudoku.Collections;

/// <summary>
/// Represents a sudoku grid that uses the mask list to construct the data structure.
/// </summary>
#if DEBUG
#if USE_TO_MASK_STRING_METHOD
[DebuggerDisplay($$"""{{{nameof(ToMaskString)}}(".+:"),nq}""")]
#else
[DebuggerDisplay($$"""{{{nameof(ToString)}}(".+:"),nq}""")]
#endif // !USE_TO_MASK_STRING_METHOD
#endif // !DEBUG
public unsafe partial struct Grid :
	IDefaultable<Grid>,
	ISimpleFormattable,
	ISimpleParseable<Grid>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<Grid, Grid>
#if FEATURE_GENERIC_MATH_IN_ARG
	,
	IValueBitwiseAndOperators<Grid, Cells, GridSegment>,
	IValueEqualityOperators<Grid, Grid>
#endif
#endif
{
	/// <summary>
	/// Indicates the default mask of a cell (an empty cell, with all 9 candidates left).
	/// </summary>
	public const short DefaultMask = EmptyMask | MaxCandidatesMask;

	/// <summary>
	/// Indicates the maximum candidate mask that used.
	/// </summary>
	public const short MaxCandidatesMask = (1 << 9) - 1;

	/// <summary>
	/// Indicates the empty mask, modifiable mask and given mask.
	/// </summary>
	public const short EmptyMask = (int)CellStatus.Empty << 9;

	/// <summary>
	/// Indicates the modifiable mask.
	/// </summary>
	public const short ModifiableMask = (int)CellStatus.Modifiable << 9;

	/// <summary>
	/// Indicates the given mask.
	/// </summary>
	public const short GivenMask = (int)CellStatus.Given << 9;


	/// <summary>
	/// Indicates the empty grid string.
	/// </summary>
	public static readonly string EmptyString = new('0', 81);

	/// <summary>
	/// Indicates the event triggered when the value is changed.
	/// </summary>
	[NotNull]
	public static readonly void* ValueChanged;

	/// <summary>
	/// Indicates the event triggered when should re-compute candidates.
	/// </summary>
	[NotNull]
	public static readonly void* RefreshingCandidates;

	/// <summary>
	/// Indicates the default grid that all values are initialized 0.
	/// </summary>
	public static readonly Grid Undefined;

	/// <summary>
	/// The empty grid that is valid during implementation or running the program
	/// (all values are <see cref="DefaultMask"/>, i.e. empty cells), which is same initialization result
	/// as the constructor <see cref="Grid()"/>.
	/// </summary>
	/// <remarks>
	/// This field is initialized by the static constructor of this structure.
	/// </remarks>
	/// <seealso cref="DefaultMask"/>
	/// <seealso cref="Grid()"/>
	public static readonly Grid Empty;

	/// <summary>
	/// Indicates the solver that uses the bitwise algorithm to solve a puzzle as fast as possible.
	/// </summary>
	private static readonly BitwiseSolver Solver = new();


	/// <summary>
	/// Indicates the inner array that stores the masks of the sudoku grid, which
	/// stores the in-time sudoku grid inner information.
	/// </summary>
	/// <remarks>
	/// The field uses the mask table of length 81 to indicate the status and all possible candidates
	/// holding for each cell. Each mask uses a <see cref="short"/> value, but only uses 11 of 16 bits.
	/// <code>
	/// 16       8       0
	///  |-------|-------|
	///  |   |--|--------|
	/// 16  12  9        0
	/// </code>
	/// Here the first-nine bits indicate whether the digit 1-9 is possible candidate in the current cell respectively,
	/// and the higher 3 bits indicate the cell status. The possible cell status are:
	/// <list type="table">
	/// <listheader>
	/// <term>Status name</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>Empty cell (i.e. <see cref="CellStatus.Empty"/>)</term>
	/// <description>The cell is currently empty, and wait for being filled.</description>
	/// </item>
	/// <item>
	/// <term>Modifiable cell (i.e. <see cref="CellStatus.Modifiable"/>)</term>
	/// <description>
	/// The cell is filled by a digit, but the digit isn't the given by the initial grid.
	/// </description>
	/// </item>
	/// <item>
	/// <term>Given cell (i.e. <see cref="CellStatus.Given"/>)</term>
	/// <description>
	/// The cell is filled by a digit, which is given by the initial grid and can't be modified.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <seealso cref="CellStatus"/>
	private fixed short _values[81];


	/// <summary>
	/// Initializes a <see cref="Grid"/> instance using the <see cref="Empty"/> instance to initialize,
	/// which is equivalent to assignment <c>this = Empty;</c>.
	/// </summary>
	/// <remarks>
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp9/feature[@name="parameterless-struct-constructor"]/target[@name="constructor"]' />
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Grid() => this = Empty;

	/// <summary>
	/// Creates an instance using grid values.
	/// </summary>
	/// <param name="gridValues">The array of grid values.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	public Grid(int[] gridValues, GridCreatingOption creatingOption = GridCreatingOption.None) :
		this(gridValues[0], creatingOption)
	{
	}

	/// <summary>
	/// Initializes a <see cref="Grid"/> instance via the array of cell digits of type <see cref="int"/>*.
	/// </summary>
	/// <param name="pGridValues">The pointer parameter indicating the array of cell digits.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="pGridValues"/> is <see langword="null"/>.
	/// </exception>
	public Grid(int* pGridValues!!, GridCreatingOption creatingOption = GridCreatingOption.None)
		: this(*pGridValues, creatingOption)
	{
	}

	/// <summary>
	/// Initializes a <see cref="Grid"/> instance via the array of cell digits
	/// of type <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <param name="gridValues">The list of cell digits.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	public Grid(ReadOnlySpan<int> gridValues, GridCreatingOption creatingOption = GridCreatingOption.None) :
		this(gridValues[0], creatingOption)
	{
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
		if (masks.Length != 81)
		{
			throw new ArgumentException("The length of the array argument should be 81.", nameof(masks));
		}

		fixed (short* pArray = masks, pValues = _values)
		{
			Unsafe.CopyBlock(pValues, pArray, sizeof(short) * 81);
		}
	}

	/// <summary>
	/// Creates a <see cref="Grid"/> instance via the pointer of the first element of the cell digit,
	/// and the creating option.
	/// </summary>
	/// <param name="firstElement">
	/// <para>The reference of the first element.</para>
	/// <para>
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp7/feature[@name="ref-returns"]/target[@name="in-parameter"]'/>
	/// </para>
	/// </param>
	/// <param name="creatingOption">The creating option.</param>
	/// <remarks>
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp7/feature[@name="ref-returns"]/target[@name="method"]'/>
	/// </remarks>
	private Grid(in int firstElement, GridCreatingOption creatingOption = GridCreatingOption.None)
	{
		fixed (int* p = &firstElement)
		{
			for (int i = 0; i < 81; i++)
			{
				if (p[i] is var value and not 0)
				{
					// Calls the indexer to trigger the event
					// (Clear the candidates in peer cells).
					this[i] = creatingOption == GridCreatingOption.MinusOne ? value - 1 : value;

					// Set the status to 'CellStatus.Given'.
					SetStatus(i, CellStatus.Given);
				}
			}
		}
	}


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static Grid()
	{
		// Initializes the empty grid.
		Empty = default;
		fixed (short* p = Empty._values)
		{
			int i = 0;
			for (short* ptrP = p; i < 81; i++)
			{
				*ptrP++ = DefaultMask;
			}
		}

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
			for (int i = 0; i < 81; i++)
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


	/// <summary>
	/// Indicates the grid has already solved. If the value is <see langword="true"/>,
	/// the grid is solved; otherwise, <see langword="false"/>.
	/// </summary>
	public readonly bool IsSolved
	{
		get
		{
			for (int i = 0; i < 81; i++)
			{
				if (GetStatus(i) == CellStatus.Empty)
				{
					return false;
				}
			}

			return SimplyValidate();
		}
	}

	/// <summary>
	/// Indicates whether the puzzle is valid, which means the puzzle contains a unique solution.
	/// </summary>
	public readonly bool IsValid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Solver.CheckValidity(ToString("0"));
	}

	/// <summary>
	/// Indicates whether the grid is <see cref="Undefined"/>, which means the grid
	/// holds totally same value with <see cref="Undefined"/>.
	/// </summary>
	/// <seealso cref="Undefined"/>
	public readonly bool IsUndefined
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this == Undefined;
	}

	/// <summary>
	/// Indicates whether the grid is <see cref="Empty"/>, which means the grid
	/// holds totally same value with <see cref="Empty"/>.
	/// </summary>
	/// <seealso cref="Empty"/>
	public readonly bool IsEmpty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this == Empty;
	}

#if DEBUG
	/// <summary>
	/// Indicates whether the grid is as same behaviors as <see cref="Undefined"/>
	/// in debugging mode.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This property checks whether all non-first masks are all 0. This checking behavior
	/// is aiming to the debugger because the debugger can't recognize the fixed buffer.
	/// </para>
	/// <para>
	/// The debugger can't recognize fixed buffer.
	/// The fixed buffer whose code is like:
	/// <code><![CDATA[
	/// private fixed short _values[81];
	/// ]]></code>
	/// However, internally, the field <c>_values</c> is implemented
	/// with a fixed buffer using a inner struct, which is just like:
	/// <code><![CDATA[
	/// [StructLayout(LayoutKind.Explicit, Size = 81 * sizeof(short))]
	/// private struct FixedBuffer
	/// {
	///     public short _internalValue;
	/// }
	/// ]]></code>
	/// And that field:
	/// <code><![CDATA[
	/// private FixedBuffer _fixedField;
	/// ]]></code>
	/// From the code we can learn that only 2 bytes of the inner struct can be detected,
	/// because the buffer struct only contains 2 bytes data.
	/// </para>
	/// </remarks>
	/// <see cref="Undefined"/>
	public readonly bool IsDebuggerUndefined
	{
		get
		{
			fixed (short* pGrid = _values)
			{
				int i = 1;
				short* p = pGrid + 1;
				while (i < 81)
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

	/// <summary>
	/// Indicates the number of total candidates.
	/// </summary>
	public readonly int CandidatesCount
	{
		get
		{
			int count = 0;
			for (int i = 0; i < 81; i++)
			{
				if (GetStatus(i) == CellStatus.Empty)
				{
					count += PopCount((uint)GetCandidates(i));
				}
			}

			return count;
		}
	}

	/// <summary>
	/// Indicates the total number of given cells.
	/// </summary>
	public readonly int GivensCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GivenCells.Count;
	}

	/// <summary>
	/// Indicates the total number of modifiable cells.
	/// </summary>
	public readonly int ModifiablesCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ModifiableCells.Count;
	}

	/// <summary>
	/// Indicates the total number of empty cells.
	/// </summary>
	public readonly int EmptiesCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => EmptyCells.Count;
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

	/// <summary>
	/// Gets the cell template that only contains the given cells.
	/// </summary>
	public readonly Cells GivenCells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetCells(&p);


			static bool p(in Grid g, int cell) => g.GetStatus(cell) == CellStatus.Given;
		}
	}

	/// <summary>
	/// Gets the cell template that only contains the modifiable cells.
	/// </summary>
	public readonly Cells ModifiableCells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetCells(&p);


			static bool p(in Grid g, int cell) => g.GetStatus(cell) == CellStatus.Modifiable;
		}
	}

	/// <summary>
	/// Indicates the cells that corresponding position in this grid is empty.
	/// </summary>
	public readonly Cells EmptyCells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetCells(&p);


			static bool p(in Grid g, int cell) => g.GetStatus(cell) == CellStatus.Empty;
		}
	}

	/// <summary>
	/// Indicates the cells that corresponding position in this grid contain two candidates.
	/// </summary>
	public readonly Cells BivalueCells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetCells(&p);


			static bool p(in Grid g, int cell) => PopCount((uint)g.GetCandidates(cell)) == 2;
		}
	}

	/// <summary>
	/// Indicates the map of possible positions of the existence of the candidate value for each digit.
	/// The return value will be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </summary>
	public readonly Cells[] CandidatesMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetMap(&p);


			static bool p(in Grid g, int cell, int digit) => g.Exists(cell, digit) is true;
		}
	}

	/// <summary>
	/// <para>
	/// Indicates the map of possible positions of the existence of each digit. The return value will
	/// be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </para>
	/// <para>
	/// Different with <see cref="CandidatesMap"/>, this property contains all givens, modifiables and
	/// empty cells only if it contains the digit in the mask.
	/// </para>
	/// </summary>
	/// <seealso cref="CandidatesMap"/>
	public readonly Cells[] DigitsMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetMap(&p);


			static bool p(in Grid g, int cell, int digit) => (g.GetCandidates(cell) >> digit & 1) != 0;
		}
	}

	/// <summary>
	/// <para>
	/// Indicates the map of possible positions of the existence of that value of each digit.
	/// The return value will be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </para>
	/// <para>
	/// Different with <see cref="CandidatesMap"/>, the value only contains the given or modifiable
	/// cells whose mask contain the set bit of that digit.
	/// </para>
	/// </summary>
	/// <seealso cref="CandidatesMap"/>
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var arr = (stackalloc int[81]);
			arr.Fill(-1);

			foreach (int cell in GivenCells)
			{
				arr[cell] = this[cell];
			}

			return new(arr);
		}
	}

	/// <summary>
	/// Indicates the solution of the grid. If failed to solve (for example,
	/// the puzzle contains multiple solutions), the property will return <see cref="Undefined"/>.
	/// </summary>
	/// <see cref="Undefined"/>
	public readonly Grid Solution
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Solver.Solve(this);
	}

	/// <summary>
	/// Gets the text code for the current sudoku grid, that can be used for parsing or formatting
	/// a sudoku grid, interacting with type <see cref="string"/>.
	/// </summary>
	internal readonly string TextCode
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ToString("#");
	}

	/// <inheritdoc/>
	bool IDefaultable<Grid>.IsDefault => IsUndefined;

	/// <inheritdoc/>
	static Grid IDefaultable<Grid>.Default => Undefined;


	/// <summary>
	/// Gets or sets the value in the specified cell.
	/// </summary>
	/// <param name="cell">The cell you want to get or set a value.</param>
	/// <value>
	/// The value you want to set. The value should be between 0 and 8. If assigning -1,
	/// that means to re-compute all candidates.
	/// </value>
	/// <returns>
	/// The value that the cell filled with. The possible values are:
	/// <list type="table">
	/// <item>
	/// <term>-2</term>
	/// <description>The status of the specified cell is <see cref="CellStatus.Undefined"/>.</description>
	/// </item>
	/// <item>
	/// <term>-1</term>
	/// <description>The status of the specified cell is <see cref="CellStatus.Empty"/>.</description>
	/// </item>
	/// <item>
	/// <term>0 to 8</term>
	/// <description>
	/// The actual value that the cell filled with. 0 is for the digit 1, 1 is for the digit 2, etc..
	/// </description>
	/// </item>
	/// </list>
	/// </returns>
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
				case >= 0 and < 9:
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

	/// <summary>
	/// Gets or sets a candidate existence case with a <see cref="bool"/> value.
	/// </summary>
	/// <param name="cell">The cell offset between 0 and 80.</param>
	/// <param name="digit">The digit between 0 and 8.</param>
	/// <value>
	/// The case you want to set. <see langword="false"/> means that this candidate
	/// doesn't exist in this current sudoku grid; otherwise, <see langword="true"/>.
	/// </value>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	public bool this[int cell, int digit]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get => (_values[cell] >> digit & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (cell is >= 0 and < 81 && digit is >= 0 and < 9)
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


	/// <inheritdoc cref="object.Equals(object?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is Grid comparer && Equals(comparer);

	/// <summary>
	/// Determine whether the specified <see cref="Grid"/> instance hold the same values
	/// as the current instance.
	/// </summary>
	/// <param name="other">The instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(in Grid other) => Equals(this, other);

	/// <summary>
	/// Check whether the current grid is valid (no duplicate values on same row, column or block).
	/// </summary>
	/// <returns>The <see cref="bool"/> result.</returns>
	public readonly bool SimplyValidate()
	{
		for (int i = 0; i < 81; i++)
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

	/// <summary>
	/// Indicates whether the current grid contains the specified candidate offset.
	/// </summary>
	/// <param name="candidate">The candidate offset.</param>
	/// <returns>
	/// The method will return a <see cref="bool"/>? value (contains three possible cases:
	/// <see langword="true"/>, <see langword="false"/> and <see langword="null"/>).
	/// All values corresponding to the cases are below:
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>
	/// The cell that the candidate specified is an empty cell <b>and</b> contains the specified digit
	/// that the candidate specified.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>
	/// The cell that the candidate specified is an empty cell <b>but doesn't</b> contain the specified digit
	/// that the candidate specified.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>
	/// The cell that the candidate specified is <b>not</b> an empty cell that the candidate specified.
	/// </description>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// Note that the method will return a <see cref="bool"/>?, so you should use the code
	/// '<c>grid.Exists(candidate) is true</c>' or '<c>grid.Exists(candidate) == true</c>'
	/// to decide whether a condition is true.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool? Exists(int candidate) => Exists(candidate / 9, candidate % 9);

	/// <summary>
	/// Indicates whether the current grid contains the digit in the specified cell.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>
	/// The method will return a <see cref="bool"/>? value (contains three possible cases:
	/// <see langword="true"/>, <see langword="false"/> and <see langword="null"/>).
	/// All values corresponding to the cases are below:
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>
	/// The cell is an empty cell <b>and</b> contains the specified digit.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>
	/// The cell is an empty cell <b>but doesn't</b> contain the specified digit.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The cell is <b>not</b> an empty cell.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// <para>
	/// Note that the method will return a <see cref="bool"/>?, so you should use the code
	/// '<c>grid.Exists(cell, digit) is true</c>' or '<c>grid.Exists(cell, digit) == true</c>'
	/// to decide whether a condition is true.
	/// </para>
	/// <para>
	/// In addition, because the type is <see cref="bool"/>? rather than <see cref="bool"/>,
	/// the result case will be more precisely than the indexer <see cref="this[int, int]"/>,
	/// which is the main difference between this method and that indexer.
	/// </para>
	/// </remarks>
	/// <seealso cref="this[int, int]"/>
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


	/// <summary>
	/// Serializes this instance to an array, where all digit value will be stored.
	/// </summary>
	/// <returns>
	/// This array. All elements are between 0 to 9, where 0 means the
	/// cell is <see cref="CellStatus.Empty"/> now.
	/// </returns>
	public readonly int[] ToArray()
	{
		int[] result = new int[81];
		for (int i = 0; i < 81; i++)
		{
			// 'this[i]' is always between -1 and 8 (-1 is empty, and 0 to 8 is 1 to 9 for human representation).
			result[i] = this[i] + 1;
		}

		return result;
	}

	/// <summary>
	/// Get a mask at the specified cell.
	/// </summary>
	/// <param name="offset">The cell offset you want to get.</param>
	/// <returns>The mask.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly short GetMask(int offset) => _values[offset];

	/// <summary>
	/// Get the candidate mask part of the specified cell.
	/// </summary>
	/// <param name="cell">The cell offset you want to get.</param>
	/// <returns>
	/// <para>
	/// The candidate mask. The return value is a 9-bit <see cref="short"/>
	/// value, where each bit will be:
	/// <list type="table">
	/// <item>
	/// <term><c>0</c></term>
	/// <description>The cell <b>doesn't contain</b> the possibility of the digit.</description>
	/// </item>
	/// <item>
	/// <term><c>1</c></term>
	/// <description>The cell <b>contains</b> the possibility of the digit.</description>
	/// </item>
	/// </list>
	/// </para>
	/// <para>
	/// For example, if the result mask is 266 (i.e. <c>0b<b>1</b>00_00<b>1</b>_0<b>1</b>0</c> in binary),
	/// the value will indicate the cell contains the digit 2, 4 and 9.
	/// </para>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly short GetCandidates(int cell) => (short)(_values[cell] & MaxCandidatesMask);

	/// <summary>
	/// Returns a reference to the element of the <see cref="Grid"/> at index zero.
	/// </summary>
	/// <returns>A reference to the element of the <see cref="Grid"/> at index zero.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public readonly ref readonly short GetPinnableReference() => ref _values[0];

	/// <summary>
	/// Get all masks and print them.
	/// </summary>
	/// <returns>The result.</returns>
	/// <remarks>
	/// Please note that the method cannot be called with a correct behavior using
	/// <see cref="DebuggerDisplayAttribute"/> to output. It seems that Visual Studio
	/// doesn't print correct values when indices of this grid aren't 0. In other words,
	/// when we call this method using <see cref="DebuggerDisplayAttribute"/>, only <c>grid[0]</c>
	/// can be output correctly, and other values will be incorrect: they're always 0.
	/// </remarks>
	public readonly string ToMaskString()
	{
		const string separator = ", ";
		fixed (short* pArr = _values)
		{
			var sb = new StringHandler(400);
			sb.AppendRangeWithSeparatorUnsafe(pArr, 81, static v => v.ToString(), separator);
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
			_ when GridFormatter.Create(format) is var f => format switch
			{
				":" => f.ToString(this).Match(RegularExpressions.ExtendedSusserEliminations) ?? string.Empty,
				"!" => f.ToString(this).RemoveAll('+'),
				".!" or "!." or "0!" or "!0" => f.ToString(this).RemoveAll('+'),
				".!:" or "!.:" or "0!:" => f.ToString(this).RemoveAll('+'),
				_ => f.ToString(this)
			}
		};

	/// <summary>
	/// Get the cell status at the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The cell status.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CellStatus GetStatus(int cell) => MaskToStatus(_values[cell]);

	/// <summary>
	/// Forms a slice out of the current grid that begins at a specified cell as the index.
	/// </summary>
	/// <param name="startCell">The cell as the index at which to begin the slice.</param>
	/// <returns>
	/// A grid segment that consists of all elements of the current grid from <paramref name="startCell"/>
	/// to the end of the grid.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly GridSegment Slice(int startCell) => Slice(startCell, 81 - startCell);

	/// <summary>
	/// Forms a slice out of the current grid starting at a specified cell as the index for a specified length.
	/// </summary>
	/// <param name="startCell">The cell as the index at which to begin this slice.</param>
	/// <param name="length">The desired length for the slice.</param>
	/// <returns>
	/// A grid segment that consists of <paramref name="length"/> elements from the current grid
	/// starting at <paramref name="startCell"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly GridSegment Slice(int startCell, int length) =>
		new(this, new Cells(startCell..(startCell + length)));

	/// <summary>
	/// Filters the cells that only satisfy the specified condition.
	/// </summary>
	/// <param name="predicate">The condition to filter cells.</param>
	/// <returns>
	/// The <see cref="GridSegment"/> instance that holds the specified cells
	/// having satisfied the specified condition.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly GridSegment Where(delegate*<in Grid, int, bool> predicate) => new(this, GetCells(predicate));

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CandidateCollectionEnumerator GetEnumerator() => EnumerateCandidates();

	/// <summary>
	/// Try to enumerate all possible candidates in the current grid.
	/// </summary>
	/// <returns>
	/// An enumerator that allows us using <see langword="foreach"/> statement
	/// to iterate all possible candidates in the current grid.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
	/// you can apply <see langword="ref"/> and <see langword="ref readonly"/> modifier
	/// onto the iteration variable:
	/// <code>
	/// foreach (ref readonly short mask in grid)
	/// {
	///     // Do something.
	/// }
	/// </code>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly MaskCollectionEnumerator EnumerateMasks()
	{
		fixed (short* arr = _values)
		{
			return new(arr);
		}
	}

	/// <summary>
	/// Reset the sudoku grid, to set all modifiable values to empty ones.
	/// </summary>
	public void Reset()
	{
		for (int i = 0; i < 81; i++)
		{
			if (GetStatus(i) == CellStatus.Modifiable)
			{
				this[i] = -1; // Reset the cell, and then re-compute all candidates.
			}
		}
	}

	/// <summary>
	/// To fix the current grid (all modifiable values will be changed to given ones).
	/// </summary>
	public void Fix()
	{
		for (int i = 0; i < 81; i++)
		{
			if (GetStatus(i) == CellStatus.Modifiable)
			{
				SetStatus(i, CellStatus.Given);
			}
		}
	}

	/// <summary>
	/// To unfix the current grid (all given values will be changed to modifiable ones).
	/// </summary>
	public void Unfix()
	{
		for (int i = 0; i < 81; i++)
		{
			if (GetStatus(i) == CellStatus.Given)
			{
				SetStatus(i, CellStatus.Modifiable);
			}
		}
	}

	/// <summary>
	/// Set the specified cell to the specified status.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="status">The status.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetStatus(int cell, CellStatus status)
	{
		ref short mask = ref _values[cell];
		short copied = mask;
		mask = (short)((int)status << 9 | mask & MaxCandidatesMask);

		var f = (delegate*<ref Grid, int, short, short, int, void>)ValueChanged;
		f(ref this, cell, copied, mask, -1);
	}

	/// <summary>
	/// Set the specified cell to the specified mask.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="mask">The mask to set.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMask(int cell, short mask)
	{
		ref short m = ref _values[cell];
		short copied = m;
		m = mask;

		var f = (delegate*<ref Grid, int, short, short, int, void>)ValueChanged;
		f(ref this, cell, copied, m, -1);
	}

#if FEATURE_GENERIC_MATH
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IEquatable<Grid>.Equals(Grid other) => Equals(this, other);
#endif

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
		var result = new Cells[9];
		for (int digit = 0; digit < 9; digit++)
		{
			ref var map = ref result[digit];
			for (int cell = 0; cell < 81; cell++)
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
		for (int cell = 0; cell < 81; cell++)
		{
			if (predicate(this, cell))
			{
				result.AddAnyway(cell);
			}
		}

		return result;
	}


	/// <summary>
	/// To determine whether two sudoku grid is totally same.
	/// </summary>
	/// <param name="left">The left one.</param>
	/// <param name="right">The right one.</param>
	/// <returns>The <see cref="bool"/> result indicating that.</returns>
	public static bool Equals(in Grid left, in Grid right)
	{
		fixed (short* pThis = left, pOther = right)
		{
			int i = 0;
			for (short* l = pThis, r = pOther; i < 81; i++, l++, r++)
			{
				if (*l != *r)
				{
					return false;
				}
			}

			return true;
		}
	}

	/// <summary>
	/// <para>Parses a string value and converts to this type.</para>
	/// <para>
	/// If you want to parse a PM grid, we recommend you use the method
	/// <see cref="Parse(string, GridParsingOption)"/> instead of this method.
	/// </para>
	/// </summary>
	/// <param name="str">The string.</param>
	/// <returns>The result instance had converted.</returns>
	/// <seealso cref="Parse(string, GridParsingOption)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(ReadOnlySpan<char> str) => new GridParser(str.ToString()).Parse();

	/// <summary>
	/// Parses a pointer that points to a string value and converts to this type.
	/// </summary>
	/// <param name="ptrStr">The pointer that points to string.</param>
	/// <returns>The result instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(char* ptrStr!!) => Parse(new string(ptrStr));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string str) => new GridParser(str).Parse();

	/// <summary>
	/// <para>
	/// Parses a string value and converts to this type.
	/// </para>
	/// <para>
	/// If you want to parse a PM grid, you should decide the mode to parse.
	/// If you use compatible mode to parse, all single values will be treated as
	/// given values; otherwise, recommended mode, which uses '<c><![CDATA[<d>]]></c>'
	/// or '<c>*d*</c>' to represent a value be a given or modifiable one. The decision
	/// will be indicated and passed by the second parameter <paramref name="compatibleFirst"/>.
	/// </para>
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the parsing operation should use compatible mode to check PM grid.
	/// </param>
	/// <returns>The result instance had converted.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string str, bool compatibleFirst) => new GridParser(str, compatibleFirst).Parse();

	/// <summary>
	/// Parses a string value and converts to this type, using a specified grid parsing type.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="gridParsingOption">The grid parsing type.</param>
	/// <returns>The result instance had converted.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string str, GridParsingOption gridParsingOption) =>
		new GridParser(str).Parse(gridParsingOption);

	/// <inheritdoc/>
	public static bool TryParse(string str, out Grid result)
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

	/// <summary>
	/// Try to parse a string and converts to this type, and returns a
	/// <see cref="bool"/> value indicating the result of the conversion.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="option">The grid parsing type.</param>
	/// <param name="result">
	/// The result parsed. If the conversion is failed,
	/// this argument will be <see cref="Undefined"/>.
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <seealso cref="Undefined"/>
	public static bool TryParse(string str, GridParsingOption option, out Grid result)
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

	/// <summary>
	/// To get the cell status through a mask.
	/// </summary>
	/// <param name="mask">The mask.</param>
	/// <returns>The cell status.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static CellStatus MaskToStatus(short mask) => (CellStatus)(mask >> 9 & 7);


	/// <summary>
	/// Gets the grid segment that uses the specified grid as the template,
	/// and a <see cref="Cells"/> instance as the pattern.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="pattern">The pattern.</param>
	/// <returns>The result grid.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GridSegment operator &(in Grid grid, in Cells pattern) => new(grid, pattern);

	/// <summary>
	/// Determine whether two <see cref="Grid"/>s are same.
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(in Grid left, in Grid right) => Equals(left, right);

	/// <summary>
	/// Determine whether two <see cref="Grid"/>s aren't same.
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(in Grid left, in Grid right) => !(left == right);

#if FEATURE_GENERIC_MATH
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Grid, Grid>.operator ==(Grid left, Grid right) => left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Grid, Grid>.operator !=(Grid left, Grid right) => left != right;

#if FEATURE_GENERIC_MATH_IN_ARG
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static GridSegment IValueBitwiseAndOperators<Grid, Cells, GridSegment>.operator &(in Grid left, Cells right) =>
		left & right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static GridSegment IValueBitwiseAndOperators<Grid, Cells, GridSegment>.operator &(Grid left, in Cells right) =>
		left & right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<Grid, Grid>.operator ==(Grid left, in Grid right) => left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<Grid, Grid>.operator ==(in Grid left, Grid right) => left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<Grid, Grid>.operator !=(Grid left, in Grid right) => left != right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<Grid, Grid>.operator !=(in Grid left, Grid right) => left != right;
#endif
#endif
}
