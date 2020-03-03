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
				OptimizedApplyingOrder = false,
			};
			var grid = Grid.Parse("000200091942015000000009080200000410000060000071000002050700000000580163460001000");
			var analysisResult = solver.Solve(grid);
			WriteLine($"{analysisResult:-#!.}");
		}
	}
}
