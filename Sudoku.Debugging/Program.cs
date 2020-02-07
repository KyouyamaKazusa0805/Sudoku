using System;
using System.IO;
using Sudoku.Data.Meta;
using Sudoku.Diagnostics;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Utils;

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
			//var solver = new ManualSolver
			//{
			//	CheckMinimumDifficultyStrictly = true,
			//	EnableBruteForce = false
			//};
			//var grid = Grid.Parse("003056000007000+306+642003+5100+3089+20+50+29040+50300050+30002060+50000+3000320001+3+21009005:917 918 428 928 971 981 697 698");
			//var analysisResult = solver.Solve(grid);
			//Console.WriteLine(analysisResult);

			// BUG checker.
			var checker = new BugChecker(
				Grid.Parse("7+1+9+2+36+548+3+58001+6+29+26+40001+738+70+50+3490+9+40080+3+50+5230+60+7+814+37000+96+51+8+56002+3+469+2300+8+17"));
			var candidateOffsets = checker.GetAllTrueCandidates();
			Console.WriteLine(CandidateCollection.ToString(candidateOffsets));

			//Line counter.
			//string solutionFolder = Solution.PathRoot;
			//var codeCounter = new CodeCounter(solutionFolder, @".*\.cs$");
			//int linesCount = codeCounter.CountCodeLines(out int filesCount);
			//Console.WriteLine($"Found {filesCount} files, {linesCount} lines.");
		}
	}
}
