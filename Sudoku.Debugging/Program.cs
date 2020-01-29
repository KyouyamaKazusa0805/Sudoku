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
			var grid = Grid.Parse("...3..1.8.5.4...9.39..6...7.342..............96..3...1..871...5.....2....4.6.8...");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine(analysisResult);
		}
	}
}
