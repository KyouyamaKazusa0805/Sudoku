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
|  45679      3          567      |  129        129        269      |  479        8          249      |
|  2          169        68       |  4          7          689      |  5          139        139      |
|  479        1479       78       |  3          5          289      |  479        1249       6        |
:---------------------------------+---------------------------------+---------------------------------:
|  356        8          2356     |  1259       4          2359     |  369        7          139      |
|  3456       456        9        |  15         13         7        |  2          1346       8        |
|  1          47         237      |  8          6          239      |  349        349        5        |
:---------------------------------+---------------------------------+---------------------------------:
|  35679      2          1        |  579        8          3459     |  3469       34569      349      |
|  8          59         35       |  6          239        23459    |  1          23459      7        |
|  35679      5679       4        |  579        239        1        |  8          23569      239      |
'---------------------------------'---------------------------------'---------------------------------'
", GridParsingType.PencilMarkedTreatSingleAsGiven);
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult:m}");
		}
	}
}
