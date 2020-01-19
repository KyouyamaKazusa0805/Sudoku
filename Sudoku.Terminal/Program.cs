using System;
using System.Reflection;
using Sudoku.Data.Meta;
using Sudoku.Solving.BruteForces.Backtracking;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Solving.BruteForces.DancingLinks;
using Sudoku.Solving.BruteForces.Linqing;
using Sudoku.Checking;
using Sudoku.Solving.Manual;

namespace Sudoku.Terminal
{
	/// <summary>
	/// The class handling all program-level actions.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		/// The main function, the main entry point of the program.
		/// </summary>
		/// <param name="args">
		/// <para>The arguments passed from console.</para>
		/// <para>
		/// Note that the arguments does not
		/// include the program name string. The first string of the array is the
		/// real argument.
		/// </para>
		/// </param>
		/// <returns>
		/// An <see cref="int"/> value used for diagnostics.
		/// When the program is executed successfully without any bugs and errors,
		/// this value will be 0; otherwise, a non-zero value.
		/// </returns>
		private static int Main(string[] args)
		{
			args[0] = args[0].ToLower();
			return args.Length switch
			{
				1 => args[0] switch
				{
					"-?" => PrintHelp(),
					"/?" => PrintHelp(),
					"--help" => PrintHelp(),
					"--version" => PrintTitle(),
					_ => -1
				},
				2 => args[0] switch
				{
					"--solve" => SolvePuzzle(args[1], "dlx"),
					_ => -1
				},
				3 => args[0] switch
				{
					"--check" => CheckPuzzle(args[1], args[2].ToLower()),
					_ => -1
				},
				4 => args[0] switch
				{
					"--solve" => args[2] == "--using" ? SolvePuzzle(args[1], args[3].ToLower()) : -1,
					_ => -1
				},
				_ => 0,
			};
		}

		/// <summary>
		/// To check the puzzle.
		/// </summary>
		/// <param name="puzzle">The puzzle string.</param>
		/// <param name="checkingType">The checking type.</param>
		/// <returns>The diagnostic value.</returns>
		private static int CheckPuzzle(string puzzle, string checkingType)
		{
			try
			{
				var grid = Grid.Parse(puzzle);
				switch (checkingType)
				{
					case "unique":
					{
						Console.WriteLine(
							$"This puzzle {(grid.IsUnique(out _) ? "has" : "does not have")} a unique solution.");
						return 0;
					}
					case "minimal":
					{
						Console.WriteLine(
							$"This puzzle {(grid.IsMinimal() ? "is" : "is not")} a minimal puzzle.");
						return 0;
					}
					case "pearl":
					{
						Console.WriteLine(
							$"This puzzle {(grid.IsPearl() ? "is" : "is not")} a pearl puzzle.");
						return 0;
					}
					case "diamond":
					{
						Console.WriteLine(
							$"This puzzle {(grid.IsDiamond() ? "is" : "is not")} a diamond puzzle.");
						return 0;
					}
					default:
					{
						Console.WriteLine($"Argument '{nameof(checkingType)}' is invalid.");
						return -1;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fatal error due to exception thrown:");
				Console.WriteLine(ex.Message);
				return -1;
			}
		}

		/// <summary>
		/// To solve the puzzle using the specified solver.
		/// </summary>
		/// <param name="puzzleStr">The puzzle string.</param>
		/// <param name="solverName">The solver name.</param>
		/// <returns>The diagnostic value.</returns>
		private static int SolvePuzzle(string puzzleStr, string solverName)
		{
			try
			{
				var grid = Grid.Parse(puzzleStr);
				switch (solverName)
				{
					case "backtracking":
					{
						Console.WriteLine(new BacktrackingSolver().Solve(grid));
						return 0;
					}
					case "bitwise":
					{
						Console.WriteLine(new BitwiseSolver().Solve(grid));
						return -1;
					}
					case "dlx":
					{
						Console.WriteLine(new DancingLinksSolver().Solve(grid));
						return 0;
					}
					case "linqing":
					{
						Console.WriteLine(new OneLineLinqSolver().Solve(grid));
						return 0;
					}
					case "manual":
					{
						Console.WriteLine(new ManualSolver().Solve(grid));
						return 0;
					}
					default:
					{
						Console.WriteLine($"Argument '{nameof(solverName)}' is invalid.");
						return -1;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fatal error due to exception thrown:");
				Console.WriteLine(ex.Message);
				return -1;
			}
		}

		/// <summary>
		/// To print the helper text on the screen of the console.
		/// </summary>
		/// <returns>The diagnostic value.</returns>
		private static int PrintHelp()
		{
			Console.WriteLine("If you want to show this helping text, please input '--help', '-?' or '/?' option.");
			Console.WriteLine("  -?: Show this.");
			Console.WriteLine("  /?: Show this.");
			Console.WriteLine("  --help: Show this.");
			Console.WriteLine("  --check {puzzle} {checkingType}:");
			Console.WriteLine("    To check an attribute of a sudoku puzzle.");
			Console.WriteLine("    Here are all 'checkingType' options:");
			Console.WriteLine("    * unique,");
			Console.WriteLine("    * minimal,");
			Console.WriteLine("    * pearl,");
			Console.WriteLine("    * diamond.");
			Console.WriteLine("  --solve {puzzle} [--using {solverName}]:");
			Console.WriteLine("    to solve the puzzle with specified solver name.");
			Console.WriteLine("    Here are all 'solverName' options:");
			Console.WriteLine("    * backtracking,");
			Console.WriteLine("    * bitwise,");
			Console.WriteLine("    * dlx (default option)");
			Console.WriteLine("    * linqing,");
			Console.WriteLine("    * manual.");
			Console.WriteLine("  --version: Show version of this program.");
			Console.WriteLine();

			return 0;
		}

		/// <summary>
		/// To print the title of the program and the version.
		/// </summary>
		/// <returns>The diagnostic value.</returns>
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
