using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.Data;
using Sudoku.Windows;
using static System.Linq.Enumerable;
using static System.Math;
using static System.StringComparison;

namespace Sudoku.Solving.BruteForces.Linqing
{
	/// <summary>
	/// Provides a solver using LINQ method.
	/// </summary>
	public sealed class OneLineLinqSolver : Solver
	{
		/// <inheritdoc/>
		public override string SolverName => Resources.GetValue("OneLineLinq");


		/// <inheritdoc/>
		public override AnalysisResult Solve(IReadOnlyGrid grid)
		{
			var stopwatch = new Stopwatch();

			stopwatch.Start();
			var results = SolveStrings(grid.ToString("0"));
			stopwatch.Stop();

			return results.Count switch
			{
				0 => throw new NoSolutionException(grid),
				1 =>
					new AnalysisResult(
						solverName: SolverName,
						puzzle: grid,
						solution: Grid.Parse(results[0]),
						hasSolved: true,
						elapsedTime: stopwatch.Elapsed,
						additional: null),
				_ => throw new MultipleSolutionsException(grid)
			};
		}


		/// <summary>
		/// Internal solving method.
		/// </summary>
		/// <param name="puzzle">The puzzle string, with placeholder character '0'.</param>
		/// <returns>The result strings (i.e. All solutions).</returns>
		private static List<string> SolveStrings(string puzzle)
		{
			const string values = "123456789";
			static int indexOf(string solution) => solution.IndexOf('0', OrdinalIgnoreCase);

			var result = new List<string> { puzzle };
			while (result.Count > 0 && indexOf(result[0]) != -1)
			{
				result = (
					from Solution in result
					let Index = indexOf(Solution)
					let Column = Index % 9
					let Block = Index - Index % 27 + Column - Index % 3
					from Value in values
					where (
						from I in Range(0, 9)
						let InRow = Solution[Index - Column + I] == Value
						let InColumn = Solution[Column + I * 9] == Value
						let InBlock = Solution[Block + I % 3 + (int)Floor(I / 3F) * 9] == Value
						where InRow || InColumn || InBlock
						select I).None()
					select $"{Solution.Substring(0, Index)}{Value}{Solution.Substring(Index + 1)}").ToList();
			}

			return result;
		}
	}
}
