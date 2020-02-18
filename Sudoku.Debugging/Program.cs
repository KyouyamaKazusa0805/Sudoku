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
			var grid = Grid.Parse("000000009005001030070060400600003002004080010030007500009005060010030700000200008", GridParsingType.PencilMarkedTreatSingleAsGiven);
			var analysisResult = solver.Solve(grid);
			Console.WriteLine($"{analysisResult:m}");
		}
	}
}
