namespace Sudoku.Preprocessing;

/// <summary>
/// Defines a backdoor searcher.
/// </summary>
public static class BackdoorSearcher
{
	/// <summary>
	/// The checker.
	/// </summary>
	private static readonly Analyzer SstsChecker = PredefinedAnalyzers.SstsOnly;


	/// <summary>
	/// Try to get all possible backdoors.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>A list of backdoors.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the grid is not unique, or the puzzle is too easy.</exception>
	public static Conclusion[] GetBackdoors(scoped in Grid grid)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(grid.IsValid, true);
		ArgumentOutOfRangeException.ThrowIfNotEqual(SstsChecker.Analyze(grid).IsSolved, false);

		var (assignmentBackdoors, eliminationBackdoors, solution) = (new List<Conclusion>(81), new List<Conclusion>(729), grid.SolutionGrid);
		foreach (var cell in grid.EmptyCells)
		{
			// Case 1: Assignments.
			var case1Playground = grid;
			case1Playground.SetDigit(cell, solution.GetDigit(cell));

			if (SstsChecker.Analyze(case1Playground).IsSolved)
			{
				assignmentBackdoors.Add(new(Assignment, cell, solution.GetDigit(cell)));

				// Case 2: Eliminations.
				foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~(1 << solution.GetDigit(cell))))
				{
					var case2Playground = grid;
					case2Playground.SetCandidateIsOn(cell, digit, false);

					if (SstsChecker.Analyze(case2Playground).IsSolved)
					{
						eliminationBackdoors.Add(new(Elimination, cell, digit));
					}
				}
			}
		}

		return [.. assignmentBackdoors, .. eliminationBackdoors];
	}
}
