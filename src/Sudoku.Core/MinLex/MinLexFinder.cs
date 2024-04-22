namespace Sudoku.MinLex;

/// <summary>
/// Represents a finder object that checks for a sudoku grid, calculating for the minimal lexicographical-ordered value for that grid.
/// </summary>
/// <remarks>
/// This object can be used for checking for duplicate for grids. If two grids are considered to be equivalent,
/// two grids will contain a same minimal lexicographic value.
/// </remarks>
public static class MinLexFinder
{
	/// <inheritdoc cref="Find(ref readonly Grid, bool)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Find(string gridString, bool findForPattern = false)
	{
		MinLexCandidate.PatCanon(gridString, out var result, findForPattern);
		return result;
	}

	/// <summary>
	/// Find for the minimal lexicographic result for a grid.
	/// </summary>
	/// <param name="grid">The specified grid.</param>
	/// <param name="findForPattern">Indicates whether the grid only searches for its minimal pattern.</param>
	/// <returns>The minimal result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Find(ref readonly Grid grid, bool findForPattern = false)
	{
		MinLexCandidate.PatCanon(grid.ToString(), out var result, findForPattern);
		return Grid.Parse(result);
	}
}
