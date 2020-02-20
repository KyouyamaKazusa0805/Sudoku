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
				//OptimizedApplyingOrder = true,
				EnableBruteForce = true,
			};
			var grid = Grid.Parse("064507000037984000000000000900200100070603020006009008000000000000842310000705840");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult}");
		}
	}
}
