namespace Sudoku.Concepts;

/// <summary>
/// Defines a backdoor searcher.
/// </summary>
public static class Backdoor
{
	/// <summary>
	/// Try to get all possible backdoors.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>A list of backdoors.</returns>
	/// <exception cref="ArgumentException">Throws when the grid is not unique, or the puzzle is too easy.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<Conclusion> GetBackdoors(ref readonly Grid grid)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(grid.PuzzleType == SudokuType.Standard, true);

		var sstsChecker = Analyzer.SstsOnly;
		return (grid, sstsChecker.Analyze(in grid)) switch
		{
			_ when !grid.GetIsValid()
				=> throw new ArgumentException(SR.ExceptionMessage("GridIsInvalidOrSolved"), nameof(grid)),
			({ IsSolved: true }, _)
				=> throw new ArgumentException(SR.ExceptionMessage("GridIsInvalidOrSolved"), nameof(grid)),
			(_, { IsSolved: true }) when grid.GetSolutionGrid() is var solution
				=>
				from candidate in grid
				let digit = solution.GetDigit(candidate / 9)
				where digit != -1
				select new Conclusion(digit == candidate % 9 ? Assignment : Elimination, candidate),
			_ => g(in grid)
		};


		ReadOnlySpan<Conclusion> g(ref readonly Grid grid)
		{
			var (assignment, elimination, solution) = (new List<Conclusion>(81), new List<Conclusion>(729), grid.GetSolutionGrid());
			foreach (var cell in grid.EmptyCells)
			{
				// Case 1: Assignments.
				var case1Playground = grid;
				case1Playground.SetDigit(cell, solution.GetDigit(cell));

				if (sstsChecker.Analyze(in case1Playground).IsSolved)
				{
					assignment.Add(new(Assignment, cell, solution.GetDigit(cell)));

					// Case 2: Eliminations.
					foreach (var digit in (Mask)(grid.GetCandidates(cell) & (Mask)~(1 << solution.GetDigit(cell))))
					{
						var case2Playground = grid;
						case2Playground.SetExistence(cell, digit, false);
						if (sstsChecker.Analyze(in case2Playground).IsSolved)
						{
							elimination.Add(new(Elimination, cell, digit));
						}
					}
				}
			}
			return (Conclusion[])[.. assignment, .. elimination];
		}
	}
}
