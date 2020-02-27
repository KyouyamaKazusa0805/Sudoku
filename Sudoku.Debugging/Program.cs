using System;
using Sudoku.Data.Meta;
using Sudoku.Solving.Manual;

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
				AnalyzeDifficultyStrictly = true
			};
			var grid = Grid.Parse("00+90+74380+30+816+90020000809+1003600709080000000109080+642+30030400009000380000857000+30:234 541 544 545 752 753 554 556 557 558 577 579 679 689 195 699");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult:-#!.}");
		}
	}
}
