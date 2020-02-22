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
			var grid = Grid.Parse("000000000000000000000000000000000000000000000000000000000000000000000000000000000");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult}");
		}
	}
}
