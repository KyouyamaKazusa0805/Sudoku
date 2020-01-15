using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.Data.Meta;
using Sudoku.Runtime;

namespace Sudoku.Solving.BruteForces.Linqing
{
	public sealed class OneLineLinqSolver : Solver
	{
		public override string SolverName => "One line LINQ";


		public override AnalysisResult Solve(Grid grid)
		{
			var stopwatch = new Stopwatch();

			stopwatch.Start();
			var results = SolveStrings(grid.ToString("0"));
			stopwatch.Stop();

			return results.Count switch
			{
				0 => throw new NoSolutionException(grid),
				1 => new AnalysisResult(
					initialGrid: grid,
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
			var result = new List<string> { puzzle };

			while (result.Count > 0 && result[0].IndexOf('0', StringComparison.OrdinalIgnoreCase) != -1)
			{
				result = new List<string>(
					from solution in result
					let index = solution.IndexOf('0', StringComparison.OrdinalIgnoreCase)
					let column = index % 9
					let block = index - index % 27 + column - index % 3
					from value in values
					where !(
						from i in Enumerable.Range(0, 9)
						let inRow = solution[index - column + i] == value
						let inColumn = solution[column + i * 9] == value
						let inBlock = solution[block + i % 3 + (int)Math.Floor(i / 3f) * 9] == value
						where inRow || inColumn || inBlock
						select i).Any()
					select $"{solution.Substring(0, index)}{value}{solution.Substring(index + 1)}");
			}

			return result;
		}
	}
}
