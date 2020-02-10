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
				//CheckMinimumDifficultyStrictly = true,
				OptimizedApplyingOrder = true,
				EnableBruteForce = false
			};
			var grid = Grid.Parse("000009810000013900000600002000005407047060530903700000300002000008370000091400000");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult}");

			//Line counter.
			//string solutionFolder = Solution.PathRoot;
			//var codeCounter = new CodeCounter(solutionFolder, @".*\.cs$");
			//int linesCount = codeCounter.CountCodeLines(out int filesCount);
			//Console.WriteLine($"Found {filesCount} files, {linesCount} lines.");
		}
	}
}
