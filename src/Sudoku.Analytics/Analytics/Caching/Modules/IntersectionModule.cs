namespace Sudoku.Analytics.Caching.Modules;

/// <summary>
/// Represents an intersection module.
/// </summary>
internal static class IntersectionModule
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
	internal static bool IsLockedCandidates(
		ref readonly Grid grid,
		ref readonly CellMap a,
		ref readonly CellMap b,
		ref readonly CellMap c,
		ref readonly CellMap emptyCells,
		out Mask digitsMask
	)
		=> (digitsMask = 0, emptyCells & c, (grid[in a], grid[in b], grid[in c])) is (_, not [], var (maskA, maskB, maskC))
		&& (digitsMask = (Mask)(maskC & (maskA ^ maskB))) != 0;
}
