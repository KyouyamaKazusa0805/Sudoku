using System;
using Sudoku.Data.Meta;
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
				AnalyzeDifficultyStrictly = true
			};
			var grid = Grid.Parse("000032005204000700000900008040259800000000050016000000007000009800000430600500000");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult:-#!.}");
		}
	}
}
