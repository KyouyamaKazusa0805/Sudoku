namespace Sudoku.AutoFiller;

/// <summary>
/// Defines a default auto filler instance.
/// </summary>
public sealed class DefaultAutoFiller : IAutoFiller
{
	/// <summary>
	/// Indicates the default solver.
	/// </summary>
	private static readonly LogicalSolver Solver = new();


	/// <inheritdoc/>
	public void Fill(scoped ref Grid grid)
	{
		if (Solver.Solve(grid) is not { IsSolved: true, DifficultyLevel: var diffLevel, SolvingPath: var path })
		{
			throw new InvalidOperationException("The target grid is not unique.");
		}

		switch (diffLevel)
		{
			case DifficultyLevel.Unknown:
			{
				throw new InvalidOperationException("The diffculty level of the target grid is unknown.");
			}
			case DifficultyLevel.Easy or DifficultyLevel.Moderate:
			{
				return;
			}
			case >= DifficultyLevel.Hard and <= DifficultyLevel.Nightmare or DifficultyLevel.LastResort:
			{
				foreach (var (stepGrid, step) in path)
				{
					if (step.DifficultyLevel is not (DifficultyLevel.Easy or DifficultyLevel.Moderate))
					{
						grid = stepGrid;
						return;
					}
				}

				break;
			}
			default:
			{
				throw new InvalidOperationException("The target grid is invalid - its difficulty level is too complex to be checked.");
			}
		}
	}
}
