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
				OnlySaveShortestPathAic = true,
				DisableSlowTechniques = false,
				HobiwanFishMaximumSize = 2,
			};
			var grid = Grid.Parse(
				"94.......1..83..7...8..2....7..4.1...8.1.6.2...6.5..3....7..4...5..23..1.......57");
			var analysisResult = solver.Solve(grid);
			WriteLine($"{analysisResult:-#!.}");
		}
	}
}
