using System;
using System.Reflection;
using Sudoku.Data.Meta;
using Sudoku.Solving.BruteForces.Backtracking;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Solving.BruteForces.DancingLinks;
using Sudoku.Solving.BruteForces.Linqing;

namespace Sudoku.Terminal
{
	internal static class Program
	{
		private static int Main(string[] args)
		{
			return args.Length switch
			{
				1 => args[0] switch
				{
					"--help" => PrintHelp(),
					"--version" => PrintTitle(),
					_ => -1
				},
				2 => args[0] switch
				{
					"--solve" => SolvePuzzle(args[1], "dlx"),
					_ => -1
				},
				4 => args[0] switch
				{
					"--solve" => args[2] == "--using" ? SolvePuzzle(args[1], args[3]) : -1,
					_ => -1
				},
				_ => 0,
			};
		}

		private static int SolvePuzzle(string puzzleStr, string solverName)
		{
			try
			{
				var grid = Grid.Parse(puzzleStr);
				switch (solverName)
				{
					case "backtracking":
						Console.WriteLine(new BacktrackingSolver().Solve(grid));
						return 0;
					case "bitwise":
						Console.WriteLine(new BitwiseSolver().Solve(grid));
						return -1;
					case "dlx":
						Console.WriteLine(new DancingLinksSolver().Solve(grid));
						return 0;
					case "linqing":
						Console.WriteLine(new OneLineLinqSolver().Solve(grid));
						return 0;
					default:
						Console.WriteLine($"Argument '{nameof(solverName)}' is invalid.");
						return -1;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fatal error due to exception thrown:");
				Console.WriteLine(ex.Message);
				return -1;
			}
		}

		private static int PrintHelp()
		{
			Console.WriteLine("If you want to show this helper text, please input '--help' option.");
			Console.WriteLine("  --help: Show this.");
			Console.WriteLine("  --solve {puzzle} [--using {solverName}]:");
			Console.WriteLine("    to solve the puzzle with specified solver name.");
			Console.WriteLine("    here are all 'solverName' options:");
			Console.WriteLine("    * backtracking,");
			Console.WriteLine("    * bitwise,");
			Console.WriteLine("    * dlx (default option)");
			Console.WriteLine("    * linqing");
			Console.WriteLine("  --version: Show version of this program.");
			Console.WriteLine();

			return 0;
		}

		private static int PrintTitle()
		{
			Console.WriteLine(".---------------------------.");
			Console.WriteLine("| Welcome to sudoku solver! |");
			Console.WriteLine("'---------------------------'");
			Console.WriteLine();
			Console.WriteLine($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
			Console.WriteLine();

			return 0;
		}
	}
}
