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
				EnableBruteForce = false,
			};
			var grid = Grid.Parse(@"
.---------------------------------.---------------------------------.---------------------------------.
|  8          1          3        |  2          9          6        |  5          47         47       |
|  6          2          47       |  47         1          5        |  8          39         39       |
|  5          47         9        |  3          47         8        |  6          1          2        |
:---------------------------------+---------------------------------+---------------------------------:
|  17         8          45       |  6          357        379      |  2          49         149      |
|  17         45         2        |  59         8          79       |  3          6          149      |
|  9          3          6        |  1          2          4        |  7          5          8        |
:---------------------------------+---------------------------------+---------------------------------:
|  2          6          17       |  8          347        19       |  49         37         5        |
|  3          9          157      |  47         457        2        |  14         8          6        |
|  4          57         8        |  59         6          137      |  19         2          37       |
'---------------------------------'---------------------------------'---------------------------------'
", GridParsingType.PencilMarkedTreatSingleAsGiven);
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult}");
		}
	}
}
