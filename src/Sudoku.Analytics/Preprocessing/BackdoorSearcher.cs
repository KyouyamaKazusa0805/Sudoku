namespace Sudoku.Preprocessing;

/// <summary>
/// Defines a backdoor searcher.
/// </summary>
public static class BackdoorSearcher
{
	/// <summary>
	/// Try to get all possible backdoors.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>A list of backdoors.</returns>
	/// <exception cref="ArgumentException">Throws when the grid is not unique, or the puzzle is too easy.</exception>
	public static Conclusion[] GetBackdoors(scoped in Grid grid)
	{
		if (!grid.IsValid)
		{
			throw new ArgumentException("The grid must be unique.", nameof(grid));
		}

		var sstsOnly = PredefinedAnalyzers.SstsOnly;
		if (sstsOnly.Analyze(grid) is { IsSolved: true })
		{
			throw new ArgumentException(
				"The puzzle cannot be solved by elementary techniques only (Singles, Locked Candidates and Subsets).",
				nameof(grid)
			);
		}

		var (assignmentBackdoors, eliminationBackdoors, solution) = (new List<Conclusion>(81), new List<Conclusion>(729), grid.SolutionGrid);
		foreach (var cell in grid.EmptyCells)
		{
			// Case 1: Assignments.
			var case1Playground = grid;
			case1Playground.SetDigit(cell, solution.GetDigit(cell));

			if (sstsOnly.Analyze(case1Playground).IsSolved)
			{
				assignmentBackdoors.Add(new(Assignment, cell, solution.GetDigit(cell)));

				// Case 2: Eliminations.
				foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~(1 << solution.GetDigit(cell))))
				{
					var case2Playground = grid;
					case2Playground.SetCandidateIsOn(cell, digit, false);

					if (sstsOnly.Analyze(case2Playground).IsSolved)
					{
						eliminationBackdoors.Add(new(Elimination, cell, digit));
					}
				}
			}
		}

		return [.. assignmentBackdoors, .. eliminationBackdoors];
	}
}
