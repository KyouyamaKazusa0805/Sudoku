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
				AnalyzeDifficultyStrictly = true,
			};
			var grid = Grid.Parse("700000020000300405006000097000006070035708140060400000690000700801005000070000001");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult:-#!.}");
		}
	}
}
