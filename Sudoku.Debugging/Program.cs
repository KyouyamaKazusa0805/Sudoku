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
				//EnableBruteForce = false,
			};
			var grid = Grid.Parse(@"
.---------------------------------.---------------------------------.---------------------------------.
|  357        3578       2        |  3578       1          38       |  4          6          9        |
|  13567      4          13678    |  35678      3678       9        |  158        158        2        |
|  9          158        168      |  2          568        4        |  3          158        7        |
:---------------------------------+---------------------------------+---------------------------------:
|  1345       2          134      |  358        3589       7        |  156        13459      1456     |
|  8          6          34       |  1          359        2        |  7          3459       45       |
|  1357       1357       9        |  356        4          36       |  2          135        8        |
:---------------------------------+---------------------------------+---------------------------------:
|  2          378        3678     |  4          3678       1        |  9          578        56       |
|  13467      9          134678   |  3678       3678       5        |  168        2          146      |
|  1467       178        5        |  9          2          68       |  168        1478       3        |
'---------------------------------'---------------------------------'---------------------------------'
", GridParsingType.PencilMarkedTreatSingleAsGiven);
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult}");
		}
	}
}
