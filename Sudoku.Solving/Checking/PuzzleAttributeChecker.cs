using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Data;
using Sudoku.Extensions;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Solving.Manual;

namespace Sudoku.Solving.Checking
{
	/// <summary>
	/// Provides some puzzle attributes validation operations.
	/// </summary>
	public static class PuzzleAttributeChecker
	{
		/// <summary>
		/// Indicates the inner solver.
		/// </summary>
		private static readonly UnsafeBitwiseSolver Solver = new();


		/// <summary>
		/// Same as <see cref="IsValid(Grid, out Grid?)"/> and <see cref="IsValid(Grid, out Grid?, out bool?)"/>,
		/// but doesn't contain any <see langword="out"/> parameters.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The <see cref="bool"/> indicating that.</returns>
		/// <seealso cref="IsValid(Grid, out Grid?)"/>
		/// <seealso cref="IsValid(Grid, out Grid?, out bool?)"/>
		public static bool IsValid(this Grid @this) =>
			Solver.CheckValidity(@this.ToString()) || Solver.CheckValidity($"{@this:~}");

		/// <summary>
		/// To check if a puzzle has only one solution or not.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The puzzle to check.</param>
		/// <param name="solutionIfValid">
		/// (<see langword="out"/> parameter) The solution if the puzzle is valid;
		/// otherwise, <see langword="null"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsValid(this Grid @this, [NotNullWhen(true)] out Grid? solutionIfValid)
		{
			solutionIfValid = null;

			if (Solver.CheckValidity(@this.ToString(), out string? solution)
				|| Solver.CheckValidity($"{@this:~}", out solution))
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
		/// <param name="sukaku">
		/// (<see langword="out"/> parameter) A <see cref="bool"/> value indicating whether the current
		/// grid is a sukaku. <see langword="true"/> is for sukaku.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsValid(this Grid @this, [NotNullWhen(true)] out bool? sukaku)
		{
			if (Solver.CheckValidity(@this.ToString()))
			{
				sukaku = false;
				return true;
			}
			else if (Solver.CheckValidity(@this.ToString("~")))
			{
				sukaku = true;
				return true;
			}
			else
			{
				sukaku = null;
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
			this Grid @this, [NotNullWhen(true)] out Grid? solutionIfValid, [NotNullWhen(true)] out bool? sukaku)
		{
			if (Solver.CheckValidity(@this.ToString(), out string? solution))
			{
				solutionIfValid = Grid.Parse(solution);
				sukaku = false;
				return true;
			}
			else if (Solver.CheckValidity(@this.ToString("~"), out solution))
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
		public static bool IsMinimal(this Grid @this)
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
				tempArrays[i] = array.CloneAs<int[]>();
				tempArrays[i][r * 9 + c] = 0;
			}

			return tempArrays.All(gridValues => !Solver.Solve(Grid.CreateInstance(gridValues)).HasSolved);
		}

		/// <summary>
		/// To check if the puzzle is pearl or not.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The puzzle to check.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsPearl(this Grid @this) =>
			@this.IsValid() && new ManualSolver().Solve(@this) is var result
			&& result.MaxDifficulty == result.PearlDifficulty;

		/// <summary>
		/// To check if the puzzle is diamond or not.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The puzzle to check.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsDiamond(this Grid @this)
		{
			// Using a faster solver to check the grid is unique or not.
			if (@this.IsValid())
			{
				var result = new ManualSolver().Solve(@this);
				var (er, pr, dr) = (result.MaxDifficulty, result.PearlDifficulty, result.DiamondDifficulty);
				return er == pr && er == dr;
			}
			else
			{
				// The puzzle doesn't have unique solution, neither pearl nor diamond one.
				return false;
			}
		}

		/// <summary>
		/// To check whether the puzzle can be solved using only simple sudoku technique set.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The puzzle.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool CanBeSolvedUsingOnlySsts(this Grid @this) =>
			@this.IsValid() && new ManualSolver().Solve(@this).DifficultyLevel <= DifficultyLevel.Moderate;

		/// <summary>
		/// Get the difficulty level of this puzzle.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The puzzle.</param>
		/// <returns>The difficulty level.</returns>
		public static DifficultyLevel GetDifficultyLevel(this Grid @this) =>
			new ManualSolver().Solve(@this).DifficultyLevel;
	}
}
