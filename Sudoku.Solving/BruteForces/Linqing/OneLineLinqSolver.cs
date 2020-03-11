using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.Data;
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
		public override string SolverName => "One line LINQ";


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
				1 => new AnalysisResult(
					puzzle: grid,
					solverName: SolverName,
					hasSolved: true,
					solution: Grid.Parse(results[0]),
					elapsedTime: stopwatch.Elapsed,
					solvingList: null,
					additional: null),
				_ => throw new MultipleSolutionsException(grid)
			};
		}


		private static List<string> SolveStrings(string puzzle)
		{
			const string values = "123456789";
			static int indexOf(string solution) => solution.IndexOf('0', OrdinalIgnoreCase);

			var result = new List<string> { puzzle };
			while (result.Count > 0 && indexOf(result[0]) != -1)
			{
				result = new List<string>(
					from solution in result
					let Index = indexOf(solution)
					let Column = Index % 9
					let Block = Index - Index % 27 + Column - Index % 3
					from value in values
					where !(
						from i in Enumerable.Range(0, 9)
						let InRow = solution[Index - Column + i] == value
						let InColumn = solution[Column + i * 9] == value
						let InBlock = solution[Block + i % 3 + (int)Floor(i / 3f) * 9] == value
						where InRow || InColumn || InBlock
						select i).Any()
					select $"{solution.Substring(0, Index)}{value}{solution.Substring(Index + 1)}");
			}

			return result;
		}
	}
}
