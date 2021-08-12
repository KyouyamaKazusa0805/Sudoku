namespace Sudoku.Solving.BruteForces
{
	/// <summary>
	/// Provides a solver using backtracking method.
	/// </summary>
	public sealed class BacktrackingSolver : ISolver
	{
		/// <summary>
		/// The temporary grid to solve.
		/// </summary>
		private SudokuGrid _grid = SudokuGrid.Undefined;


		/// <inheritdoc/>
		public string SolverName => TextResources.Current.Backtracking;


		/// <inheritdoc/>
		/// <exception cref="NoSolutionException">Throws when the puzzle contains no solutions.</exception>
		public AnalysisResult Solve(in SudokuGrid grid)
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

				return new(SolverName, grid, true, stopwatch.Elapsed)
				{
					Solution = new(result ?? throw new NoSolutionException(grid), GridCreatingOption.MinusOne)
				};
			}
			catch (MultipleSolutionsException ex)
			{
				stopwatch.Stop();

				return new(SolverName, grid, false, stopwatch.Elapsed) { Additional = ex.Message };
			}
		}

		/// <summary>
		/// Solve backtrackingly.
		/// </summary>
		/// <param name="solutionsCount">
		/// The number of solutions.
		/// </param>
		/// <param name="result">
		/// The result array.
		/// </param>
		/// <param name="gridValues">All grid values.</param>
		/// <param name="finishedCellsCount">The number of cells had finished.</param>
		/// <exception cref="MultipleSolutionsException">
		/// Throws when the puzzle contains multiple solutions.
		/// </exception>
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

				// We should catch the result.
				// If we use normal assignment, we well get the
				// initial grid rather a solution, because
				// this is a recursive function!!!
				result = gridValues.CloneAs<int[]>();
				return; // Exit the recursion.
			}

			if (gridValues[finishedCellsCount] != 0)
			{
				BacktrackinglySolve(ref solutionsCount, ref result, gridValues, finishedCellsCount + 1);
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
						BacktrackinglySolve(ref solutionsCount, ref result, gridValues, finishedCellsCount + 1);
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
}
