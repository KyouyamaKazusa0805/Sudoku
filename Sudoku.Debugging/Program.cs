using System;
using Sudoku.Data.Meta;
using Sudoku.Diagnostics;
using Sudoku.Solving.Manual;

namespace Sudoku.Debugging
{
	/// <summary>
	/// The class aiming to this console application.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		/// The main function, which is the main entry point
		/// of this console application.
		/// </summary>
		private static void Main()
		{
			// Manual solver tester.
			var solver = new ManualSolver
			{
				CheckMinimumDifficultyStrictly = true,
				EnableBruteForce = false
			};
			var grid = Grid.Parse(".+8135.....5+2.+6+8...9+36.+71.+5+8..+58..7.+4248.3...9..+7..4.+856+79+18+35+42+81+4.9..7.+5+23.4.8+9+1:246 148 648 654 656 264 167 667 684 686");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine(analysisResult);

			//Line counter.
			//string solutionFolder = Solution.PathRoot;
			//var codeCounter = new CodeCounter(solutionFolder, @".*\.cs$");
			//int linesCount = codeCounter.CountCodeLines(out int filesCount);
			//Console.WriteLine($"Found {filesCount} files, {linesCount} lines.");
		}
	}
}
