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
			var grid = Grid.Parse("5.7....1....56..87..427...........6.......8.5....5..42.32.9.4..1..3..........265.");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult:-#!.}");
		}
	}
}
