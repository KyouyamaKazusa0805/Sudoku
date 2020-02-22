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
			var grid = Grid.Parse(".....9..22473....9.9....13.6.3.7...5...5..31...5.2..9.....1.9..8.96...4....2.7...");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult}");
		}
	}
}
