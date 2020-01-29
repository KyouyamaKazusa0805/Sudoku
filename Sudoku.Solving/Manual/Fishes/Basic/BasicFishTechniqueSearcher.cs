using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Fishes.Basic
{
	/// <summary>
	/// Encapsulates a normal fish technique searcher, same as
	/// <see cref="NormalFishTechniqueSearcher"/>, but this searcher only
	/// searches basic fish (without any fins).
	/// </summary>
	/// <seealso cref="NormalFishTechniqueSearcher"/>
	[Obsolete("We suggest you to use 'NormalFishTechniqueSearcher'.")]
	public sealed class BasicFishTechniqueSearcher : FishTechniqueSearcher
	{
		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<TechniqueInfo>();

			result.AddRange(TakeAllBasicFishBySize(grid, 2, true));
			result.AddRange(TakeAllBasicFishBySize(grid, 3, true));
			result.AddRange(TakeAllBasicFishBySize(grid, 4, true));
			result.AddRange(TakeAllBasicFishBySize(grid, 2, false));
			result.AddRange(TakeAllBasicFishBySize(grid, 3, false));
			result.AddRange(TakeAllBasicFishBySize(grid, 4, false));

			return result;
		}


		/// <summary>
		/// Searches all basic fish of the specified size.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="size">The size.</param>
		/// <param name="searchRow">
		/// Indicates the method is searched row or column.
		/// <c>true</c> is for searching rows and <c>false</c>
		/// is for searching columns.
		/// </param>
		/// <returns>All technique information searched.</returns>
		private static IReadOnlyList<BasicFishTechniqueInfo> TakeAllBasicFishBySize(
			Grid grid, int size, bool searchRow)
		{
			Contract.Requires(size >= 2 && size <= 4);

			var result = new List<BasicFishTechniqueInfo>();

			for (int digit = 0; digit < 9; digit++)
			{
				int regionStart = searchRow ? 9 : 18;
				for (int r1 = regionStart; r1 < regionStart + 9 - size; r1++)
				{
					if (grid.HasDigitValue(digit, r1))
					{
						continue;
					}

					short mask = grid.GetDigitAppearingMask(digit, r1);
					for (int r2 = r1 + 1; r2 < regionStart + 10 - size; r2++)
					{
						if (grid.HasDigitValue(digit, r2))
						{
							continue;
						}

						short mask2 = (short)(grid.GetDigitAppearingMask(digit, r2) | mask);
						if (size == 2)
						{
							if (mask2.CountSet() == 2)
							{
								// X-Wing found.
								var conclusions = GetConclusions(
									grid, digit, mask2, searchRow, r1, r2);
								if (conclusions.Count != 0)
								{
									var coverSets = GetCoverSets(mask2, searchRow);
									result.Add(
										new BasicFishTechniqueInfo(
											conclusions,
											views: new[]
											{
											new View(
												cellOffsets: null,
												candidateOffsets:
													GetHighlightedCandidates(grid, digit, r1, r2),
												regionOffsets: new List<(int, int)>
												{
													(0, r1),
													(0, r2),
													(1, coverSets[0]),
													(1, coverSets[1]),
												},
												linkMasks: null)
											},
											digit,
											baseSets: new List<int> { r1, r2 },
											coverSets));
								}
							}
						}
						else // size > 2
						{
							for (int r3 = r2 + 1; r3 < regionStart + 11 - size; r3++)
							{
								if (grid.HasDigitValue(digit, r3))
								{
									continue;
								}

								short mask3 = (short)(grid.GetDigitAppearingMask(digit, r3) | mask2);
								if (size == 3)
								{
									if (mask3.CountSet() == 3)
									{
										// Swordfish found.
										var conclusions = GetConclusions(
											grid, digit, mask3, searchRow, r1, r2, r3);
										if (conclusions.Count != 0)
										{
											var coverSets = GetCoverSets(mask3, searchRow);
											result.Add(
												new BasicFishTechniqueInfo(
													conclusions,
													views: new[]
													{
													new View(
														cellOffsets: null,
														candidateOffsets:
															GetHighlightedCandidates(
																grid, digit, r1, r2, r3),
														regionOffsets: new List<(int, int)>
														{
															(0, r1),
															(0, r2),
															(0, r3),
															(1, coverSets[0]),
															(1, coverSets[1]),
															(1, coverSets[2])
														},
														linkMasks: null)
													},
													digit,
													baseSets: new List<int> { r1, r2, r3 },
													coverSets));
										}
									}
								}
								else // size == 4
								{
									for (int r4 = r3 + 1; r4 < regionStart + 8; r4++)
									{
										if (grid.HasDigitValue(digit, r4))
										{
											continue;
										}

										short mask4 = (short)(grid.GetDigitAppearingMask(digit, r4) | mask3);
										if (mask4.CountSet() == 4)
										{
											// Jellyfish found.
											var conclusions = GetConclusions(
												grid, digit, mask4, searchRow, r1, r2, r3, r4);
											if (conclusions.Count != 0)
											{
												var coverSets = GetCoverSets(mask4, searchRow);
												result.Add(
													new BasicFishTechniqueInfo(
														conclusions,
														views: new[]
														{
															new View(
																cellOffsets: null,
																candidateOffsets:
																	GetHighlightedCandidates(
																		grid, digit, r1, r2, r3, r4),
																regionOffsets: new List<(int, int)>
																{
																	(0, r1),
																	(0, r2),
																	(0, r3),
																	(0, r4),
																	(1, coverSets[0]),
																	(1, coverSets[1]),
																	(1, coverSets[2]),
																	(1, coverSets[3])
																},
																linkMasks: null)
														},
														digit,
														baseSets: new List<int> { r1, r2, r3, r4 },
														coverSets));
											}
										} // if mask4.CountSet() == 4
									} // for r4 (r3 + 1)..(regionStart + 8)
								} // else (if size == 3)
							} // for r3 (r2 + 1)..(regionStart + 11 - size)
						} // else (if size == 2)
					} // for r2 (r1 + 1)..(regionStart + 10 - size)
				} // for r1 regionStart..(regionStart + 9 - size)
			} // for digit 0..9

			return result;
		}

		/// <summary>
		/// Get cover sets from the specified mask.
		/// </summary>
		/// <param name="mask">The mask.</param>
		/// <param name="searchRow">Indicates the method is searched rows or columns.</param>
		/// <returns>All cover set region offsets.</returns>
		private static IReadOnlyList<int> GetCoverSets(short mask, bool searchRow) =>
			new List<int>(from pos in FindSets(mask) select 9 + (searchRow ? pos + 9 : pos));

		/// <summary>
		/// Get all highlight candidate offsets.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="baseSets">The base sets.</param>
		/// <returns>All highlight candidate offsets and their IDs.</returns>
		private static IReadOnlyList<(int, int)> GetHighlightedCandidates(
			Grid grid, int digit, params int[] baseSets)
		{
			var result = new List<(int, int)>();

			foreach (int baseSet in baseSets)
			{
				short mask = grid.GetDigitAppearingMask(digit, baseSet);
				foreach (int pos in FindSets(mask))
				{
					int offset = RegionUtils.GetCellOffset(baseSet, pos);
					if (grid.GetCellStatus(offset) == CellStatus.Empty && !grid[offset, digit])
					{
						result.Add((0, offset * 9 + digit));
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Get conclusions after searched a fish.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="mask">The mask.</param>
		/// <param name="searchRow">Indicates the method is searched rows or columns.</param>
		/// <param name="baseSets">All base sets.</param>
		/// <returns>All conclusions.</returns>
		private static IReadOnlyList<Conclusion> GetConclusions(
			Grid grid, int digit, short mask, bool searchRow, params int[] baseSets)
		{
			var result = new List<Conclusion>();
			int regionStart = searchRow ? 9 : 18;
			for (int region = regionStart; region < regionStart + 9; region++)
			{
				if (baseSets.Contains(region))
				{
					continue;
				}

				foreach (int pos in FindSets(mask))
				{
					int offset = RegionUtils.GetCellOffset(region, pos);
					if (grid.GetCellStatus(offset) == CellStatus.Empty && !grid[offset, digit])
					{
						result.Add(new Conclusion(ConclusionType.Elimination, offset, digit));
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Find all set bits in a <see cref="short"/> value
		/// (only iterating for 9 times).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The set bits.</returns>
		private static IEnumerable<int> FindSets(short value)
		{
			for (int i = 0; i < 9; i++, value >>= 1)
			{
				if ((value & 1) != 0)
				{
					yield return i;
				}
			}
		}
	}
}
