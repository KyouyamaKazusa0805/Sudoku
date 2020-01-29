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
			var grid = Grid.Parse("............3.9........7149.......31.2...89.4.96.7...8..8.......326........9..72.");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine(analysisResult);
		}
	}
}
