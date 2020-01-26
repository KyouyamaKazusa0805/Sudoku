using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
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
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<RegularWingTechniqueInfo>();

			// Check all bivalue cells.
			(GridMap _map, int _count) pair = (new GridMap(), 0);
			int i = 0;
			foreach (var (status, mask) in grid)
			{
				if (status == CellStatus.Empty && (~mask & 511).CountSet() == 2)
				{
					pair._map[i] = true;
					pair._count++; // 'count' is only used for quickening the code running.
				}

				i++;
			}

			// Iterates on size.
			for (int size = 3; size <= _size; size++)
			{
				result.AddRange(TakeAllBySize(grid, in pair, size));
			}

			return result;
		}


		/// <summary>
		/// Take all technique steps by the specified size.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="bivalueCellsMap">(in parameter) bivalue cell information pair.</param>
		/// <param name="size">The size.</param>
		/// <returns>All technique steps.</returns>
		/// <remarks>
		/// Parameter <paramref name="bivalueCellsMap"/> is passed by reference and cannot 
		/// be modified because the instance is a pair of values, passed by
		/// reference can make the value passing more simple.
		/// </remarks>
		private static IReadOnlyList<RegularWingTechniqueInfo> TakeAllBySize(
			Grid grid, in (GridMap _map, int _count) bivalueCellsMap, int size)
		{
			// Check bivalue cells.
			// If the number of bivalue cells is less than the specified size,
			// which means that the bivalue cells is not enough for construct
			// this technique structure, so we should return the empty list.
			if (bivalueCellsMap._count < size)
			{
				return Array.Empty<RegularWingTechniqueInfo>();
			}

			// Start searching.
			var result = new List<RegularWingTechniqueInfo>();
			int pivot = 0;
			foreach (var (status, mask) in grid)
			{
				int count = (~mask & 511).CountSet();
				if (status != CellStatus.Empty || count != size && count != size - 1)
				{
					goto Label_ContinueLoop;
				}

				var pivotPeersMap = new GridMap(pivot) { [pivot] = false };
				var intersection = bivalueCellsMap._map & pivotPeersMap;
				if (intersection.Count < size)
				{
					goto Label_ContinueLoop;
				}

				// Start awful 8-layer for loop.
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
							short pivotMask = (short)(grid.GetMask(pivot) & 511);
							int[] cells = new[] { c1, c2 };
							short inter = 511, union = 0;
							for (int i = 0; i < 2; i++)
							{
								short tempMask = (short)(grid.GetMask(cells[i]) & 511);
								inter &= tempMask;
								union |= tempMask;
							}
							short interWithoutPivot = inter;
							short unionWithoutPivot = union;
							inter &= pivotMask;
							union |= pivotMask;

							if ((~inter & 511).CountSet() == 3)
							{
								// Check whether incompleted.
								int typeCount = (~union & 511).CountSet();
								if (typeCount != 0 && typeCount != 1)
								{
									continue;
								}

								// Regular wings found.
								// Check and find all eliminations.
								bool isIncompleted = typeCount == 0;
								int firstCell = cells[0];
								var map = new GridMap(firstCell) { [firstCell] = false };
								for (int i = 1; i < cells.Length; i++)
								{
									int cell = cells[i];
									map &= new GridMap(cell) { [cell] = false };
								}
								if (!isIncompleted)
								{
									map &= pivotPeersMap;
								}

								if (map.Count == 0)
								{
									// None of cells can be found and eliminated.
									continue;
								}

								// Check eliminations.
								var conclusions = new List<Conclusion>();
								int zDigit = (isIncompleted ? ~unionWithoutPivot & 511 : ~union & 511).FindFirstSet();
								foreach (int offset in map.Offsets)
								{
									if (grid.CandidateExists(offset, zDigit))
									{
										conclusions.Add(
											new Conclusion(
												ConclusionType.Elimination, offset, zDigit));
									}
								}

								if (conclusions.Count != 0)
								{
									// Add to 'result'.
									result.Add(
										new RegularWingTechniqueInfo(
											conclusions,
											views: new List<View>
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
							}
						}
						else // size > 3
						{

						}
					}
				}

			Label_ContinueLoop:
				pivot++;
			}

			return result;
		}
	}
}
