using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Wings.Regular
{
	/// <summary>
	/// Encapsulates a <b>regular wing</b> technique solver.
	/// </summary>
	[TechniqueDisplay("Regular Wing")]
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


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 42;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			// Iterates on size.
			for (int size = 3; size <= _size; size++)
			{
				TakeAllBySize(accumulator, grid, size);
			}
		}


		/// <summary>
		/// Take all technique steps by the specified size.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="size">The size.</param>
		private static void TakeAllBySize(IBag<TechniqueInfo> result, IReadOnlyGrid grid, int size)
		{
			// Check bivalue cells.
			// If the number of bivalue cells is less than the specified size,
			// which means that the bivalue cells is not enough for construct
			// this technique structure, so we should return the empty list.
			if (BivalueMap.Count < size)
			{
				return;
			}

			// Start searching.
			var span2 = (Span<int>)stackalloc int[2];
			var span3 = (Span<int>)stackalloc int[3];
			var span4 = (Span<int>)stackalloc int[4];
			int pivot = 0;
			foreach (var (status, mask) in grid)
			{
				int count = (~mask & 511).CountSet();
				if (status != CellStatus.Empty || count != size && count != size - 1)
				{
					goto Label_ContinueLoop;
				}

				var pivotPeersMap = new GridMap(pivot, false);
				var intersection = BivalueMap & pivotPeersMap;
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
							(span2[0], span2[1]) = (c1, c2);
							if (CheckWhetherBivalueCellsAreSame(grid, span2))
							{
								continue;
							}

							RecordValues(
								grid, pivot, span2, out short pivotMask,
								out short inter, out short union,
								out short interWithoutPivot, out short unionWithoutPivot);

							// Check.
							if ((~inter & 511).CountSet() == 3)
							{
								// Check whether incompleted.
								int typeCount = (~union & 511).CountSet();
								if (typeCount != 0 && typeCount != 1)
								{
									continue;
								}

								bool isIncompleted =
									(~unionWithoutPivot & 511).CountSet() == 1 && (~union & 511).CountSet() == 0;

								// Regular wings found.
								// Check and find all eliminations.
								CheckAndSearchEliminations(pivotPeersMap, span2, pivot, isIncompleted, out var map);

								if (map.IsEmpty)
								{
									// None of cells can be found and eliminated.
									continue;
								}

								// Check eliminations.
								var conclusions = GetConclusions(grid, union, unionWithoutPivot, isIncompleted, map);

								if (conclusions.Count != 0)
								{
									// Add to 'result'.
									GatherConclusion(grid, result, pivot, span2, pivotMask, inter, conclusions);
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
									(span3[0], span3[1], span3[2]) = (c1, c2, c3);
									if (CheckWhetherBivalueCellsAreSame(grid, span3))
									{
										continue;
									}

									RecordValues(
										grid, pivot, span3, out short pivotMask,
										out short inter, out short union,
										out short interWithoutPivot, out short unionWithoutPivot);

									// Check.
									if ((~inter & 511).CountSet() == 4)
									{
										// Check whether incompleted.
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
											pivotPeersMap, span3, pivot,isIncompleted, out var map);

										if (map.IsEmpty)
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
											GatherConclusion(grid, result, pivot, span3,pivotMask, inter, conclusions);
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
											(span4[0], span4[1], span4[2], span4[3]) = (c1, c2, c3, c4);
											if (CheckWhetherBivalueCellsAreSame(grid, span4))
											{
												continue;
											}

											RecordValues(
												grid, pivot, span4, out short pivotMask,
												out short inter, out short union,
												out short interWithoutPivot, out short unionWithoutPivot);

											// Check.
											if ((~inter & 511).CountSet() == 5)
											{
												// Check whether incompleted.
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
													pivotPeersMap, span4, pivot, isIncompleted, out var map);

												if (map.IsEmpty)
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
														grid, result, pivot, span4, pivotMask, inter, conclusions);
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
		private static bool CheckWhetherBivalueCellsAreSame(IReadOnlyGrid grid, ReadOnlySpan<int> cells)
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
		/// <param name="isIncompleted">Indicates whether the technique is incompleted.</param>
		/// <param name="map">
		/// (<see langword="out"/> parameter) The result grid intersection map.
		/// </param>
		private static void CheckAndSearchEliminations(
			GridMap pivotPeersMap, ReadOnlySpan<int> cells, int pivot, bool isIncompleted, out GridMap map)
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
			map.Remove(pivot);
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
			IReadOnlyGrid grid, IBag<TechniqueInfo> result, int pivot, ReadOnlySpan<int> cells,
			short pivotMask, short inter, IReadOnlyList<Conclusion> conclusions)
		{
			int[] cellsArray = cells.ToArray();
			int elimDigit = conclusions[0].Digit;
			result.Add(
				new RegularWingTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: null,
							candidateOffsets:
								new List<(int, int)>((
									from cell in cellsArray
									from digit in grid.GetCandidatesReversal(cell).GetAllSets()
									select (digit == elimDigit ? 1 : 0, cell * 9 + digit)).Concat(
									from digit in grid.GetCandidatesReversal(pivot).GetAllSets()
									select (digit == elimDigit ? 1 : 0, pivot * 9 + digit))),
							regionOffsets: null,
							links: null)
					},
					pivot,
					pivotCandidatesCount: (~pivotMask & 511).CountSet(),
					digits: (~inter & 511).GetAllSets().ToArray(),
					cellOffsets: cellsArray));
		}

		/// <summary>
		/// Get all conclusions.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="union">The union mask.</param>
		/// <param name="unionWithoutPivot">The union mask without using pivot mask.</param>
		/// <param name="isIncompleted">
		/// Indicates whether the technique is incompleted.
		/// </param>
		/// <param name="map">The intersection grid map.</param>
		/// <returns>The conclusions.</returns>
		private static IReadOnlyList<Conclusion> GetConclusions(
			IReadOnlyGrid grid, short union, short unionWithoutPivot, bool isIncompleted, GridMap map)
		{
			var conclusions = new List<Conclusion>();
			int valueToCheck = isIncompleted ? unionWithoutPivot : union;
			int zDigit = (~valueToCheck & 511).FindFirstSet();
			if (zDigit == -1)
			{
				// No possible value to eliminate.
				return conclusions;
			}
			foreach (int offset in map)
			{
				if (grid.Exists(offset, zDigit) is true)
				{
					conclusions.Add(new Conclusion(Elimination, offset, zDigit));
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
			IReadOnlyGrid grid, int pivot, ReadOnlySpan<int> cells, out short pivotMask,
			out short inter, out short union, out short interWithoutPivot, out short unionWithoutPivot)
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
