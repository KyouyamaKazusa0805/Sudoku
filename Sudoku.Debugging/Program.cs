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
				EnableBruteForce = true,
			};
			var grid = Grid.Parse(".37658..4....34.764..1.75.32..3.5..88.......55..8.6..16.84.2..734.76....7..58346.");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult:m}");
		}
	}
}
