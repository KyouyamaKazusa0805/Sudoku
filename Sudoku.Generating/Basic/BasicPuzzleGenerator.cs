using System;
using Sudoku.Data.Meta;
using Sudoku.Solving;
using Sudoku.Solving.BruteForces.DancingLinks;

namespace Sudoku.Generating.Basic
{
	public class BasicPuzzleGenerator : PuzzleGenerator
	{
		private static readonly Random Rng = new Random();

		private static readonly DancingLinksSolver Solver = new DancingLinksSolver();


		public override Grid Generate()
		{
			static bool[] GetMask(out int count)
			{
				bool[] result = new bool[81];
				count = Rng.Next(18, 30);
				for (int i = count - 1; i >= 0; i--)
				{
					int pos;
					do
					{
						pos = Rng.Next(0, 80);
					} while (result[pos]);
					result[pos] = true;
				}

				return result;
			}

			bool hasSolved;
			bool[] pattern = GetMask(out int count);
			int[] series = new int[81];
			do
			{
				Array.Clear(series, 0, series.Length);

				for (int i = 0; i < count; i++)
				{
					if (pattern[i])
					{
						series[i] = Rng.Next(1, 9);
					}
				}

				(_, hasSolved, _) = Solver.Solve(series);
			} while (!hasSolved);

			return Grid.CreateInstance(series);
		}
	}
}
