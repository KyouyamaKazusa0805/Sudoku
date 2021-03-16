using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Extensions;
using System.Linq;
using Sudoku.Data;
using Sudoku.Resources;

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

			return results.Count switch
			{
				0 => throw new NoSolutionException(grid),
				1 => new(SolverName, grid, true, stopwatch.Elapsed) { Solution = SudokuGrid.Parse(results[0]) },
				_ => throw new MultipleSolutionsException(grid)
			};
		}


		/// <summary>
		/// Internal solving method.
		/// </summary>
		/// <param name="puzzle">The puzzle string, with placeholder character '0'.</param>
		/// <returns>The result strings (i.e. All solutions).</returns>
		private static IReadOnlyList<string> SolveStrings(string puzzle)
		{
			const string values = "123456789";
			var result = new List<string> { puzzle };

			while (result.Count > 0 && result[0].IndexOf('0', StringComparison.OrdinalIgnoreCase) != -1)
			{
				result = (
					from solution in result
					let index = solution.IndexOf('0', StringComparison.OrdinalIgnoreCase)
					let column = index % 9
					let block = index - index % 27 + column - index % 3
					from value in values
					where !query(solution, index, column, block, value).Any()
					select $"{solution[..index]}{value.ToString()}{solution[(index + 1)..]}"
				).ToList();

				IEnumerable<int> query(string solution, int index, int column, int block, char value) =>
					from i in Enumerable.Range(0, 9)
					let inRow = solution[index - column + i] == value
					let inColumn = solution[column + i * 9] == value
					let inBlock = solution[block + i % 3 + (int)Math.Floor(i / 3F) * 9] == value
					where inRow || inColumn || inBlock
					select i;
			}

			return result;
		}
	}
}
