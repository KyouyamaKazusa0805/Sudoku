using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Data.Meta;
using Sudoku.Solving.BruteForces.DancingLinks;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Utils;

namespace Sudoku.Checking
{
	public static class PuzzleAttributeChecker
	{
		public static bool IsUnique(this Grid @this, [NotNullWhen(true)] out Grid? solutionIfUnique)
		{
			(_, bool hasSolved, _, var solution, _) = new DancingLinksSolver().Solve(@this);
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

		public static bool IsMinimal(this Grid @this)
		{
			int hintCount = 0;
			var array = @this.ToArray();
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

			var tempArrays = new int[hintCount][];
			for (int i = 0; i < hintCount; i++)
			{
				var (r, c) = valueList.Dequeue();
				tempArrays[i] = (int[])array.Clone();
				tempArrays[i][CellUtils.GetOffset(r, c)] = 0;
			}

			var solver = new DancingLinksSolver();
			return tempArrays.All(gridValues =>
			{
				(_, bool hasSolved, _, _, _) = solver.Solve(gridValues);
				return !hasSolved;
			});
		}

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
	}
}
