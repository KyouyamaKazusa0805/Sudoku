namespace Sudoku.Solving.Manual.Checkers;

/// <summary>
/// Provides a searcher that checks the missing candidate for a sudoku grid.
/// </summary>
public static class MissingCandidateSearcher
{
	/// <summary>
	/// Indicates the inner solver.
	/// </summary>
	private static readonly FastSolver Solver = new();


	/// <summary>
	/// Gets the missing candidate that makes the grid have unique solution when it's restored.
	/// Returns <c>-1</c> when this metohd doesn't find out any possible missing candidates.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>
	/// The candidate found and confirmed being missing, whose valid range is from 0 to 728.
	/// <c>-1</c> will be returned if no possible missing candidate found. For example,
	/// at least two missing candidates should be checked.
	/// </returns>
	/// <exception cref="InvalidPuzzleException">
	/// Throws when the puzzle contain no solution or a unique solution.
	/// </exception>
	public static unsafe int GetMissingCandidate(in Grid grid)
	{
		if (Solver.Solve(grid.ToString("0"), null, 2) is 0 or 1)
		{
			throw new InvalidPuzzleException(grid, "the puzzle must contain more than one solution.");
		}

		foreach (int candidate in grid)
		{
			var newGrid = grid;
			newGrid[candidate / 9, candidate % 9] = true;

			if (Solver.CheckValidity(newGrid.ToString("0")))
			{
				return candidate;
			}
		}

		return -1;
	}
}
