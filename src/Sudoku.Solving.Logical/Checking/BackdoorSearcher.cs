namespace Sudoku.Checking;

/// <summary>
/// Defines a backdoor searcher.
/// </summary>
internal static class BackdoorSearcher
{
	/// <summary>
	/// Indicates the bitwise solver.
	/// </summary>
	private static readonly BitwiseSolver BitwiseSolver = new();


	/// <summary>
	/// Try to get all possible backdoors.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>A list of backdoors.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the grid is not unique, or the puzzle is too easy.
	/// </exception>
	public static Conclusion[] GetBackdoors(scoped in Grid grid)
	{
		if (!grid.IsValid())
		{
			throw new ArgumentException("The grid must be unique.", nameof(grid));
		}

		if (CommonLogicalSolvers.SstsOnly.Solve(grid) is { IsSolved: true })
		{
			throw new ArgumentException(
				"The puzzle cannot be solved by elementary techniques only (Singles, Locked Candidates and Subsets).",
				nameof(grid)
			);
		}

		var assignmentBackdoors = new List<Conclusion>(81);
		var eliminationBackdoors = new List<Conclusion>(729);
		var solution = BitwiseSolver.Solve(grid);
		foreach (var cell in grid.EmptyCells)
		{
			// Case 1: Assignments.
			var case1Playground = grid;
			case1Playground[cell] = solution[cell];

			if (!CommonLogicalSolvers.SstsOnly.Solve(case1Playground).IsSolved)
			{
				continue;
			}

			assignmentBackdoors.Add(new(Assignment, cell, solution[cell]));

			// Case 2: Eliminations.
			foreach (var digit in (short)(grid.GetCandidates(cell) & ~(1 << solution[cell])))
			{
				var case2Playground = grid;
				case2Playground[cell, digit] = false;

				if (!CommonLogicalSolvers.SstsOnly.Solve(case2Playground).IsSolved)
				{
					continue;
				}

				eliminationBackdoors.Add(new(Elimination, cell, digit));
			}
		}

		return assignmentBackdoors.Concat(eliminationBackdoors).ToArray();
	}
}
