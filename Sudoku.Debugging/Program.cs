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
				//AnalyzeDifficultyStrictly = true,
			};
			var grid = Grid.Parse(
				"00378609+207+6+902+8138+9+2000000600000+324200030+9059300000010+6000000832050+80400804612+30:737 545 571 573 773 783 793",
				GridParsingType.Susser);
			var analysisResult = solver.Solve(grid);
			WriteLine($"{analysisResult:-#!.}");
		}
	}
}
