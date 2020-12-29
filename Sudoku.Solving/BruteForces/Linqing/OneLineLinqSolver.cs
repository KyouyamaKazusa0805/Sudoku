using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Extensions;
using System.Linq;
using Sudoku.Data;
using Sudoku.Runtime;
using Sudoku.Windows;
using static System.Math;

namespace Sudoku.Solving.BruteForces.Linqing
{
	/// <summary>
	/// Provides a solver using LINQ method.
	/// </summary>
	public sealed class OneLineLinqSolver : ISolver
	{
		/// <inheritdoc/>
		public string SolverName => Resources.GetValue("OneLineLinq");


		/// <inheritdoc/>
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
					where !(
						from i in Enumerable.Range(0, 9)
						let inRow = solution[index - column + i] == value
						let inColumn = solution[column + i * 9] == value
						let inBlock = solution[block + i % 3 + (int)Floor(i / 3f) * 9] == value
						where inRow || inColumn || inBlock
						select i
					).Any()
					select $"{solution[0..index]}{value}{solution[(index + 1)..]}").ToList();
			}

			return result;
		}
	}
}
