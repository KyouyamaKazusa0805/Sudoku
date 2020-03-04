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
				"0000+10083001380046+82300+401070+215003400+4007800060040070005800000000500000207401000:614 616 517 527 735 946 951 952 258 958 159 559 161 961 266 966 267 277 977 678 279 979 287 987 688 289 989 997 998 999");
			var analysisResult = solver.Solve(grid);
			WriteLine($"{analysisResult:-#!.}");
		}
	}
}
