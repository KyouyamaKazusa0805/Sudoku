using System.Diagnostics;
using Sudoku.Data.Meta;
using Sudoku.Runtime;

namespace Sudoku.Solving.Bf.Backtracking
{
	public sealed class BacktrackingSolver : BruteForceSolver
	{
		public override string Name => "Backtracking";


		public override Grid? Solve(Grid grid, out AnalysisInfo analysisInfo)
		{
			int[,] gridValues = grid.ToArray();
			int[,]? result = null;
			int solutionsCount = 0;
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			try
			{
				BacktrackinglySolve(ref solutionsCount, ref result, gridValues, 0);
				stopwatch.Stop();

				analysisInfo = new AnalysisInfo(Name, null, stopwatch.Elapsed, true);
				return new Grid(result ?? throw new NoSolutionException());
			}
			catch (MultipleSolutionsException)
			{
				// Multiple solutions.
				stopwatch.Stop();

				analysisInfo = new AnalysisInfo(Name, null, stopwatch.Elapsed, false);
				return null;
			}
			catch (NoSolutionException)
			{
				// No solution.
				stopwatch.Stop();

				analysisInfo = new AnalysisInfo(Name, null, stopwatch.Elapsed, false);
				return null;
			}
		}


		private static void BacktrackinglySolve(
			ref int solutionsCount, ref int[,]? result, int[,] gridValues, int finishedCellsCount)
		{
			// Solution found.
			if (finishedCellsCount == 81)
			{
				if (++solutionsCount > 1)
				{
					throw new MultipleSolutionsException();
				}
				else // solutionCount <= 1
				{
					// We should catch the result.
					// If we use normal assignment, we well get the
					// initial grid rather a solution, because
					// This is a recursive function!!!
					result = (int[,])gridValues.Clone();
					return; // Exit the recursion.
				}
			}

			int r = finishedCellsCount / 9, c = finishedCellsCount % 9;
			if (gridValues[r, c] != 0)
			{
				BacktrackinglySolve(
					ref solutionsCount, ref result, gridValues, finishedCellsCount + 1);
			}
			else
			{
				// Here may try 9 times.
				// Of course, you can add a new variable to save
				// all candidates to let the algorithm run faster.
				for (int i = 0; i < 9; i++)
				{
					gridValues[r, c]++; // Only use value increment operator.
					if (IsValid(gridValues, r, c))
					{
						BacktrackinglySolve(
							ref solutionsCount, ref result, gridValues, finishedCellsCount + 1);
					}
				}

				// All values are wrong, which means the value before
				// we calculate is already wrong.
				// Backtracking the cell...
				gridValues[r, c] = 0;
			}
		}

		private static bool IsValid(int[,] gridValues, int r, int c)
		{
			int number = gridValues[r, c];
			int i, j, ii, jj;

			// Check lines.
			for (i = 0; i < 9; i++)
			{
				if (i != r && gridValues[i, c] == number || i != c && gridValues[r, i] == number)
				{
					return false;
				}
			}

			// Check blocks.
			for (i = r / 3 * 3, ii = r / 3 * 3; i < ii + 3; i++)
			{
				for (j = c / 3 * 3, jj = c / 3 * 3; j < jj + 3; j++)
				{
					if ((i != r || j != c) && gridValues[i, j] == number)
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
