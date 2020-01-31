using System;
using System.IO;
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
			//var solver = new ManualSolver
			//{
			//	OptimizedApplyingOrder = true
			//};
			//var grid = Grid.Parse("290800000030120040000000300370400500049080130006007094008000000060059010000008069");
			//var analysisResult = solver.Solve(grid);
			//Console.WriteLine(analysisResult);

			// Line counter.
			string solutionFolder = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
			var codeCounter = new CodeCounter(solutionFolder, @".*\.cs$");
			Console.WriteLine(codeCounter.CountCodeLines());
		}
	}
}
