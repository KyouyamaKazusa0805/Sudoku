using Sudoku.Data.Meta;
using Sudoku.Solving;
using Sudoku.Solving.Bf.Dlx;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Sudoku.Checking
{
	public static class PuzzleAttributeChecker
	{
		public static bool IsUnique(this Grid grid, out Grid? resultIfUnique)
		{
			try
			{
				var result = new DancingLinksSolver().Solve(grid, out _);
				resultIfUnique = result;
				return result is not null;
			}
			catch
			{
				resultIfUnique = default;
				return false;
			}
		}

		public static bool IsMinimal(this Grid grid)
		{
			int hintCount = 0;
			var array = grid.ToArray();
			var valueList = new Queue<(int, int)>();
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					if (array[i, j] != 0)
					{
						hintCount++;
						valueList.Enqueue((i, j));
					}
				}
			}

			int[][,] tempArrays = new int[hintCount][,];
			for (int i = 0; i < hintCount; i++)
			{
				var (r, c) = valueList.Dequeue();
				tempArrays[i] = (int[,])array.Clone();
				tempArrays[i][r, c] = 0;
			}

			var solver = new DancingLinksSolver();
			return tempArrays.All(gridArray => solver.Solve(gridArray, out _) is null);
		}

		public static bool IsPearl(this Grid grid)
		{
			// Using a faster solver to check the grid is unique or not.
			if (grid.IsUnique(out _))
			{
				new ManualSolver().Solve(grid, out var info);
				var (er, pr) = (info.PuzzleDifficulty, info.PearlDifficulty);
				return er == pr;
			}
			else
			{
				// The puzzle does not have unique solution, neither pearl nor diamond one.
				return false;
			}
		}

		public static bool IsDiamond(this Grid grid)
		{
			// Using a faster solver to check the grid is unique or not.
			if (grid.IsUnique(out _))
			{
				new ManualSolver().Solve(grid, out var info);
				var (er, pr, dr) = (info.PuzzleDifficulty, info.PearlDifficulty, info.DiamondDifficulty);
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
