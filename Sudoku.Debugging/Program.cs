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
				EnableBruteForce = false,
			};
			var grid = Grid.Parse("800603540040002068706048010000080201108000090030701080000860070080200004009030820:115 123 327 641 544 752 657 559 659 561 661 579 381 183 583 585 785 586 692");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult}");
		}
	}
}
