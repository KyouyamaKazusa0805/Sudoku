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
		public override IReadOnlyGrid Generate() => Generate(17, 30, Central);

		/// <summary>
		/// Generate a puzzle with the specified information.
		/// </summary>
		/// <param name="min">The minimum hints of the puzzle.</param>
		/// <param name="max">The maximum hints of the puzzle.</param>
		/// <param name="symmetricalType">The symmetrical type.</param>
		/// <returns>The grid.</returns>
		public unsafe IReadOnlyGrid Generate(int min, int max, SymmetricalType symmetricalType)
		{
			char[] emptySeries = new char[82];
			char[] solutionArray = new char[82];
			char[] puzzleArray = new char[82];
			fixed (char *solutionPtr = solutionArray, puzzlePtr = puzzleArray)
			{
				do
				{
					Array.Copy(emptySeries, puzzleArray, 82);
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
							puzzlePtr[cell] = (char)(Rng.Next(1, 9) + '0');
						} while (CheckDuplicate(puzzleArray, cell));
					}
				} while (Solver.Solve(puzzlePtr, null, 2) == 0);

				// Now we remove some digits from the grid.
				do
				{
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
							solutionPtr[tCell] = '0';
							totalMap[tCell] = true;
							tempMap[tCell] = true;
						}
					} while (81 - totalMap.Count < min || 81 - totalMap.Count > max);
				} while (Solver.Solve(puzzlePtr, solutionPtr, 2) != 1);

				return Grid.Parse(new string(puzzlePtr));
			}
		}


		private static bool CheckDuplicate(char[] gridArray, int cell)
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
