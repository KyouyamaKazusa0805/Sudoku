using Sudoku.Collections;

namespace Sudoku.Solving.BruteForces;

/// <summary>
/// Provides a solver using backtracking method.
/// </summary>
public sealed class BacktrackingSolver : IPuzzleSolver
{
	/// <summary>
	/// The temporary grid to solve.
	/// </summary>
	private Grid _grid = Grid.Undefined;


	/// <inheritdoc/>
	public ISolverResult Solve(in Grid puzzle, CancellationToken cancellationToken = default)
	{
		_grid = puzzle;

		int[] gridValues = puzzle.ToArray();
		int[]? result = null;
		int solutionsCount = 0;
		var stopwatch = new Stopwatch();

		try
		{
			stopwatch.Start();
			InternalSolve(ref solutionsCount, ref result, gridValues, 0);
			stopwatch.Stop();

			var solverResult = new BruteForceSolverResult(puzzle);
			return result is null
				? solverResult with
				{
					IsSolved = false,
					ElapsedTime = stopwatch.Elapsed,
					FailedReason = FailedReason.PuzzleHasNoSolution
				}
				: solverResult with
				{
					ElapsedTime = stopwatch.Elapsed,
					Solution = new(result, GridCreatingOption.MinusOne)
				};
		}
		catch (InvalidOperationException)
		{
			stopwatch.Stop();

			return new BruteForceSolverResult(
				puzzle,
				IsSolved: false,
				ElapsedTime: stopwatch.Elapsed,
				FailedReason: FailedReason.PuzzleHasMultipleSolutions
			);
		}
	}

	/// <inheritdoc/>
	public ValueTask<ISolverResult> SolveAsync(in Grid puzzle, CancellationToken cancellationToken = default) =>
		new(Solve(puzzle, cancellationToken));

	/// <summary>
	/// Solve the puzzle backtrackingly.
	/// </summary>
	/// <param name="solutionsCount">The number of solutions.</param>
	/// <param name="result">The result array.</param>
	/// <param name="gridValues">All grid values.</param>
	/// <param name="finishedCellsCount">The number of cells had finished.</param>
	/// <exception cref="InvalidOperationException">Throws when the puzzle contains multiple solutions.</exception>
	private void InternalSolve(ref int solutionsCount, ref int[]? result, int[] gridValues, int finishedCellsCount)
	{
		if (finishedCellsCount == 81)
		{
			// Solution found.
			if (++solutionsCount > 1)
			{
				throw new InvalidOperationException($"{nameof(_grid)} contains multiple solutions.");
			}

			// We should catch the result.
			// If we use normal assignment, we well get the
			// initial grid rather a solution, because
			// this is a recursive function!!!
			result = (int[])gridValues.Clone();
			return; // Exit the recursion.
		}

		if (gridValues[finishedCellsCount] != 0)
		{
			InternalSolve(ref solutionsCount, ref result, gridValues, finishedCellsCount + 1);
		}
		else
		{
			// Here may try 9 times.
			// Of course, you can add a new variable to save
			// all candidates to let the algorithm run faster.
			int r = finishedCellsCount / 9, c = finishedCellsCount % 9;
			for (int i = 0; i < 9; i++)
			{
				gridValues[finishedCellsCount]++; // Only use value increment operator.
				if (isValid(gridValues, r, c))
				{
					InternalSolve(ref solutionsCount, ref result, gridValues, finishedCellsCount + 1);
				}
			}

			// All values are wrong, which means the value before
			// we calculate is already wrong.
			// Backtracking the cell...
			gridValues[finishedCellsCount] = 0;
		}


		static bool isValid(int[] gridValues, int r, int c)
		{
			int number = gridValues[r * 9 + c];

			// Check lines.
			for (int i = 0; i < 9; i++)
			{
				if (i != r && gridValues[i * 9 + c] == number || i != c && gridValues[r * 9 + i] == number)
				{
					return false;
				}
			}

			// Check blocks.
			for (int ii = r / 3 * 3, i = ii, iiPlus3 = ii + 3; i < iiPlus3; i++)
			{
				for (int jj = c / 3 * 3, j = jj, jjPlus3 = jj + 3; j < jjPlus3; j++)
				{
					if ((i != r || j != c) && gridValues[i * 9 + j] == number)
					{
						return false;
					}
				}
			}

			// All region are checked and passed, return true.
			return true;
		}
	}
}
