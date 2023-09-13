#define TARGET_64BIT
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.SourceGeneration;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sudoku.Algorithm.Solving;
using Sudoku.Analytics;
using Sudoku.Concepts.Primitive;
using Sudoku.Rendering;
using Sudoku.Runtime.MaskServices;
using Sudoku.Text.Formatting;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.ConclusionType;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Concepts;

using unsafe CandidatesRefreshingCallbackFunc = delegate*<ref Grid, void>;
using GridImpl = IGrid<Grid, HouseMask, int, Mask, Cell, Digit, Candidate, House, CellMap, Conclusion, Conjugate>;
using unsafe ValueChangedCallbackFunc = delegate*<ref Grid, Cell, Mask, Mask, Digit, void>;

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
[InlineArray(81)]
[CollectionBuilder(typeof(Grid), nameof(Create))]
[LargeStructure]
[Equals]
[ToString]
[EqualityOperators]
public unsafe partial struct Grid : GridImpl
{
	/// <summary>
	/// Indicates the default mask of a cell (an empty cell, with all 9 candidates left).
	/// </summary>
	public const Mask DefaultMask = EmptyMask | MaxCandidatesMask;

	/// <summary>
	/// Indicates the maximum candidate mask that used.
	/// </summary>
	public const Mask MaxCandidatesMask = (1 << 9) - 1;

	/// <summary>
	/// Indicates the empty mask, modifiable mask and given mask.
	/// </summary>
	public const Mask EmptyMask = (Mask)CellState.Empty << 9;

	/// <summary>
	/// Indicates the modifiable mask.
	/// </summary>
	public const Mask ModifiableMask = (Mask)CellState.Modifiable << 9;

	/// <summary>
	/// Indicates the given mask.
	/// </summary>
	public const Mask GivenMask = (Mask)CellState.Given << 9;


	/// <summary>
	/// Indicates the empty grid string.
	/// </summary>
	public static readonly string EmptyString = new('0', 81);

	/// <summary>
	/// Indicates the event triggered when the value is changed.
	/// </summary>
	public static readonly void* ValueChanged = (ValueChangedCallbackFunc)(&OnValueChanged);

	/// <summary>
	/// Indicates the event triggered when should re-compute candidates.
	/// </summary>
	public static readonly void* RefreshingCandidates = (CandidatesRefreshingCallbackFunc)(&OnRefreshingCandidates);

	/// <inheritdoc cref="GridImpl.Empty"/>
	public static readonly Grid Empty = [DefaultMask];

	/// <inheritdoc cref="GridImpl.Undefined"/>
	public static readonly Grid Undefined;

	/// <summary>
	/// Indicates the backing solver.
	/// </summary>
	private static readonly BitwiseSolver BackingSolver = new();


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
	/// Creates a <see cref="Grid"/> instance via the pointer of the first element of the cell digit,
	/// and the creating option.
	/// </summary>
	/// <param name="firstElement">
	/// <para>The reference of the first element.</para>
	/// <para>
	/// <include file='../../global-doc-comments.xml' path='g/csharp7/feature[@name="ref-returns"]/target[@name="in-parameter"]'/>
	/// </para>
	/// </param>
	/// <param name="creatingOption">The creating option.</param>
	/// <remarks>
	/// <include file='../../global-doc-comments.xml' path='g/csharp7/feature[@name="ref-returns"]/target[@name="method"]'/>
	/// </remarks>
	/// <exception cref="ArgumentNullRefException">
	/// Throws when the argument <paramref name="firstElement"/> is <see langword="null"/> reference.
	/// </exception>
	private Grid(scoped in Digit firstElement, GridCreatingOption creatingOption = GridCreatingOption.None)
	{
		ArgumentNullRefException.ThrowIfNullRef(ref AsRef(firstElement));

		// Firstly we should initialize the inner values.
		this = Empty;

		// Then traverse the array (span, pointer or etc.), to get refresh the values.
		var minusOneEnabled = creatingOption == GridCreatingOption.MinusOne;
		for (var i = 0; i < 81; i++)
		{
			var value = AddByteOffset(ref AsRef(firstElement), (nuint)(i * sizeof(Digit)));
			if ((minusOneEnabled ? value - 1 : value) is var realValue and not -1)
			{
				// Calls the indexer to trigger the event (Clear the candidates in peer cells).
				SetDigit(i, realValue);

				// Set the state to 'CellState.Given'.
				SetState(i, CellState.Given);
			}
		}
	}


	/// <inheritdoc/>
	public readonly bool IsSolved
	{
		get
		{
			for (var i = 0; i < 81; i++)
			{
				if (GetState(i) == CellState.Empty)
				{
					return false;
				}
			}

			for (var i = 0; i < 81; i++)
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

	/// <inheritdoc/>
	public readonly bool IsValid => BackingSolver.CheckValidity(ToString());

	/// <inheritdoc/>
	public readonly bool IsMinimal => CheckMinimal(out _);

	/// <inheritdoc/>
	public readonly int CandidatesCount
	{
		get
		{
			var count = 0;
			for (var i = 0; i < 81; i++)
			{
				if (GetState(i) == CellState.Empty)
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GivenCells.Count;
	}

	/// <inheritdoc/>
	public readonly int ModifiablesCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ModifiableCells.Count;
	}

	/// <inheritdoc/>
	public readonly int EmptiesCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => EmptyCells.Count;
	}

	/// <inheritdoc/>
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

	/// <inheritdoc/>
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

	/// <inheritdoc/>
	public readonly CellMap GivenCells => GetMap(&GridCellPredicates.GivenCells);

	/// <inheritdoc/>
	public readonly CellMap ModifiableCells => GetMap(&GridCellPredicates.ModifiableCells);

	/// <inheritdoc/>
	public readonly CellMap EmptyCells => GetMap(&GridCellPredicates.EmptyCells);

	/// <inheritdoc/>
	public readonly CellMap BivalueCells => GetMap(&GridCellPredicates.BivalueCells);

	/// <inheritdoc/>
	public readonly CellMap[] CandidatesMap => GetMaps(&GridCellPredicates.CandidatesMap);

	/// <inheritdoc/>
	public readonly CellMap[] DigitsMap => GetMaps(&GridCellPredicates.DigitsMap);

	/// <inheritdoc/>
	public readonly CellMap[] ValuesMap => GetMaps(&GridCellPredicates.ValuesMap);

	/// <inheritdoc/>
	public readonly Conjugate[] ConjugatePairs
	{
		get
		{
			var conjugatePairs = new List<Conjugate>();
			for (var (digit, candidatesMap) = (0, CandidatesMap); digit < 9; digit++)
			{
				scoped ref readonly var cellsMap = ref candidatesMap[digit];
				foreach (var houseMap in HousesMap)
				{
					if ((houseMap & cellsMap) is { Count: 2 } temp)
					{
						conjugatePairs.Add(new(temp, digit));
					}
				}
			}

			return [.. conjugatePairs];
		}
	}

	/// <inheritdoc/>
	public readonly Grid ResetGrid => Preserve(GivenCells);

	/// <inheritdoc/>
	public readonly Grid UnfixedGrid
	{
		get
		{
			var result = this;
			result.Unfix();

			return result;
		}
	}

	/// <inheritdoc/>
	public readonly Grid FixedGrid
	{
		get
		{
			var result = this;
			result.Fix();

			return result;
		}
	}

	/// <inheritdoc/>
	public readonly Grid SolutionGrid
	{
		get
		{
			return BackingSolver.Solve(this) is { IsUndefined: false } solution ? unfix(solution, GivenCells) : Undefined;


			static Grid unfix(scoped in Grid solution, scoped in CellMap pattern)
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
	readonly int IReadOnlyCollection<Digit>.Count => 81;

	/// <inheritdoc/>
	static Mask GridImpl.DefaultMask => DefaultMask;

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

	/// <inheritdoc/>
	static Grid GridImpl.Empty => Empty;

	/// <inheritdoc/>
	static Grid GridImpl.Undefined => Undefined;


	/// <inheritdoc/>
	public Mask this[scoped in CellMap cells]
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

	/// <inheritdoc/>
	public readonly Mask this[scoped in CellMap cells, bool withValueCells]
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

	/// <inheritdoc/>
	public readonly unsafe Mask this[scoped in CellMap cells, bool withValueCells, GridMaskMergingMethod mergingMethod]
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
				_ => default(delegate*<ref Mask, in Grid, Cell, void>)
			};

			foreach (var cell in cells)
			{
				if (!withValueCells && GetState(cell) == CellState.Empty || withValueCells)
				{
					mergingFunctionPtr(ref result, this, cell);
				}
			}

			return result;


			static void andNot(scoped ref Mask result, scoped in Grid grid, Cell cell) => result &= (Mask)~grid[cell];

			static void and(scoped ref Mask result, scoped in Grid grid, Cell cell) => result &= grid[cell];

			static void or(scoped ref Mask result, scoped in Grid grid, Cell cell) => result |= grid[cell];
		}
	}

	/// <inheritdoc/>
	readonly ref Mask GridImpl.this[Cell cell] => ref AsRef(this[cell]);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(scoped in Grid other)
		=> InternalEqualsByRef(ref AsByteRef(ref AsRef(this[0])), ref AsByteRef(ref AsRef(other[0])), sizeof(Mask) * 81);

	/// <inheritdoc/>
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

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool ExactlyValidate(out Grid solutionIfValid, [NotNullWhen(true)] out bool? sukaku)
	{
		SkipInit(out solutionIfValid);
		if (BackingSolver.CheckValidity(ToString(), out var solution))
		{
			solutionIfValid = Parse(solution);
			sukaku = false;
			return true;
		}

		if (BackingSolver.CheckValidity(ToString("~"), out solution))
		{
			solutionIfValid = Parse(solution);
			sukaku = true;
			return true;
		}
		sukaku = null;
		return false;
	}

	/// <inheritdoc/>
	public readonly bool CheckMinimal(out Candidate firstCandidateMakePuzzleNotMinimal)
	{
		switch (this)
		{
			case { IsValid: false }:
			{
				throw new InvalidOperationException("The puzzle is not unique.");
			}
			case { IsSolved: true, GivenCells.Count: 81 }:
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
						firstCandidateMakePuzzleNotMinimal = cell * 9 + GetDigit(cell);
						return false;
					}
				}

				firstCandidateMakePuzzleNotMinimal = -1;
				return true;
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool GetCandidateIsOn(Cell cell, Digit digit) => (this[cell] >> digit & 1) != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool? Exists(Candidate candidate) => Exists(candidate / 9, candidate % 9);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool? Exists(Cell cell, Digit digit) => GetState(cell) == CellState.Empty ? GetCandidateIsOn(cell, digit) : null;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
		=> this switch { { IsUndefined: true } => 0, { IsEmpty: true } => 1, _ => ToString("#").GetHashCode() };

	/// <inheritdoc/>
	public readonly int LeastTimesOf(Digit digit, HouseMask houses, out HouseMask leastHousesUsed)
	{
		var digitsAppearedInSuchHouses = CandidatesMap[digit];
		for (var size = 1; size <= PopCount((uint)houses); size++)
		{
			foreach (var houseCombination in houses.GetAllSets().GetSubsets(size))
			{
				var houseCells = CellMap.Empty;
				foreach (var house in houseCombination)
				{
					houseCells |= HousesMap[house];
				}

				digitsAppearedInSuchHouses &= houseCells;

				if ((houseCells & digitsAppearedInSuchHouses) != digitsAppearedInSuchHouses)
				{
					// Not fully-covered.
					continue;
				}

				leastHousesUsed = houseCombination.Aggregate(CommonMethods.BitMerger);
				return size;
			}
		}

		return leastHousesUsed = 0;
	}

	/// <inheritdoc/>
	public readonly Digit[] ToArray()
	{
		var result = new Digit[81];
		for (var i = 0; i < 81; i++)
		{
			// -1..8 -> 0..9
			result[i] = GetDigit(i) + 1;
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Mask GetCandidates(Cell cell) => (Mask)(this[cell] & MaxCandidatesMask);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format)
		=> this switch
		{
			{ IsEmpty: true } => $"<{nameof(Empty)}>",
			{ IsUndefined: true } => $"<{nameof(Undefined)}>",
			_ => GridFormatterFactory.GetBuiltInFormatter(format)?.ToString(this) ?? throw new FormatException("The specified format is invalid.")
		};

	/// <summary>
	/// Gets <see cref="string"/> representation of the current grid, using pre-defined grid formatters.
	/// </summary>
	/// <param name="gridFormatter">
	/// The grid formatter instance to format the current grid.
	/// </param>
	/// <returns>The <see cref="string"/> result.</returns>
	/// <remarks>
	/// <para>
	/// The target and supported types are stored in namespace <see cref="Text.Formatting"/>.
	/// If you don't remember the full format strings, you can try this method instead by passing
	/// actual <see cref="IGridFormatter"/> instances.
	/// </para>
	/// <para>
	/// For example, by using Susser formatter <see cref="SusserFormat"/> instances:
	/// <code><![CDATA[
	/// // Suppose the variable is of type 'Grid'.
	/// var grid = ...;
	/// 
	/// // Creates a Susser-based formatter, with placeholder text as '0',
	/// // missing candidates output and modifiable distinction.
	/// var formatter = SusserFormat.Default with
	/// {
	///     Placeholder = '0',
	///     WithCandidates = true,
	///     WithModifiables = true
	/// };
	/// 
	/// // Using this method to get the target string representation.
	/// string targetStr = grid.ToString(formatter);
	/// 
	/// // Output the result.
	/// Console.WriteLine(targetStr);
	/// ]]></code>
	/// </para>
	/// <para>
	/// In some cases we suggest you use this method instead of calling <see cref="ToString(string?)"/>
	/// because you may not remember all possible string formats.
	/// </para>
	/// </remarks>
	/// <seealso cref="Text.Formatting"/>
	/// <seealso cref="IGridFormatter"/>
	/// <seealso cref="SusserFormat"/>
	/// <seealso cref="ToString(string?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(IGridFormatter gridFormatter)
		=> this switch
		{
			{ IsUndefined: true } => $"<{nameof(Undefined)}>",
			{ IsEmpty: true } => $"<{nameof(Empty)}>",
			_ => gridFormatter.ToString(this)
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CellState GetState(Cell cell) => MaskOperations.MaskToCellState(this[cell]);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Digit GetDigit(Cell cell)
		=> GetState(cell) switch
		{
			CellState.Empty => -1,
			CellState.Modifiable or CellState.Given => TrailingZeroCount(this[cell]),
			_ => throw new InvalidOperationException("The grid cannot keep invalid cell state value.")
		};

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	public readonly OneDimensionalArrayEnumerator<Candidate> EnumerateCandidates()
	{
		var candidates = new Candidate[CandidatesCount];
		for (var (cell, i) = (0, 0); cell < 81; cell++)
		{
			if (GetState(cell) == CellState.Empty)
			{
				foreach (var digit in GetCandidates(cell))
				{
					candidates[i++] = cell * 9 + digit;
				}
			}
		}
		return candidates.Enumerate();
	}

	/// <inheritdoc/>
	public readonly ReadOnlySpan<Candidate> Where(Func<Candidate, bool> predicate)
	{
		var (result, i) = (new Candidate[CandidatesCount], 0);
		foreach (var candidate in EnumerateCandidates())
		{
			if (predicate(candidate))
			{
				result[i++] = candidate;
			}
		}
		return result.AsSpan()[..i];
	}

	/// <inheritdoc/>
	public readonly ReadOnlySpan<TResult> Select<TResult>(Func<Candidate, TResult> selector)
	{
		var (result, i) = (new TResult[CandidatesCount], 0);
		foreach (var candidate in EnumerateCandidates())
		{
			result[i++] = selector(candidate);
		}
		return result.AsSpan()[..i];
	}

	/// <inheritdoc/>
	public void Reset()
	{
		for (var i = 0; i < 81; i++)
		{
			if (GetState(i) == CellState.Modifiable)
			{
				SetDigit(i, -1); // Reset the cell, and then re-compute all candidates.
			}
		}
	}

	/// <inheritdoc/>
	public void Fix()
	{
		for (var i = 0; i < 81; i++)
		{
			if (GetState(i) == CellState.Modifiable)
			{
				SetState(i, CellState.Given);
			}
		}
	}

	/// <inheritdoc/>
	public void Unfix()
	{
		for (var i = 0; i < 81; i++)
		{
			if (GetState(i) == CellState.Given)
			{
				SetState(i, CellState.Modifiable);
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Apply(Conclusion conclusion)
	{
		_ = conclusion is { Cell: var cell, Digit: var digit, ConclusionType: var type };
		switch (type)
		{
			case Assignment:
			{
				SetDigit(cell, digit);
				break;
			}
			case Elimination:
			{
				SetCandidateIsOn(cell, digit, false);
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

	/// <inheritdoc/>
	public void ApplyAll(Conclusion[] conclusions)
	{
		foreach (var conclusion in conclusions)
		{
			Apply(conclusion);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetState(Cell cell, CellState state)
	{
		scoped ref var mask = ref this[cell];
		var copied = mask;
		mask = (Mask)((int)state << 9 | mask & MaxCandidatesMask);

		((ValueChangedCallbackFunc)ValueChanged)(ref this, cell, copied, mask, -1);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMask(Cell cell, Mask mask)
	{
		scoped ref var newMask = ref this[cell];
		var originalMask = newMask;
		newMask = mask;

		((ValueChangedCallbackFunc)ValueChanged)(ref this, cell, originalMask, newMask, -1);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetDigit(Cell cell, Digit digit)
	{
		switch (digit)
		{
			case -1 when GetState(cell) == CellState.Modifiable:
			{
				// If 'value' is -1, we should reset the grid.
				// Note that reset candidates may not trigger the event.
				this[cell] = DefaultMask;

				((CandidatesRefreshingCallbackFunc)RefreshingCandidates)(ref this);

				break;
			}
			case >= 0 and < 9:
			{
				scoped ref var result = ref this[cell];
				var copied = result;

				// Set cell state to 'CellState.Modifiable'.
				result = (Mask)(ModifiableMask | 1 << digit);

				// To trigger the event, which is used for eliminate all same candidates in peer cells.
				((ValueChangedCallbackFunc)ValueChanged)(ref this, cell, copied, result, digit);

				break;
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetCandidateIsOn(Cell cell, Digit digit, bool isOn)
	{
		if (cell is >= 0 and < 81 && digit is >= 0 and < 9)
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
			((ValueChangedCallbackFunc)ValueChanged)(ref this, cell, copied, this[cell], -1);
		}
	}


#pragma warning disable CS1584, CS1658
	/// <inheritdoc cref="GridImpl.GetMap(delegate*{in TSelf, int, bool})"/>
#pragma warning restore CS1584, CS1658
	[ExplicitInterfaceImpl(typeof(IGrid<,,,,,,,,,,>))]
	private readonly CellMap GetMap(delegate*<in Grid, Cell, bool> predicate)
	{
		var result = CellMap.Empty;
		for (var cell = 0; cell < 81; cell++)
		{
			if (predicate(this, cell))
			{
				result.Add(cell);
			}
		}

		return result;
	}

#pragma warning disable CS1584, CS1658
	/// <inheritdoc cref="GridImpl.GetMaps(delegate*{in Grid, int, int, bool})"/>
#pragma warning restore CS1584, CS1658
	[ExplicitInterfaceImpl(typeof(IGrid<,,,,,,,,,,>))]
	private readonly CellMap[] GetMaps(delegate*<in Grid, Cell, Digit, bool> predicate)
	{
		var result = new CellMap[9];
		for (var digit = 0; digit < 9; digit++)
		{
			scoped ref var map = ref result[digit];
			for (var cell = 0; cell < 81; cell++)
			{
				if (predicate(this, cell, digit))
				{
					map.Add(cell);
				}
			}
		}

		return result;
	}

	/// <inheritdoc cref="IGrid{TSelf, THouseMask, TConjuagteMask, TMask, TCell, TDigit, TCandidate, THouse, TBitStatusMap, TConclusion, TConjugate}.Preserve(in TBitStatusMap)"/>
	[ExplicitInterfaceImpl(typeof(IGrid<,,,,,,,,,,>))]
	private readonly Grid Preserve(scoped in CellMap pattern)
	{
		var result = this;
		foreach (var cell in ~pattern)
		{
			result.SetDigit(cell, -1);
		}

		return result;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Create(Digit[] gridValues, GridCreatingOption creatingOption = 0) => new(gridValues[0], creatingOption);

	/// <inheritdoc/>
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
				for (var cell = 0; cell < 81; cell++)
				{
					result[cell] = uniformValue;
				}
				return result;
			}
			case 81:
			{
				var result = Undefined;
				for (var cell = 0; cell < 81; cell++)
				{
					result[cell] = rawMaskValues[cell];
				}
				return result;
			}
			default:
			{
				throw new InvalidOperationException($"The argument '{nameof(rawMaskValues)}' must contain 81 elements.");
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Create(scoped ReadOnlySpan<Digit> gridValues, GridCreatingOption creatingOption = 0) => new(gridValues[0], creatingOption);

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

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string str, GridParsingOption gridParsingOption) => new GridParser(str).Parse(gridParsingOption);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(scoped ReadOnlySpan<char> str) => new GridParser(str.ToString()).Parse();

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
	public static bool TryParse(string str, GridParsingOption option, out Grid result)
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
			result = Parse(str, option);
			return true;
		}
		catch (FormatException)
		{
			result = Undefined;
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
	private static bool InternalEqualsByRef(ref byte first, ref byte second, nuint length)
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
				differentBits = loadUshort(ref first);
				differentBits -= loadUshort(ref second);
			}
			if ((length & 1) != 0)
			{
				differentBits |= (uint)AddByteOffset(ref first, offset) - AddByteOffset(ref second, offset);
			}

			result = differentBits == 0;

			goto Result;
		}
#if TARGET_64BIT
		else
		{
			var offset = length - sizeof(uint);
			var differentBits = loadUint(ref first) - loadUint(ref second);
			differentBits |= loadUint2(ref first, offset) - loadUint2(ref second, offset);
			result = differentBits == 0;

			goto Result;
		}
#endif
	Longer:
		// Only check that the ref is the same if buffers are large, and hence its worth avoiding doing unnecessary comparisons.
		if (!AreSame(ref first, ref second))
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
						if (Vector256.LoadUnsafe(ref first, offset) != Vector256.LoadUnsafe(ref second, offset))
						{
							goto NotEqual;
						}

						offset += (nuint)Vector256<byte>.Count;
					} while (lengthToExamine > offset);
				}

				// Do final compare as Vector256<byte>.Count from end rather than start.
				if (Vector256.LoadUnsafe(ref first, lengthToExamine) == Vector256.LoadUnsafe(ref second, lengthToExamine))
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
						if (Vector128.LoadUnsafe(ref first, offset) != Vector128.LoadUnsafe(ref second, offset))
						{
							goto NotEqual;
						}

						offset += (nuint)Vector128<byte>.Count;
					} while (lengthToExamine > offset);
				}

				// Do final compare as Vector128<byte>.Count from end rather than start.
				if (Vector128.LoadUnsafe(ref first, lengthToExamine) == Vector128.LoadUnsafe(ref second, lengthToExamine))
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
					if (loadVector(ref first, offset) != loadVector(ref second, offset))
					{
						goto NotEqual;
					}

					offset += (nuint)Vector<byte>.Count;
				} while (lengthToExamine > offset);
			}

			// Do final compare as Vector<byte>.Count from end rather than start.
			if (loadVector(ref first, lengthToExamine) == loadVector(ref second, lengthToExamine))
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
			var differentBits = loadNuint(ref first) - loadNuint(ref second);
			differentBits |= loadUint2(ref first, offset) - loadUint2(ref second, offset);
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
					if (loadNuint2(ref first, offset) != loadNuint2(ref second, offset))
					{
						goto NotEqual;
					}
					offset += (nuint)sizeof(nuint);
				} while (lengthToExamine > offset);
			}

			// Do final compare as sizeof(nuint) from end rather than start.
			result = loadNuint2(ref first, lengthToExamine) == loadNuint2(ref second, lengthToExamine);
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
		static ushort loadUshort(scoped ref byte start) => ReadUnaligned<ushort>(ref start);

#if TARGET_64BIT
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static uint loadUint(scoped ref byte start) => ReadUnaligned<uint>(ref start);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static uint loadUint2(scoped ref byte start, nuint offset) => ReadUnaligned<uint>(ref AddByteOffset(ref start, offset));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static nuint loadNuint(scoped ref byte start) => ReadUnaligned<nuint>(ref start);
#endif

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static nuint loadNuint2(scoped ref byte start, nuint offset) => ReadUnaligned<nuint>(ref AddByteOffset(ref start, offset));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Vector<byte> loadVector(scoped ref byte start, nuint offset) => ReadUnaligned<Vector<byte>>(ref AddByteOffset(ref start, offset));
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
		for (var i = 0; i < 81; i++)
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


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Grid(Mask[] maskArray)
	{
		var result = Empty;
		CopyBlock(ref AsByteRef(ref result[0]), ref AsByteRef(ref maskArray[0]), (uint)(sizeof(Mask) * maskArray.Length));

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator checked Grid(Mask[] maskArray)
	{
		static bool maskMatcher(Mask element) => element >> 9 is 0 or 1 or 2 or 4;
		ArgumentOutOfRangeException.ThrowIfNotEqual(maskArray.Length, 81);
		ArgumentOutOfRangeException.ThrowIfNotEqual(Array.TrueForAll(maskArray, maskMatcher), true);

		var result = Empty;
		CopyBlock(ref AsByteRef(ref result[0]), ref AsByteRef(ref maskArray[0]), sizeof(Mask) * 81);

		return result;
	}

	/// <inheritdoc/>
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
	/// <exception cref="InvalidOperationException">Throws when the target text is <see langword="null"/>.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Grid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> Grid.Parse(reader.GetString() ?? throw new InvalidOperationException("The target value text cannot be null."));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Write(Utf8JsonWriter writer, Grid value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString(SusserFormat.Full));
}

/// <summary>
/// Represents a list of methods to filter the cells.
/// </summary>
file static class GridCellPredicates
{
	/// <summary>
	/// Determines whether the specified cell in the specified grid is a given cell.
	/// </summary>
	/// <param name="g">The grid.</param>
	/// <param name="cell">The cell to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool GivenCells(scoped in Grid g, Cell cell) => g.GetState(cell) == CellState.Given;

	/// <summary>
	/// Determines whether the specified cell in the specified grid is a modifiable cell.
	/// </summary>
	/// <inheritdoc cref="GivenCells(in Grid, int)"/>
	public static bool ModifiableCells(scoped in Grid g, Cell cell) => g.GetState(cell) == CellState.Modifiable;

	/// <summary>
	/// Determines whether the specified cell in the specified grid is an empty cell.
	/// </summary>
	/// <inheritdoc cref="GivenCells(in Grid, int)"/>
	public static bool EmptyCells(scoped in Grid g, Cell cell) => g.GetState(cell) == CellState.Empty;

	/// <summary>
	/// Determines whether the specified cell in the specified grid is a bi-value cell, which means the cell is an empty cell,
	/// and contains and only contains 2 candidates.
	/// </summary>
	/// <inheritdoc cref="GivenCells(in Grid, int)"/>
	public static bool BivalueCells(scoped in Grid g, Cell cell) => PopCount((uint)g.GetCandidates(cell)) == 2;

	/// <summary>
	/// Checks the existence of the specified digit in the specified cell.
	/// </summary>
	/// <param name="g">The grid.</param>
	/// <param name="cell">The cell to be checked.</param>
	/// <param name="digit">The digit to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool CandidatesMap(scoped in Grid g, Cell cell, Digit digit) => g.Exists(cell, digit) is true;

	/// <summary>
	/// Checks the existence of the specified digit in the specified cell, or whether the cell is a value cell, being filled by the digit.
	/// </summary>
	/// <inheritdoc cref="CandidatesMap(in Grid, int, int)"/>
	public static bool DigitsMap(scoped in Grid g, Cell cell, Digit digit) => (g.GetCandidates(cell) >> digit & 1) != 0;

	/// <summary>
	/// Checks whether the cell is a value cell, being filled by the digit.
	/// </summary>
	/// <inheritdoc cref="CandidatesMap(in Grid, int, int)"/>
	public static bool ValuesMap(scoped in Grid g, Cell cell, Digit digit) => g.GetDigit(cell) == digit;
}
