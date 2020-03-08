using Sudoku.Data;
using Sudoku.Solving.Manual;
using static System.Console;

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
				CheckAlmostLockedQuadruple = true,
				AnalyzeDifficultyStrictly = true,
				DisableSlowTechniques = false,
			};
			var grid = Grid.Parse(
				"040009300000030000000000800090005010800004700050070000002307490700400008580000000");
			var analysisResult = solver.Solve(grid);
			WriteLine($"{analysisResult:-#!.}");
		}
	}
}
