namespace Sudoku.Checking;

/// <summary>
/// Provides a searcher that checks the missing candidate for a sudoku grid.
/// </summary>
public static class MissingCandidateSearcher
{
	/// <summary>
	/// Indicates the inner solver.
	/// </summary>
	private static readonly BitwiseSolver Solver = new();


	/// <summary>
	/// Gets the missing candidate that makes the grid have unique solution when it's restored.
	/// Returns <c>-1</c> when this method doesn't find out any possible missing candidates.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>
	/// The candidate found and confirmed being missing, whose valid range is from 0 to 728.
	/// <see langword="null"/> will be returned if no possible missing candidate found.
	/// For example, at least two missing candidates should be checked.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the puzzle contain no solution or a unique solution.
	/// </exception>
	public static unsafe int? GetMissingCandidate(in Grid grid)
	{
		// Optimization: Check the symmetry. User-created sudoku puzzles often contain patterns with symmetry.
		return Solver.Solve(grid.ToString("0"), null, 2) switch
		{
			0 or 1 => throw new InvalidOperationException($"The {nameof(grid)} must contain more than one solution."),
			_ => tryWithSymmetry(grid) ?? tryWithNoSymmetry(grid)
		};


		static int? tryWithSymmetry(in Grid grid)
		{
			var handlers = stackalloc delegate*<ref Grid, ref Grid>[]
			{
				&GridTransformations.RotatePi,
				&GridTransformations.MirrorLeftRight,
				&GridTransformations.MirrorTopBottom,
				&GridTransformations.MirrorDiagonal,
				&GridTransformations.MirrorAntidiagonal,
				&GridTransformations.RotateClockwise,
				&GridTransformations.RotateCounterclockwise
			};

			for (int i = 0; i < 7; i++)
			{
				var copied = grid;
				if ((grid.GivenCells ^ handlers[i](ref copied).GivenCells) is [var cell])
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						var newGrid = grid;
						newGrid[cell] = digit;

						if (Solver.CheckValidity(newGrid.ToString("0")))
						{
							return cell * 9 + digit;
						}
					}
				}
			}

			return null;
		}

		static int? tryWithNoSymmetry(in Grid grid)
		{
			foreach (int candidate in grid)
			{
				var newGrid = grid;
				newGrid[candidate / 9, candidate % 9] = true;

				if (Solver.CheckValidity(newGrid.ToString("0")))
				{
					return candidate;
				}
			}

			return null;
		}
	}
}
