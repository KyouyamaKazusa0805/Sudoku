using Sudoku.Data;
using Sudoku.Generating;
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
			var generator = new ExtendedGenerator();
			for (int i = 0; i < 20; i++)
			{
				var puzzle = generator.Generate();
				WriteLine($"{puzzle:.}");
			}

			// Manual solver tester.
			//var solver = new ManualSolver
			//{
			//	CheckAlmostLockedQuadruple = true,
			//	OptimizedApplyingOrder = false,
			//};
			//var grid = Grid.Parse("000200091942015000000009080200000410000060000071000002050700000000580163460001000");
			//var analysisResult = solver.Solve(grid);
			//WriteLine($"{analysisResult:-#!.}");
		}
	}
}
