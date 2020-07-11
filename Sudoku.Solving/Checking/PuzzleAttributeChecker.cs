using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Data;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Solving.Manual;
using Level = Sudoku.Solving.DifficultyLevel;

namespace Sudoku.Solving.Checking
{
	/// <summary>
	/// Provides some puzzle attributes validation operations.
	/// </summary>
	[DebuggerStepThrough]
	public static class PuzzleAttributeChecker
	{
		/// <summary>
		/// To check if a puzzle has only one solution or not.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The puzzle to check.</param>
		/// <param name="solutionIfValid">
		/// (<see langword="out"/> parameter) The solution if the puzzle is valid;
		/// otherwise, <see langword="null"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsValid(this IReadOnlyGrid @this, [NotNullWhen(true)] out IReadOnlyGrid? solutionIfValid)
		{
			solutionIfValid = null;

			if (new BitwiseSolver().CheckValidity(@this.ToString(), out string? solution)
				|| new SukakuBitwiseSolver().CheckValidity(@this.ToString("~"), out solution))
			{
				solutionIfValid = Grid.Parse(solution);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// To check if a puzzle has only one solution or not.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The puzzle to check.</param>
		/// <param name="solutionIfValid">
		/// (<see langword="out"/> parameter) The solution if the puzzle is valid;
		/// otherwise, <see langword="null"/>.
		/// </param>
		/// <param name="sukaku">
		/// (<see langword="out"/> parameter) Indicates whether the current mode is sukaku mode.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsValid(
			this IReadOnlyGrid @this, [NotNullWhen(true)] out IReadOnlyGrid? solutionIfValid,
			[NotNullWhen(true)] out bool? sukaku)
		{
			if (new BitwiseSolver().CheckValidity(@this.ToString(), out string? solution))
			{
				solutionIfValid = Grid.Parse(solution);
				sukaku = false;
				return true;
			}
			else if (new SukakuBitwiseSolver().CheckValidity(@this.ToString("~"), out solution))
			{
				solutionIfValid = Grid.Parse(solution);
				sukaku = true;
				return true;
			}
			else
			{
				solutionIfValid = null;
				sukaku = null;
				return false;
			}
		}

		/// <summary>
		/// To check if the puzzle is minimal or not.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The puzzle to check.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsMinimal(this IReadOnlyGrid @this)
		{
			int hintCount = 0;
			int[] array = @this.ToArray();
			var valueList = new Queue<(int, int)>();
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					if (array[i * 9 + j] != 0)
					{
						hintCount++;
						valueList.Enqueue((i, j));
					}
				}
			}

			int[][] tempArrays = new int[hintCount][];
			for (int i = 0; i < hintCount; i++)
			{
				var (r, c) = valueList.Dequeue();
				tempArrays[i] = (int[])array.Clone();
				tempArrays[i][r * 9 + c] = 0;
			}

			var solver = new BitwiseSolver();
			return tempArrays.All(gridValues => !solver.Solve(Grid.CreateInstance(gridValues)).HasSolved);
		}

		/// <summary>
		/// To check if the puzzle is pearl or not.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The puzzle to check.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsPearl(this IReadOnlyGrid @this)
		{
			// Using a faster solver to check the grid is unique or not.
			if (@this.IsValid(out _))
			{
				var result = new ManualSolver().Solve(@this);
				var (er, pr) = (result.MaxDifficulty, result.PearlDifficulty);
				return er == pr;
			}
			else
			{
				// The puzzle does not have unique solution, neither pearl nor diamond one.
				return false;
			}
		}

		/// <summary>
		/// To check if the puzzle is diamond or not.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The puzzle to check.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsDiamond(this IReadOnlyGrid @this)
		{
			// Using a faster solver to check the grid is unique or not.
			if (@this.IsValid(out _))
			{
				var result = new ManualSolver().Solve(@this);
				var (er, pr, dr) = (result.MaxDifficulty, result.PearlDifficulty, result.DiamondDifficulty);
				return er == pr && er == dr;
			}
			else
			{
				// The puzzle does not have unique solution, neither pearl nor diamond one.
				return false;
			}
		}

		/// <summary>
		/// To check whether the puzzle can be solved using only simple sudoku technique set.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The puzzle.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool CanBeSolvedUsingOnlySsts(this IReadOnlyGrid @this) =>
			@this.IsValid(out _) && new ManualSolver().Solve(@this).DifficultyLevel <= Level.Moderate;

		/// <summary>
		/// Get the difficulty level of this puzzle.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The puzzle.</param>
		/// <returns>The difficulty level.</returns>
		public static Level DifficultyLevel(this IReadOnlyGrid @this) =>
			new ManualSolver().Solve(@this).DifficultyLevel;
	}
}
