using System;
using Sudoku.Data.Meta;
using Sudoku.Diagnostics;
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
				CheckMinimumDifficultyStrictly = true,
				EnableBruteForce = false
			};
			var grid = Grid.Parse("+7+3+9000+2+4+5+1+643+258+7+9+82+59+47+31+60780006500+4+2000+9+80051000430+41+62+73+59+8+5+936+187+2+4+2+8+7000+1+6+3:146 946 666");
			var analysisResult = solver.Solve(grid);
			Console.WriteLine(analysisResult);

			//Line counter.
			//string solutionFolder = Solution.PathRoot;
			//var codeCounter = new CodeCounter(solutionFolder, @".*\.cs$");
			//int linesCount = codeCounter.CountCodeLines(out int filesCount);
			//Console.WriteLine($"Found {filesCount} files, {linesCount} lines.");
		}
	}
}
