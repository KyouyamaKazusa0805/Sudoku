namespace Sudoku.Solving.BruteForces
{
	/// <summary>
	/// Provides a solver using LINQ method.
	/// </summary>
	public sealed class OneLineLinqSolver : ISolver
	{
		/// <inheritdoc/>
		public string SolverName => TextResources.Current.OneLineLinq;


		/// <inheritdoc/>
		/// <exception cref="NoSolutionException">Throws when the puzzle has no valid solution.</exception>
		/// <exception cref="MultipleSolutionsException">
		/// Throws when the puzzle has multiple solutions.
		/// </exception>
		public AnalysisResult Solve(in SudokuGrid grid)
		{
			var stopwatch = new Stopwatch();

			stopwatch.Start();
			var results = SolveStrings(grid.ToString("0"));
			stopwatch.Stop();

			return results switch
			{
				{ Count: 0 } => throw new NoSolutionException(grid),
				{ Count: 1 } => new(SolverName, grid, true, stopwatch.Elapsed)
				{
					Solution = SudokuGrid.Parse(results[0])
				},
				{ Count: not (0 or 1) } => throw new MultipleSolutionsException(grid)
			};
		}


		/// <summary>
		/// Internal solving method.
		/// </summary>
		/// <param name="puzzle">The puzzle string, with placeholder character '0'.</param>
		/// <returns>The result strings (i.e. All solutions).</returns>
		private static IReadOnlyList<string> SolveStrings(string puzzle)
		{
			const string digits = "123456789";
			var result = new List<string> { puzzle };

			while (result.Count > 0 && result[0].IndexOf('0', StringComparison.OrdinalIgnoreCase) != -1)
			{
#pragma warning disable IDE0055
				result = (
					// Iterate on each possible intermediate grid.
					from solution in result

					// Find empty cells.
					let index = solution.IndexOf('0', StringComparison.OrdinalIgnoreCase)

					// Get the current column, and current block.
					// Here we don't use the row value.
					let pair = (Column: index % 9, Block: index - index % 27 + index % 9 - index % 3)

					// Iterate on each possible digit.
					from digit in digits

					// Find any duplicate cases (in the current row, column or block).
					// Here we should make a nested query to find all duplicates.
					let duplicateCases =

						// Iterate on each region number.
						from i in Enumerable.Range(0, 9)

						// Check whether the current region contains any duplicate digits.
						// If so, store this digit into the list.
						let duplicatesInRow = solution[index - pair.Column + i] == digit
						let duplicatesInColumn = solution[pair.Column + i * 9] == digit
						let duplicatesInBlock = solution[pair.Block + i % 3 + (int)Floor(i / 3F) * 9] == digit
						where duplicatesInRow || duplicatesInColumn || duplicatesInBlock

						// Gather the result.
						select i

					// Then check whether the duplicate list contains any elements.
					// If so, the grid is invalid.
					where !duplicateCases.Any()

					// Gather the result.
					select solution.ReplaceAt(index, digit)
				).ToList();
#pragma warning restore IDE0055
			}

			return result;
		}
	}
}
