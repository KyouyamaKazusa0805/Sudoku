using System;
using Sudoku.Data.Meta;
using Sudoku.Solving.BruteForces.DancingLinks;

namespace Sudoku.Generating.Basic
{
	/// <summary>
	/// Provides a basic puzzle generator.
	/// </summary>
	public class BasicPuzzleGenerator : PuzzleGenerator
	{
		/// <summary>
		/// The random number generator (RNG).
		/// </summary>
		private static readonly Random Rng = new Random();

		/// <summary>
		/// The solver when checking whether the puzzle is unique.
		/// </summary>
		private static readonly DancingLinksSolver Solver = new DancingLinksSolver();


		/// <inheritdoc/>
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
