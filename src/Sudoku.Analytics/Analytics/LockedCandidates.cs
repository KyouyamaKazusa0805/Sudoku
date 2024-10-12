namespace Sudoku.Analytics;

/// <summary>
/// Provides a way to calculate whether a pattern is a locked candidates or not.
/// </summary>
public static class LockedCandidates
{
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
	public static bool IsLockedCandidates(
		ref readonly Grid grid,
		ref readonly CellMap a,
		ref readonly CellMap b,
		ref readonly CellMap c,
		ref readonly CellMap emptyCells,
		out Mask digitsMask
	)
	{
		if (!(emptyCells & c))
		{
			digitsMask = 0;
			return false;
		}

		var (maskA, maskB, maskC) = (grid[in a], grid[in b], grid[in c]);
		return (digitsMask = (Mask)(maskC & (maskA ^ maskB))) != 0;
	}
}
