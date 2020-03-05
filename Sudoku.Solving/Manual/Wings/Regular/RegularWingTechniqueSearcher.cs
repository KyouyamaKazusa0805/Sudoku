using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Wings.Regular
{
	/// <summary>
	/// Encapsulates a regular wing technique solver.
	/// </summary>
	public sealed class RegularWingTechniqueSearcher : WingTechniqueSearcher
	{
		/// <summary>
		/// The size.
		/// </summary>
		private readonly int _size;


		/// <summary>
		/// Initializes an instance with the specified size.
		/// </summary>
		/// <param name="size">The size.</param>
		public RegularWingTechniqueSearcher(int size) => _size = size;


		/// <inheritdoc/>
		public override int Priority { get; set; } = 42;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			// Search for all bivalue cells.
			var map = grid.GetBivalueCellsMap(out int count);
			var pair = (map, count);

			// Iterates on size.
			for (int size = 3; size <= _size; size++)
			{
				TakeAllBySize(accumulator, grid, in pair, size);
			}
		}


		/// <summary>
		/// Take all technique steps by the specified size.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="bivalueCellsMap">
		/// (<see langword="in"/> parameter) bivalue cell information pair.
		/// </param>
		/// <param name="size">The size.</param>
		/// <remarks>
		/// Parameter <paramref name="bivalueCellsMap"/> is passed by reference and cannot 
		/// be modified because the instance is a pair of values, passed by
		/// reference can make the value passing more simple.
		/// </remarks>
		private static void TakeAllBySize(
			IBag<TechniqueInfo> result, IReadOnlyGrid grid,
			in (GridMap _map, int _count) bivalueCellsMap, int size)
		{
			// Check bivalue cells.
			// If the number of bivalue cells is less than the specified size,
			// which means that the bivalue cells is not enough for construct
			// this technique structure, so we should return the empty list.
			if (bivalueCellsMap._count < size)
			{
				return;
			}

			// Start searching.
			int pivot = 0;
			foreach (var (status, mask) in grid)
			{
				int count = (~mask & 511).CountSet();
				if (status != CellStatus.Empty || count != size && count != size - 1)
				{
					goto Label_ContinueLoop;
				}

				var pivotPeersMap = new GridMap(pivot, false);
				var intersection = bivalueCellsMap._map & pivotPeersMap;
				if (intersection.Count < size - 1)
				{
					goto Label_ContinueLoop;
				}

				// Start awful 'for' loop.
				int[] offsets = intersection.ToArray();
				for (int i1 = 0, length = offsets.Length; i1 < length - size + 2; i1++)
				{
					int c1 = offsets[i1];
					for (int i2 = i1 + 1; i2 < length - size + 3; i2++)
					{
						int c2 = offsets[i2];
						if (size == 3)
						{
							// Record all values.
							int[] cells = new[] { c1, c2 };
							if (CheckWhetherBivalueCellsAreSame(grid, cells))
							{
								continue;
							}

							RecordValues(
								grid, pivot, cells, out short pivotMask,
								out short inter, out short union,
								out short interWithoutPivot, out short unionWithoutPivot);

							// Check.
							if ((~inter & 511).CountSet() == 3)
							{
								// Check whether uncompleted.
								int typeCount = (~union & 511).CountSet();
								if (typeCount != 0 && typeCount != 1)
								{
									continue;
								}

								bool isIncompleted =
									(~unionWithoutPivot & 511).CountSet() == 1
									&& (~union & 511).CountSet() == 0;

								// Regular wings found.
								// Check and find all eliminations.
								CheckAndSearchEliminations(
									pivotPeersMap, cells, pivot,
									isIncompleted, out var map);

								if (map.Count == 0)
								{
									// None of cells can be found and eliminated.
									continue;
								}

								// Check eliminations.
								var conclusions = GetConclusions(
									grid, union, unionWithoutPivot, isIncompleted, map);

								if (conclusions.Count != 0)
								{
									// Add to 'result'.
									GatherConclusion(
										grid, result, pivot, cells,
										pivotMask, inter, conclusions);
								}
							}
						}
						else // size > 3
						{
							for (int i3 = i2 + 1; i3 < length - size + 4; i3++)
							{
								int c3 = offsets[i3];
								if (size == 4)
								{
									// Record all values.
									int[] cells = new[] { c1, c2, c3 };
									if (CheckWhetherBivalueCellsAreSame(grid, cells))
									{
										continue;
									}

									RecordValues(
										grid, pivot, cells, out short pivotMask,
										out short inter, out short union,
										out short interWithoutPivot, out short unionWithoutPivot);

									// Check.
									if ((~inter & 511).CountSet() == 4)
									{
										// Check whether uncompleted.
										int typeCount = (~union & 511).CountSet();
										if (typeCount != 0 && typeCount != 1)
										{
											continue;
										}

										bool isIncompleted =
											(~unionWithoutPivot & 511).CountSet() == 1
											&& (~union & 511).CountSet() == 0;

										// Regular wings found.
										// Check and find all eliminations.
										CheckAndSearchEliminations(
											pivotPeersMap, cells, pivot,
											isIncompleted, out var map);

										if (map.Count == 0)
										{
											// None of cells can be found and eliminated.
											continue;
										}

										// Check eliminations.
										var conclusions = GetConclusions(
											grid, union, unionWithoutPivot, isIncompleted, map);

										if (conclusions.Count != 0)
										{
											// Add to 'result'.
											GatherConclusion(
												grid, result, pivot, cells,
												pivotMask, inter, conclusions);
										}
									}
								}
								else // size > 4
								{
									for (int i4 = i3 + 1; i4 < length - size + 5; i4++)
									{
										int c4 = offsets[i4];
										if (size == 5)
										{
											// Record all values.
											int[] cells = new[] { c1, c2, c3, c4 };
											if (CheckWhetherBivalueCellsAreSame(grid, cells))
											{
												continue;
											}

											RecordValues(
												grid, pivot, cells, out short pivotMask,
												out short inter, out short union,
												out short interWithoutPivot, out short unionWithoutPivot);

											// Check.
											if ((~inter & 511).CountSet() == 5)
											{
												// Check whether uncompleted.
												int typeCount = (~union & 511).CountSet();
												if (typeCount != 0 && typeCount != 1)
												{
													continue;
												}

												bool isIncompleted =
													(~unionWithoutPivot & 511).CountSet() == 1
													&& (~union & 511).CountSet() == 0;

												// Regular wings found.
												// Check and find all eliminations.
												CheckAndSearchEliminations(
													pivotPeersMap, cells, pivot,
													isIncompleted, out var map);

												if (map.Count == 0)
												{
													// None of cells can be found and eliminated.
													continue;
												}

												// Check eliminations.
												var conclusions = GetConclusions(
													grid, union, unionWithoutPivot,
													isIncompleted, map);

												if (conclusions.Count != 0)
												{
													// Add to 'result'.
													GatherConclusion(
														grid, result, pivot, cells,
														pivotMask, inter, conclusions);
												}
											}
										}
										else // size > 5
										{
											throw new NotSupportedException(
												"Because of the searching is too deep " +
												"so I do not write code for the case " +
												"when size is greater than 5.");
										}
									}
								}
							}
						}
					}
				}

			Label_ContinueLoop:
				pivot++;
			}
		}

		/// <summary>
		/// Check whether the two of all bivalue cells has whole same candidates.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="cells">The cells to check.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		private static bool CheckWhetherBivalueCellsAreSame(IReadOnlyGrid grid, int[] cells)
		{
			for (int i = 0, length = cells.Length; i < length - 1; i++)
			{
				for (int j = i + 1; j < length; j++)
				{
					if (grid.GetMask(cells[i]) == grid.GetMask(cells[j]))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Check and search eliminations.
		/// </summary>
		/// <param name="pivotPeersMap">
		/// The grid map for the peers of the pivot cell.
		/// </param>
		/// <param name="cells">All body cells.</param>
		/// <param name="pivot">The pivot cell.</param>
		/// <param name="isIncompleted">
		/// (<see langword="out"/> parameter) Indicates whether the technique is uncompleted.
		/// </param>
		/// <param name="map">
		/// (<see langword="out"/> parameter) The result grid intersection map.
		/// </param>
		private static void CheckAndSearchEliminations(
			GridMap pivotPeersMap, int[] cells,
			int pivot, bool isIncompleted, out GridMap map)
		{
			int firstCell = cells[0];
			map = new GridMap(firstCell, false);
			for (int i = 1; i < cells.Length; i++)
			{
				int cell = cells[i];
				map &= new GridMap(cell, false);
			}
			if (!isIncompleted)
			{
				map &= pivotPeersMap;
			}

			// Pivot cell may be recorded into the 'map'.
			map[pivot] = false;
		}

		/// <summary>
		/// Gather the conclusion.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="result">The result.</param>
		/// <param name="pivot">The pivot cell.</param>
		/// <param name="cells">All body cells.</param>
		/// <param name="pivotMask">The mask of the pivot cell.</param>
		/// <param name="inter">The intersection mask.</param>
		/// <param name="conclusions">The conclusions.</param>
		private static void GatherConclusion(
			IReadOnlyGrid grid, IBag<TechniqueInfo> result,
			int pivot, int[] cells, short pivotMask,
			short inter, IReadOnlyList<Conclusion> conclusions)
		{
			result.Add(
				new RegularWingTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets:
								new List<(int, int)>(
									from cell in cells
									select (0, cell)),
							candidateOffsets:
								new List<(int, int)>((
									from digit in Enumerable.Range(0, 9)
									from cell in cells
									where !grid[cell, digit]
									select (0, cell * 9 + digit)).Concat(
									from digit in Enumerable.Range(0, 9)
									where !grid[pivot, digit]
									select (0, pivot * 9 + digit))),
							regionOffsets: null,
							linkMasks: null)
					},
					pivot,
					pivotCandidatesCount: (~pivotMask & 511).CountSet(),
					digits: (~inter & 511).GetAllSets().ToArray(),
					cellOffsets: cells));
		}

		/// <summary>
		/// Get all conclusions.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="union">The union mask.</param>
		/// <param name="unionWithoutPivot">The union mask without using pivot mask.</param>
		/// <param name="isIncompleted">
		/// Indicates whether the technique is uncompleted.
		/// </param>
		/// <param name="map">The intersection grid map.</param>
		/// <returns>The conclusions.</returns>
		private static IReadOnlyList<Conclusion> GetConclusions(
			IReadOnlyGrid grid, short union, short unionWithoutPivot,
			bool isIncompleted, GridMap map)
		{
			var conclusions = new List<Conclusion>();
			int valueToCheck = isIncompleted ? unionWithoutPivot : union;
			int zDigit = (~valueToCheck & 511).FindFirstSet();
			if (zDigit == -1)
			{
				// No possible value to eliminate.
				return conclusions;
			}
			foreach (int offset in map.Offsets)
			{
				if (grid.CandidateExists(offset, zDigit))
				{
					conclusions.Add(new Conclusion(ConclusionType.Elimination, offset, zDigit));
				}
			}

			return conclusions;
		}

		/// <summary>
		/// Record all values.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="pivot">The pivot cell.</param>
		/// <param name="cells">All body cells.</param>
		/// <param name="pivotMask">
		/// (<see langword="out"/> parameter) The mask of the pivot cell.
		/// </param>
		/// <param name="inter">(<see langword="out"/> parameter) The intersection mask.</param>
		/// <param name="union">(<see langword="out"/> parameter) The union mask.</param>
		/// <param name="interWithoutPivot">
		/// (<see langword="out"/> parameter) The intersection mask without pivot mask.
		/// </param>
		/// <param name="unionWithoutPivot">
		/// (<see langword="out"/> parameter) The union mask without pivot mask.
		/// </param>
		private static void RecordValues(
			IReadOnlyGrid grid, int pivot, int[] cells, out short pivotMask,
			out short inter, out short union,
			out short interWithoutPivot, out short unionWithoutPivot)
		{
			(pivotMask, inter, union) = (grid.GetCandidates(pivot), 511, 0);
			for (int i = 0, length = cells.Length; i < length; i++)
			{
				short tempMask = grid.GetCandidates(cells[i]);
				inter &= tempMask;
				union |= tempMask;
			}
			(interWithoutPivot, unionWithoutPivot) = (inter, union);
			inter &= pivotMask;
			union |= pivotMask;
		}
	}
}
