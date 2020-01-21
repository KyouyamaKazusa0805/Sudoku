using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Data.Meta;
using Sudoku.Solving;
using Sudoku.Solving.BruteForces.DancingLinks;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Utils;

namespace Sudoku.Checking
{
	/// <summary>
	/// Provides some puzzle attributes validation operations.
	/// </summary>
	public static class PuzzleAttributeChecker
	{
		/// <summary>
		/// To check if a puzzle has only one solution or not.
		/// </summary>
		/// <param name="this">The puzzle to check.</param>
		/// <param name="solutionIfUnique">
		/// (out parameter) The solution if the puzzle is unique; otherwise, <c>null</c>.
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
		/// <param name="this">The puzzle to check.</param>
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
		/// <param name="this">The puzzle to check.</param>
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
		/// <param name="this">The puzzle to check.</param>
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
		/// To check if the puzzle is itto ryu or not.
		/// </summary>
		/// <param name="this">The puzzle to check.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsIttoRyu(this Grid @this)
		{
			var cloneation = @this.Clone();
			var (_, hasSolved, _, _, difficultyLevel, _, steps, _) = new ManualSolver
			{
				IttoRyuWhenPossible = true,
				EnableFullHouse = false,
				EnableLastDigit = false
			}.Solve(cloneation);
			if (hasSolved && difficultyLevel == DifficultyLevels.Easy)
			{
				int digit = 0;
				foreach (var (_, _, _, conclusions) in steps!)
				{
					int checkDigit = conclusions[0].Digit;
					if (checkDigit == digit)
					{
						continue;
					}

					if (checkDigit == (digit + 1) % 9)
					{
						digit = checkDigit;
						continue;
					}

					return false;
				}

				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
