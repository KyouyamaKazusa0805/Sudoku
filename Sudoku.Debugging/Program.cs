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
			var grid = Grid.Parse(".....47.5..2.37.......2..68.5....4.381...9.......461......1.5.49.........8......6");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine(analysisResult);
		}
	}
}
