using System.SourceGeneration;
using Sudoku.Analytics;

namespace Sudoku.Concepts.Primitive;

/// <summary>
/// Represents a role that executes like a sudoku grid concept.
/// </summary>
/// <typeparam name="TSelf">The type of the implementation.</typeparam>
/// <typeparam name="THouseMask">The type of te house mask.</typeparam>
/// <typeparam name="TConjugateMask">The type of conjugate pair mask.</typeparam>
/// <typeparam name="TMask">The type of the bit mask.</typeparam>
/// <typeparam name="TCell">The type of the cell.</typeparam>
/// <typeparam name="TDigit">The type of the digit.</typeparam>
/// <typeparam name="TCandidate">The type of the candidate.</typeparam>
/// <typeparam name="THouse">The type of the house.</typeparam>
/// <typeparam name="TBitStatusMap">The type of the bit status map.</typeparam>
/// <typeparam name="TConclusion">The type of the conclusion.</typeparam>
/// <typeparam name="TConjugate">The type of the conjugate pair.</typeparam>
public partial interface IGrid<TSelf, THouseMask, TConjugateMask, TMask, TCell, TDigit, TCandidate, THouse, TBitStatusMap, TConclusion, TConjugate> :
	//IEquatable<TSelf>,
	IEqualityOperators<TSelf, TSelf, bool>,
	IMinMaxValue<TSelf>,
	IParsable<TSelf>,
	IReadOnlyCollection<TDigit>,
	ISimpleFormattable,
	ISimpleParsable<TSelf>
	where TSelf : IGrid<TSelf, THouseMask, TConjugateMask, TMask, TCell, TDigit, TCandidate, THouse, TBitStatusMap, TConclusion, TConjugate>
	where THouseMask : unmanaged, IBinaryInteger<THouseMask>
	where TConjugateMask : unmanaged, IBinaryInteger<TConjugateMask>
	where TMask : unmanaged, IBinaryInteger<TMask>
	where TCell : unmanaged, IBinaryInteger<TCell>
	where TDigit : unmanaged, IBinaryInteger<TDigit>
	where TCandidate : unmanaged, IBinaryInteger<TCandidate>
	where THouse : unmanaged, IBinaryInteger<THouse>
	where TBitStatusMap : unmanaged, IBitStatusMap<TBitStatusMap, TCell>
	where TConclusion : IConclusion<TConclusion, TMask>
	where TConjugate : IConjugatePair<TConjugate, THouseMask, TConjugateMask, TCell, TDigit, THouse, TBitStatusMap>
{
	/// <summary>
	/// Indicates the grid has already solved. If the value is <see langword="true"/>, the grid is solved; otherwise, <see langword="false"/>.
	/// </summary>
	public abstract bool IsSolved { get; }

	/// <summary>
	/// Indicates whether the grid is <see cref="Undefined"/>, which means the grid
	/// holds totally same value with <see cref="Undefined"/>.
	/// </summary>
	/// <seealso cref="Undefined"/>
	public virtual bool IsUndefined => (TSelf)this == TSelf.Undefined;

	/// <summary>
	/// Indicates whether the grid is <see cref="Empty"/>, which means the grid
	/// holds totally same value with <see cref="Empty"/>.
	/// </summary>
	/// <seealso cref="Empty"/>
	public virtual bool IsEmpty => (TSelf)this == TSelf.Empty;

	/// <summary>
	/// Indicates whether the puzzle has a unique solution.
	/// </summary>
	public abstract bool IsValid { get; }

	/// <summary>
	/// Determines whether the puzzle is a minimal puzzle, which means the puzzle will become multiple solution
	/// if arbitrary one given digit will be removed from the grid.
	/// </summary>
	public abstract bool IsMinimal { get; }

	/// <summary>
	/// Indicates the number of total candidates.
	/// </summary>
	public abstract int CandidatesCount { get; }

	/// <summary>
	/// Indicates the total number of given cells.
	/// </summary>
	public abstract int GivensCount { get; }

	/// <summary>
	/// Indicates the total number of modifiable cells.
	/// </summary>
	public abstract int ModifiablesCount { get; }

	/// <summary>
	/// Indicates the total number of empty cells.
	/// </summary>
	public abstract int EmptiesCount { get; }

	/// <summary>
	/// <para>Indicates which houses are empty houses.</para>
	/// <para>An <b>Empty House</b> is a house holding 9 empty cells, i.e. all cells in this house are empty.</para>
	/// <para>
	/// The property returns a <typeparamref name="THouseMask"/> value as a mask that contains all possible house indices.
	/// For example, if the row 5, column 5 and block 5 (1-9) are null houses, the property will return
	/// the result <typeparamref name="THouseMask"/> value, <c>000010000_000010000_000010000</c> as binary.
	/// </para>
	/// </summary>
	public abstract THouseMask EmptyHouses { get; }

	/// <summary>
	/// <para>Indicates which houses are completed, regardless of ways of filling.</para>
	/// <para><inheritdoc cref="EmptyHouses" path="//summary/para[3]"/></para>
	/// </summary>
	public abstract THouseMask FullHouses { get; }

	/// <summary>
	/// Gets a cell list that only contains the given cells.
	/// </summary>
	public abstract TBitStatusMap GivenCells { get; }

	/// <summary>
	/// Gets a cell list that only contains the modifiable cells.
	/// </summary>
	public abstract TBitStatusMap ModifiableCells { get; }

	/// <summary>
	/// Indicates a cell list whose corresponding position in this grid is empty.
	/// </summary>
	public abstract TBitStatusMap EmptyCells { get; }

	/// <summary>
	/// Indicates a cell list whose corresponding position in this grid contain two candidates.
	/// </summary>
	public abstract TBitStatusMap BivalueCells { get; }

	/// <summary>
	/// Indicates the map of possible positions of the existence of the candidate value for each digit.
	/// The return value will be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </summary>
	public abstract TBitStatusMap[] CandidatesMap { get; }

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
	public abstract TBitStatusMap[] DigitsMap { get; }

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
	public abstract TBitStatusMap[] ValuesMap { get; }

	/// <summary>
	/// Indicates all possible conjugate pairs appeared in this grid.
	/// </summary>
	public abstract TConjugate[] ConjugatePairs { get; }

	/// <summary>
	/// Gets the grid where all modifiable cells are empty cells (i.e. the initial one).
	/// </summary>
	public abstract TSelf ResetGrid { get; }

	/// <summary>
	/// Indicates the solution of the current grid. If the puzzle has no solution or multiple solutions,
	/// this property will return <see cref="Undefined"/>.
	/// </summary>
	/// <seealso cref="Undefined"/>
	public abstract TSelf SolutionGrid { get; }

	/// <summary>
	/// Indicates the fixed grid for the current grid, meaning all modifiable digits will be replaced with given ones.
	/// </summary>
	public virtual TSelf FixedGrid
	{
		get
		{
			var result = (TSelf)this;
			result.Fix();

			return result;
		}
	}

	/// <summary>
	/// Indicates the unfixed grid for the current grid, meaning all given digits will be replaced with modifiable ones.
	/// </summary>
	public virtual TSelf UnfixedGrid
	{
		get
		{
			var result = (TSelf)this;
			result.Unfix();

			return result;
		}
	}

	/// <summary>
	/// The empty grid that is valid during implementation or running the program (all values are <see cref="DefaultMask"/>, i.e. empty cells).
	/// </summary>
	/// <remarks>
	/// This field is initialized by the static constructor of this structure.
	/// </remarks>
	/// <seealso cref="DefaultMask"/>
	public static abstract TSelf Empty { get; }

	/// <summary>
	/// Indicates the default grid that all values are initialized 0. This value is equivalent to <see langword="default"/>(<typeparamref name="TSelf"/>).
	/// </summary>
	/// <remarks>
	/// This value can be used for non-candidate-based sudoku operations, e.g. a sudoku grid canvas.
	/// </remarks>
	public static virtual TSelf? Undefined => default;

	/// <summary>
	/// Indicates the default mask of a cell (an empty cell, with all 9 candidates left).
	/// </summary>
	public static abstract TMask DefaultMask { get; }


	/// <summary>
	/// Indexer member kind that allows interaction with low-level mask operation.
	/// </summary>
	/// <param name="cell">The index of the cell.</param>
	/// <returns>A reference to the mask at the specified cell offset.</returns>
	public abstract ref TMask this[TCell cell] { get; }

	/// <summary>
	/// Creates a mask of type <typeparamref name="TMask"/> that represents the usages of digits 1 to 9,
	/// ranged in a specified list of cells in the current sudoku grid.
	/// </summary>
	/// <param name="cells">The list of cells to gather the usages on all digits.</param>
	/// <returns>A mask of type <typeparamref name="TMask"/> that represents the usages of digits 1 to 9.</returns>
	public abstract TMask this[scoped in TBitStatusMap cells] { get; set; }

	/// <summary>
	/// <inheritdoc cref="this[in TBitStatusMap]" path="/summary"/>
	/// </summary>
	/// <param name="cells"><inheritdoc cref="this[in TBitStatusMap]" path="/param[@name='cells']"/></param>
	/// <param name="withValueCells">
	/// Indicates whether the value cells (given or modifiable ones) will be included to be gathered.
	/// If <see langword="true"/>, all value cells (no matter what kind of cell) will be summed up.
	/// </param>
	/// <returns><inheritdoc cref="this[in TBitStatusMap]" path="/returns"/></returns>
	public abstract TMask this[scoped in TBitStatusMap cells, bool withValueCells] { get; }

	/// <summary>
	/// <inheritdoc cref="this[in TBitStatusMap]" path="/summary"/>
	/// </summary>
	/// <param name="cells"><inheritdoc cref="this[in TBitStatusMap]" path="/param[@name='cells']"/></param>
	/// <param name="withValueCells">
	/// <inheritdoc cref="this[in TBitStatusMap, bool]" path="/param[@name='withValueCells']"/>
	/// </param>
	/// <param name="mergingMethod">
	/// </param>
	/// <returns><inheritdoc cref="this[in TBitStatusMap]" path="/returns"/></returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when <paramref name="mergingMethod"/> is not defined.</exception>
	public abstract TMask this[scoped in TBitStatusMap cells, bool withValueCells, GridMaskMergingMethod mergingMethod] { get; }


	/// <summary>
	/// Determine whether the specified <typeparamref name="TSelf"/> instance hold the same values as the current instance.
	/// </summary>
	/// <param name="other">The instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[ExplicitInterfaceImpl(typeof(IEquatable<>))]
	public abstract bool Equals(scoped in TSelf other);

	/// <summary>
	/// Determine whether the digit in the target cell may be duplicated with a certain cell in the peers of the current cell,
	/// if the digit is filled into the cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public abstract bool DuplicateWith(TCell cell, TDigit digit);

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
	public abstract bool ExactlyValidate(out TSelf solutionIfValid, [NotNullWhen(true)] out bool? sukaku);

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
	public abstract bool CheckMinimal(out TCandidate firstCandidateMakePuzzleNotMinimal);

	/// <summary>
	/// Sets a candidate existence case with a <see cref="bool"/> value.
	/// </summary>
	/// <param name="cell"><inheritdoc cref="SetCandidateIsOn(TCell, TDigit, bool)" path="/param[@name='cell']"/></param>
	/// <param name="digit"><inheritdoc cref="SetCandidateIsOn(TCell, TDigit, bool)" path="/param[@name='digit']"/></param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	public abstract bool GetCandidateIsOn(TCell cell, TDigit digit);

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
	/// the result case will be more precisely than the indexer <see cref="GetCandidateIsOn(TCell, TDigit)"/>,
	/// which is the main difference between this method and that indexer.
	/// </para>
	/// </remarks>
	/// <seealso cref="GetCandidateIsOn(TCell, TDigit)"/>
	public abstract bool? Exists(TCell cell, TDigit digit);

	/// <summary>
	/// Try to get the minimum times that the specified digit, describing it can be filled with the specified houses.
	/// </summary>
	/// <param name="digit">The digit to be checked.</param>
	/// <param name="houses">The houses that the digit can be filled with.</param>
	/// <param name="leastHousesUsed">
	/// <para>One of all possibilities of the combination of least using for houses appearing.</para>
	/// <para>The number of bits is always same as return value if the return value is not 0.</para>
	/// </param>
	/// <returns>
	/// <para>The number of times that the digit can be filled with the specified houses, at least.</para>
	/// <para>
	/// If any one of the houses from argument <paramref name="houses"/> doesn't contain that digit,
	/// or the digit has already been filled with that house as a value, the value will be 0. No exception will be thrown.
	/// </para>
	/// </returns>
	/// <remarks>
	/// For example, the following diagram shows for a sample grid.
	/// <code><![CDATA[
	///          c2           c4                    c8
	///    b1---------------,---------------,-----------------,
	///    | 5   1     469  | 2    489  7   | 46    3     48  |
	/// r2 | 7   2346  2346 | 48   1    38  | 2456  2458  9   |
	///    | 39  234   8    | 6    349  5   | 7     1     24  |
	///    :----------------+---------------+-----------------:
	/// r4 | 2   67    5    | 478  48   689 | 1     49    3   |
	///    | 39  8     369  | 1    234  26  | 245   7     245 |
	///    | 4   37    1    | 57   235  239 | 8     29    6   |
	///    :----------------+---------------+-----------------:
	///    | 1   25    7    | 9    6    4   | 3     258   258 |
	/// r8 | 6   2345  234  | 58   7    28  | 9     245   1   |
	///    | 8   9     24   | 3    25   1   | 245   6     7   |
	///    '----------------'---------------'-----------------'
	/// ]]></code>
	/// If we check for the digit 4 in houses <c>c248</c>, we can get the result number 3,
	/// meaning we must fill with at least 4 times of the digit 4 into the columns 2, 4 and 8, they are:
	/// <list type="bullet">
	/// <item>Row 2</item>
	/// <item>Row 4</item>
	/// <item>Row 8</item>
	/// <item>Block 1</item>
	/// </list>
	/// Therefore, the method will return 4 as the final answer.
	/// </remarks>
	public abstract int LeastTimesOf(TDigit digit, THouseMask houses, out THouseMask leastHousesUsed);

	/// <summary>
	/// Serializes this instance to an array, where all digit value will be stored.
	/// </summary>
	/// <returns>
	/// This array. All elements are between 0 and 9, where 0 means the cell is <see cref="CellState.Empty"/> now.
	/// </returns>
	public abstract TDigit[] ToArray();

	/// <summary>
	/// Get the candidate mask part of the specified cell.
	/// </summary>
	/// <param name="cell">The cell offset you want to get.</param>
	/// <returns>
	/// <para>
	/// The candidate mask. The return value is a 9-bit <typeparamref name="TMask"/> value, where each bit will be:
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
	public abstract TMask GetCandidates(TCell cell);

	/// <summary>
	/// Get the cell state at the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The cell state.</returns>
	public abstract CellState GetState(TCell cell);

	/// <summary>
	/// Try to get the digit filled in the specified cell.
	/// </summary>
	/// <param name="cell">The cell used.</param>
	/// <returns>The digit that the current cell filled. If the cell is empty, return -1.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified cell keeps a wrong cell state value. For example, <see cref="CellState.Undefined"/>.
	/// </exception>
	public abstract TDigit GetDigit(TCell cell);

	/// <summary>
	/// Filters the candidates that satisfies the specified condition.
	/// </summary>
	/// <param name="predicate">The condition to filter candidates.</param>
	/// <returns>All candidates satisfied the specified condition.</returns>
	public abstract ReadOnlySpan<Candidate> Where(Func<Candidate, bool> predicate);

	/// <summary>
	/// Projects each element of a sequence into a new form.
	/// </summary>
	/// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <returns>
	/// An array of <typeparamref name="TResult"/> elements converted.
	/// </returns>
	public abstract ReadOnlySpan<TResult> Select<TResult>(Func<TCandidate, TResult> selector);

	/// <summary>
	/// Reset the sudoku grid, to set all modifiable values to empty ones.
	/// </summary>
	public abstract void Reset();

	/// <summary>
	/// To fix the current grid (all modifiable values will be changed to given ones).
	/// </summary>
	public abstract void Fix();

	/// <summary>
	/// To unfix the current grid (all given values will be changed to modifiable ones).
	/// </summary>
	public abstract void Unfix();

	/// <summary>
	/// Try to apply the specified conclusion.
	/// </summary>
	/// <param name="conclusion">The conclusion to be applied.</param>
	public abstract void Apply(TConclusion conclusion);

	/// <summary>
	/// Try to apply the specified array of conclusions.
	/// </summary>
	/// <param name="conclusions">The conclusions to be applied.</param>
	public virtual void Apply(TConclusion[] conclusions)
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
	public abstract void SetState(TCell cell, CellState state);

	/// <summary>
	/// Set the specified cell to the specified mask.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="mask">The mask to set.</param>
	public abstract void SetMask(TCell cell, TMask mask);

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
	public abstract void SetDigit(TCell cell, TDigit digit);

	/// <summary>
	/// Sets the target candidate state.
	/// </summary>
	/// <param name="cell">The cell offset between 0 and 80.</param>
	/// <param name="digit">The digit between 0 and 8.</param>
	/// <param name="isOn">
	/// The case you want to set. <see langword="false"/> means that this candidate
	/// doesn't exist in this current sudoku grid; otherwise, <see langword="true"/>.
	/// </param>
	public abstract void SetCandidateIsOn(TCell cell, TDigit digit, bool isOn);

	/// <summary>
	/// Called by properties <see cref="EmptyCells"/> and <see cref="BivalueCells"/>.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The map.</returns>
	/// <seealso cref="EmptyCells"/>
	/// <seealso cref="BivalueCells"/>
	protected virtual unsafe TBitStatusMap GetMap(delegate*<in TSelf, TCell, bool> predicate)
	{
		var result = TBitStatusMap.Empty;
		for (var (cell, i) = (TCell.Zero, 0); i < 81; cell++, i++)
		{
			if (predicate((TSelf)this, cell))
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
	protected virtual unsafe TBitStatusMap[] GetMaps(delegate*<in TSelf, TCell, TDigit, bool> predicate)
	{
		var result = new TBitStatusMap[9];
		for (var (digit, i) = (TDigit.Zero, 0); i < 9; digit++, i++)
		{
			scoped ref var map = ref result[i];
			for (var (cell, j) = (TCell.Zero, 0); j < 81; cell++, j++)
			{
				if (predicate((TSelf)this, cell, digit))
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
	protected virtual TSelf Preserve(scoped in TBitStatusMap pattern)
	{
		var result = (TSelf)this;
		foreach (var cell in ~pattern)
		{
			result.SetDigit(cell, -TDigit.One);
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => ToArray().GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<TDigit> IEnumerable<TDigit>.GetEnumerator() => ((IEnumerable<TDigit>)ToArray()).GetEnumerator();

	/// <summary>
	/// Creates a <typeparamref name="TSelf"/> instance using grid values.
	/// </summary>
	/// <param name="gridValues">The array of grid values.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	public static abstract TSelf Create(TDigit[] gridValues, GridCreatingOption creatingOption = 0);

	/// <summary>
	/// Creates a <typeparamref name="TSelf"/> instance with the specified mask array.
	/// </summary>
	/// <param name="masks">The masks.</param>
	/// <exception cref="ArgumentException">Throws when <see cref="Array.Length"/> is out of valid range.</exception>
	public static abstract TSelf Create(TMask[] masks);

	/// <summary>
	/// Creates a <typeparamref name="TSelf"/> instance via the array of cell digits
	/// of type <see cref="ReadOnlySpan{T}"/> of <typeparamref name="TDigit"/>.
	/// </summary>
	/// <param name="gridValues">The list of cell digits.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	public static abstract TSelf Create(scoped ReadOnlySpan<TDigit> gridValues, GridCreatingOption creatingOption = 0);

	/// <summary>
	/// <inheritdoc cref="ISimpleParsable{TSelf}.Parse(string)" path="/summary"/>
	/// </summary>
	/// <param name="str"><inheritdoc cref="ISimpleParsable{TSelf}.Parse(string)" path="/param[@name='str']"/></param>
	/// <returns>The <typeparamref name="TSelf"/> instance.</returns>
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
	public static new abstract TSelf Parse(string str);

	/// <summary>
	/// Parses a string value and converts to this type, using a specified grid parsing type.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="gridParsingOption">The grid parsing type.</param>
	/// <returns>The result instance had converted.</returns>
	public static abstract TSelf Parse(string str, GridParsingOption gridParsingOption);

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
	public static abstract TSelf Parse(scoped ReadOnlySpan<char> str);

	/// <inheritdoc cref="ISimpleParsable{TSelf}.TryParse(string, out TSelf)"/>
	public static new virtual bool TryParse(string str, [NotNullWhen(true)] out TSelf? result)
	{
		try
		{
			result = TSelf.Parse(str);
			return !result.IsUndefined;
		}
		catch (FormatException)
		{
			result = TSelf.Undefined;
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
	public static abstract bool TryParse(string str, GridParsingOption option, out TSelf result);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static TSelf IParsable<TSelf>.Parse(string s, IFormatProvider? provider) => TSelf.Parse(s);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IParsable<TSelf>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [NotNullWhen(true)] out TSelf? result)
	{
		result = TSelf.Undefined;
		return s is not null && TSelf.TryParse(s, out result);
	}

	/// <summary>
	/// Converts the specified array elements into the target <typeparamref name="TSelf"/> instance, without any value boundary checking.
	/// </summary>
	/// <param name="maskArray">An array of the target mask. The array must be of a valid length.</param>
	public static abstract explicit operator TSelf(TMask[] maskArray);

	/// <summary>
	/// Converts the specified array elements into the target <typeparamref name="TSelf"/> instance, with value boundary checking.
	/// </summary>
	/// <param name="maskArray">
	/// <inheritdoc cref="op_Explicit(TMask[])" path="/param[@name='maskArray']"/>
	/// </param>
	/// <exception cref="ArgumentException">
	/// Throws when at least one element in the mask array is greater than 0b100__111_111_111 (i.e. 2559) or less than 0.
	/// </exception>
	public static abstract explicit operator checked TSelf(TMask[] maskArray);

	/// <summary>
	/// Implicit cast from <see cref="string"/> code to its equivalent <typeparamref name="TSelf"/> instance representation.
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

	public static abstract explicit operator TSelf([ConstantExpected] string? gridCode);
}
