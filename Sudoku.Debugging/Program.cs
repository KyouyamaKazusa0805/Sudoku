using Sudoku.Data;
using static System.Console;
using solver = Sudoku.Solving.Manual;

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
			var solver = new solver::ManualSolver
			{
				CheckAlmostLockedQuadruple = true,
				AnalyzeDifficultyStrictly = true,
			};
			var grid = Grid.Parse(
				"000000083001380046023000010700150034000007800060040070005800000000500000207401000");
			var analysisResult = solver.Solve(grid);
			WriteLine($"{analysisResult:-#!.}");
		}
	}
}
