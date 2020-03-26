using System;
using System.Collections;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Encapsulates a <b>chute clue cover</b> (CCC) technique searcher.
	/// </summary>
	[Slow, HighAllocation, TechniqueDisplay("Chute Clue Cover")]
	public sealed class CccTechniqueSearcher : LastResortTechniqueSearcher
	{
		/// <inheritdoc/>
		public static int Priority { get; set; } = 90;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			SearchForFloors(accumulator, grid);
			//SearchForTowers(accumulator, grid);
		}


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
					if (!(v ?? true) && grid.CandidateExists(cell, digit))
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
								regionOffsets: null,
								links: null)
						},
						count: solutions.Count));

			Label_GC:
				// Enable garbage collection.
				solutions.Clear();
				GC.Collect();
			}
		}

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
					if (!(v ?? true) && grid.CandidateExists(cell, digit))
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
								regionOffsets: null,
								links: null)
						},
						count: solutions.Count));

			Label_GC:
				// Enable garbage collection.
				solutions.Clear();
				GC.Collect();
			}
		}

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
					if (grid.GetCellStatus(currentCell) == CellStatus.Empty)
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
			foreach (int currentCell in GridMap.GetCellsIn(r / 3 * 3 + c / 3))
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
					if (grid.GetCellStatus(currentCell) == CellStatus.Empty)
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
					if (grid.GetCellStatus(currentCell) == CellStatus.Empty)
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
			foreach (int currentCell in GridMap.GetCellsIn(r / 3 * 3 + c / 3))
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
					if (grid.GetCellStatus(currentCell) == CellStatus.Empty)
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
