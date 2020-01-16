using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Subsets
{
	public sealed class SubsetStepFinder : StepFinder
	{
		public override IList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<TechniqueInfo>();

			#region Naked subset
			result.AddRange(TakeAllNakedSubsetsBySize(grid, 2));
			result.AddRange(TakeAllNakedSubsetsBySize(grid, 3));
			result.AddRange(TakeAllNakedSubsetsBySize(grid, 4));
			#endregion

			#region Hidden subset
			result.AddRange(TakeAllHiddenSubsetsBySize(grid, 2));
			result.AddRange(TakeAllHiddenSubsetsBySize(grid, 3));
			result.AddRange(TakeAllHiddenSubsetsBySize(grid, 4));
			#endregion

			return result;
		}


		#region Naked Subsets utils
		private static IList<NakedSubsetTechniqueInfo> TakeAllNakedSubsetsBySize(
			Grid grid, int size)
		{
			Contract.Requires(size >= 2 && size <= 4);

			var result = new List<NakedSubsetTechniqueInfo>();

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
							if (mask.CountSet() == 7)
							{
								// Naked pair found.
								var digits = new List<int>((511 & ~mask2).GetAllSets());
								var offsets = new List<int> { pos1, pos2 };
								var conclusions =
									GetNakedSubsetConclusions(
										grid, offsets, digits, out bool? isLocked);
								if (conclusions.Count != 0)
								{
									// Gather this conclusion.
									GatherConclusion(
										grid, result, region, digits,
										offsets, conclusions, isLocked);
								}
							}
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
									if (mask.CountSet() == 6)
									{
										// Naked triple found.
										var digits = new List<int>((511 & ~mask3).GetAllSets());
										var offsets = new List<int> { pos1, pos2, pos3 };
										var conclusions =
											GetNakedSubsetConclusions(
												grid, offsets, digits, out bool? isLocked);
										if (conclusions.Count != 0)
										{
											GatherConclusion(
												grid, result, region, digits,
												offsets, conclusions, isLocked);
										}
									}
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
										if (mask.CountSet() == 5)
										{
											// Naked triple found.
											var digits = new List<int>((511 & ~mask4).GetAllSets());
											var offsets = new List<int> { pos1, pos2, pos3, pos4 };
											var conclusions =
												GetNakedSubsetConclusions(
													grid, offsets, digits, out bool? isLocked);
											if (conclusions.Count != 0)
											{
												GatherConclusion(
													grid, result, region, digits,
													offsets, conclusions, isLocked);
											}
										} // if 9 - mask.CountSet() == 4
									} // for i4 (i3 + 1)..9
								} // else (if size == 3)
							} // for i3 (i2 + 1)..(12 - size)
						} // else (if size == 2)
					} // for i2 (i1 + 1)..(11 - size)
				} // for i1 0..(10 - size)
			} // for region 0..27

			return result;
		}

		private static void GatherConclusion(
			Grid grid, IList<NakedSubsetTechniqueInfo> result, int region,
			IReadOnlyList<int> digits, IReadOnlyList<int> offsets,
			IReadOnlyList<Conclusion> conclusions, bool? isLocked)
		{
			result.Add(
				new NakedSubsetTechniqueInfo(
					conclusions,
					views: new List<View>
					{
						new View(
							cellOffsets: null,
							candidateOffsets: GetNakedSubsetsHighlightedCandidateOffsets(grid, offsets, digits),
							regionOffsets: new List<(int, int)>
							{
								(0, region)
							},
							linkMasks: null)
					},
					regionOffset: region,
					cellOffsets: offsets,
					digits,
					isLocked));
		}

		private static IReadOnlyList<Conclusion> GetNakedSubsetConclusions(
			Grid grid, IReadOnlyList<int> offsets,
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
				var tempMap = new GridMap(firstOffset);
				for (int j = firstOffsetIndex + 1; j < offsets.Count; j++)
				{
					int offset = offsets[j];
					if (!grid[offset, digit])
					{
						tempMap &= new GridMap(offset);
					}
				}
				series[i] = tempMap.Count > 9;

				// Set bit 'false' on themselves (do not regard them as eliminations).
				foreach (int offset in offsets)
				{
					tempMap[offset] = false;
				}

				// Add eliminations by each digit.
				foreach (int offset in tempMap.Offsets)
				{
					if (grid.GetCellStatus(offset) == CellStatus.Empty)
					{
						if (!grid[offset, digit])
						{
							result.Add(new Conclusion(ConclusionType.Elimination, offset, digit));
						}
					}
				}
			}
			static bool lockedJudger(bool v) => v;
			isLocked = series.Any(lockedJudger) ? series.All(lockedJudger) : (bool?)null;
			return result;
		}

		private static IReadOnlyList<(int, int)> GetNakedSubsetsHighlightedCandidateOffsets(
			Grid grid, IReadOnlyList<int> offsets, IReadOnlyList<int> digits)
		{
			var result = new List<(int, int)>();
			foreach (int offset in offsets)
			{
				foreach (int digit in digits)
				{
					if (!grid[offset, digit])
					{
						// The candidate does exist in the current grid,
						// which should be highlighted.
						result.Add((0, offset));
					}
				}
			}

			return result;
		}
		#endregion

		#region Hidden Subsets utils
		private static IList<HiddenSubsetTechniqueInfo> TakeAllHiddenSubsetsBySize(
			Grid grid, int size)
		{
			var result = new List<HiddenSubsetTechniqueInfo>();

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
							if (mask2.CountSet() == 2)
							{
								// Hidden pair found.
								var digits = new List<int> { d1, d2 };
								var conclusions =
									GetHiddenSubsetsConclusions(
										grid, region, mask2, digits, out var cellOffsets,
										out var highlightedCandidates);
								if (conclusions.Count != 0)
								{
									result.Add(
										new HiddenSubsetTechniqueInfo(
											conclusions,
											views: new List<View>
											{
												new View(
													cellOffsets: null,
													candidateOffsets: highlightedCandidates,
													regionOffsets: new List<(int, int)>
													{
														(0, region)
													},
													linkMasks: null)
											},
											regionOffset: region,
											cellOffsets,
											digits));
								}
							}
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
									if (mask3.CountSet() == 3)
									{
										// Hidden triple found.
										var digits = new List<int> { d1, d2, d3 };
										var conclusions =
											GetHiddenSubsetsConclusions(
												grid, region, mask3, digits, out var cellOffsets,
												out var highlightedCandidates);
										if (conclusions.Count != 0)
										{
											result.Add(
												new HiddenSubsetTechniqueInfo(
													conclusions,
													views: new List<View>
													{
														new View(
															cellOffsets: null,
															candidateOffsets: highlightedCandidates,
															regionOffsets: new List<(int, int)>
															{
																(0, region)
															},
															linkMasks: null)
													},
													regionOffset: region,
													cellOffsets,
													digits));
										}
									}
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
										if (mask4.CountSet() == 4)
										{
											// Hidden quadruple found.
											var digits = new List<int> { d1, d2, d3, d4 };
											var conclusions =
												GetHiddenSubsetsConclusions(
													grid, region, mask4, digits, out var cellOffsets,
													out var highlightedCandidates);
											if (conclusions.Count != 0)
											{
												result.Add(
													new HiddenSubsetTechniqueInfo(
														conclusions,
														views: new List<View>
														{
															new View(
																cellOffsets: null,
																candidateOffsets: highlightedCandidates,
																regionOffsets: new List<(int, int)>
																{
																	(0, region)
																},
																linkMasks: null)
														},
														regionOffset: region,
														cellOffsets,
														digits));
											}
										} // if mask.CountSet() == 4
									} // for d4 (d3 + 1)..9
								} // else (if size == 3 && mask3.CountSet() == 3)
							} // for d3 (d2 + 1)..(12 - size)
						} // else (if size == 2 && mask2.CountSet() == 2)
					} // for d2 (d1 + 1)..(11 - size)
				} // for d1 0..(10 - size)
			} // for region 0..27

			return result;
		}

		private static IReadOnlyList<Conclusion> GetHiddenSubsetsConclusions(
			Grid grid, int region, int mask, IReadOnlyList<int> digits,
			out IReadOnlyList<int> cellOffsetList,
			out IReadOnlyList<(int, int)> highlightedCandidates)
		{
			var tempCellList = new List<int>();
			var tempCandList = new List<(int, int)>();
			var result = new List<Conclusion>();
			var offsets = RegionUtils.GetCellOffsets(region);
			int i = 0;
			foreach (int offset in offsets)
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
								// These candidates are highlighted candidates.
								tempCandList.Add((0, offset * 9 + digit));
							}
							else
							{
								// Does not contain in the list.
								// These candidates are eliminations.
								result.Add(
									new Conclusion(ConclusionType.Elimination, offset, digit));
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
		#endregion
	}
}
