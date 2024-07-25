namespace Sudoku.Concepts.Primitives;

/// <summary>
/// Represents a type that contains properties inside a grid.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
public interface IGridProperties<TSelf> : IGridConstants<TSelf> where TSelf : unmanaged, IGridProperties<TSelf>
{
	/// <summary>
	/// Try to get the symmetry of the puzzle.
	/// </summary>
	public abstract SymmetricType Symmetry { get; }

	/// <summary>
	/// Indicates the total number of given cells.
	/// </summary>
	public abstract Cell GivensCount { get; }

	/// <summary>
	/// Indicates the total number of modifiable cells.
	/// </summary>
	public abstract Cell ModifiablesCount { get; }

	/// <summary>
	/// Indicates the total number of empty cells.
	/// </summary>
	public abstract Cell EmptiesCount { get; }

	/// <summary>
	/// Gets a cell list that only contains the given cells.
	/// </summary>
	public abstract CellMap GivenCells { get; }

	/// <summary>
	/// Gets a cell list that only contains the modifiable cells.
	/// </summary>
	public abstract CellMap ModifiableCells { get; }

	/// <summary>
	/// Indicates a cell list whose corresponding position in this grid is empty.
	/// </summary>
	public abstract CellMap EmptyCells { get; }

	/// <summary>
	/// Indicates the number of total candidates.
	/// </summary>
	public abstract Candidate CandidatesCount { get; }

	/// <summary>
	/// Indicates a cell list whose corresponding position in this grid contain two candidates.
	/// </summary>
	public abstract CellMap BivalueCells { get; }

	/// <summary>
	/// Indicates the map of possible positions of the existence of the candidate value for each digit.
	/// The return value will be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </summary>
	public abstract ReadOnlySpan<CellMap> CandidatesMap { get; }

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
	public abstract ReadOnlySpan<CellMap> DigitsMap { get; }

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
	public abstract ReadOnlySpan<CellMap> ValuesMap { get; }

	/// <summary>
	/// Indicates all possible candidates in the current grid.
	/// </summary>
	public abstract ReadOnlySpan<Candidate> Candidates { get; }

	/// <summary>
	/// Indicates all possible conjugate pairs appeared in this grid.
	/// </summary>
	public abstract ReadOnlySpan<Conjugate> ConjugatePairs { get; }

	/// <summary>
	/// <para>Indicates which houses are empty houses.</para>
	/// <para>An <b>Empty House</b> is a house holding 9 empty cells, i.e. all cells in this house are empty.</para>
	/// <para>
	/// The property returns a <see cref="HouseMask"/> value as a mask that contains all possible house indices.
	/// For example, if the row 5, column 5 and block 5 (1-9) are null houses, the property will return
	/// the result <see cref="HouseMask"/> value, <c>000010000_000010000_000010000</c> as binary.
	/// </para>
	/// </summary>
	public abstract HouseMask EmptyHouses { get; }

	/// <summary>
	/// <para>Indicates which houses are completed, regardless of ways of filling.</para>
	/// <para><inheritdoc cref="EmptyHouses" path="//summary/para[3]"/></para>
	/// </summary>
	public abstract HouseMask CompletedHouses { get; }

	/// <summary>
	/// Gets the grid where all modifiable cells are empty cells (i.e. the initial one).
	/// </summary>
	public abstract TSelf ResetGrid { get; }

	/// <summary>
	/// Indicates the unfixed grid for the current grid, meaning all given digits will be replaced with modifiable ones.
	/// </summary>
	public abstract TSelf UnfixedGrid { get; }

	/// <summary>
	/// Indicates the fixed grid for the current grid, meaning all modifiable digits will be replaced with given ones.
	/// </summary>
	public abstract TSelf FixedGrid { get; }


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
	protected static abstract void OnValueChanged(ref TSelf @this, Cell cell, Mask oldMask, Mask newMask, Digit setValue);

	/// <summary>
	/// Event handler on refreshing candidates.
	/// </summary>
	/// <param name="this">The grid itself.</param>
	protected static abstract void OnRefreshingCandidates(ref TSelf @this);

	/// <summary>
	/// Called by properties <see cref="EmptyCells"/> and <see cref="BivalueCells"/>.
	/// </summary>
	/// <param name="this">The current instance.</param>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The map.</returns>
	/// <seealso cref="EmptyCells"/>
	/// <seealso cref="BivalueCells"/>
	protected static unsafe CellMap GetMap(ref readonly TSelf @this, delegate*<ref readonly TSelf, Cell, bool> predicate)
	{
		var result = CellMap.Empty;
		for (var cell = 0; cell < CellsCount; cell++)
		{
			if (predicate(in @this, cell))
			{
				result.Add(cell);
			}
		}
		return result;
	}

	/// <summary>
	/// Called by properties <see cref="CandidatesMap"/>, <see cref="DigitsMap"/> and <see cref="ValuesMap"/>.
	/// </summary>
	/// <param name="this">The current instance.</param>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The map indexed by each digit.</returns>
	/// <seealso cref="CandidatesMap"/>
	/// <seealso cref="DigitsMap"/>
	/// <seealso cref="ValuesMap"/>
	protected static unsafe CellMap[] GetMaps(ref readonly TSelf @this, delegate*<ref readonly TSelf, Cell, Digit, bool> predicate)
	{
		var result = new CellMap[CellCandidatesCount];
		for (var digit = 0; digit < CellCandidatesCount; digit++)
		{
			ref var map = ref result[digit];
			for (var cell = 0; cell < CellsCount; cell++)
			{
				if (predicate(in @this, cell, digit))
				{
					map.Add(cell);
				}
			}
		}
		return result;
	}
}
