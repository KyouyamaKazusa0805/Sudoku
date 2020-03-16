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
				OnlySaveShortestPathAic = true
			};
			var grid = Grid.Parse(
				"+3+4512+8+900+97+6000+28+1+28+10003450000000+10+100600030402081+509+70+4000+128+8+190406+5+30+2+38+107+94");
			var analysisResult = solver.Solve(grid);
			WriteLine($"{analysisResult:-#!.}");
		}
	}
}
