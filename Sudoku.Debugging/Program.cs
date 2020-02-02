using System;
using System.IO;
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
			var grid = Grid.Parse("300009742000500600000000010000000061109860000000003000001020030040005007500000028");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine(analysisResult);

			//Line counter.
			//string solutionFolder = Solution.PathRoot;
			//var codeCounter = new CodeCounter(solutionFolder, @".*\.cs$");
			//Console.WriteLine($"{codeCounter.CountCodeLines()} lines.");
		}
	}
}
