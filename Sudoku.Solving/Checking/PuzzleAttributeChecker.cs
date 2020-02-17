using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Data.Meta;
using Sudoku.Solving.BruteForces.DancingLinks;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Utils;

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
		/// <param name="solutionIfUnique">
		/// (<see langword="out"/> parameter) The solution if the puzzle is unique;
		/// otherwise, <see langword="null"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsUnique(this Grid @this, [NotNullWhen(true)] out Grid? solutionIfUnique)
		{
			var (_, hasSolved, _, solution, _) = new DancingLinksSolver().Solve(@this);
			if (hasSolved)
			{
				solutionIfUnique = solution;
				return true;
			}
			else
			{
				solutionIfUnique = null;
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
					if (array[CellUtils.GetOffset(i, j)] != 0)
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
				tempArrays[i][CellUtils.GetOffset(r, c)] = 0;
			}

			var solver = new DancingLinksSolver();
			return tempArrays.All(gridValues =>
			{
				var (_, hasSolved, _, _, _) = solver.Solve(gridValues);
				return !hasSolved;
			});
		}

		/// <summary>
		/// To check if the puzzle is pearl or not.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The puzzle to check.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsPearl(this Grid @this)
		{
			// Using a faster solver to check the grid is unique or not.
			if (@this.IsUnique(out _))
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
		public static bool IsDiamond(this Grid @this)
		{
			// Using a faster solver to check the grid is unique or not.
			if (@this.IsUnique(out _))
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
		public static bool CanBeSolvedUsingOnlySsts(this Grid @this)
		{
			if (!@this.IsUnique(out _))
			{
				return false;
			}

			var solver = new ManualSolver();
			var (_, _, _, _, level) = solver.Solve(@this);
			return level <= DifficultyLevel.Advanced;
		}
	}
}
