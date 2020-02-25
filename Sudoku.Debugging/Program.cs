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
				OptimizedApplyingOrder = true,
			};
			var grid = Grid.Parse("000000000000000002000001000001030040005607300030020008002060500650008904900400007");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult:-#!.}");
		}
	}
}
