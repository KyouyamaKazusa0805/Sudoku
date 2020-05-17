using System;
using System.Collections;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Encapsulates a <b>chute clue cover</b> (CCC) technique searcher.
	/// </summary>
	[TechniqueDisplay("Chute Clue Cover")]
	[HighAllocation]
	public sealed class CccTechniqueSearcher : LastResortTechniqueSearcher
	{
		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 90;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = false;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			SearchForFloors(accumulator, grid);
			//SearchForTowers(accumulator, grid);
		}

		/// <summary>
		/// Search for floors.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		private void SearchForFloors(IBag<TechniqueInfo> result, IReadOnlyGrid grid)
		{
			var series = (Span<int>)stackalloc int[27];
			for (int i = 0; i < 3; i++)
			{
				// Initialize.
				for (int o = 0; o < 27; o++)
				{
					// All elements in 'series' is in range 0 to 9,
					// where 0 is for empty cell.
					series[o] = grid[i * 27 + o] + 1;
				}

				// Get all partial solutions.
				var solutions = new List<int[]>();
				GetAllPartialSolutionsForFloorsRecursively(series.ToArray(), solutions, grid, i, 0);

				// Set all possible candidates.
				var bitmap = new BitArray(27 * 9);
				foreach (int[] solution in solutions)
				{
					for (int index = 0; index < 27; index++)
					{
						int digit = solution[index] - 1;
						bitmap[index * 9 + digit] = true;
					}
				}

				// Record all eliminations.
				var conclusions = new List<Conclusion>();
				int z = 0;
				foreach (bool? v in bitmap)
				{
					int cell = i * 27 + z / 9;
					int digit = z % 9;
					if (!(v ?? true) && grid.Exists(cell, digit) is true)
					{
						conclusions.Add(new Conclusion(ConclusionType.Elimination, cell, digit));
					}

					z++;
				}

				if (conclusions.Count == 0)
				{
					goto Label_GC;
				}

				result.Add(
					new CccTechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets: null,
								regionOffsets: new[]
								{
									(0, i * 3 + 9),
									(0, i * 3 + 10),
									(0, i * 3 + 11)
								},
								links: null)
						},
						count: solutions.Count));

			Label_GC:
				// Enable garbage collection.
				solutions.Clear();
				GC.Collect();
			}
		}

		/// <summary>
		/// Search for towers. The method has bugs to fix so I may not use this method.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		private void SearchForTowers(IBag<TechniqueInfo> result, IReadOnlyGrid grid)
		{
			var series = (Span<int>)stackalloc int[27];
			for (int i = 0; i < 3; i++)
			{
				// Initialize.
				for (int o = 0; o < 27; o++)
				{
					// All elements in 'series' is in range 0 to 9,
					// where 0 is for empty cell.
					series[o] = grid[o / 3 * 9 + o % 3 + i * 3] + 1;
				}

				// Get all partial solutions.
				var solutions = new List<int[]>();
				GetAllPartialSolutionsForTowersRecursively(series.ToArray(), solutions, grid, i, 0);

				// Set all possible candidates.
				var bitmap = new BitArray(27 * 9);
				foreach (int[] solution in solutions)
				{
					for (int index = 0; index < 27; index++)
					{
						int digit = solution[index] - 1;
						bitmap[index * 9 + digit] = true;
					}
				}

				// Record all eliminations.
				var conclusions = new List<Conclusion>();
				int z = 0;
				foreach (bool? v in bitmap)
				{
					int cell = z / 3 * 9 + z % 3 + i * 3;
					int digit = z % 9;
					if (!(v ?? true) && grid.Exists(cell, digit) is true)
					{
						conclusions.Add(new Conclusion(ConclusionType.Elimination, cell, digit));
					}

					z++;
				}

				if (conclusions.Count == 0)
				{
					goto Label_GC;
				}

				result.Add(
					new CccTechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets: null,
								regionOffsets: new[]
								{
									(0, i * 3 + 18),
									(0, i * 3 + 19),
									(0, i * 3 + 20)
								},
								links: null)
						},
						count: solutions.Count));

			Label_GC:
				// Enable garbage collection.
				solutions.Clear();
				GC.Collect();
			}
		}

		/// <summary>
		/// Get all partial solution for towers recursively.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <param name="solutions">All solutions.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="i">The current index.</param>
		/// <param name="n">The current cell to fill.</param>
		private static void GetAllPartialSolutionsForTowersRecursively(
			int[] series, IList<int[]> solutions, IReadOnlyGrid grid, int i, int n)
		{
			if (n == 27)
			{
				solutions.Add((int[])series.Clone());
				return;
			}

			if (series[n] != 0)
			{
				GetAllPartialSolutionsForTowersRecursively(series, solutions, grid, i, n + 1);
			}
			else
			{
				foreach (int digit in grid.GetCandidatesReversal(n / 3 * 9 + n % 3 + i * 3).GetAllSets())
				{
					series[n] = digit + 1;
					if (!IsValidTower(grid, series, i, n))
					{
						continue;
					}

					GetAllPartialSolutionsForTowersRecursively(series, solutions, grid, i, n + 1);
				}

				// Roll back.
				series[n] = 0;
			}
		}

		/// <summary>
		/// Get all partial solution for floors recursively.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <param name="solutions">All solutions.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="i">The current index.</param>
		/// <param name="n">The current cell to fill.</param>
		private static void GetAllPartialSolutionsForFloorsRecursively(
			int[] series, IList<int[]> solutions, IReadOnlyGrid grid, int i, int n)
		{
			if (n == 27)
			{
				solutions.Add((int[])series.Clone());
				return;
			}

			if (series[n] != 0)
			{
				GetAllPartialSolutionsForFloorsRecursively(series, solutions, grid, i, n + 1);
			}
			else
			{
				foreach (int digit in grid.GetCandidatesReversal(i * 27 + n).GetAllSets())
				{
					series[n] = digit + 1;
					if (!IsValidFloor(grid, series, n))
					{
						continue;
					}

					GetAllPartialSolutionsForFloorsRecursively(series, solutions, grid, i, n + 1);
				}

				// Roll back.
				series[n] = 0;
			}
		}

		/// <summary>
		/// Check whether the specified position is valid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="series">The series.</param>
		/// <param name="i">The current index.</param>
		/// <param name="n">The current cell to fill.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		private static bool IsValidTower(IReadOnlyGrid grid, int[] series, int i, int n)
		{
			int r = n / 3, c = n % 3 + i * 3;

			// Check rows.
			for (int o = 0; o < 9; o++)
			{
				if (o == c)
				{
					continue;
				}

				int currentCell = r * 9 + o;
				if (o < 3)
				{
					if (series[n] == series[currentCell])
					{
						return false;
					}
				}
				else
				{
					if (grid.GetStatus(currentCell) == CellStatus.Empty)
					{
						continue;
					}

					if (series[n] == grid[currentCell] + 1)
					{
						return false;
					}
				}
			}

			// Check columns.
			for (int o = 0; o < 9; o++)
			{
				if (o == r)
				{
					continue;
				}

				if (series[n] == series[o * 9 + c])
				{
					return false;
				}
			}

			// Check blocks.
			foreach (int currentCell in RegionCells[r / 3 * 3 + c / 3])
			{
				if (n == currentCell)
				{
					continue;
				}

				if (currentCell < 27)
				{
					if (series[n] == series[currentCell])
					{
						return false;
					}
				}
				else
				{
					if (grid.GetStatus(currentCell) == CellStatus.Empty)
					{
						continue;
					}

					if (series[n] == grid[currentCell] + 1)
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Check whether the specified position is valid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="series">The series.</param>
		/// <param name="n">The current cell to fill.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		private static bool IsValidFloor(IReadOnlyGrid grid, int[] series, int n)
		{
			int r = n / 9, c = n % 9;

			// Check rows.
			for (int o = 0; o < 9; o++)
			{
				if (o == c)
				{
					continue;
				}

				if (series[n] == series[r * 9 + o])
				{
					return false;
				}
			}

			// Check columns.
			for (int o = 0; o < 9; o++)
			{
				if (o == r)
				{
					continue;
				}

				int currentCell = o * 9 + c;
				if (o < 3)
				{
					if (series[n] == series[currentCell])
					{
						return false;
					}
				}
				else
				{
					if (grid.GetStatus(currentCell) == CellStatus.Empty)
					{
						continue;
					}

					if (series[n] == grid[currentCell] + 1)
					{
						return false;
					}
				}
			}

			// Check blocks.
			foreach (int currentCell in RegionCells[r / 3 * 3 + c / 3])
			{
				if (n == currentCell)
				{
					continue;
				}

				if (currentCell < 27)
				{
					if (series[n] == series[currentCell])
					{
						return false;
					}
				}
				else
				{
					if (grid.GetStatus(currentCell) == CellStatus.Empty)
					{
						continue;
					}

					if (series[n] == grid[currentCell] + 1)
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
