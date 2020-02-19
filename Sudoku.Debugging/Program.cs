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
				AnalyzeDifficultyStrictly = true,
				//OptimizedApplyingOrder = true,
				EnableBruteForce = true,
			};
			var grid = Grid.Parse(@"
.---------------------------------.---------------------------------.---------------------------------.
|  6          589        2589     |  7          19         38       |  1235       4          23       |
|  127        3          27       |  5          4          6        |  12         8          9        |
|  189        589        4        |  2          19         38       |  7          35         6        |
:---------------------------------+---------------------------------+---------------------------------:
|  28         1          238      |  48         7          9        |  346        36         5        |
|  47         479        6        |  3          2          5        |  8          79         1        |
|  5          789        389      |  18         6          14       |  349        2          37       |
:---------------------------------+---------------------------------+---------------------------------:
|  3          46789      789      |  16         5          14       |  269        679        278      |
|  49         2          59       |  46         38         7        |  569        1          38       |
|  78         5678       1        |  9          38         2        |  356        3567       4        |
'---------------------------------'---------------------------------'---------------------------------'
", GridParsingType.PencilMarkedTreatSingleAsGiven);
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult}");
		}
	}
}
