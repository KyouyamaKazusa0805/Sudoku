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
			var grid = Grid.Parse(".9...2...6..4....3..5..628....8....4.1...........27........341...65.43.2..9.1..76");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine(analysisResult);
		}
	}
}
