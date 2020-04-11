using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Utils;
using static Sudoku.GridProcessings;
using Action = System.Action<System.Collections.Generic.IBag<Sudoku.Solving.TechniqueInfo>, Sudoku.Data.IReadOnlyGrid, int>;

namespace Sudoku.Solving.Manual.Subsets
{
	/// <summary>
	/// Encapsulates a <b>subset</b> technique searcher.
	/// </summary>
	[TechniqueDisplay("Subsets")]
	public sealed class SubsetTechniqueSearcher : TechniqueSearcher
	{
		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 30;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			for (int size = 2; size <= 4; size++)
			{
				foreach (var act in new Action[] { TakeAllNakedSubsetsBySize, TakeAllHiddenSubsetsBySize })
				{
					act(accumulator, grid, size);
				}
			}
		}

		/// <summary>
		/// Get all naked subsets technique information, for searching the specified size.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="size">The size.</param>
		/// <returns>All technique information searched.</returns>
		private static void TakeAllNakedSubsetsBySize(
			IBag<TechniqueInfo> result, IReadOnlyGrid grid, int size)
		{
			// Iterates on each region.
			for (int region = 0; region < 27; region++)
			{
				// Level 1 (Get first cell mask).
				for (int i1 = 0; i1 < 10 - size; i1++)
				{
					int pos1 = RegionUtils.GetCellOffset(region, i1);
					if (grid.GetCellStatus(pos1) != CellStatus.Empty)
					{
						continue;
					}

					// Level 2.
					short mask1 = grid.GetMask(pos1);
					for (int i2 = i1 + 1; i2 < 11 - size; i2++)
					{
						int pos2 = RegionUtils.GetCellOffset(region, i2);
						if (grid.GetCellStatus(pos2) != CellStatus.Empty)
						{
							continue;
						}

						short mask2 = (short)(grid.GetMask(pos2) & mask1);
						if (size == 2)
						{
							// Check if is naked pair or not.
							int mask = mask2 & 511;
							if (mask.CountSet() != 7)
							{
								continue;
							}

							// Naked pair found.
							var digits = new List<int>((511 & ~mask2).GetAllSets());
							var offsets = new[] { pos1, pos2 };
							var conclusions =
								GetNakedSubsetConclusions(
									grid, offsets, digits, out bool? isLocked);

							if (conclusions.Count == 0)
							{
								continue;
							}

							// Gather this conclusion.
							GatherConclusion(
								grid, result, region, digits,
								offsets, conclusions, isLocked);
						}
						else // size > 2
						{
							// Level 3.
							for (int i3 = i2 + 1; i3 < 12 - size; i3++)
							{
								int pos3 = RegionUtils.GetCellOffset(region, i3);
								if (grid.GetCellStatus(pos3) != CellStatus.Empty)
								{
									continue;
								}

								short mask3 = (short)(grid.GetMask(pos3) & mask2);
								if (size == 3)
								{
									// Check if is naked triple or not.
									int mask = mask3 & 511;
									if (mask.CountSet() != 6)
									{
										continue;
									}

									// Naked triple found.
									var digits = new List<int>((511 & ~mask3).GetAllSets());
									var offsets = new[] { pos1, pos2, pos3 };
									var conclusions =
										GetNakedSubsetConclusions(
											grid, offsets, digits, out bool? isLocked);

									if (conclusions.Count == 0)
									{
										continue;
									}

									GatherConclusion(
										grid, result, region, digits,
										offsets, conclusions, isLocked);
								}
								else // size == 4
								{
									// Level 4. Final level.
									for (int i4 = i3 + 1; i4 < 9; i4++)
									{
										int pos4 = RegionUtils.GetCellOffset(region, i4);
										if (grid.GetCellStatus(pos4) != CellStatus.Empty)
										{
											continue;
										}

										short mask4 = (short)(grid.GetMask(pos4) & mask3);

										// Check if is naked quadruple or not.
										int mask = mask4 & 511;
										if (mask.CountSet() != 5)
										{
											continue;
										}

										// Naked triple found.
										var digits = new List<int>((511 & ~mask4).GetAllSets());
										var offsets = new[] { pos1, pos2, pos3, pos4 };
										var conclusions =
											GetNakedSubsetConclusions(
												grid, offsets, digits, out bool? isLocked);

										if (conclusions.Count == 0)
										{
											continue;
										}

										GatherConclusion(
											grid, result, region, digits,
											offsets, conclusions, isLocked);
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// To gather a conclusion with essential information.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="result">The information instances searched.</param>
		/// <param name="region">The region offset.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="offsets">The cell offsets.</param>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="isLocked">Indicates whether the subset is locked.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void GatherConclusion(
			IReadOnlyGrid grid, IBag<TechniqueInfo> result, int region,
			IReadOnlyList<int> digits, IReadOnlyList<int> offsets,
			IReadOnlyList<Conclusion> conclusions, bool? isLocked)
		{
			result.Add(
				new NakedSubsetTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: null,
							candidateOffsets: GetNakedSubsetsHighlightedCandidateOffsets(grid, offsets, digits),
							regionOffsets: new[] { (0, region) },
							links: null)
					},
					regionOffset: region,
					cellOffsets: offsets,
					digits,
					isLocked));
		}

		/// <summary>
		/// Get conclusions after searched a subset technique.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="offsets">All cell offsets.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="isLocked">
		/// (<see langword="out"/> parameter) Indicates whether the subset is locked.
		/// </param>
		/// <returns>All conclusions.</returns>
		private static IReadOnlyList<Conclusion> GetNakedSubsetConclusions(
			IReadOnlyGrid grid, IReadOnlyList<int> offsets,
			IReadOnlyList<int> digits, out bool? isLocked)
		{
			var result = new List<Conclusion>();
			int count = digits.Count;
			bool[] series = new bool[count];
			for (int i = 0; i < count; i++)
			{
				// Get first offset whose value has been set.
				int digit = digits[i];
				int firstOffset = offsets[0], firstOffsetIndex = 0;
				for (int j = 0; j < offsets.Count; j++)
				{
					int offset = offsets[j];
					if (!grid[offset, digit])
					{
						firstOffset = offset;
						firstOffsetIndex = j;
						break;
					}
				}

				// Get intersection by each digit.
				var tempMap = new GridMap(firstOffset, false);
				for (int j = firstOffsetIndex + 1; j < offsets.Count; j++)
				{
					int offset = offsets[j];
					if (!grid[offset, digit])
					{
						tempMap &= new GridMap(offset, false);
					}
				}
				series[i] = tempMap.Count > 9;

				// Set bit 'false' on themselves (do not regard them as eliminations).
				foreach (int offset in offsets)
				{
					tempMap.Remove(offset);
				}

				// Add eliminations by each digit.
				foreach (int offset in tempMap.Offsets)
				{
					if (grid.Exists(offset, digit) is true)
					{
						result.Add(new Conclusion(ConclusionType.Elimination, offset, digit));
					}
				}
			}
			bool? isLockedTemp = series.Any(v => v) ? series.All(v => v) : (bool?)null;
			isLocked = digits.Count == 4
				? isLockedTemp != true ? isLockedTemp : false
				: isLockedTemp;
			return result;
		}

		/// <summary>
		/// Get all candidate offsets highlight in a specified naked subset technique.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="offsets">All cell offsets.</param>
		/// <param name="digits">All digits.</param>
		/// <returns>All candidate offsets and its ID.</returns>
		private static IReadOnlyList<(int, int)> GetNakedSubsetsHighlightedCandidateOffsets(
			IReadOnlyGrid grid, IReadOnlyList<int> offsets, IReadOnlyList<int> digits)
		{
			var result = new List<(int, int)>();
			foreach (int offset in offsets)
			{
				foreach (int digit in digits)
				{
					if (!grid[offset, digit])
					{
						// The candidate does exist in the current grid,
						// which should be highlight.
						result.Add((0, offset * 9 + digit));
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Get all hidden subsets technique information, for searching the specified size.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="size">The size.</param>
		/// <returns>All technique information searched.</returns>
		private static void TakeAllHiddenSubsetsBySize(
			IBag<TechniqueInfo> result, IReadOnlyGrid grid, int size)
		{
			for (int region = 0; region < 27; region++)
			{
				for (int d1 = 0; d1 < 10 - size; d1++)
				{
					if (grid.HasDigitValue(d1, region))
					{
						continue;
					}

					short mask = grid.GetDigitAppearingMask(d1, region);
					for (int d2 = d1 + 1; d2 < 11 - size; d2++)
					{
						if (grid.HasDigitValue(d2, region))
						{
							continue;
						}

						short mask2 = (short)(grid.GetDigitAppearingMask(d2, region) | mask);
						if (size == 2)
						{
							if (mask2.CountSet() != 2)
							{
								continue;
							}

							// Hidden pair found.
							var digits = new[] { d1, d2 };
							var conclusions =
								GetHiddenSubsetsConclusions(
									grid, region, mask2, digits, out var cellOffsets,
									out var highlightedCandidates);

							if (conclusions.Count == 0)
							{
								continue;
							}

							result.Add(
								new HiddenSubsetTechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: null,
											candidateOffsets: highlightedCandidates,
											regionOffsets: new[]
											{
												(0, region)
											},
											links: null)
									},
									regionOffset: region,
									cellOffsets,
									digits));
						}
						else // size > 2
						{
							for (int d3 = d2 + 1; d3 < 12 - size; d3++)
							{
								if (grid.HasDigitValue(d3, region))
								{
									continue;
								}

								short mask3 = (short)(grid.GetDigitAppearingMask(d3, region) | mask2);
								if (size == 3)
								{
									if (mask3.CountSet() != 3)
									{
										continue;
									}

									// Hidden triple found.
									var digits = new[] { d1, d2, d3 };
									var conclusions =
										GetHiddenSubsetsConclusions(
											grid, region, mask3, digits, out var cellOffsets,
											out var highlightedCandidates);

									if (conclusions.Count == 0)
									{
										continue;
									}

									result.Add(
										new HiddenSubsetTechniqueInfo(
											conclusions,
											views: new[]
											{
												new View(
													cellOffsets: null,
													candidateOffsets: highlightedCandidates,
													regionOffsets: new[]
													{
														(0, region)
													},
													links: null)
											},
											regionOffset: region,
											cellOffsets,
											digits));
								}
								else
								{
									for (int d4 = d3 + 1; d4 < 9; d4++)
									{
										if (grid.HasDigitValue(d4, region))
										{
											continue;
										}

										// 'size == 4' is always true.
										// Now check hidden quadruple.
										short mask4 = (short)(grid.GetDigitAppearingMask(d4, region) | mask3);
										if (mask4.CountSet() != 4)
										{
											continue;
										}

										// Hidden quadruple found.
										var digits = new[] { d1, d2, d3, d4 };
										var conclusions =
											GetHiddenSubsetsConclusions(
												grid, region, mask4, digits, out var cellOffsets,
												out var highlightedCandidates);

										if (conclusions.Count == 0)
										{
											continue;
										}

										result.Add(
											new HiddenSubsetTechniqueInfo(
												conclusions,
												views: new[]
												{
													new View(
														cellOffsets: null,
														candidateOffsets: highlightedCandidates,
														regionOffsets: new[]
														{
															(0, region)
														},
														links: null)
												},
												regionOffset: region,
												cellOffsets,
												digits));
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Get conclusions after a hidden subset searched.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="region">The region offset.</param>
		/// <param name="mask">
		/// The mask that calculated in
		/// <see cref="TakeAllHiddenSubsetsBySize(IBag{TechniqueInfo}, IReadOnlyGrid, int)"/>.
		/// </param>
		/// <param name="digits">All digits.</param>
		/// <param name="cellOffsetList">(<see langword="out"/> parameter) All cell offsets.</param>
		/// <param name="highlightedCandidates">
		/// (<see langword="out"/> parameter) All highlight candidate offsets.
		/// </param>
		/// <returns>All conclusions.</returns>
		private static IReadOnlyList<Conclusion> GetHiddenSubsetsConclusions(
			IReadOnlyGrid grid, int region, short mask, IReadOnlyList<int> digits,
			out IReadOnlyList<int> cellOffsetList,
			out IReadOnlyList<(int, int)> highlightedCandidates)
		{
			var tempCellList = new List<int>();
			var tempCandList = new List<(int, int)>();
			var result = new List<Conclusion>();
			int i = 0;
			foreach (int offset in RegionCells[region])
			{
				if ((mask & 1) != 0)
				{
					tempCellList.Add(offset);
					for (int digit = 0; digit < 9; digit++)
					{
						if (!grid[offset, digit])
						{
							if (digits.Contains(digit))
							{
								// Contains in the list.
								// These candidates are highlight candidates.
								tempCandList.Add((0, offset * 9 + digit));
							}
							else
							{
								// Does not contain in the list.
								// These candidates are eliminations.
								result.Add(new Conclusion(ConclusionType.Elimination, offset, digit));
							}
						}
					}
				}

				mask >>= 1;
				i++;
			}

			(cellOffsetList, highlightedCandidates) = (tempCellList, tempCandList);
			return result;
		}
	}
}
