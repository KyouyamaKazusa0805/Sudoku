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
				CheckGurthSymmetricalPlacement = true,
				OptimizedApplyingOrder = true,
				EnableBruteForce = true,
			};
			var grid = Grid.Parse("2...5...1..39...2..1...73..76....4..4.5.....8.98....6...158..3..2.6.91..3...74..2");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult}");
		}
	}
}
