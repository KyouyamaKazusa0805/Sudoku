using System.Text;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Solving.BruteForces.Bitwise;

namespace Sudoku.Solving.Generating
{
	/// <summary>
	/// Provides an extended puzzle generator.
	/// </summary>
	public sealed class ExtendedPuzzleGenerator : PuzzleGenerator
	{
		/// <summary>
		/// A fast solver.
		/// </summary>
		private static readonly BitwiseSolver Solver = new BitwiseSolver();


		/// <inheritdoc/>
		public override IReadOnlyGrid Generate()
		{
			var puzzle = new StringBuilder() { Length = 81 };
			var solution = new StringBuilder() { Length = 81 };
			var emptyGridStr = new StringBuilder(Grid.EmptyString);
			while (true)
			{
				emptyGridStr.CopyTo(puzzle);
				emptyGridStr.CopyTo(solution);
				GenerateAnswerGrid(puzzle, solution);

				int[] holeCells = new int[81];
				CreatePattern(holeCells);
				for (int trial = 0; trial < 1000; trial++)
				{
					for (int cell = 0; cell < 81; cell++)
					{
						int p = holeCells[cell];
						char temp = solution[p];
						solution[p] = '0';

						if (!Solver.CheckValidity(solution.ToString(), out _))
						{
							// Reset the value.
							solution[p] = temp;
						}
					}

					if (Solver.CheckValidity(solution.ToString(), out _))
					{
						return Grid.Parse(solution.ToString());
					}

					RecreatePattern(holeCells);
				}
			}
		}

		/// <summary>
		/// To generate an answer grid.
		/// </summary>
		/// <param name="puzzle">The puzzle string.</param>
		/// <param name="solution">The solution string.</param>
		private static void GenerateAnswerGrid(StringBuilder puzzle, StringBuilder solution)
		{
			do
			{
				for (int i = 0; i < 81; i++)
				{
					puzzle[i] = '0';
				}

				var map = GridMap.Empty;
				for (int i = 0; i < 16; i++)
				{
					while (true)
					{
						int cell = Rng.Next(0, 81);
						if (!map[cell])
						{
							map[cell] = true;
							break;
						}
					}
				}

				foreach (int cell in map.Offsets)
				{
					do
					{
						puzzle[cell] = (char)(Rng.Next(1, 9) + '0');
					} while (CheckDuplicate(puzzle, cell));
				}
			} while (Solver.Solve(puzzle.ToString(), solution, 2) == 0);
		}

		/// <summary>
		/// Check whether the digit in its peer cells has duplicate ones.
		/// </summary>
		/// <param name="gridArray">The grid array.</param>
		/// <param name="cell">The cell.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		private static bool CheckDuplicate(StringBuilder gridArray, int cell)
		{
			char value = gridArray[cell];
			foreach (int c in new GridMap(cell, false).Offsets)
			{
				if (value != '0' && gridArray[c] == value)
				{
					return true;
				}
			}

			return false;
		}

		private static void CreatePattern(int[] pattern)
		{
			//[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static void swap<T>(ref T left, ref T right) { var temp = left; left = right; right = temp; }
			//[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static double rnd() => Rng.NextDouble();
			int[] box = { 0, 6, 54, 60, 3, 27, 33, 57, 30 };
			int[,] t = { { 0, 1, 2 }, { 0, 2, 1 }, { 1, 0, 2 }, { 1, 2, 0 }, { 2, 0, 1 }, { 2, 1, 0 } };

			int a = 54, b = 0;
			for (int i = 0; i < 9; i++)
			{
				int n = (int)(rnd() * 6);
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 3; k++)
					{
						pattern[(k == t[n, j] ? ref a : ref b)++] = box[i] + j * 9 + k;
					}
				}
			}

			for (int i = 23; i >= 0; i--)
			{
				swap(ref pattern[i], ref pattern[(int)((i + 1) * rnd())]);
			}
			for (int i = 47; i >= 24; i--)
			{
				swap(ref pattern[i], ref pattern[24 + (int)((i - 23) * rnd())]);
			}
			for (int i = 53; i >= 48; i--)
			{
				swap(ref pattern[i], ref pattern[48 + (int)((i - 47) * rnd())]);
			}
			for (int i = 80; i >= 54; i--)
			{
				swap(ref pattern[i], ref pattern[54 + (int)(27 * rnd())]);
			}
		}

		private static void RecreatePattern(int[] pattern)
		{
			//[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static void swap<T>(ref T left, ref T right) { var temp = left; left = right; right = temp; }
			//[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static double rnd() => Rng.NextDouble();

			for (int i = 23; i >= 0; i--)
			{
				swap(ref pattern[i], ref pattern[(int)((i + 1) * rnd())]);
			}
			for (int i = 47; i >= 24; i--)
			{
				swap(ref pattern[i], ref pattern[24 + (int)((i - 23) * rnd())]);
			}
			for (int i = 53; i >= 48; i--)
			{
				swap(ref pattern[i], ref pattern[48 + (int)((i - 47) * rnd())]);
			}
			for (int i = 80; i >= 54; i--)
			{
				swap(ref pattern[i], ref pattern[54 + (int)(27 * rnd())]);
			}
		}
	}
}
