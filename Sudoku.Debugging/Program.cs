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
			var solver = new ManualSolver
			{
				OptimizedApplyingOrder = true
			};
			var grid = Grid.Parse(".73..........3......8...2.1...7..8.6...6284.32....4.5.5.9.......4..........5123..");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine(analysisResult);
		}
	}
}
