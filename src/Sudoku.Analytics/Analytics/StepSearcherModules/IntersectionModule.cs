namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents an intersection module.
/// </summary>
internal static class IntersectionModule
{
	/// <summary>
	/// Try to create a list of <see cref="CellViewNode"/>s indicating the crosshatching base cells.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="house">The house.</param>
	/// <param name="cells">The cells.</param>
	/// <returns>A list of <see cref="CellViewNode"/> instances.</returns>
	public static ReadOnlySpan<CellViewNode> GetCrosshatchBaseCells(
		scoped ref readonly Grid grid,
		Digit digit,
		House house,
		scoped ref readonly CellMap cells
	)
	{
		var info = Crosshatching.GetCrosshatchingInfo(in grid, digit, house, in cells);
		if (info is not var (combination, emptyCellsShouldBeCovered, emptyCellsNotNeedToBeCovered))
		{
			return [];
		}

		var result = new List<CellViewNode>();
		foreach (var c in combination)
		{
			result.Add(new(ColorIdentifier.Normal, c) { RenderingMode = DirectModeOnly });
		}
		foreach (var c in emptyCellsShouldBeCovered)
		{
			var p = emptyCellsNotNeedToBeCovered.Contains(c) ? ColorIdentifier.Auxiliary2 : ColorIdentifier.Auxiliary1;
			result.Add(new(p, c) { RenderingMode = DirectModeOnly });
		}

		return result.AsReadOnlySpan();
	}

	/// <summary>
	/// Indicates whether the pattern is a locked candidates.
	/// </summary>
	/// <param name="grid">Indicates the grid used.</param>
	/// <param name="a">Indicates the 6 cells in the line, but not intersects with block.</param>
	/// <param name="b">Indicates the 6 cells in the block, but not intersects with line.</param>
	/// <param name="c">Indicates the intersection 3 cells.</param>
	/// <param name="emptyCells">The empty cells in the pattern.</param>
	/// <param name="digitsMask">Indicates the mask of digits that are locked candidates in the pattern.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsLockedCandidates(
		scoped ref readonly Grid grid,
		scoped ref readonly CellMap a,
		scoped ref readonly CellMap b,
		scoped ref readonly CellMap c,
		scoped ref readonly CellMap emptyCells,
		out Mask digitsMask
	)
		=> (digitsMask = 0, emptyCells & c, grid[in a], grid[in b], grid[in c]) is (_, not [], var maskA, var maskB, var maskC)
		&& (digitsMask = (Mask)(maskC & (maskA ^ maskB))) != 0;
}
