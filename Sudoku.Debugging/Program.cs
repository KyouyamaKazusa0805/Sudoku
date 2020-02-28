using Sudoku.Data.Meta;
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
				OptimizedApplyingOrder = false
			};
			var grid = Grid.Parse("..5..4.7.4...2...3.6.3..8....3..5......1.8..57......9...9.3...7...4...6.6....79..");
			var analysisResult = solver.Solve(grid);
			WriteLine($"{analysisResult:-#!.}");
		}
	}
}
