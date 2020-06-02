using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static System.Algorithms;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

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
				GetNakedSubsetsBySize(accumulator, grid, size);
				GetHiddenSubsetsBySize(accumulator, grid, size);
			}
		}

		/// <summary>
		/// Get all naked subsets technique information, for searching the specified size.
		/// </summary>
		/// <param name="accumulator">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="size">The size.</param>
		/// <returns>All technique information searched.</returns>
		private static void GetNakedSubsetsBySize(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, int size)
		{
			for (int region = 0; region < 27; region++)
			{
				var currentEmptyMap = RegionMaps[region] & EmptyMap;
				if (currentEmptyMap.Count < 2)
				{
					continue;
				}

				foreach (int[] cells in GetCombinationsOfArray(currentEmptyMap.ToArray(), size))
				{
					short mask = 0;
					foreach (int cell in cells)
					{
						mask |= grid.GetCandidatesReversal(cell);
					}
					if (mask.CountSet() != size)
					{
						continue;
					}

					// Naked subset found. Now check eliminations.
					short flagMask = 0;
					var conclusions = new List<Conclusion>();
					foreach (int digit in mask.GetAllSets())
					{
						var map = (new GridMap(cells) & CandMaps[digit]).PeerIntersection & CandMaps[digit];
						flagMask |= map.AllSetsAreInOneRegion(out _) ? (short)0 : (short)(1 << digit);

						foreach (int cell in map)
						{
							conclusions.Add(new Conclusion(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<(int, int)>();
					foreach (int cell in cells)
					{
						foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
						{
							candidateOffsets.Add((0, cell * 9 + digit));
						}
					}

					accumulator.Add(
						new NakedSubsetTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: new[] { (0, region) },
									links: null)
							},
							regionOffset: region,
							cellOffsets: cells,
							digits: mask.GetAllSets().ToArray(),
							isLocked:
								true switch
								{
									_ when flagMask == mask => true,
									_ when flagMask != 0 => false,
									_ => null
								}));
				}
			}
		}

		/// <summary>
		/// Get all hidden subsets technique information, for searching the specified size.
		/// </summary>
		/// <param name="accumulator">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="size">The size.</param>
		/// <returns>All technique information searched.</returns>
		private static void GetHiddenSubsetsBySize(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, int size)
		{
			for (int region = 0; region < 27; region++)
			{
				for (int d1 = 0; d1 < 10 - size; d1++)
				{
					if (ValueMaps[d1].Overlaps(RegionMaps[region]))
					{
						continue;
					}

					short mask = (RegionMaps[region] & CandMaps[d1]).GetSubviewMask(region);
					for (int d2 = d1 + 1; d2 < 11 - size; d2++)
					{
						if (ValueMaps[d2].Overlaps(RegionMaps[region]))
						{
							continue;
						}

						short mask2 = (short)((RegionMaps[region] & CandMaps[d2]).GetSubviewMask(region) | mask);
						if (size == 2)
						{
							if (mask2.CountSet() != 2)
							{
								continue;
							}

							// Hidden pair found.
							int[] digits = new[] { d1, d2 };
							var conclusions =
								GetHiddenSubsetsConclusions(
									grid, region, mask2, digits, out var cellOffsets,
									out var highlightedCandidates);

							if (conclusions.Count == 0)
							{
								continue;
							}

							accumulator.Add(
								new HiddenSubsetTechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: null,
											candidateOffsets: highlightedCandidates,
											regionOffsets: new[] { (0, region) },
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
								if (ValueMaps[d3].Overlaps(RegionMaps[region]))
								{
									continue;
								}

								short mask3 = (short)((RegionMaps[region] & CandMaps[d3]).GetSubviewMask(region) | mask2);
								if (size == 3)
								{
									if (mask3.CountSet() != 3)
									{
										continue;
									}

									// Hidden triple found.
									int[] digits = new[] { d1, d2, d3 };
									var conclusions =
										GetHiddenSubsetsConclusions(
											grid, region, mask3, digits, out var cellOffsets,
											out var highlightedCandidates);

									if (conclusions.Count == 0)
									{
										continue;
									}

									accumulator.Add(
										new HiddenSubsetTechniqueInfo(
											conclusions,
											views: new[]
											{
												new View(
													cellOffsets: null,
													candidateOffsets: highlightedCandidates,
													regionOffsets: new[] { (0, region) },
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
										if (ValueMaps[d4].Overlaps(RegionMaps[region]))
										{
											continue;
										}

										// 'size == 4' is always true.
										// Now check hidden quadruple.
										short mask4 = (short)(
											(RegionMaps[region] & CandMaps[d4]).GetSubviewMask(region) | mask3);
										if (mask4.CountSet() != 4)
										{
											continue;
										}

										// Hidden quadruple found.
										int[] digits = new[] { d1, d2, d3, d4 };
										var conclusions =
											GetHiddenSubsetsConclusions(
												grid, region, mask4, digits, out var cellOffsets,
												out var highlightedCandidates);

										if (conclusions.Count == 0)
										{
											continue;
										}

										accumulator.Add(
											new HiddenSubsetTechniqueInfo(
												conclusions,
												views: new[]
												{
													new View(
														cellOffsets: null,
														candidateOffsets: highlightedCandidates,
														regionOffsets: new[] { (0, region) },
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
		/// <see cref="GetHiddenSubsetsBySize(IBag{TechniqueInfo}, IReadOnlyGrid, int)"/>.
		/// </param>
		/// <param name="digits">All digits.</param>
		/// <param name="cellOffsetList">(<see langword="out"/> parameter) All cell offsets.</param>
		/// <param name="highlightedCandidates">
		/// (<see langword="out"/> parameter) All highlight candidate offsets.
		/// </param>
		/// <returns>All conclusions.</returns>
		private static IReadOnlyList<Conclusion> GetHiddenSubsetsConclusions(
			IReadOnlyGrid grid, int region, short mask, IReadOnlyList<int> digits,
			out IReadOnlyList<int> cellOffsetList, out IReadOnlyList<(int, int)> highlightedCandidates)
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
								result.Add(new Conclusion(Elimination, offset, digit));
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
