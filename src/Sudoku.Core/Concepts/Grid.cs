#undef EMPTY_GRID_STRING_CONSTANT
#define IMPL_INTERFACE_MIN_MAX_VALUE
#define IMPL_INTERFACE_FORMATTABLE
#undef SYNC_ROOT_VIA_METHODIMPL
#define SYNC_ROOT_VIA_OBJECT
#undef SYNC_ROOT_VIA_THREAD_LOCAL
#define TARGET_64BIT
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
/// <para>
/// Begin with C# 12, we can use feature "Inline Arrays" to access internal masks as raw values via indexers.
/// For example, You can use <c>grid[cellIndex]</c> to get the raw mask at the index <c>cellIndex</c>, whose value is between 0 and 81
/// (include 0 but not include 81).
/// </para>
/// </remarks>
[JsonConverter(typeof(Converter))]
[DebuggerDisplay($$"""{{{nameof(ToString)}}("#")}""")]
[InlineArray(CellsCount)]
[CollectionBuilder(typeof(Grid), nameof(Create))]
[DebuggerStepThrough]
[LargeStructure]
[Equals]
[ToString]
[EqualityOperators]
public partial struct Grid :
	IEnumerable<Digit>,
	IEquatable<Grid>,
	IEqualityOperators<Grid, Grid, bool>,
#if IMPL_INTERFACE_FORMATTABLE
	IFormattable,
#endif
#if IMPL_INTERFACE_MIN_MAX_VALUE
	IMinMaxValue<Grid>,
#endif
	IReadOnlyCollection<Digit>,
	ISimpleFormattable,
	ISimpleParsable<Grid>
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
	/// Indicates the number of cells of a sudoku grid.
	/// </summary>
	private const int CellsCount = 81;

	/// <summary>
	/// Indicates the number of candidates appeared in a cell.
	/// </summary>
	private const int CellCandidatesCount = 9;


#if !EMPTY_GRID_STRING_CONSTANT
	/// <summary>
	/// Indicates the empty grid string.
	/// </summary>
	public static readonly string EmptyString = new('0', CellsCount);
#endif

	/// <summary>
	/// Indicates the event triggered when the value is changed.
	/// </summary>
	public static readonly unsafe void* ValueChanged = (ValueChangedHandler)(&OnValueChanged);

	/// <summary>
	/// Indicates the event triggered when should re-compute candidates.
	/// </summary>
	public static readonly unsafe void* RefreshingCandidates = (RefreshingCandidatesHandler)(&OnRefreshingCandidates);

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


	/// <summary>
	/// Indicates the internal grid parsers.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	private static readonly IConceptParser<Grid>[] Parsers = [
		new MultipleLineGridParser(),
		new SimpleMultipleLineGridParser(),
		new PencilmarkingGridParser(),
		new SusserGridParser(),
		new SusserGridParser(true),
		new ExcelGridParser(),
		new OpenSudokuGridParser(),
		new SukakuGridParser(),
		new SukakuGridParser(true)
	];


	/// <summary>
	/// Indicates the inner array that stores the masks of the sudoku grid, which stores the in-time sudoku grid inner information.
	/// </summary>
	/// <remarks>
	/// The field uses the mask table of length 81 to indicate the state and all possible candidates
	/// holding for each cell. Each mask uses a <see cref="Mask"/> value, but only uses 11 of 16 bits.
	/// <code>
	/// | 16  15  14  13  12  11  10  9   8   7   6   5   4   3   2   1   0 |
	/// |-------------------|-----------|-----------------------------------|
	/// |   |   |   |   |   | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 |
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
	private Grid(scoped ref readonly Digit firstElement, GridCreatingOption creatingOption = GridCreatingOption.None)
	{
		Ref.ThrowIfNullRef(in firstElement);

		// Firstly we should initialize the inner values.
		this = Empty;

		// Then traverse the array (span, pointer or etc.), to get refresh the values.
		var minusOneEnabled = creatingOption == GridCreatingOption.MinusOne;
		for (var i = 0; i < CellsCount; i++)
		{
			var value = Unsafe.Add(ref Ref.AsMutableRef(in firstElement), i);
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
						foreach (var cell in Peers[i])
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
	public readonly bool IsValid
	{
#if SYNC_ROOT_VIA_METHODIMPL && !SYNC_ROOT_VIA_OBJECT
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.Synchronized)]
		get => Solver.CheckValidity(ToString());
#elif SYNC_ROOT_VIA_OBJECT && !SYNC_ROOT_VIA_METHODIMPL
		get
		{
			lock (PuzzleSolvingSynchronizer)
			{
				return Solver.CheckValidity(ToString());
			}
		}
#elif SYNC_ROOT_VIA_THREAD_LOCAL
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Solver.Value!.CheckValidity(ToString());
#else
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Solver.CheckValidity(ToString());
#endif
	}

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
	/// Indicates the number of total candidates.
	/// </summary>
	public readonly int CandidatesCount
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
			for (var (houseIndex, valueCells) = (0, ~EmptyCells); houseIndex < 27; houseIndex++)
			{
				if (valueCells / houseIndex == 0)
				{
					result |= 1 << houseIndex;
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
				foreach (var cell in HouseCells[houseIndex])
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
			scoped var candidatesMap = CandidatesMap;
			for (var digit = 0; digit < CellCandidatesCount; digit++)
			{
				scoped ref readonly var cellsMap = ref candidatesMap[digit];
				foreach (var houseMap in HousesMap)
				{
					if ((houseMap & cellsMap) is { Count: 2 } temp)
					{
						conjugatePairs.Add(new(in temp, digit));
					}
				}
			}

			return conjugatePairs.ToArray();
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


			static Grid unfix(scoped ref readonly Grid solution, scoped ref readonly CellMap pattern)
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

#if IMPL_INTERFACE_MIN_MAX_VALUE
	/// <summary>
	/// Indicates the minimum possible grid value that the current type can reach.
	/// </summary>
	/// <remarks>
	/// This value is found out via backtracking algorithm. For more information, please visit type <see cref="BacktrackingSolver"/>.
	/// </remarks>
	/// <seealso cref="BacktrackingSolver"/>
	static Grid IMinMaxValue<Grid>.MinValue => (Grid)"123456789456789123789123456214365897365897214897214365531642978642978531978531642";

	/// <summary>
	/// Indicates the maximum possible grid value that the current type can reach.
	/// </summary>
	/// <remarks>
	/// This value is found out via backtracking algorithm. For more information, please visit type <see cref="BacktrackingSolver"/>.
	/// </remarks>
	/// <seealso cref="BacktrackingSolver"/>
	static Grid IMinMaxValue<Grid>.MaxValue => (Grid)"987654321654321987321987654896745213745213896213896745579468132468132579132579468";
#endif


	/// <summary>
	/// Creates a mask of type <see cref="Mask"/> that represents the usages of digits 1 to 9,
	/// ranged in a specified list of cells in the current sudoku grid.
	/// </summary>
	/// <param name="cells">The list of cells to gather the usages on all digits.</param>
	/// <returns>A mask of type <see cref="Mask"/> that represents the usages of digits 1 to 9.</returns>
	public Mask this[scoped ref readonly CellMap cells]
	{
		readonly get
		{
			var result = (Mask)0;
			foreach (var cell in cells)
			{
				result |= this[cell];
			}

			return (Mask)(result & MaxCandidatesMask);
		}

		[SuppressMessage("Style", "IDE0251:Make member 'readonly'", Justification = "<Pending>")]
		set
		{
			foreach (var cell in cells)
			{
				this[cell] = value;
			}
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
	public readonly Mask this[scoped ref readonly CellMap cells, bool withValueCells]
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
	public readonly unsafe Mask this[scoped ref readonly CellMap cells, bool withValueCells, GridMaskMergingMethod mergingMethod]
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
				_ => default(MaskMergingFunc)
			};

			foreach (var cell in cells)
			{
				if (!withValueCells && GetState(cell) == CellState.Empty || withValueCells)
				{
					mergingFunctionPtr(ref result, in this, cell);
				}
			}

			return result;


			static void andNot(scoped ref Mask result, scoped ref readonly Grid grid, Cell cell) => result &= (Mask)~grid[cell];

			static void and(scoped ref Mask result, scoped ref readonly Grid grid, Cell cell) => result &= grid[cell];

			static void or(scoped ref Mask result, scoped ref readonly Grid grid, Cell cell) => result |= grid[cell];
		}
	}


	/// <summary>
	/// Determine whether the specified <see cref="Grid"/> instance hold the same values as the current instance.
	/// </summary>
	/// <param name="other">The instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExplicitInterfaceImpl(typeof(IEquatable<>))]
	public readonly bool Equals(scoped ref readonly Grid other)
		=> InternalEqualsByRef(in Ref.AsReadOnlyByteRef(in this[0]), in Ref.AsReadOnlyByteRef(in other[0]), sizeof(Mask) * CellsCount);

	/// <summary>
	/// Determine whether the digit in the target cell may be duplicated with a certain cell in the peers of the current cell,
	/// if the digit is filled into the cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public readonly bool DuplicateWith(Cell cell, Digit digit)
	{
		foreach (var tempCell in Peers[cell])
		{
			if (GetDigit(tempCell) == digit)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// <para>
	/// Determines whether the current grid is valid, checking on both normal and sukaku cases
	/// and returning a <see cref="bool"/>? value indicating whether the current sudoku grid is valid
	/// only on sukaku case.
	/// </para>
	/// <para>
	/// For more information, please see the introduction about the parameter
	/// <paramref name="sukaku"/>.
	/// </para>
	/// </summary>
	/// <param name="solutionIfValid">
	/// The solution if the puzzle is valid; otherwise, <see cref="Undefined"/>.
	/// </param>
	/// <param name="sukaku">Indicates whether the current mode is sukaku mode.<list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>The puzzle is a sukaku puzzle.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>The puzzle is a normal sudoku puzzle.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The puzzle is invalid.</description>
	/// </item>
	/// </list>
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <seealso cref="Undefined"/>
#if SYNC_ROOT_VIA_METHODIMPL && !SYNC_ROOT_VIA_OBJECT
	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.Synchronized)]
#else
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public readonly bool ExactlyValidate(out Grid solutionIfValid, [NotNullWhen(true)] out bool? sukaku)
	{
		Unsafe.SkipInit(out solutionIfValid);

#if SYNC_ROOT_VIA_OBJECT && !SYNC_ROOT_VIA_METHODIMPL
		lock (PuzzleSolvingSynchronizer)
#endif
		{
			if (Solver
#if SYNC_ROOT_VIA_THREAD_LOCAL
				.Value!
#endif
				.CheckValidity(ToString(), out var solution))
			{
				solutionIfValid = Parse(solution);
				sukaku = false;
				return true;
			}

			if (Solver
#if SYNC_ROOT_VIA_THREAD_LOCAL
				.Value!
#endif
				.CheckValidity(ToString("~"), out solution))
			{
				solutionIfValid = Parse(solution);
				sukaku = true;
				return true;
			}
		}

		sukaku = null;
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
				throw new InvalidOperationException("The puzzle is not unique.");
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

	/// <summary>
	/// Serializes this instance to an array, where all digit value will be stored.
	/// </summary>
	/// <returns>
	/// This array. All elements are between 0 and 9, where 0 means the cell is <see cref="CellState.Empty"/> now.
	/// </returns>
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
	/// This array. All elements are the raw masks that between 0 and 511.
	/// </returns>
	public readonly Mask[] ToCandidateMaskArray()
	{
		var result = new Mask[CellsCount];
		for (var cell = 0; cell < 81; cell++)
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

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format)
		=> this switch
		{
			{ IsEmpty: true } => $"<{nameof(Empty)}>",
			{ IsUndefined: true } => $"<{nameof(Undefined)}>",
			_ => GridFormatterFactory.GetBuiltInConverter(format)?.Converter(in this) ?? throw new FormatException("The specified format is invalid.")
		};

#if IMPL_INTERFACE_FORMATTABLE
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format, IFormatProvider? formatProvider)
		=> formatProvider switch
		{
			null => ToString(format),
			_ => formatProvider.GetFormat(typeof(ICustomFormatter)) switch
			{
				ICustomFormatter formatter => formatter.Format(format, this, formatProvider),
				_ => ToString(format)
			}
		};
#endif

	/// <summary>
	/// Try to convert the current instance into an equivalent <see cref="string"/> representation,
	/// using the specified formatting rule defined in argument <paramref name="converter"/>.
	/// </summary>
	/// <typeparam name="T">The type of the converter instance.</typeparam>
	/// <param name="converter">A converter instance that defines the conversion rule.</param>
	/// <returns>The target <see cref="string"/> representation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString<T>(T converter) where T : IConceptConverter<Grid>
		=> this switch
		{
			{ IsUndefined: true } => $"<{nameof(Undefined)}>",
			{ IsEmpty: true } => $"<{nameof(Empty)}>",
			_ => converter.Converter(in this)
		};

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
			_ => throw new InvalidOperationException("The grid cannot keep invalid cell state value.")
		};

	/// <summary>
	/// Reset the sudoku grid, making all modifiable values to empty ones.
	/// </summary>
	public void Reset()
	{
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
		if (ToString("#") is var p && p.Contains(':'))
		{
			this = Parse(p[..p.IndexOf(':')]);
		}
	}

	/// <summary>
	/// To fix the current grid (all modifiable values will be changed to given ones).
	/// </summary>
	public void Fix()
	{
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
	/// <inheritdoc cref="ApplyAll(Conclusion[])" path="/summary"/>
	/// </summary>
	/// <param name="renderable">The renderable instance providing with conclusions to be applied.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Apply(IRenderable renderable) => ApplyAll(renderable.Conclusions);

	/// <summary>
	/// Try to apply the specified array of conclusions.
	/// </summary>
	/// <param name="conclusions">The conclusions to be applied.</param>
	public void ApplyAll(Conclusion[] conclusions)
	{
		foreach (var conclusion in conclusions)
		{
			Apply(conclusion);
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
		scoped ref var mask = ref this[cell];
		var copied = mask;
		mask = (Mask)((int)state << CellCandidatesCount | mask & MaxCandidatesMask);

		((ValueChangedHandler)ValueChanged)(ref this, cell, copied, mask, -1);
	}

	/// <summary>
	/// Set the specified cell to the specified mask.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="mask">The mask to set.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void SetMask(Cell cell, Mask mask)
	{
		scoped ref var newMask = ref this[cell];
		var originalMask = newMask;
		newMask = mask;

		((ValueChangedHandler)ValueChanged)(ref this, cell, originalMask, newMask, -1);
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
				this[cell] = DefaultMask;

				((RefreshingCandidatesHandler)RefreshingCandidates)(ref this);

				break;
			}
			case >= 0 and < CellCandidatesCount:
			{
				scoped ref var result = ref this[cell];
				var copied = result;

				// Set cell state to 'CellState.Modifiable'.
				result = (Mask)(ModifiableMask | 1 << digit);

				// To trigger the event, which is used for eliminate all same candidates in peer cells.
				((ValueChangedHandler)ValueChanged)(ref this, cell, copied, result, digit);

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
			((ValueChangedHandler)ValueChanged)(ref this, cell, copied, this[cell], -1);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Digit>)this).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator<Digit> IEnumerable<Digit>.GetEnumerator() => ((IEnumerable<Digit>)ToArray()).GetEnumerator();

	/// <summary>
	/// Called by properties <see cref="EmptyCells"/> and <see cref="BivalueCells"/>.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The map.</returns>
	/// <seealso cref="EmptyCells"/>
	/// <seealso cref="BivalueCells"/>
	private readonly unsafe CellMap GetMap(CellPredicateFunc predicate)
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
	private readonly unsafe CellMap[] GetMaps(CellMapPredicateFunc predicate)
	{
		var result = new CellMap[CellCandidatesCount];
		for (var digit = 0; digit < CellCandidatesCount; digit++)
		{
			scoped ref var map = ref result[digit];
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
	private readonly Grid Preserve(scoped ref readonly CellMap pattern)
	{
		var result = this;
		foreach (var cell in ~pattern)
		{
			result.SetDigit(cell, -1);
		}

		return result;
	}


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
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static Grid Create(scoped ReadOnlySpan<Mask> rawMaskValues)
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
	/// Creates a <see cref="Grid"/> instance via the array of cell digits
	/// of type <see cref="ReadOnlySpan{T}"/> of <see cref="Digit"/>.
	/// </summary>
	/// <param name="gridValues">The list of cell digits.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Create(scoped ReadOnlySpan<Digit> gridValues, GridCreatingOption creatingOption = 0)
		=> new(in gridValues[0], creatingOption);

	/// <summary>
	/// <inheritdoc cref="ISimpleParsable{TSelf}.Parse(string)" path="/summary"/>
	/// </summary>
	/// <param name="str"><inheritdoc cref="ISimpleParsable{TSelf}.Parse(string)" path="/param[@name='str']"/></param>
	/// <returns>The <see cref="Grid"/> instance.</returns>
	/// <remarks>
	/// We suggest you use <see cref="op_Explicit(string)"/> to achieve same goal if the passing argument is a constant.
	/// For example:
	/// <code><![CDATA[
	/// var grid1 = (Grid)"123456789456789123789123456214365897365897214897214365531642978642978531978531642";
	/// var grid2 = (Grid)"987654321654321987321987654896745213745213896213896745579468132468132579132579468";
	/// var grid3 = Grid.Parse(stringCode); // 'stringCode' is a string, not null.
	/// ]]></code>
	/// </remarks>
	/// <seealso cref="op_Explicit(string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string str)
	{
		var containsMultilineLimits = str.Contains("-+-");
		var containsTab = str.Contains('\t');
		switch (str.Length, containsMultilineLimits, containsTab)
		{
			case (729, _, _) when new ExcelGridParser().Parser(str) is { IsUndefined: false } grid:
			{
				return grid;
			}
			case (_, true, _):
			{
				foreach (var parser in Parsers[..3])
				{
					if (parser.Parser(str) is { IsUndefined: false } grid)
					{
						return grid;
					}
				}

				break;
			}
			case (_, _, true) when new ExcelGridParser().Parser(str) is { IsUndefined: false } grid:
			{
				return grid;
			}
			default:
			{
				for (var trial = 0; trial < Parsers.Length; trial++)
				{
					if (Parsers[trial].Parser(str) is { IsUndefined: false } grid)
					{
						return grid;
					}
				}

				break;
			}
		}

		return Undefined;
	}

	/// <summary>
	/// <para>Parses a string value and converts to this type.</para>
	/// <para>
	/// If you want to parse a PM grid, we recommend you use the method
	/// <see cref="ParseExact(string, GridParsingOption)"/> instead of this method.
	/// </para>
	/// </summary>
	/// <param name="str">The string.</param>
	/// <returns>The result instance had converted.</returns>
	/// <seealso cref="ParseExact(string, GridParsingOption)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(scoped ReadOnlySpan<char> str) => Parse(str.ToString());

	/// <summary>
	/// Parses a string value and converts to this type, using a specified grid parsing type.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="gridParsingOption">The grid parsing type.</param>
	/// <returns>The result instance had converted.</returns>
	/// <exception cref="FormatException">Throws when the string text cannot be parsed.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid ParseExact(string str, GridParsingOption gridParsingOption)
		=> gridParsingOption switch
		{
			GridParsingOption.Susser => ParseExact(str, new SusserGridParser()),
			GridParsingOption.ShortenSusser => ParseExact(str, new SusserGridParser(true)),
			GridParsingOption.Table => ParseExact(str, new MultipleLineGridParser()),
			GridParsingOption.PencilMarked => ParseExact(str, new PencilmarkingGridParser()),
			GridParsingOption.SimpleTable => ParseExact(str, new SimpleMultipleLineGridParser()),
			GridParsingOption.Sukaku => ParseExact(str, new SukakuGridParser()),
			GridParsingOption.SukakuSingleLine => ParseExact(str, new SukakuGridParser(true)),
			GridParsingOption.Excel => ParseExact(str, new ExcelGridParser()),
			GridParsingOption.OpenSudoku => ParseExact(str, new OpenSudokuGridParser()),
			_ => throw new ArgumentOutOfRangeException(nameof(gridParsingOption))
		} is { IsUndefined: false } result ? result : throw new FormatException("The target instance cannot be parsed.");

	/// <summary>
	/// Parses the specified <see cref="string"/> text and convert into a grid parser instance,
	/// using the specified parsing rule.
	/// </summary>
	/// <typeparam name="T">The type of the parser.</typeparam>
	/// <param name="str">The string text to be parsed.</param>
	/// <param name="parser">The parser instance to be used.</param>
	/// <returns>A valid grid parsed.</returns>
	/// <exception cref="FormatException">Throws when the target grid parser instance cannot parse it.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid ParseExact<T>(string str, T parser) where T : IConceptParser<Grid>
		=> parser.Parser(str) is { IsUndefined: false } result ? result : throw new FormatException("The target instance cannot be parsed.");

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

	/// <summary>
	/// Try to parse a string and converts to this type, and returns a
	/// <see cref="bool"/> value indicating the result of the conversion.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="option">The grid parsing type.</param>
	/// <param name="result">
	/// The result parsed. If the conversion is failed, this argument will be <see cref="Undefined"/>.
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <seealso cref="Undefined"/>
	public static bool TryParse(string str, GridParsingOption option, out Grid result)
	{
		try
		{
			result = ParseExact(str, option);
			return true;
		}
		catch (FormatException)
		{
			result = Undefined;
			return false;
		}
	}

	/// <inheritdoc cref="TryParse(string, out Grid)"/>
	public static bool TryParse(Utf8String str, out Grid result)
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

	/// <inheritdoc cref="TryParse(string, GridParsingOption, out Grid)"/>
	public static bool TryParse(Utf8String str, GridParsingOption option, out Grid result)
	{
		try
		{
			result = ParseExact(str, option);
			return true;
		}
		catch (FormatException)
		{
			result = Undefined;
			return false;
		}
	}

	/// <summary>
	/// Try to parse the specified <see cref="string"/> text and convert into a <see cref="Grid"/> instance,
	/// using the specified parsing rule. If the parsing operation is failed, return <see langword="false"/> to report the failure case.
	/// No exceptions will be thrown.
	/// </summary>
	/// <typeparam name="T">The type of the parser.</typeparam>
	/// <param name="str">The string text to be parsed.</param>
	/// <param name="parser">The parser instance to be used.</param>
	/// <param name="result">A parsed value of type <see cref="Grid"/>.</param>
	/// <returns>Indicates whether the parsing operation is successful.</returns>
	public static bool TryParseExact<T>(string str, T parser, out Grid result) where T : IConceptParser<Grid>
	{
		try
		{
			result = parser.Parser(str);
			return true;
		}
		catch (FormatException)
		{
			result = default;
			return false;
		}
	}

	/// <summary>
	/// Determines whether two sequences are considered equal on respective bits.
	/// </summary>
	/// <param name="first">The first sequence.</param>
	/// <param name="second">The second sequence.</param>
	/// <param name="length">
	/// The total bits of the sequence to be compared. Please note that two sequences
	/// <paramref name="first"/> and <paramref name="second"/> must hold a same length.
	/// </param>
	/// <returns>A <see cref="bool"/> result indicating whether they are considered equal.</returns>
	/// <remarks>
	/// Optimized byte-based <c>SequenceEquals</c>.
	/// The <paramref name="length"/> parameter for this one is declared a <see langword="nuint"/> rather than <see cref="int"/>
	/// as we also use it for types other than <see cref="byte"/> where the length can exceed 2Gb once scaled by <see langword="sizeof"/>(T).
	/// </remarks>
	/// <!--
	/// Licensed to the .NET Foundation under one or more agreements.
	/// The .NET Foundation licenses this file to you under the MIT license.
	/// https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/SpanHelpers.Byte.cs,998a36a55f580ab1
	/// -->
	private static unsafe bool InternalEqualsByRef(scoped ref readonly byte first, scoped ref readonly byte second, nuint length)
	{
		bool result;

		// Use nint for arithmetic to avoid unnecessary 64->32->64 truncations.
		if (length >= (nuint)sizeof(nuint))
		{
			// Conditional jmp forward to favor shorter lengths. (See comment at "Equal:" label)
			// The longer lengths can make back the time due to branch misprediction better than shorter lengths.
			goto Longer;
		}

#if TARGET_64BIT
		// On 32-bit, this will always be true since sizeof(nuint) == 4.
		if (length < sizeof(uint))
#endif
		{
			var differentBits = 0U;
			var offset = length & 2;
			if (offset != 0)
			{
				differentBits = loadUshort(in first);
				differentBits -= loadUshort(in second);
			}
			if ((length & 1) != 0)
			{
				differentBits |= (uint)Unsafe.AddByteOffset(ref Ref.AsMutableRef(in first), offset)
					- Unsafe.AddByteOffset(ref Ref.AsMutableRef(in second), offset);
			}

			result = differentBits == 0;

			goto Result;
		}
#if TARGET_64BIT
		else
		{
			var offset = length - sizeof(uint);
			var differentBits = loadUint(in first) - loadUint(in second);
			differentBits |= loadUint2(in first, offset) - loadUint2(in second, offset);
			result = differentBits == 0;

			goto Result;
		}
#endif
	Longer:
		// Only check that the ref is the same if buffers are large, and hence its worth avoiding doing unnecessary comparisons.
		if (!Ref.MemoryLocationAreSame(in first, in second))
		{
			// C# compiler inverts this test, making the outer goto the conditional jmp.
			goto Vector;
		}

		// This becomes a conditional jmp forward to not favor it.
		goto Equal;

	Result:
		return result;

	Equal:
		// When the sequence is equal; which is the longest execution, we want it to determine that
		// as fast as possible so we do not want the early outs to be "predicted not taken" branches.
		return true;

	Vector:
		if (Vector128.IsHardwareAccelerated)
		{
			if (Vector256.IsHardwareAccelerated && length >= (nuint)Vector256<byte>.Count)
			{
				var offset = (nuint)0;
				var lengthToExamine = length - (nuint)Vector256<byte>.Count;

				// Unsigned, so it shouldn't have overflowed larger than length (rather than negative).
				Debug.Assert(lengthToExamine < length);
				if (lengthToExamine != 0)
				{
					do
					{
						if (Vector256.LoadUnsafe(in first, offset) != Vector256.LoadUnsafe(in second, offset))
						{
							goto NotEqual;
						}

						offset += (nuint)Vector256<byte>.Count;
					} while (lengthToExamine > offset);
				}

				// Do final compare as Vector256<byte>.Count from end rather than start.
				if (Vector256.LoadUnsafe(in first, lengthToExamine) == Vector256.LoadUnsafe(in second, lengthToExamine))
				{
					// C# compiler inverts this test, making the outer goto the conditional jmp.
					goto Equal;
				}

				// This becomes a conditional jmp forward to not favor it.
				goto NotEqual;
			}

			if (length >= (nuint)Vector128<byte>.Count)
			{
				var offset = (nuint)0;
				var lengthToExamine = length - (nuint)Vector128<byte>.Count;

				// Unsigned, so it shouldn't have overflowed larger than length (rather than negative).
				Debug.Assert(lengthToExamine < length);
				if (lengthToExamine != 0)
				{
					do
					{
						if (Vector128.LoadUnsafe(in first, offset) != Vector128.LoadUnsafe(in second, offset))
						{
							goto NotEqual;
						}

						offset += (nuint)Vector128<byte>.Count;
					} while (lengthToExamine > offset);
				}

				// Do final compare as Vector128<byte>.Count from end rather than start.
				if (Vector128.LoadUnsafe(in first, lengthToExamine) == Vector128.LoadUnsafe(in second, lengthToExamine))
				{
					// C# compiler inverts this test, making the outer goto the conditional jmp.
					goto Equal;
				}

				// This becomes a conditional jmp forward to not favor it.
				goto NotEqual;
			}
		}
		else if (Vector.IsHardwareAccelerated && length >= (nuint)Vector<byte>.Count)
		{
			var offset = (nuint)0;
			var lengthToExamine = length - (nuint)Vector<byte>.Count;

			// Unsigned, so it shouldn't have overflowed larger than length (rather than negative).
			Debug.Assert(lengthToExamine < length);
			if (lengthToExamine > 0)
			{
				do
				{
					if (loadVector(in first, offset) != loadVector(in second, offset))
					{
						goto NotEqual;
					}

					offset += (nuint)Vector<byte>.Count;
				} while (lengthToExamine > offset);
			}

			// Do final compare as Vector<byte>.Count from end rather than start.
			if (loadVector(in first, lengthToExamine) == loadVector(in second, lengthToExamine))
			{
				// C# compiler inverts this test, making the outer goto the conditional jmp.
				goto Equal;
			}

			// This becomes a conditional jmp forward to not favor it.
			goto NotEqual;
		}

#if TARGET_64BIT
		if (Vector128.IsHardwareAccelerated)
		{
			Debug.Assert(length <= (nuint)sizeof(nuint) * 2);

			var offset = length - (nuint)sizeof(nuint);
			var differentBits = loadNuint(in first) - loadNuint(in second);
			differentBits |= loadUint2(in first, offset) - loadUint2(in second, offset);
			result = differentBits == 0;
			goto Result;
		}
		else
#endif
		{
			Debug.Assert(length >= (nuint)sizeof(nuint));

			var offset = (nuint)0;
			var lengthToExamine = length - (nuint)sizeof(nuint);
			// Unsigned, so it shouldn't have overflowed larger than length (rather than negative).
			Debug.Assert(lengthToExamine < length);
			if (lengthToExamine > 0)
			{
				do
				{
					// Compare unsigned so not do a sign extend mov on 64 bit.
					if (loadNuint2(in first, offset) != loadNuint2(in second, offset))
					{
						goto NotEqual;
					}
					offset += (nuint)sizeof(nuint);
				} while (lengthToExamine > offset);
			}

			// Do final compare as sizeof(nuint) from end rather than start.
			result = loadNuint2(in first, lengthToExamine) == loadNuint2(in second, lengthToExamine);
			goto Result;
		}

	NotEqual:
		// As there are so many true/false exit points the JIT will coalesce them to one location.
		// We want them at the end so the conditional early exit jmps are all jmp forwards
		// so the branch predictor in a uninitialized state will not take them e.g.
		// - loops are conditional jmps backwards and predicted.
		// - exceptions are conditional forwards jmps and not predicted.
		return false;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ushort loadUshort(scoped ref readonly byte start) => Unsafe.ReadUnaligned<ushort>(in start);

#if TARGET_64BIT
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static uint loadUint(scoped ref readonly byte start) => Unsafe.ReadUnaligned<uint>(in start);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static uint loadUint2(scoped ref readonly byte start, nuint offset)
			=> Unsafe.ReadUnaligned<uint>(ref Unsafe.AddByteOffset(ref Ref.AsMutableRef(in start), offset));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static nuint loadNuint(scoped ref readonly byte start) => Unsafe.ReadUnaligned<nuint>(in start);
#endif

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static nuint loadNuint2(scoped ref readonly byte start, nuint offset)
			=> Unsafe.ReadUnaligned<nuint>(ref Unsafe.AddByteOffset(ref Ref.AsMutableRef(in start), offset));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Vector<byte> loadVector(scoped ref readonly byte start, nuint offset)
			=> Unsafe.ReadUnaligned<Vector<byte>>(ref Unsafe.AddByteOffset(ref Ref.AsMutableRef(in start), offset));
	}

	/// <summary>
	/// The light-weight event handler for <see cref="ValueChanged"/>.
	/// </summary>
	/// <param name="this">The grid itself.</param>
	/// <param name="cell">Indicates the cell changed.</param>
	/// <param name="oldMask">Indicates the original mask representing the original digits in that cell.</param>
	/// <param name="newMask">Indicates the mask representing the digits updated.</param>
	/// <param name="setValue">
	/// Indicates the set value. If to clear the cell, the value will be -1.
	/// In fact, if the value is -1, this method will do nothing.
	/// </param>
	/// <seealso cref="ValueChanged"/>
	private static void OnValueChanged(scoped ref Grid @this, Cell cell, Mask oldMask, Mask newMask, Digit setValue)
	{
		if (setValue != -1)
		{
			foreach (var peerCell in Peers[cell])
			{
				if (@this.GetState(peerCell) == CellState.Empty)
				{
					// You can't do this because of being invoked recursively.
					//@this.SetCandidateIsOn(peerCell, setValue, false);

					@this[peerCell] &= (Mask)~(1 << setValue);
				}
			}
		}
	}

	/// <summary>
	/// The light-weight event handler for <see cref="RefreshingCandidates"/>.
	/// </summary>
	/// <param name="this">The grid itself.</param>
	/// <seealso cref="RefreshingCandidates"/>
	private static void OnRefreshingCandidates(scoped ref Grid @this)
	{
		for (var i = 0; i < CellsCount; i++)
		{
			if (@this.GetState(i) == CellState.Empty)
			{
				// Remove all appeared digits.
				var mask = MaxCandidatesMask;
				foreach (var cell in Peers[i])
				{
					if (@this.GetDigit(cell) is var digit and not -1)
					{
						mask &= (Mask)~(1 << digit);
					}
				}

				@this[i] = (Mask)(EmptyMask | mask);
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
		Unsafe.CopyBlock(ref Ref.AsByteRef(ref result[0]), in Ref.AsReadOnlyByteRef(in maskArray[0]), (uint)(sizeof(Mask) * maskArray.Length));

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
		Unsafe.CopyBlock(ref Ref.AsByteRef(ref result[0]), in Ref.AsReadOnlyByteRef(in maskArray[0]), sizeof(Mask) * CellsCount);

		return result;
	}

	/// <summary>
	/// Implicit cast from <see cref="string"/> code to its equivalent <see cref="Grid"/> instance representation.
	/// </summary>
	/// <param name="gridCode">The grid code.</param>
	/// <remarks>
	/// <para>
	/// This explicit operator has same meaning for method <see cref="Parse(string)"/>. You can also use
	/// <see cref="Parse(string)"/> to get the same result as this operator.
	/// </para>
	/// <para>
	/// If the argument being passed is <see langword="null"/>, this operator will return <see cref="Undefined"/>
	/// as the final result, whose behavior is the only one that is different with method <see cref="Parse(string)"/>.
	/// That method will throw a <see cref="FormatException"/> instance to report the invalid argument being passed.
	/// </para>
	/// </remarks>
	/// <exception cref="FormatException">
	/// See exception thrown cases for method <see cref="ISimpleParsable{TSimpleParseable}.Parse(string)"/>.
	/// </exception>
	/// <seealso cref="Undefined"/>
	/// <seealso cref="Parse(string)"/>
	/// <seealso cref="ISimpleParsable{TSimpleParseable}.Parse(string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Grid([ConstantExpected] string? gridCode) => gridCode is null ? Undefined : Parse(gridCode);
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
	public override Grid Read(scoped ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> Grid.Parse(reader.GetString()!);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Write(Utf8JsonWriter writer, Grid value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString(SusserGridConverter.Full));
}
