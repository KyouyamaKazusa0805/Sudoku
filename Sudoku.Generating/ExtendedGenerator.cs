using System;
using System.Text;
using Sudoku.Data;
using Sudoku.Solving.BruteForces.Bitwise;
using static Sudoku.Data.SymmetricalType;

namespace Sudoku.Generating
{
	/// <summary>
	/// Encapsulates an extended puzzle generator.
	/// </summary>
	public sealed class ExtendedGenerator : PuzzleGenerator
	{
		/// <summary>
		/// The random number generator.
		/// </summary>
		private static readonly Random Rng = new Random();

		/// <summary>
		/// The dancing links solver.
		/// </summary>
		private static readonly BitwiseSolver Solver = new BitwiseSolver();


		/// <inheritdoc/>
		public override IReadOnlyGrid Generate() => Generate(28, Central);

		/// <summary>
		/// Generate a puzzle with the specified information.
		/// </summary>
		/// <param name="max">The maximum hints of the puzzle.</param>
		/// <param name="symmetricalType">The symmetrical type.</param>
		/// <returns>The grid.</returns>
		public IReadOnlyGrid Generate(int max, SymmetricalType symmetricalType)
		{
			var puzzle = new StringBuilder(Grid.EmptyString);
			var solution = new StringBuilder(Grid.EmptyString);
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

			// Now we remove some digits from the grid.
			var tempSb = new StringBuilder(solution.ToString());
			string result;
			do
			{
				for (int i = 0; i < 81; i++)
				{
					solution[i] = tempSb[i];
				}

				var totalMap = GridMap.Empty;
				do
				{
					int cell;
					do
					{
						cell = Rng.Next(0, 81);
					} while (totalMap[cell]);

					int r = cell / 9, c = cell % 9;
					int[] series = symmetricalType switch
					{
						Central => new[] { r * 9 + c, (8 - r) * 9 + 8 - c },
						Diagonal => new[] { r * 9 + c, c * 9 + r },
						AntiDiagonal => new[] { r * 9 + c, (8 - c) * 9 + 8 - r },
						XAxis => new[] { r * 9 + c, (8 - r) * 9 + c },
						YAxis => new[] { r * 9 + c, r * 9 + 8 - c },
						_ => Array.Empty<int>()
					};

					// Get new value of 'last'.
					var tempMap = GridMap.Empty;
					foreach (int tCell in series)
					{
						solution[tCell] = '0';
						totalMap[tCell] = true;
						tempMap[tCell] = true;
					}
				} while (81 - totalMap.Count > max);
			} while (!Solver.CheckValidity(result = solution.ToString(), out _));

			return Grid.Parse(result);
		}


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
	}
}
