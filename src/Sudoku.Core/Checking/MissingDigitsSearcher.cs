namespace Sudoku.Checking;

/// <summary>
/// Provides a searcher that checks the missing digits for a sudoku grid.
/// </summary>
public static class MissingDigitsSearcher
{
	/// <summary>
	/// Indicates the inner solver.
	/// </summary>
	private static readonly BitwiseSolver Solver = new();


	/// <summary>
	/// Finds all missing digits that makes the grid have unique solution when one of them are filled in the grid.
	/// Returns <see langword="null"/> when this method doesn't find out any possible missing candidates.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>
	/// An array of digits found and confirmed being missing, whose valid range is from 0 to 728.
	/// <see langword="null"/> will be returned if no possible missing candidate found.
	/// For example, the grid won't be valid until at least two missing digits should be filled at the same time.
	/// </returns>
	public static unsafe int[]? GetMissingDigits(scoped in Grid grid)
	{
		if (Solver.Solve(grid.ToString("0"), null, 2) < 2)
		{
			return null;
		}

		using scoped var list = new ValueList<int>(byte.MaxValue);
		foreach (var candidate in grid)
		{
			var newGrid = grid;
			newGrid[candidate / 9] = candidate % 9;

			if (Solver.CheckValidity(newGrid.ToString("!0")))
			{
				list.Add(candidate);
			}
		}

		return list is [] ? null : list.ToArray();
	}
}
