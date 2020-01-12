using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Subsets
{
	public sealed class SubsetsStepFinder : StepFinder
	{
		public override IList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<TechniqueInfo>();

			result.AddRange(TakeAllNakedSubsetsBySize(grid, 2));
			result.AddRange(TakeAllNakedSubsetsBySize(grid, 3));
			result.AddRange(TakeAllNakedSubsetsBySize(grid, 4));

			return result;
		}

		private static IList<SubsetTechniqueInfo> TakeAllNakedSubsetsBySize(Grid grid, int size)
		{
			Contract.Requires(size >= 2 && size <= 4);

			var result = new List<SubsetTechniqueInfo>();

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
							if (9 - mask.CountSet() == 2)
							{
								// Naked pair found.
								var digits = new List<int>((511 & ~mask2).GetAllSets());
								var offsets = new List<int> { pos1, pos2 };
								var conclusions = GetNakedSubsetConclusions(grid, offsets, digits);
								if (conclusions.Count != 0)
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
											digits));
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
									if (9 - mask.CountSet() == 3)
									{
										// Naked triple found.
										var digits = new List<int>((511 & ~mask3).GetAllSets());
										var offsets = new List<int> { pos1, pos2, pos3 };
										var conclusions = GetNakedSubsetConclusions(grid, offsets, digits);
										if (conclusions.Count != 0)
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
													digits));
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
										if (9 - mask.CountSet() == 4)
										{
											// Naked triple found.
											var digits = new List<int>((511 & ~mask4).GetAllSets());
											var offsets = new List<int> { pos1, pos2, pos3, pos4 };
											var conclusions = GetNakedSubsetConclusions(grid, offsets, digits);
											if (conclusions.Count != 0)
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
														digits));
											}
										}
									}
								}
							}
						}
					}
				}
			}

			return result;
		}

		private static ICollection<Conclusion> GetNakedSubsetConclusions(
			Grid grid, IList<int> offsets, ICollection<int> digits)
		{
			var result = new List<Conclusion>();
			var map = new GridMap(offsets[0]);
			for (int i = 1; i < offsets.Count; i++)
			{
				// Get intersections (in order to find eliminations).
				map &= new GridMap(offsets[i]);
			}
			foreach (int offset in offsets)
			{
				// Set bit 'false' on themselves (do not regard them as eliminations).
				map[offset] = false;
			}

			// Get all eliminations.
			foreach (int offset in map.Offsets)
			{
				if (grid.GetCellStatus(offset) == CellStatus.Empty)
				{
					foreach (int digit in digits)
					{
						if (!grid[offset, digit])
						{
							result.Add(new Conclusion(ConclusionType.Elimination, offset, digit));
						}
					}
				}
			}

			return result;
		}

		private static IReadOnlyList<(int, int)> GetNakedSubsetsHighlightedCandidateOffsets(
			Grid grid, ICollection<int> offsets, ICollection<int> digits)
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
	}
}
