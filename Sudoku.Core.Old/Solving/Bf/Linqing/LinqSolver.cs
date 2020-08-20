using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.Data.Literals;
using Sudoku.Data.Meta;

namespace Sudoku.Solving.Bf.Linqing
{
	public sealed class LinqSolver : BruteForceSolver
	{
		public override string Name => "Linqing";


		public override Grid? Solve(Grid grid, out AnalysisInfo analysisInfo)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var results = SolveStrings(grid.ToString("g0", null));

			stopwatch.Stop();
			if (results.Count == 1)
			{
				analysisInfo = new(Name, null, stopwatch.Elapsed, true);
				return Grid.Parse(results[0]);
			}
			else
			{
				analysisInfo = new(Name, null, stopwatch.Elapsed, false);
				return null;
			}
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
						from i in Values.DigitRange
						let inRow = solution[index - column + i] == value
						let inColumn = solution[column + i * 9] == value
						let inBlock = solution[block + i % 3 + (int)Math.Floor(i / 3f) * 9] == value
						where inRow || inColumn || inBlock
						select i
					).Any()
					select $"{solution.Substring(0, index)}{value}{solution.Substring(index + 1)}");
			}

			return result;
		}
	}
}
