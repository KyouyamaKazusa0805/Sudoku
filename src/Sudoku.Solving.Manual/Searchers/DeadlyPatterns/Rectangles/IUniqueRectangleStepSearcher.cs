namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for unique rectangle steps.
/// </summary>
public interface IUniqueRectangleStepSearcher : IDeadlyPatternStepSearcher
{
	/// <summary>
	/// Indicates whether the UR can be incomplete. In other words,
	/// some of UR candidates can be removed before the pattern forms.
	/// </summary>
	/// <remarks>
	/// For example, the complete pattern is:
	/// <code><![CDATA[
	/// ab  |  ab
	/// ab  |  ab
	/// ]]></code>
	/// This is a complete pattern, and we may remove an <c>ab</c> in a certain corner.
	/// The incomplete pattern may not contain all four <c>ab</c>s in the structure.
	/// </remarks>
	public abstract bool AllowIncompleteUniqueRectangles { get; set; }

	/// <summary>
	/// Indicates whether the searcher can search for extended URs.
	/// </summary>
	/// <remarks>
	/// The basic types are type 1 to type 6, all other types are extended ones.
	/// </remarks>
	public abstract bool SearchForExtendedUniqueRectangles { get; set; }


	/// <summary>
	/// Check preconditions.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is searching for ARs.</param>
	/// <returns>Indicates whether the UR is passed to check.</returns>
	protected internal static sealed bool CheckPreconditions(scoped in Grid grid, int[] urCells, bool arMode)
	{
		byte emptyCountWhenArMode = 0, modifiableCount = 0;
		foreach (int urCell in urCells)
		{
			switch (grid.GetStatus(urCell))
			{
				case CellStatus.Given:
				case CellStatus.Modifiable when !arMode:
				{
					return false;
				}
				case CellStatus.Empty when arMode:
				{
					emptyCountWhenArMode++;
					break;
				}
				case CellStatus.Modifiable:
				{
					modifiableCount++;
					break;
				}
			}
		}

		return modifiableCount != 4 && emptyCountWhenArMode != 4;
	}

	/// <summary>
	/// To determine whether the specified house forms a conjugate pair
	/// of the specified digit, and the cells where they contain the digit
	/// is same as the given map contains.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="map">The map.</param>
	/// <param name="houseIndex">The house index.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected internal static sealed bool IsConjugatePair(int digit, scoped in Cells map, int houseIndex)
		=> (HouseMaps[houseIndex] & CandidatesMap[digit]) == map;

	/// <summary>
	/// Get a cell that can't see each other.
	/// </summary>
	/// <param name="urCells">The UR cells.</param>
	/// <param name="cell">The current cell.</param>
	/// <returns>The diagonal cell.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the specified argument <paramref name="cell"/> is invalid.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected internal static sealed int GetDiagonalCell(int[] urCells, int cell)
		=> cell == urCells[0]
			? urCells[3]
			: cell == urCells[1]
				? urCells[2]
				: cell == urCells[2]
					? urCells[1]
					: urCells[0];

	/// <summary>
	/// Get whether two cells are in a same house.
	/// </summary>
	/// <param name="cell1">The cell 1 to check.</param>
	/// <param name="cell2">The cell 2 to check.</param>
	/// <param name="houseIndex">
	/// The result houses that both two cells lie in. If the cell can't be found, this argument will be 0.
	/// </param>
	/// <returns>
	/// The <see cref="bool"/> value indicating whether the another cell is same house as the current one.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected internal static sealed bool IsSameHouseCell(int cell1, int cell2, out int houseIndex)
	{
		int v = (Cells.Empty + cell1 + cell2).CoveredHouses;
		(bool r, houseIndex) = v != 0 ? (true, v) : (false, 0);
		return r;
	}

	/// <summary>
	/// Get all highlight cells.
	/// </summary>
	/// <param name="urCells">The all UR cells used.</param>
	/// <returns>The list of highlight cells.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected internal static sealed IEnumerable<CellViewNode> GetHighlightCells(int[] urCells)
		=> new CellViewNode[]
		{
			new(DisplayColorKind.Normal, urCells[0]),
			new(DisplayColorKind.Normal, urCells[1]),
			new(DisplayColorKind.Normal, urCells[2]),
			new(DisplayColorKind.Normal, urCells[3])
		};
}
