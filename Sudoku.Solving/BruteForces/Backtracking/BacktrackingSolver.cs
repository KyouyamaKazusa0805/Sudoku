using System;
using System.Diagnostics;
using Sudoku.Data;
using Sudoku.Runtime;
using Sudoku.Solving.Extensions;
using static Sudoku.Solving.Utils.CellUtils;

namespace Sudoku.Solving.BruteForces.Backtracking
{
	/// <summary>
	/// Provides a solver using backtracking method.
	/// </summary>
	public sealed class BacktrackingSolver : Solver
	{
		/// <summary>
		/// The temporary grid to solve.
		/// </summary>
		private IReadOnlyGrid _grid = null!;


		/// <inheritdoc/>
		public override string SolverName => "Backtracking";


		/// <inheritdoc/>
		public override AnalysisResult Solve(IReadOnlyGrid grid)
		{
			_grid = grid;

			int[] gridValues = grid.ToArray();
			int[]? result = null;
			int solutionsCount = 0;
			var stopwatch = new Stopwatch();

			try
			{
				stopwatch.Start();
				BacktrackinglySolve(ref solutionsCount, ref result, gridValues, 0);
				stopwatch.Stop();

				return new AnalysisResult(
					puzzle: grid,
					solverName: SolverName,
					hasSolved: true,
					solution: Grid.CreateInstance(result ?? throw new NoSolutionException(grid)),
					elapsedTime: stopwatch.Elapsed,
					solvingList: null,
					additional: null);
			}
			catch (Exception ex)
			{
				stopwatch.StopAnyway();

				return new AnalysisResult(
					puzzle: grid,
					solverName: SolverName,
					hasSolved: false,
					solution: null,
					elapsedTime: stopwatch.Elapsed,
					solvingList: null,
					additional: ex.Message);
			}
		}

		/// <summary>
		/// Solve backtrackingly.
		/// </summary>
		/// <param name="solutionsCount">
		/// (<see langword="ref"/> parameter) The number of solutions.
		/// </param>
		/// <param name="result">
		/// (<see langword="ref"/> parameter) The result array.
		/// </param>
		/// <param name="gridValues">All grid values.</param>
		/// <param name="finishedCellsCount">The number of cells had finished.</param>
		private void BacktrackinglySolve(
			ref int solutionsCount, ref int[]? result, int[] gridValues, int finishedCellsCount)
		{
			if (finishedCellsCount == 81)
			{
				// Solution found.
				if (++solutionsCount > 1)
				{
					throw new MultipleSolutionsException(_grid);
				}
				else // solutionCount <= 1
				{
					// We should catch the result.
					// If we use normal assignment, we well get the
					// initial grid rather a solution, because
					// this is a recursive function!!!
					result = (int[])gridValues.Clone();
					return; // Exit the recursion.
				}
			}

			int r = finishedCellsCount / 9, c = finishedCellsCount % 9;
			if (gridValues[GetOffset(r, c)] != 0)
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
					gridValues[GetOffset(r, c)]++; // Only use value increment operator.
					if (IsValid(gridValues, r, c))
					{
						BacktrackinglySolve(
							ref solutionsCount, ref result, gridValues, finishedCellsCount + 1);
					}
				}

				// All values are wrong, which means the value before
				// we calculate is already wrong.
				// Backtracking the cell...
				gridValues[GetOffset(r, c)] = 0;
			}
		}


		private static bool IsValid(int[] gridValues, int r, int c)
		{
			int number = gridValues[GetOffset(r, c)];

			// Check lines.
			for (int i = 0; i < 9; i++)
			{
				if (i != r && gridValues[GetOffset(i, c)] == number
					|| i != c && gridValues[GetOffset(r, i)] == number)
				{
					return false;
				}
			}

			// Check blocks.
			for (int ii = r / 3 * 3, i = ii; i < ii + 3; i++)
			{
				for (int jj = c / 3 * 3, j = jj; j < jj + 3; j++)
				{
					if ((i != r || j != c) && gridValues[GetOffset(i, j)] == number)
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
