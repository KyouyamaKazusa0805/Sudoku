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
			};
			var grid = Grid.Parse(
				"0005+200+460+4093602100+6+8400900094+8+500286+2+3+7+9+4154+50+6+129+8008006+300037029400+86000+58000:113 523 171 571 577 779 587 192 399 799",
				GridParsingType.Susser);
			var analysisResult = solver.Solve(grid);
			WriteLine($"{analysisResult:-#!.}");
		}
	}
}
