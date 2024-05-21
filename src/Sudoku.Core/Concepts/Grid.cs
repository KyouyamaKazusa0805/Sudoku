#undef EMPTY_GRID_STRING_CONSTANT
#undef SYNC_ROOT_VIA_METHODIMPL
#define SYNC_ROOT_VIA_OBJECT
#undef SYNC_ROOT_VIA_THREAD_LOCAL
#if SYNC_ROOT_VIA_METHODIMPL && SYNC_ROOT_VIA_OBJECT && SYNC_ROOT_VIA_THREAD_LOCAL
#line 1 "Grid.cs"
#error You cannot set all three symbols 'SYNC_ROOT_VIA_METHODIMPL', 'SYNC_ROOT_VIA_OBJECT' and 'SYNC_ROOT_VIA_THREAD_LOCAL' - they are designed by the same purpose. You should only define at most one of three symbols.
#line default
#elif SYNC_ROOT_VIA_METHODIMPL && SYNC_ROOT_VIA_OBJECT
#line 1 "Grid.cs"
#error Don't set both symbols 'SYNC_ROOT_VIA_METHODIMPL' and 'SYNC_ROOT_VIA_OBJECT'.
#line default
#elif SYNC_ROOT_VIA_METHODIMPL && SYNC_ROOT_VIA_THREAD_LOCAL
#line 1 "Grid.cs"
#error Don't set both symbols 'SYNC_ROOT_VIA_METHODIMPL' and 'SYNC_ROOT_VIA_THREAD_LOCAL'.
#line default
#elif SYNC_ROOT_VIA_OBJECT && SYNC_ROOT_VIA_THREAD_LOCAL
#line 1 "Grid.cs"
#error Don't set both symbols 'SYNC_ROOT_VIA_OBJECT' and 'SYNC_ROOT_VIA_THREAD_LOCAL'.
#line default
#elif !SYNC_ROOT_VIA_METHODIMPL && !SYNC_ROOT_VIA_OBJECT && !SYNC_ROOT_VIA_THREAD_LOCAL
#line 1 "Grid.cs"
#warning No sync-root mode is selected, meaning we cannot use this type in multi-threading (i.e. this type becomes thread-unsafe) because some members will rely on pointers and shared memory, which is unsafe. You can ONLY use property 'IsValid', 'SolutionGrid' and method 'ExactlyValidate' in this type inside a lock statement.
#line default
#endif

namespace Sudoku.Concepts;

/// <summary>
/// Represents a sudoku grid that uses the mask list to construct the data structure.
/// </summary>
/// <remarks>
/// <para><include file="../../global-doc-comments.xml" path="/g/large-structure"/></para>
/// </remarks>
[JsonConverter(typeof(Converter))]
[DebuggerDisplay($$"""{{{nameof(ToString)}}("#")}""")]
[InlineArray(CellsCount)]
[CollectionBuilder(typeof(Grid), nameof(Create))]
[DebuggerStepThrough]
[LargeStructure]
[ComparisonOperators]
[Equals]
[EqualityOperators]
public partial struct Grid :
	IComparable<Grid>,
	IComparisonOperators<Grid, Grid, bool>,
	IEnumerable<Digit>,
	IEquatable<Grid>,
	IEqualityOperators<Grid, Grid, bool>,
	IFormattable,
	IJsonSerializable<Grid>,
	IMinMaxValue<Grid>,
	IParsable<Grid>,
	IReadOnlyCollection<Digit>,
	ISelectMethod<Grid, Candidate>,
	ITokenizable<Grid>,
	IToArrayMethod<Grid, Digit>,
	IWhereMethod<Grid, Candidate>
{
	/// <summary>
	/// Indicates the default mask of a cell (an empty cell, with all 9 candidates left).
	/// </summary>
	public const Mask DefaultMask = EmptyMask | MaxCandidatesMask;

	/// <summary>
	/// Indicates the maximum candidate mask that used.
	/// </summary>
	public const Mask MaxCandidatesMask = (1 << CellCandidatesCount) - 1;

	/// <summary>
	/// Indicates the empty mask, modifiable mask and given mask.
	/// </summary>
	public const Mask EmptyMask = (Mask)CellState.Empty << CellCandidatesCount;

	/// <summary>
	/// Indicates the modifiable mask.
	/// </summary>
	public const Mask ModifiableMask = (Mask)CellState.Modifiable << CellCandidatesCount;

	/// <summary>
	/// Indicates the given mask.
	/// </summary>
	public const Mask GivenMask = (Mask)CellState.Given << CellCandidatesCount;

#if EMPTY_GRID_STRING_CONSTANT
	/// <summary>
	/// Indicates the empty grid string.
	/// </summary>
	public const string EmptyString = "000000000000000000000000000000000000000000000000000000000000000000000000000000000";
#endif

	/// <summary>
	/// Indicates the shifting bits count for header bits.
	/// </summary>
	internal const int HeaderShift = CellCandidatesCount + 3;

	/// <summary>
	/// Indicates ths header bits describing the sudoku type is a Sukaku.
	/// </summary>
	internal const Mask SukakuHeader = (int)SudokuType.Sukaku << HeaderShift;

	/// <summary>
	/// Indicates the number of cells of a sudoku grid.
	/// </summary>
	private const Cell CellsCount = 81;

	/// <summary>
	/// Indicates the number of candidates appeared in a cell.
	/// </summary>
	private const Digit CellCandidatesCount = 9;


#if !EMPTY_GRID_STRING_CONSTANT
	/// <summary>
	/// Indicates the empty grid string.
	/// </summary>
	public static readonly string EmptyString = new('0', CellsCount);
#endif

	/// <summary>
	/// The empty grid that is valid during implementation or running the program (all values are <see cref="DefaultMask"/>, i.e. empty cells).
	/// </summary>
	/// <remarks>
	/// This field is initialized by the static constructor of this structure.
	/// </remarks>
	/// <seealso cref="DefaultMask"/>
	public static readonly Grid Empty = [DefaultMask];

	/// <summary>
	/// Indicates the default grid that all values are initialized 0. This value is equivalent to <see langword="default"/>(<see cref="Grid"/>).
	/// </summary>
	/// <remarks>
	/// This value can be used for non-candidate-based sudoku operations, e.g. a sudoku grid canvas.
	/// </remarks>
	public static readonly Grid Undefined;

#if SYNC_ROOT_VIA_OBJECT && !SYNC_ROOT_VIA_METHODIMPL
	/// <summary>
	/// The internal field that can be used for making threads run in order while using <see cref="Solver"/>,
	/// keeping the type being thread-safe.
	/// </summary>
	/// <seealso cref="Solver"/>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	private static readonly object PuzzleSolvingSynchronizer = new();
#endif

	/// <summary>
	/// Indicates the backing solver.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[EditorBrowsable(EditorBrowsableState.Never)]
#if SYNC_ROOT_VIA_THREAD_LOCAL
	private static readonly ThreadLocal<BitwiseSolver> Solver = new(static () => new());
#else
	private static readonly BitwiseSolver Solver = new();
#endif

	/// <inheritdoc cref="IJsonSerializable{TSelf}.DefaultOptions"/>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	private static readonly JsonSerializerOptions DefaultOptions = new() { Converters = { new Converter() } };


	/// <summary>
	/// Indicates the inner array that stores the masks of the sudoku grid, which stores the in-time sudoku grid inner information.
	/// </summary>
	/// <remarks>
	/// The field uses the mask table of length 81 to indicate the state and all possible candidates
	/// holding for each cell. Each mask uses a <see cref="Mask"/> value, but only uses 11 of 16 bits.
	/// <code>
	/// | 16  15  14  13  12  11  10  9   8   7   6   5   4   3   2   1   0 |
	/// |-------------------|-----------|-----------------------------------|
	/// |    unused bits    | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 |
	/// '-------------------|-----------|-----------------------------------'
	///                      \_________/ \_________________________________/
	///                          (2)                     (1)
	/// </code>
	/// Here the 9 bits in (1) indicate whether each digit is possible candidate in the current cell for each bit respectively,
	/// and the higher 3 bits in (2) indicate the cell state. The possible cell state are:
	/// <list type="table">
	/// <listheader>
	/// <term>State name</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>Empty cell (i.e. <see cref="CellState.Empty"/>)</term>
	/// <description>The cell is currently empty, and wait for being filled.</description>
	/// </item>
	/// <item>
	/// <term>Modifiable cell (i.e. <see cref="CellState.Modifiable"/>)</term>
	/// <description>The cell is filled by a digit, but the digit isn't the given by the initial grid.</description>
	/// </item>
	/// <item>
	/// <term>Given cell (i.e. <see cref="CellState.Given"/>)</term>
	/// <description>The cell is filled by a digit, which is given by the initial grid and can't be modified.</description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <seealso cref="CellState"/>
	private Mask _values;


	/// <summary>
	/// Creates a <see cref="Grid"/> instance via the pointer of the first element of the cell digit, and the creating option.
	/// </summary>
	/// <param name="firstElement">The reference of the first element.</param>
	/// <param name="creatingOption">The creating option.</param>
	/// <exception cref="ArgumentNullRefException">
	/// Throws when the argument <paramref name="firstElement"/> is <see langword="null"/> reference.
	/// </exception>
	private Grid(ref readonly Digit firstElement, GridCreatingOption creatingOption = GridCreatingOption.None)
	{
		@ref.ThrowIfNullRef(in firstElement);

		// Firstly we should initialize the inner values.
		this = Empty;

		// Then traverse the array (span, pointer or etc.), to get refresh the values.
		var minusOneEnabled = creatingOption == GridCreatingOption.MinusOne;
		for (var i = 0; i < CellsCount; i++)
		{
			var value = @ref.Add(ref @ref.AsMutableRef(in firstElement), i);
			if ((minusOneEnabled ? value - 1 : value) is var realValue and not -1)
			{
				// Calls the indexer to trigger the event (Clear the candidates in peer cells).
				SetDigit(i, realValue);

				// Set the state to 'CellState.Given'.
				SetState(i, CellState.Given);
			}
		}
	}


	/// <summary>
	/// Indicates the grid has already solved. If the value is <see langword="true"/>, the grid is solved; otherwise, <see langword="false"/>.
	/// </summary>
	public readonly bool IsSolved
	{
		get
		{
			for (var i = 0; i < CellsCount; i++)
			{
				if (GetState(i) == CellState.Empty)
				{
					return false;
				}
			}

			for (var i = 0; i < CellsCount; i++)
			{
				switch (GetState(i))
				{
					case CellState.Given or CellState.Modifiable:
					{
						var curDigit = GetDigit(i);
						foreach (var cell in PeersMap[i])
						{
							if (curDigit == GetDigit(cell))
							{
								return false;
							}
						}
						break;
					}
					case CellState.Empty:
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
	}

	/// <summary>
	/// Indicates whether the grid is <see cref="Undefined"/>, which means the grid holds totally same value with <see cref="Undefined"/>.
	/// </summary>
	/// <seealso cref="Undefined"/>
	public readonly bool IsUndefined
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this == Undefined;
	}

	/// <summary>
	/// Indicates whether the grid is <see cref="Empty"/>, which means the grid holds totally same value with <see cref="Empty"/>.
	/// </summary>
	/// <seealso cref="Empty"/>
	public readonly bool IsEmpty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this == Empty;
	}

	/// <summary>
	/// Indicates whether the puzzle has a unique solution.
	/// </summary>
	public readonly bool IsValid => IsSolved || Uniqueness == Uniqueness.Unique;

	/// <summary>
	/// Determines whether the puzzle is a minimal puzzle, which means the puzzle will become multiple solution
	/// if arbitrary one given digit will be removed from the grid.
	/// </summary>
	public readonly bool IsMinimal => CheckMinimal(out _);

	/// <summary>
	/// Determines whether the current grid contains any missing candidates.
	/// </summary>
	public readonly bool ContainsAnyMissingCandidates => ResetGrid == ResetCandidatesGrid.ResetGrid && this != ResetCandidatesGrid;

	/// <summary>
	/// Indicates the token of the grid at the initial state.
	/// </summary>
	/// <remarks>
	/// A raw string example is:
	/// <code><![CDATA[
	/// 35i4ra00rlr4btf9a8s573tsk1ldni00ccfg094v02pk54ff1hc6e7
	/// ]]></code>
	/// We should cut them by 6 characters as a group:
	/// <code><![CDATA[
	/// 35i4ra 00rlr4 btf9a8 s573ts k1ldni 00ccfg 094v02 pk54ff 1hc6e7
	/// ]]></code>
	/// 9 groups in total.
	/// Then we should convert it into a valid 9-digit number by treating them as 32-based integers.
	/// Finally, combinate all groups, then we are done.
	/// The final text is
	/// <code><![CDATA[
	/// 106500970000907108400008520945000380672839410000406000009600002860000751051780039
	/// ]]></code>
	/// </remarks>
	/// <exception cref="NotSupportedException">Throws when the puzzle type is Sukaku.</exception>
	public readonly string Token
	{
		get
		{
			if (PuzzleType == SudokuType.Sukaku)
			{
				throw new NotSupportedException(ResourceDictionary.ExceptionMessage("NotSupportedForSukakuPuzzles"));
			}

			var convertedString = ToString("0");
			var values = from str in convertedString.CutOfLength(9) select int.Parse(str);
			var sb = new StringBuilder(54);
			for (var i = 8; i >= 0; i--)
			{
				var value = values[i];
				var stack = new Stack<int>();
				while (value != 0)
				{
					stack.Push(value & 31);
					value >>= 5;
				}

				foreach (var element in stack)
				{
					sb.Append(Base32CharSpan[element]);
				}
			}

			return sb.ToString();
		}
	}

	/// <summary>
	/// Indicates the total number of given cells.
	/// </summary>
	public readonly Cell GivensCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GivenCells.Count;
	}

	/// <summary>
	/// Indicates the total number of modifiable cells.
	/// </summary>
	public readonly Cell ModifiablesCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ModifiableCells.Count;
	}

	/// <summary>
	/// Indicates the total number of empty cells.
	/// </summary>
	public readonly Cell EmptiesCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => EmptyCells.Count;
	}

	/// <summary>
	/// Indicates the number of total candidates.
	/// </summary>
	public readonly Candidate CandidatesCount
	{
		get
		{
			var count = 0;
			for (var i = 0; i < CellsCount; i++)
			{
				if (GetState(i) == CellState.Empty)
				{
					count += PopCount((uint)GetCandidates(i));
				}
			}

			return count;
		}
	}

	/// <summary>
	/// <para>Indicates which houses are empty houses.</para>
	/// <para>An <b>Empty House</b> is a house holding 9 empty cells, i.e. all cells in this house are empty.</para>
	/// <para>
	/// The property returns a <see cref="HouseMask"/> value as a mask that contains all possible house indices.
	/// For example, if the row 5, column 5 and block 5 (1-9) are null houses, the property will return
	/// the result <see cref="HouseMask"/> value, <c>000010000_000010000_000010000</c> as binary.
	/// </para>
	/// </summary>
	public readonly HouseMask EmptyHouses
	{
		get
		{
			var result = 0;
			for (var (house, valueCells) = (0, ~EmptyCells); house < 27; house++)
			{
				if (Subview.ReduceCellByHouse(in valueCells, house) == 0)
				{
					result |= 1 << house;
				}
			}
			return result;
		}
	}

	/// <summary>
	/// <para>Indicates which houses are completed, regardless of ways of filling.</para>
	/// <para><inheritdoc cref="EmptyHouses" path="//summary/para[3]"/></para>
	/// </summary>
	public readonly HouseMask FullHouses
	{
		get
		{
			var emptyCells = EmptyCells;
			var result = 0;
			for (var houseIndex = 0; houseIndex < 27; houseIndex++)
			{
				var isCompleted = true;
				foreach (var cell in HousesCells[houseIndex])
				{
					if (emptyCells.Contains(cell))
					{
						// The current cells is empty, breaking the rule of full house.
						isCompleted = false;
						break;
					}
				}
				if (!isCompleted)
				{
					continue;
				}

				result |= 1 << houseIndex;
			}
			return result;
		}
	}

	/// <summary>
	/// Try to get the symmetry of the puzzle.
	/// </summary>
	public readonly SymmetricType Symmetry => GivenCells.Symmetry;

	/// <summary>
	/// Checks the uniqueness of the current sudoku puzzle.
	/// </summary>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <exception cref="InvalidOperationException">Throws when the puzzle has already been solved.</exception>
	public readonly Uniqueness Uniqueness
	{
#if SYNC_ROOT_VIA_METHODIMPL && !SYNC_ROOT_VIA_OBJECT
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.Synchronized)]
#endif
		get
		{
			if (IsSolved)
			{
				throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("CannotSolveAPuzzleAlreadySolved"));
			}

			long count;
#if SYNC_ROOT_VIA_OBJECT && !SYNC_ROOT_VIA_METHODIMPL
			lock (PuzzleSolvingSynchronizer)
#endif
			{
				count = Solver
#if SYNC_ROOT_VIA_THREAD_LOCAL
				.Value!
#endif
					.SolveString(ToString(), out _, 2);
			}

			return count switch { 0 => Uniqueness.Bad, 1 => Uniqueness.Unique, _ => Uniqueness.Multiple };
		}
	}

	/// <summary>
	/// Indicates the type of the puzzle.
	/// </summary>
	/// <remarks>
	/// Although the property type supports for other values, this property can only return a value
	/// either <see cref="SudokuType.Standard"/> or <see cref="SudokuType.Sukaku"/>.
	/// </remarks>
	/// <seealso cref="SudokuType.Standard"/>
	/// <seealso cref="SudokuType.Sukaku"/>
	public readonly SudokuType PuzzleType => GetHeaderBits(0) switch { SukakuHeader => SudokuType.Sukaku, _ => SudokuType.Standard };

	/// <summary>
	/// Gets a cell list that only contains the given cells.
	/// </summary>
	public readonly unsafe CellMap GivenCells => GetMap(&GridCellPredicates.GivenCells);

	/// <summary>
	/// Gets a cell list that only contains the modifiable cells.
	/// </summary>
	public readonly unsafe CellMap ModifiableCells => GetMap(&GridCellPredicates.ModifiableCells);

	/// <summary>
	/// Indicates a cell list whose corresponding position in this grid is empty.
	/// </summary>
	public readonly unsafe CellMap EmptyCells => GetMap(&GridCellPredicates.EmptyCells);

	/// <summary>
	/// Indicates a cell list whose corresponding position in this grid contain two candidates.
	/// </summary>
	public readonly unsafe CellMap BivalueCells => GetMap(&GridCellPredicates.BivalueCells);

	/// <summary>
	/// Indicates the map of possible positions of the existence of the candidate value for each digit.
	/// The return value will be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </summary>
	public readonly unsafe ReadOnlySpan<CellMap> CandidatesMap => GetMaps(&GridCellPredicates.CandidatesMap);

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
	public readonly unsafe ReadOnlySpan<CellMap> DigitsMap => GetMaps(&GridCellPredicates.DigitsMap);

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
	public readonly unsafe ReadOnlySpan<CellMap> ValuesMap => GetMaps(&GridCellPredicates.ValuesMap);

	/// <summary>
	/// Indicates all possible candidates in the current grid.
	/// </summary>
	public readonly ReadOnlySpan<Candidate> Candidates
	{
		get
		{
			var candidates = new Candidate[CandidatesCount];
			for (var (cell, i) = (0, 0); cell < CellsCount; cell++)
			{
				if (GetState(cell) == CellState.Empty)
				{
					foreach (var digit in GetCandidates(cell))
					{
						candidates[i++] = cell * CellCandidatesCount + digit;
					}
				}
			}
			return candidates;
		}
	}

	/// <summary>
	/// Indicates all possible conjugate pairs appeared in this grid.
	/// </summary>
	public readonly ReadOnlySpan<Conjugate> ConjugatePairs
	{
		get
		{
			var conjugatePairs = new List<Conjugate>();
			var candidatesMap = CandidatesMap;
			for (var digit = 0; digit < CellCandidatesCount; digit++)
			{
				ref readonly var cellsMap = ref candidatesMap[digit];
				foreach (var houseMap in HousesMap)
				{
					if ((houseMap & cellsMap) is { Count: 2 } temp)
					{
						conjugatePairs.Add(new(in temp, digit));
					}
				}
			}
			return conjugatePairs.AsReadOnlySpan();
		}
	}

	/// <summary>
	/// Gets the grid where all modifiable cells are empty cells (i.e. the initial one).
	/// </summary>
	public readonly Grid ResetGrid => Preserve(GivenCells);

	/// <summary>
	/// Gets the grid where all empty cells are filled with all possible candidates.
	/// </summary>
	public readonly Grid ResetCandidatesGrid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var result = this;
			result.ResetCandidates();
			return result;
		}
	}

	/// <summary>
	/// Indicates the unfixed grid for the current grid, meaning all given digits will be replaced with modifiable ones.
	/// </summary>
	public readonly Grid UnfixedGrid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var result = this;
			result.Unfix();
			return result;
		}
	}

	/// <summary>
	/// Indicates the fixed grid for the current grid, meaning all modifiable digits will be replaced with given ones.
	/// </summary>
	public readonly Grid FixedGrid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var result = this;
			result.Fix();
			return result;
		}
	}

	/// <summary>
	/// Indicates the solution of the current grid. If the puzzle has no solution or multiple solutions,
	/// this property will return <see cref="Undefined"/>.
	/// </summary>
	/// <seealso cref="Undefined"/>
	public readonly Grid SolutionGrid
	{
#if SYNC_ROOT_VIA_METHODIMPL && !SYNC_ROOT_VIA_OBJECT
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.Synchronized)]
#endif
		get
		{
#if SYNC_ROOT_VIA_OBJECT && !SYNC_ROOT_VIA_METHODIMPL
			lock (PuzzleSolvingSynchronizer)
#endif
			{
				return Solver
#if SYNC_ROOT_VIA_THREAD_LOCAL
					.Value!
#endif
					.Solve(in this) is { IsUndefined: false } solution ? unfix(in solution, GivenCells) : Undefined;
			}


			static Grid unfix(ref readonly Grid solution, ref readonly CellMap pattern)
			{
				var result = solution;
				foreach (var cell in ~pattern)
				{
					if (result.GetState(cell) == CellState.Given)
					{
						result.SetState(cell, CellState.Modifiable);
					}
				}
				return result;
			}
		}
	}

	/// <inheritdoc/>
	readonly int IReadOnlyCollection<Digit>.Count => CellsCount;


	/// <inheritdoc/>
	/// <summary>
	/// The character span that indicates all possible characters appeared in a number with base 32.
	/// </summary>
	internal static ReadOnlySpan<char> Base32CharSpan => "0123456789abcdefghijklmnopqrstuv";

	/// <summary>
	/// Indicates the minimum possible grid value that the current type can reach.
	/// </summary>
	/// <remarks>
	/// This value is found out via backtracking algorithm. For more information, please visit type <see cref="BacktrackingSolver"/>.
	/// </remarks>
	/// <seealso cref="BacktrackingSolver"/>
	static Grid IMinMaxValue<Grid>.MinValue => Parse("123456789456789123789123456214365897365897214897214365531642978642978531978531642");

	/// <summary>
	/// Indicates the maximum possible grid value that the current type can reach.
	/// </summary>
	/// <remarks>
	/// This value is found out via backtracking algorithm. For more information, please visit type <see cref="BacktrackingSolver"/>.
	/// </remarks>
	/// <seealso cref="BacktrackingSolver"/>
	static Grid IMinMaxValue<Grid>.MaxValue => Parse("987654321654321987321987654896745213745213896213896745579468132468132579132579468");

	/// <inheritdoc/>
	static JsonSerializerOptions IJsonSerializable<Grid>.DefaultOptions => DefaultOptions;


	/// <summary>
	/// Creates a mask of type <see cref="Mask"/> that represents the usages of digits 1 to 9,
	/// ranged in a specified list of cells in the current sudoku grid.
	/// </summary>
	/// <param name="cells">The list of cells to gather the usages on all digits.</param>
	/// <returns>A mask of type <see cref="Mask"/> that represents the usages of digits 1 to 9.</returns>
	public readonly Mask this[ref readonly CellMap cells]
	{
		get
		{
			var result = (Mask)0;
			foreach (var cell in cells)
			{
				result |= this[cell];
			}
			return (Mask)(result & MaxCandidatesMask);
		}
	}

	/// <summary>
	/// <inheritdoc cref="this[ref readonly CellMap]" path="/summary"/>
	/// </summary>
	/// <param name="cells"><inheritdoc cref="this[ref readonly CellMap]" path="/param[@name='cells']"/></param>
	/// <param name="withValueCells">
	/// Indicates whether the value cells (given or modifiable ones) will be included to be gathered.
	/// If <see langword="true"/>, all value cells (no matter what kind of cell) will be summed up.
	/// </param>
	/// <returns><inheritdoc cref="this[ref readonly CellMap]" path="/returns"/></returns>
	public readonly Mask this[ref readonly CellMap cells, bool withValueCells]
	{
		get
		{
			var result = (Mask)0;
			foreach (var cell in cells)
			{
				if (!withValueCells && GetState(cell) != CellState.Empty || withValueCells)
				{
					result |= this[cell];
				}
			}
			return (Mask)(result & MaxCandidatesMask);
		}
	}

	/// <summary>
	/// <inheritdoc cref="this[ref readonly CellMap]" path="/summary"/>
	/// </summary>
	/// <param name="cells"><inheritdoc cref="this[ref readonly CellMap]" path="/param[@name='cells']"/></param>
	/// <param name="withValueCells">
	/// <inheritdoc cref="this[ref readonly CellMap, bool]" path="/param[@name='withValueCells']"/>
	/// </param>
	/// <param name="mergingMethod">
	/// </param>
	/// <returns><inheritdoc cref="this[ref readonly CellMap]" path="/returns"/></returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when <paramref name="mergingMethod"/> is not defined.</exception>
	public readonly unsafe Mask this[ref readonly CellMap cells, bool withValueCells, GridMaskMergingMethod mergingMethod]
	{
		get
		{
			var result = mergingMethod switch
			{
				GridMaskMergingMethod.AndNot => MaxCandidatesMask,
				GridMaskMergingMethod.And => MaxCandidatesMask,
				GridMaskMergingMethod.Or => (Mask)0,
				_ => throw new ArgumentOutOfRangeException(nameof(mergingMethod))
			};
			var mergingFunctionPtr = mergingMethod switch
			{
				GridMaskMergingMethod.AndNot => &andNot,
				GridMaskMergingMethod.And => &and,
				GridMaskMergingMethod.Or => &or,
				_ => default(delegate*<ref Mask, ref readonly Grid, Cell, void>)
			};
			foreach (var cell in cells)
			{
				if (!withValueCells && GetState(cell) == CellState.Empty || withValueCells)
				{
					mergingFunctionPtr(ref result, in this, cell);
				}
			}
			return result;


			static void andNot(ref Mask result, ref readonly Grid grid, Cell cell) => result &= (Mask)~grid[cell];

			static void and(ref Mask result, ref readonly Grid grid, Cell cell) => result &= grid[cell];

			static void or(ref Mask result, ref readonly Grid grid, Cell cell) => result |= grid[cell];
		}
	}


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly void Deconstruct(out CellMap givenCells, out CellMap modifiableCells, out CellMap emptyCells)
		=> (givenCells, modifiableCells, emptyCells) = (GivenCells, ModifiableCells, EmptyCells);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly void Deconstruct(out CellMap givenCells, out CellMap modifiableCells, out CellMap emptyCells, out CellMap bivalueCells)
		=> ((givenCells, modifiableCells, emptyCells), bivalueCells) = (this, BivalueCells);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly void Deconstruct(
		out CellMap emptyCells,
		out CellMap bivalueCells,
		out ReadOnlySpan<CellMap> candidatesMap,
		out ReadOnlySpan<CellMap> digitsMap,
		out ReadOnlySpan<CellMap> valuesMap
	)
	{
		(emptyCells, bivalueCells) = (EmptyCells, BivalueCells);
		candidatesMap = CandidatesMap;
		digitsMap = DigitsMap;
		valuesMap = ValuesMap;
	}

	/// <summary>
	/// Determine whether the specified <see cref="Grid"/> instance hold the same values as the current instance.
	/// </summary>
	/// <param name="other">The instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(ref readonly Grid other) => Equals(in other, GridComparison.Default);

	/// <summary>
	/// Determine whether the specified <see cref="Grid"/> instance hold the same values as the current instance,
	/// by using the specified comparison type.
	/// </summary>
	/// <param name="other">The instance to compare.</param>
	/// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="comparisonType"/> is not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(ref readonly Grid other, GridComparison comparisonType)
		=> comparisonType switch
		{
			GridComparison.Default => this[..].SequenceEqual(other[..]),
			GridComparison.IncludingTransforms => this.GetMinLexGrid() == other.GetMinLexGrid(),
			_ => throw new ArgumentOutOfRangeException(nameof(comparisonType))
		};

	/// <summary>
	/// Determine whether the digit in the target cell may be duplicated with a certain cell in the peers of the current cell,
	/// if the digit is filled into the cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public readonly bool DuplicateWith(Cell cell, Digit digit)
	{
		foreach (var tempCell in PeersMap[cell])
		{
			if (GetDigit(tempCell) == digit)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Determines whether the puzzle is a minimal puzzle, which means the puzzle will become multiple solution
	/// if arbitrary one given digit will be removed from the grid.
	/// </summary>
	/// <param name="firstCandidateMakePuzzleNotMinimal">
	/// <para>
	/// Indicates the first found candidate that can make the puzzle not minimal, which means
	/// if we remove the digit in the cell, the puzzle will still keep unique.
	/// </para>
	/// <para>If the return value is <see langword="true"/>, this argument will be -1.</para>
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <exception cref="InvalidOperationException">Throws when the puzzle is invalid (i.e. not unique).</exception>
	public readonly bool CheckMinimal(out Candidate firstCandidateMakePuzzleNotMinimal)
	{
		switch (this)
		{
			case { IsValid: false }:
			{
				throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("GridMultipleSolutions"));
			}
			case { IsSolved: true, GivenCells.Count: CellsCount }:
			{
				// Very special case: all cells are givens.
				// The puzzle is considered not a minimal puzzle, because any digit in the grid can be removed.
				firstCandidateMakePuzzleNotMinimal = GetDigit(0);
				return false;
			}
			default:
			{
				var gridCopied = UnfixedGrid;
				foreach (var cell in gridCopied.ModifiableCells)
				{
					var newGrid = gridCopied;
					newGrid.SetDigit(cell, -1);
					newGrid.Fix();

					if (newGrid.IsValid)
					{
						firstCandidateMakePuzzleNotMinimal = cell * CellCandidatesCount + GetDigit(cell);
						return false;
					}
				}

				firstCandidateMakePuzzleNotMinimal = -1;
				return true;
			}
		}
	}

	/// <summary>
	/// Sets a candidate existence case with a <see cref="bool"/> value.
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <inheritdoc cref="SetExistence(Cell, Digit, bool)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool GetExistence(Cell cell, Digit digit) => (this[cell] >> digit & 1) != 0;

	/// <inheritdoc cref="Exists(Cell, Digit)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool? Exists(Candidate candidate) => Exists(candidate / CellCandidatesCount, candidate % CellCandidatesCount);

	/// <summary>
	/// Indicates whether the current grid contains the digit in the specified cell.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>
	/// The method will return a <see cref="bool"/>? value
	/// (containing three possible cases: <see langword="true"/>, <see langword="false"/> and <see langword="null"/>).
	/// All values corresponding to the cases are below:
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>Case description on this value</description>
	/// </listheader>
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
	/// the result case will be more precisely than the indexer <see cref="GetExistence(Cell, Digit)"/>,
	/// which is the main difference between this method and that indexer.
	/// </para>
	/// </remarks>
	/// <seealso cref="GetExistence(Cell, Digit)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool? Exists(Cell cell, Digit digit) => GetState(cell) == CellState.Empty ? GetExistence(cell, digit) : null;

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
		=> this switch { { IsUndefined: true } => 0, { IsEmpty: true } => 1, _ => ToString("#").GetHashCode() };

	/// <inheritdoc cref="IComparable{T}.CompareTo(T)"/>
	/// <exception cref="InvalidOperationException">Throws when the puzzle type is Sukaku.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int CompareTo(ref readonly Grid other)
		=> PuzzleType != SudokuType.Sukaku && other.PuzzleType != SudokuType.Sukaku
			? ToString("#").CompareTo(other.ToString("#"))
			: throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("ComparableGridMustBeStandard"));

	/// <summary>
	/// Compares the current instance with another object of the same type and returns an integer
	/// that indicates whether the current instance precedes, follows or occurs in the same position in the sort order as the other object.
	/// </summary>
	/// <param name="other">The other object to be compared.</param>
	/// <param name="comparisonType">The comparison type to be used.</param>
	/// <returns>A value that indicates the relative order of the objects being compared.</returns>
	/// <exception cref="InvalidOperationException">Throws when one of the grids to be compared is a Sukaku puzzle.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="comparisonType"/> is not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int CompareTo(ref readonly Grid other, GridComparison comparisonType)
		=> (PuzzleType, other.PuzzleType) switch
		{
			(not SudokuType.Sukaku, not SudokuType.Sukaku) => comparisonType switch
			{
				GridComparison.Default => ToString("#").CompareTo(other.ToString("#")),
				GridComparison.IncludingTransforms => this.GetMinLexGrid().ToString("#").CompareTo(other.GetMinLexGrid().ToString("#")),
				_ => throw new ArgumentOutOfRangeException(nameof(comparisonType))
			},
			_ => throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("ComparableGridMustBeStandard"))
		};

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly string ToString() => PuzzleType == SudokuType.Sukaku ? ToString("~") : ToString(default(string));

	/// <inheritdoc/>
	public readonly Digit[] ToArray()
	{
		var result = new Digit[CellsCount];
		for (var i = 0; i < CellsCount; i++)
		{
			// -1..8 -> 0..9
			result[i] = GetDigit(i) + 1;
		}
		return result;
	}

	/// <summary>
	/// Serializes this instance to an array, where all digit value will be stored.
	/// </summary>
	/// <returns>
	/// This array. All elements are the raw masks that between 0 and <see cref="MaxCandidatesMask"/> (i.e. 511).
	/// </returns>
	/// <seealso cref="MaxCandidatesMask"/>
	public readonly Mask[] ToCandidateMaskArray()
	{
		var result = new Mask[CellsCount];
		for (var cell = 0; cell < CellsCount; cell++)
		{
			result[cell] = (Mask)(this[cell] & MaxCandidatesMask);
		}
		return result;
	}

	/// <summary>
	/// Get the candidate mask part of the specified cell.
	/// </summary>
	/// <param name="cell">The cell offset you want to get.</param>
	/// <returns>
	/// <para>
	/// The candidate mask. The return value is a 9-bit <see cref="Mask"/> value, where each bit will be:
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
	public readonly Mask GetCandidates(Cell cell) => (Mask)(this[cell] & MaxCandidatesMask);

	/// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format) => ToString(format, null);

	/// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(IFormatProvider? formatProvider)
		=> (formatProvider ?? new SusserGridFormatInfo()) switch
		{
			GridFormatInfo f => f.FormatGrid(in this),
			CultureInfo c => ToString(c),
			_ => throw new FormatException()
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format, IFormatProvider? formatProvider)
		=> this switch
		{
			{ IsEmpty: true } => $"<{nameof(Empty)}>",
			{ IsUndefined: true } => $"<{nameof(Undefined)}>",
			_ => formatProvider switch
			{
				GridFormatInfo f => f.FormatGrid(in this),
				CultureInfo c => ToString(c),
				not null when formatProvider.GetFormat(typeof(GridFormatInfo)) is GridFormatInfo g => g.FormatGrid(in this),
				_ => GridFormatInfo.Create(format).Unwrap().FormatGrid(in this)
			}
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(CultureInfo? culture)
		=> ToString(culture?.Name.ToLower() switch { ['e', 'n', ..] => "@:", ['z', 'h', ..] => ".", _ => "#" });

	/// <summary>
	/// Get the cell state at the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The cell state.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CellState GetState(Cell cell) => MaskOperations.MaskToCellState(this[cell]);

	/// <summary>
	/// Try to get the digit filled in the specified cell.
	/// </summary>
	/// <param name="cell">The cell used.</param>
	/// <returns>The digit that the current cell filled. If the cell is empty, return -1.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified cell keeps a wrong cell state value. For example, <see cref="CellState.Undefined"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Digit GetDigit(Cell cell)
		=> GetState(cell) switch
		{
			CellState.Empty => -1,
			CellState.Modifiable or CellState.Given => TrailingZeroCount(this[cell]),
			_ => throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("GridInvalidCellState"))
		};

	/// <summary>
	/// Reset the sudoku grid, making all modifiable values to empty ones.
	/// </summary>
	public void Reset()
	{
		if (PuzzleType != SudokuType.Standard)
		{
			// Don't handle if the puzzle type is not a valid standard sudoku puzzle.
			return;
		}

		for (var i = 0; i < CellsCount; i++)
		{
			if (GetState(i) == CellState.Modifiable)
			{
				SetDigit(i, -1); // Reset the cell, and then re-compute all candidates.
			}
		}
	}

	/// <summary>
	/// Reset the sudoku grid, but only making candidates to be reset to the initial state related to the current grid
	/// from given and modifiable values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ResetCandidates()
	{
		if (PuzzleType != SudokuType.Standard)
		{
			// Don't handle if the puzzle type is not a valid standard sudoku puzzle.
			return;
		}

		if (ToString("#") is var p && p.IndexOf(':') is var colonTokenPos and not -1)
		{
			this = Parse(p[..colonTokenPos]);
		}
	}

	/// <summary>
	/// To fix the current grid (all modifiable values will be changed to given ones).
	/// </summary>
	public void Fix()
	{
		if (PuzzleType != SudokuType.Standard)
		{
			// Don't handle if the puzzle type is not a valid standard sudoku puzzle.
			return;
		}

		for (var i = 0; i < CellsCount; i++)
		{
			if (GetState(i) == CellState.Modifiable)
			{
				SetState(i, CellState.Given);
			}
		}
	}

	/// <summary>
	/// To unfix the current grid (all given values will be changed to modifiable ones).
	/// </summary>
	public void Unfix()
	{
		if (PuzzleType != SudokuType.Standard)
		{
			// Don't handle if the puzzle type is not a valid standard sudoku puzzle.
			return;
		}

		for (var i = 0; i < CellsCount; i++)
		{
			if (GetState(i) == CellState.Given)
			{
				SetState(i, CellState.Modifiable);
			}
		}
	}

	/// <summary>
	/// Try to apply the specified conclusion.
	/// </summary>
	/// <param name="conclusion">The conclusion to be applied.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Apply(Conclusion conclusion)
	{
		_ = conclusion is var (type, cell, digit);
		switch (type)
		{
			case Assignment:
			{
				SetDigit(cell, digit);
				break;
			}
			case Elimination:
			{
				SetExistence(cell, digit, false);
				break;
			}
		}
	}

	/// <summary>
	/// Set the specified cell to the specified state.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="state">The state.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void SetState(Cell cell, CellState state)
	{
		ref var mask = ref this[cell];
		var copied = mask;
		mask = (Mask)((Mask)(GetHeaderBits(cell) | (Mask)((int)state << CellCandidatesCount)) | (Mask)(mask & MaxCandidatesMask));
		OnValueChanged(ref this, cell, copied, mask, -1);
	}

	/// <summary>
	/// Set the specified cell to the specified mask.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="mask">The mask to set.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void SetMask(Cell cell, Mask mask)
	{
		ref var newMask = ref this[cell];
		var originalMask = newMask;
		newMask = mask;
		OnValueChanged(ref this, cell, originalMask, newMask, -1);
	}

	/// <summary>
	/// Replace the specified cell with the specified digit.
	/// </summary>
	/// <param name="cell">The cell to be set.</param>
	/// <param name="digit">The digit to be set.</param>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="digit"/> is invalid (e.g. -1).</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ReplaceDigit(Cell cell, Digit digit)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(digit is >= 0 and < 9, true);

		SetDigit(cell, -1);
		SetDigit(cell, digit);
	}

	/// <summary>
	/// Set the specified digit into the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">
	/// <para>
	/// The value you want to set. The value should be between 0 and 8.
	/// If assigning -1, the grid will execute an implicit behavior that candidates in <b>all</b> empty cells will be re-computed.
	/// </para>
	/// <para>
	/// The values set into the grid will be regarded as the modifiable values.
	/// If the cell contains a digit, it will be covered when it is a modifiable value.
	/// If the cell is a given cell, the setter will do nothing.
	/// </para>
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void SetDigit(Cell cell, Digit digit)
	{
		switch (digit)
		{
			case -1 when GetState(cell) == CellState.Modifiable:
			{
				// If 'value' is -1, we should reset the grid.
				// Note that reset candidates may not trigger the event.
				this[cell] = (Mask)(GetHeaderBits(cell) | DefaultMask);

				OnRefreshingCandidates(ref this);
				break;
			}
			case >= 0 and < CellCandidatesCount:
			{
				ref var result = ref this[cell];
				var copied = result;

				// Set cell state to 'CellState.Modifiable'.
				result = (Mask)(GetHeaderBits(cell) | ModifiableMask | 1 << digit);

				// To trigger the event, which is used for eliminate all same candidates in peer cells.
				OnValueChanged(ref this, cell, copied, result, digit);
				break;
			}
		}
	}

	/// <summary>
	/// Sets the target candidate state.
	/// </summary>
	/// <param name="cell">The cell offset between 0 and 80.</param>
	/// <param name="digit">The digit between 0 and 8.</param>
	/// <param name="isOn">
	/// The case you want to set. <see langword="false"/> means that this candidate
	/// doesn't exist in this current sudoku grid; otherwise, <see langword="true"/>.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void SetExistence(Cell cell, Digit digit, bool isOn)
	{
		if (cell is >= 0 and < CellsCount && digit is >= 0 and < CellCandidatesCount)
		{
			var copied = this[cell];
			if (isOn)
			{
				this[cell] |= (Mask)(1 << digit);
			}
			else
			{
				this[cell] &= (Mask)~(1 << digit);
			}

			// To trigger the event.
			OnValueChanged(ref this, cell, copied, this[cell], -1);
		}
	}

	/// <inheritdoc/>
	readonly bool IEquatable<Grid>.Equals(Grid other) => Equals(in other);

	/// <inheritdoc/>
	readonly int IComparable<Grid>.CompareTo(Grid other) => CompareTo(in other);

	/// <inheritdoc/>
	readonly string IJsonSerializable<Grid>.ToJsonString() => JsonSerializer.Serialize(this, DefaultOptions);

	/// <inheritdoc/>
	readonly IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Digit>)this).GetEnumerator();

	/// <inheritdoc/>
	readonly IEnumerable<Candidate> IWhereMethod<Grid, Candidate>.Where(Func<Candidate, bool> predicate)
		=> this.Where(predicate).ToArray();

	/// <inheritdoc/>
	readonly IEnumerator<Digit> IEnumerable<Digit>.GetEnumerator() => ((IEnumerable<Digit>)ToArray()).GetEnumerator();

	/// <inheritdoc/>
	readonly IEnumerable<TResult> ISelectMethod<Grid, Candidate>.Select<TResult>(Func<Candidate, TResult> selector)
		=> this.Select(selector).ToArray();

	/// <summary>
	/// Gets the header 4 bits. The value can be <see cref="SudokuType.Sukaku"/> if and only if the puzzle is Sukaku,
	/// and the argument <paramref name="cell"/> is 0.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The header 4 bits, represented as a <see cref="Mask"/>, left-shifted.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private readonly Mask GetHeaderBits(Cell cell) => (Mask)(this[cell] >> HeaderShift << HeaderShift);

	/// <summary>
	/// Gets the header 4 bits. The value can be <see cref="SudokuType.Sukaku"/> if and only if the puzzle is Sukaku,
	/// and the argument <paramref name="cell"/> is 0.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The header 4 bits, represented as a <see cref="Mask"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private readonly Mask GetHeaderBitsUnshifted(Cell cell) => (Mask)(this[cell] >> HeaderShift);

	/// <summary>
	/// Called by properties <see cref="EmptyCells"/> and <see cref="BivalueCells"/>.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The map.</returns>
	/// <seealso cref="EmptyCells"/>
	/// <seealso cref="BivalueCells"/>
	private readonly unsafe CellMap GetMap(delegate*<ref readonly Grid, Cell, bool> predicate)
	{
		var result = CellMap.Empty;
		for (var cell = 0; cell < CellsCount; cell++)
		{
			if (predicate(in this, cell))
			{
				result.Add(cell);
			}
		}
		return result;
	}

	/// <summary>
	/// Called by properties <see cref="CandidatesMap"/>, <see cref="DigitsMap"/> and <see cref="ValuesMap"/>.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The map indexed by each digit.</returns>
	/// <seealso cref="CandidatesMap"/>
	/// <seealso cref="DigitsMap"/>
	/// <seealso cref="ValuesMap"/>
	private readonly unsafe CellMap[] GetMaps(delegate*<ref readonly Grid, Cell, Digit, bool> predicate)
	{
		var result = new CellMap[CellCandidatesCount];
		for (var digit = 0; digit < CellCandidatesCount; digit++)
		{
			ref var map = ref result[digit];
			for (var cell = 0; cell < CellsCount; cell++)
			{
				if (predicate(in this, cell, digit))
				{
					map.Add(cell);
				}
			}
		}
		return result;
	}

	/// <summary>
	/// Gets a sudoku grid, removing all value digits not appearing in the specified <paramref name="pattern"/>.
	/// </summary>
	/// <param name="pattern">The pattern.</param>
	/// <returns>The result grid.</returns>
	private readonly Grid Preserve(ref readonly CellMap pattern)
	{
		if (PuzzleType != SudokuType.Standard)
		{
			return this;
		}

		var result = this;
		foreach (var cell in ~pattern)
		{
			result.SetDigit(cell, -1);
		}
		return result;
	}

	/// <summary>
	/// Appends for Sukaku puzzle header.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AddSukakuHeader() => this[0] |= SukakuHeader;

	/// <summary>
	/// Removes for Sukaku puzzle header.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void RemoveSukakuHeader() => this[0] &= (1 << HeaderShift) - 1;


	/// <inheritdoc/>
	public static bool TryParse(string str, out Grid result)
	{
		try
		{
			result = Parse(str);
			return !result.IsUndefined;
		}
		catch (FormatException)
		{
			result = Undefined;
			return false;
		}
	}

	/// <inheritdoc/>
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Grid result)
	{
		try
		{
			if (s is null)
			{
				result = Undefined;
				return false;
			}

			result = Parse(s, provider);
			return !result.IsUndefined;
		}
		catch (FormatException)
		{
			result = Undefined;
			return false;
		}
	}

	/// <summary>
	/// Creates a <see cref="Grid"/> instance using the specified token of length 54.
	/// </summary>
	/// <param name="token">Indicates the token.</param>
	/// <returns>A <see cref="Grid"/> result.</returns>
	/// <exception cref="FormatException">Throws when the length of the argument mismatched.</exception>
	public static Grid Create(string token)
		=> token.Length switch
		{
			54 => Parse(
				string.Concat(
					from i in Digits
					let segment = GetDigitViaToken(token[(i * 6)..((i + 1) * 6)]).ToString()
					select segment.PadLeft(9, '0')
				)
			),
			_ => throw new FormatException(string.Format(ResourceDictionary.ExceptionMessage("LengthMustBeMatched"), 54))
		};

	/// <summary>
	/// Creates a <see cref="Grid"/> instance using grid values.
	/// </summary>
	/// <param name="gridValues">The array of grid values.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Create(Digit[] gridValues, GridCreatingOption creatingOption = 0) => new(in gridValues[0], creatingOption);

	/// <summary>
	/// Creates a <see cref="Grid"/> instance with the specified mask array.
	/// </summary>
	/// <param name="masks">The masks.</param>
	/// <exception cref="ArgumentException">Throws when <see cref="Array.Length"/> is out of valid range.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Create(Mask[] masks) => checked((Grid)masks);

	/// <summary>
	/// Creates a <see cref="Grid"/> instance via the array of cell digits
	/// of type <see cref="ReadOnlySpan{T}"/> of <see cref="Digit"/>.
	/// </summary>
	/// <param name="gridValues">The list of cell digits.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Create(ReadOnlySpan<Digit> gridValues, GridCreatingOption creatingOption = 0)
		=> new(in gridValues[0], creatingOption);

	/// <inheritdoc cref="IParsable{TSelf}.Parse(string, IFormatProvider)"/>
	public static Grid Parse(string str)
	{
		var parsers = (GridFormatInfo[])[
			new MultipleLineGridFormatInfo(),
			new MultipleLineGridFormatInfo { RemoveGridLines = true },
			new PencilmarkGridFormatInfo(),
			new SusserGridFormatInfo(),
			new SusserGridFormatInfo { ShortenSusser = true },
			new CsvGridFormatInfo(),
			new OpenSudokuGridFormatInfo(),
			new SukakuGridFormatInfo(),
			new SukakuGridFormatInfo { Multiline = true }
		];


		// The core branches on parsing grids. Here we may leave a bug that we cannot determine if a puzzle is a Sukaku.
		var grid = Undefined;
		switch (str.Length, str.Contains("-+-"), str.Contains('\t'))
		{
			case (729, _, _) when parseAsSukaku(str, out var g): return g;
			case (_, false, true) when parseAsExcel(str, out var g): return g;
			case (_, true, _) when parseMultipleLines(str, out var g): grid = g; break;
			case var _ when parseAll(str, out var g): grid = g; break;
		}
		if (grid.IsUndefined)
		{
			return Undefined;
		}

		// Here need an extra check. Sukaku puzzles can be output as a normal pencil-mark grid format.
		// We should check whether the puzzle is a Sukaku in fact or not.
		// This is a bug fix for type 'PencilmarkingGridParser', which cannot determine whether a puzzle is a Sukaku.
		// I define that a Sukaku must contain 0 given cells, meaning all values should be candidates or modifiable values.
		// If so, we should treat it as a Sukaku instead of a standard sudoku puzzle.
		if (grid.GivensCount < 17)
		{
			reduceGivenCells(ref grid);
			grid.AddSukakuHeader();
		}
		return grid;


		static void reduceGivenCells(ref Grid grid)
		{
			foreach (ref var mask in grid)
			{
				if (MaskOperations.MaskToCellState(mask) != CellState.Empty)
				{
					mask = (Mask)((Mask)CellState.Empty << CellCandidatesCount | mask & MaxCandidatesMask);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool parseAsSukaku(string str, out Grid result)
		{
			if (new SukakuGridFormatInfo().ParseGrid(str) is { IsUndefined: false } g)
			{
				g.AddSukakuHeader();
				result = g;
				return true;
			}

			result = Undefined;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool parseAsExcel(string str, out Grid result)
		{
			if (new CsvGridFormatInfo().ParseGrid(str) is { IsUndefined: false } g)
			{
				result = g;
				return true;
			}

			result = Undefined;
			return false;
		}

		bool parseMultipleLines(string str, out Grid result)
		{
			foreach (var parser in parsers[..3])
			{
				if (parser.ParseGrid(str) is { IsUndefined: false } g)
				{
					result = g;
					return true;
				}
			}

			result = Undefined;
			return false;
		}

		bool parseAll(string str, out Grid result)
		{
			for (var trial = 0; trial < parsers.Length; trial++)
			{
				var currentParser = parsers[trial];
				if (currentParser.ParseGrid(str) is { IsUndefined: false } g)
				{
					result = g;
					return true;
				}
			}

			result = Undefined;
			return false;
		}
	}

#pragma warning disable format
	/// <inheritdoc/>
	public static Grid Parse(string s, IFormatProvider? provider)
		=> provider switch
		{
			GridFormatInfo g => g.ParseGrid(s),
			CultureInfo { Name: var n } => n.ToLower() switch
			{
				['e', 'n', ..] => new PencilmarkGridFormatInfo().ParseGrid(s),
				['z', 'h', ..] => new SusserGridFormatInfo().ParseGrid(s),
				_ => Parse(s)
			},
			_ => Parse(s)
		};
#pragma warning restore format

	/// <summary>
	/// Parses a string value and converts to this type.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <returns>The result instance had converted.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(ReadOnlySpan<char> str) => Parse(str.ToString());

	/// <summary>
	/// Get digit via token.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <returns>The result digit.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int GetDigitViaToken(string s)
		=> (Base32CharSpan.IndexOf(s[0]) << 25) + (Base32CharSpan.IndexOf(s[1]) << 20) + (Base32CharSpan.IndexOf(s[2]) << 15)
		+ (Base32CharSpan.IndexOf(s[3]) << 10) + (Base32CharSpan.IndexOf(s[4]) << 5) + Base32CharSpan.IndexOf(s[5]);

	/// <inheritdoc/>
	static Grid IJsonSerializable<Grid>.FromJsonString(string jsonString) => JsonSerializer.Deserialize<Grid>(jsonString, DefaultOptions);

	/// <summary>
	/// Event handler on value changed.
	/// </summary>
	/// <param name="this">The grid itself.</param>
	/// <param name="cell">Indicates the cell changed.</param>
	/// <param name="oldMask">Indicates the original mask representing the original digits in that cell.</param>
	/// <param name="newMask">Indicates the mask representing the digits updated.</param>
	/// <param name="setValue">
	/// Indicates the set value. If to clear the cell, the value will be -1.
	/// In fact, if the value is -1, this method will do nothing.
	/// </param>
	private static void OnValueChanged(ref Grid @this, Cell cell, Mask oldMask, Mask newMask, Digit setValue)
	{
		if (setValue == -1)
		{
			// This method will do nothing if 'setValue' is -1.
			return;
		}

		foreach (var peerCell in PeersMap[cell])
		{
			if (@this.GetState(peerCell) == CellState.Empty)
			{
				@this[peerCell] &= (Mask)~(1 << setValue);
			}
		}
	}

	/// <summary>
	/// Event handler on refreshing candidates.
	/// </summary>
	/// <param name="this">The grid itself.</param>
	private static void OnRefreshingCandidates(ref Grid @this)
	{
		for (var cell = 0; cell < CellsCount; cell++)
		{
			if (@this.GetState(cell) == CellState.Empty)
			{
				// Remove all appeared digits.
				var mask = MaxCandidatesMask;
				foreach (var currentCell in PeersMap[cell])
				{
					if (@this.GetDigit(currentCell) is var digit and not -1)
					{
						mask &= (Mask)~(1 << digit);
					}
				}
				@this[cell] = (Mask)((Mask)(@this.GetHeaderBits(cell) | EmptyMask) | mask);
			}
		}
	}

	/// <summary>
	/// Returns a <see cref="Grid"/> instance via the raw mask values.
	/// </summary>
	/// <param name="rawMaskValues">
	/// <para>The raw mask values.</para>
	/// <para>
	/// This value can contain 1 or 81 elements.
	/// If the array contain 1 element, all elements in the target sudoku grid will be initialized by it, the uniform value;
	/// if the array contain 81 elements, elements will be initialized by the array one by one using the array elements respectively.
	/// </para>
	/// </param>
	/// <returns>A <see cref="Grid"/> result.</returns>
	/// <remarks><b><i>
	/// This creation ignores header bits. Please don't use this method in the puzzle creation.
	/// </i></b></remarks>
	private static Grid Create(ReadOnlySpan<Mask> rawMaskValues)
	{
		switch (rawMaskValues.Length)
		{
			case 0:
			{
				return Undefined;
			}
			case 1:
			{
				var result = Undefined;
				var uniformValue = rawMaskValues[0];
				for (var cell = 0; cell < CellsCount; cell++)
				{
					result[cell] = uniformValue;
				}
				return result;
			}
			case CellsCount:
			{
				var result = Undefined;
				for (var cell = 0; cell < CellsCount; cell++)
				{
					result[cell] = rawMaskValues[cell];
				}
				return result;
			}
			default:
			{
				throw new InvalidOperationException($"The argument '{nameof(rawMaskValues)}' must contain {CellsCount} elements.");
			}
		}
	}


	/// <summary>
	/// Converts the specified array elements into the target <see cref="Grid"/> instance, without any value boundary checking.
	/// </summary>
	/// <param name="maskArray">An array of the target mask. The array must be of a valid length.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Grid(Mask[] maskArray)
	{
		var result = Empty;
		Unsafe.CopyBlock(
			ref @ref.ByteRef(ref result[0]),
			in @ref.ReadOnlyByteRef(in maskArray[0]),
			(uint)(sizeof(Mask) * maskArray.Length)
		);
		return result;
	}

	/// <summary>
	/// Converts the specified array elements into the target <see cref="Grid"/> instance, with value boundary checking.
	/// </summary>
	/// <param name="maskArray">
	/// <inheritdoc cref="op_Explicit(Mask[])" path="/param[@name='maskArray']"/>
	/// </param>
	/// <exception cref="ArgumentException">
	/// Throws when at least one element in the mask array is greater than 0b100__111_111_111 (i.e. 2559) or less than 0.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator checked Grid(Mask[] maskArray)
	{
		static bool maskMatcher(Mask element) => element >> CellCandidatesCount is 0 or 1 or 2 or 4;
		ArgumentOutOfRangeException.ThrowIfNotEqual(maskArray.Length, CellsCount);
		ArgumentOutOfRangeException.ThrowIfNotEqual(Array.TrueForAll(maskArray, maskMatcher), true);

		var result = Empty;
		Unsafe.CopyBlock(ref @ref.ByteRef(ref result[0]), in @ref.ReadOnlyByteRef(in maskArray[0]), sizeof(Mask) * CellsCount);
		return result;
	}
}

/// <summary>
/// Indicates the JSON converter of the current type.
/// </summary>
file sealed class Converter : JsonConverter<Grid>
{
	/// <inheritdoc/>
	public override bool HandleNull => true;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Grid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.GetString() is { } s ? Grid.Parse(s) : Grid.Undefined;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Write(Utf8JsonWriter writer, Grid value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString("#"));
}
