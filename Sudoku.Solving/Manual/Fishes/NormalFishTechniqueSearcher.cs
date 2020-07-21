using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Constants.Values;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Encapsulates a <b>normal fish</b> technique searcher. Fins can also be found.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.XWing))]
	[SearcherProperty(32)]
	public sealed class NormalFishTechniqueSearcher : FishTechniqueSearcher
	{
		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			for (int size = 2; size <= 4; size++)
			{
				foreach (bool searchRow in BooleanValues)
				{
					AccumulateAllBySize(accumulator, grid, size, searchRow);
				}
			}
		}


		/// <summary>
		/// Searches all basic fish of the specified size.
		/// </summary>
		/// <param name="accumulator">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="size">The size.</param>
		/// <param name="searchRow">
		/// Indicates the solver will searching rows or columns.
		/// </param>
		/// <returns>The result.</returns>
		private static void AccumulateAllBySize(
			IList<TechniqueInfo> accumulator, IReadOnlyGrid grid, int size, bool searchRow)
		{
			int baseSetStart = searchRow ? 9 : 18;
			int coverSetStart = searchRow ? 18 : 9;
			var baseSets2 = (Span<int>)stackalloc int[2];
			var coverSets2 = (Span<int>)stackalloc int[2];
			var baseSets3 = (Span<int>)stackalloc int[3];
			var coverSets3 = (Span<int>)stackalloc int[3];
			var baseSets4 = (Span<int>)stackalloc int[4];
			var coverSets4 = (Span<int>)stackalloc int[4];
			for (int digit = 0; digit < 9; digit++)
			{
				var candMap = CandMaps[digit];
				if (candMap.IsEmpty)
				{
					continue;
				}

				for (int bs1 = baseSetStart; bs1 < baseSetStart + 10 - size; bs1++)
				{
					var baseMap1 = RegionMaps[bs1] & candMap;
					if (baseMap1.Count < 2)
					{
						continue;
					}

					for (int bs2 = bs1 + 1; bs2 < baseSetStart + 11 - size; bs2++)
					{
						var baseMap2 = RegionMaps[bs2] & candMap;
						if (baseMap2.Count < 2)
						{
							continue;
						}

						short baseMask = (short)(baseMap1.GetSubviewMask(bs1) | baseMap2.GetSubviewMask(bs2));
						if (size == 2)
						{
							int finAndBodyCount = baseMask.CountSet();
							if (finAndBodyCount >= 5)
							{
								continue;
							}

							// Search (Finned) (Sashimi) X-Wing.
							for (int cs1 = coverSetStart, i = 0; cs1 < coverSetStart + 8; cs1++, i++)
							{
								// Check whether this cover set has 'digit'.
								if ((baseMask >> i & 1) == 0)
								{
									continue;
								}

								for (int cs2 = cs1 + 1, j = i + 1; cs2 < coverSetStart + 9; cs2++, j++)
								{
									// Check whether this cover set has 'digit'.
									if ((baseMask >> j & 1) == 0)
									{
										continue;
									}

									// Confirm all elimination cells.
									baseSets2[0] = bs1;
									baseSets2[1] = bs2;
									coverSets2[0] = cs1;
									coverSets2[1] = cs2;
									var bodyMap = GridMap.Empty;
									var elimMap = GridMap.Empty;
									GetGridMap(ref bodyMap, baseSets2, CandMaps[digit]);
									GetGridMap(ref elimMap, coverSets2, CandMaps[digit]);
									bodyMap &= elimMap;
									elimMap -= bodyMap;

									// Check the existence of fin.
									var finCellsMap = GridMap.Empty;
									if (finAndBodyCount == 2) // size == 2
									{
										goto CheckWhetherTheNumberOfIntersectionCellsIsNotZero;
									}

									// Confirm all fin cells.
									short finMask = (short)(baseMask & ~(1 << i | 1 << j) & Grid.MaxCandidatesMask);
									foreach (int baseSet in baseSets2)
									{
										foreach (int x in finMask.GetAllSets())
										{
											int possibleFinCellOffset = RegionCells[baseSet][x];
											if (CandMaps[digit][possibleFinCellOffset])
											{
												finCellsMap.Add(possibleFinCellOffset);
											}
										}
									}

									// Check the number of fins is less than 3.
									if (finCellsMap.Count > 2)
									{
										continue;
									}

									// And all fins do not lie on any cover sets.
									var coverSetsMap = GridMap.Empty;
									foreach (int coverSet in coverSets2)
									{
										coverSetsMap |= RegionMaps[coverSet];
									}
									if (coverSetsMap.Overlaps(finCellsMap))
									{
										continue;
									}

									// Get intersection.
									foreach (int finCell in finCellsMap)
									{
										elimMap &= PeerMaps[finCell];
									}

								CheckWhetherTheNumberOfIntersectionCellsIsNotZero:
									// Check whether the number of intersection cells is not 0.
									if (elimMap.IsNotEmpty)
									{
										// Finned/Sashimi X-Wing found.
										// Check eliminations.
										var conclusions = new List<Conclusion>();
										foreach (int offset in elimMap)
										{
											conclusions.Add(new Conclusion(Elimination, offset * 9 + digit));
										}
										if (conclusions.Count == 0)
										{
											continue;
										}

										// Eliminations does exist.
										// Check all highlight candidates.
										var candidateOffsets = new List<(int, int)>();
										foreach (int cell in bodyMap)
										{
											candidateOffsets.Add((0, cell * 9 + digit));
										}
										if (finCellsMap.IsNotEmpty)
										{
											foreach (int cell in finCellsMap)
											{
												candidateOffsets.Add((1, cell * 9 + digit));
											}
										}

										// Check the fish is sashimi, normal finned or normal.
										bool? isSashimi = null;
										if (finCellsMap.IsNotEmpty)
										{
											isSashimi = true;
											int finCell = finCellsMap.SetAt(0);
											int block = finCell / 9 / 3 * 3 + finCell % 9 / 3;
											foreach (int offset in bodyMap)
											{
												if (offset / 9 / 3 * 3 + offset % 9 / 3 == block)
												{
													isSashimi = false;
													break;
												}
											}
										}

										accumulator.Add(
											new NormalFishTechniqueInfo(
												conclusions,
												views: new[]
												{
													new View(
														cellOffsets: null,
														candidateOffsets,
														regionOffsets: new[]
														{
															(0, bs1), (0, bs2),
															(1, cs1), (1, cs2)
														},
														links: null),
													GetDirectView(
														grid, digit, baseSets2, coverSets2, searchRow, finCellsMap)
												},
												digit,
												baseSets: baseSets2.ToArray(),
												coverSets: coverSets2.ToArray(),
												finCellOffsets: finCellsMap.ToArray(),
												isSashimi));
									}
								}
							}
						}
						else // size > 2
						{
							for (int bs3 = bs2 + 1; bs3 < baseSetStart + 12 - size; bs3++)
							{
								// Get the appearing mask of 'digit' in 'bs3'.
								var baseMap3 = RegionMaps[bs3] & candMap;
								if (baseMap3.Count < 2)
								{
									continue;
								}

								baseMask |= baseMap3.GetSubviewMask(bs3);
								if (size == 3)
								{
									int finAndBodyCount = baseMask.CountSet();
									if (finAndBodyCount >= 6)
									{
										continue;
									}

									// Search (Finned) (Sashimi) Swordfish.
									for (int cs1 = coverSetStart, i = 0; cs1 < coverSetStart + 7; cs1++, i++)
									{
										// Check whether this cover set has 'digit'.
										if ((baseMask >> i & 1) == 0)
										{
											continue;
										}

										for (int cs2 = cs1 + 1, j = i + 1; cs2 < coverSetStart + 8; cs2++, j++)
										{
											// Check whether this cover set has 'digit'.
											if ((baseMask >> j & 1) == 0)
											{
												continue;
											}

											for (int cs3 = cs2 + 1, k = j + 1; cs3 < coverSetStart + 9; cs3++, k++)
											{
												// Check whether this cover set has 'digit'.
												if ((baseMask >> k & 1) == 0)
												{
													continue;
												}

												// Confirm all elimination cells.
												baseSets3[0] = bs1;
												baseSets3[1] = bs2;
												baseSets3[2] = bs3;
												coverSets3[0] = cs1;
												coverSets3[1] = cs2;
												coverSets3[2] = cs3;
												var bodyMap = GridMap.Empty;
												var elimMap = GridMap.Empty;
												GetGridMap(ref bodyMap, baseSets3, CandMaps[digit]);
												GetGridMap(ref elimMap, coverSets3, CandMaps[digit]);
												bodyMap &= elimMap;
												elimMap -= bodyMap;

												// Check the existence of fin.
												var finCellsMap = GridMap.Empty;
												if (finAndBodyCount == 3) // size == 3
												{
													goto CheckWhetherTheNumberOfIntersectionCellsIsNotZero;
												}

												// Confirm all fin cells.
												short finMask = (short)(
													baseMask & ~(1 << i | 1 << j | 1 << k) & Grid.MaxCandidatesMask);
												foreach (int baseSet in baseSets3)
												{
													foreach (int x in finMask.GetAllSets())
													{
														int possibleFinCellOffset = RegionCells[baseSet][x];
														if (grid.Exists(possibleFinCellOffset, digit) is true)
														{
															finCellsMap.Add(possibleFinCellOffset);
														}
													}
												}

												// Check the number of fins is less than 5.
												if (finCellsMap.Count > 4)
												{
													continue;
												}

												// And all fins do not lie on any cover sets.
												var coverSetsMap = GridMap.Empty;
												foreach (int coverSet in coverSets3)
												{
													coverSetsMap |= RegionMaps[coverSet];
												}
												if (coverSetsMap.Overlaps(finCellsMap))
												{
													continue;
												}

												// Get intersection.
												foreach (int finCell in finCellsMap)
												{
													elimMap &= PeerMaps[finCell];
												}

											CheckWhetherTheNumberOfIntersectionCellsIsNotZero:
												// Check whether the number of intersection cells is not 0.
												if (elimMap.IsNotEmpty)
												{
													// Finned/Sashimi X-Wing found.
													// Check eliminations.
													var conclusions = new List<Conclusion>();
													foreach (int cell in elimMap)
													{
														conclusions.Add(new Conclusion(Elimination, cell * 9 + digit));
													}
													if (conclusions.Count == 0)
													{
														continue;
													}

													// Eliminations does exist.
													// Check all highlight candidates.
													var candidateOffsets = new List<(int, int)>();
													foreach (int cell in bodyMap)
													{
														candidateOffsets.Add((0, cell * 9 + digit));
													}
													if (finCellsMap.IsNotEmpty)
													{
														foreach (int cell in finCellsMap)
														{
															candidateOffsets.Add((1, cell * 9 + digit));
														}
													}

													// Check the fish is sashimi, normal finned or normal.
													bool? isSashimi = null;
													if (finCellsMap.IsNotEmpty)
													{
														isSashimi = true;
														int finCell = finCellsMap.SetAt(0);
														int block = finCell / 9 / 3 * 3 + finCell % 9 / 3;
														foreach (int offset in bodyMap)
														{
															if (offset / 9 / 3 * 3 + offset % 9 / 3 == block)
															{
																isSashimi = false;
																break;
															}
														}
													}

													accumulator.Add(
														new NormalFishTechniqueInfo(
															conclusions,
															views: new[]
															{
																new View(
																	cellOffsets: null,
																	candidateOffsets,
																	regionOffsets: new[]
																	{
																		(0, bs1), (0, bs2), (0, bs3),
																		(1, cs1), (1, cs2), (1, cs3)
																	},
																	links: null),
																GetDirectView(
																	grid, digit, baseSets3,
																	coverSets3, searchRow, finCellsMap)
															},
															digit,
															baseSets: baseSets3.ToArray(),
															coverSets: coverSets3.ToArray(),
															finCellOffsets: finCellsMap.ToArray(),
															isSashimi));
												}
											}
										}
									}
								}
								else // size == 4
								{
									for (int bs4 = bs3 + 1; bs4 < baseSetStart + 9; bs4++)
									{
										// Get the appearing mask of 'digit' in 'bs4'.
										var baseMap4 = RegionMaps[bs4] & candMap;
										if (baseMap4.Count < 2)
										{
											continue;
										}

										baseMask |= baseMap4.GetSubviewMask(bs4);
										int finAndBodyCount = baseMask.CountSet();
										if (finAndBodyCount >= 7)
										{
											continue;
										}

										// Search (Finned) (Sashimi) Jellyfish.
										for (int cs1 = coverSetStart, i = 0; cs1 < coverSetStart + 6; cs1++, i++)
										{
											// Check whether this cover set has 'digit'.
											if ((baseMask >> i & 1) == 0)
											{
												continue;
											}

											for (int cs2 = cs1 + 1, j = i + 1; cs2 < coverSetStart + 7; cs2++, j++)
											{
												// Check whether this cover set has 'digit'.
												if ((baseMask >> j & 1) == 0)
												{
													continue;
												}

												for (int cs3 = cs2 + 1, k = j + 1; cs3 < coverSetStart + 8; cs3++, k++)
												{
													// Check whether this cover set has 'digit'.
													if ((baseMask >> k & 1) == 0)
													{
														continue;
													}

													for (int cs4 = cs3 + 1, l = k + 1; cs4 < coverSetStart + 9; cs4++, l++)
													{
														// Check whether this cover set has 'digit'.
														if ((baseMask >> l & 1) == 0)
														{
															continue;
														}

														// Confirm all elimination cells.
														baseSets4[0] = bs1;
														baseSets4[1] = bs2;
														baseSets4[2] = bs3;
														baseSets4[3] = bs4;
														coverSets4[0] = cs1;
														coverSets4[1] = cs2;
														coverSets4[2] = cs3;
														coverSets4[3] = cs4;
														var bodyMap = GridMap.Empty;
														var elimMap = GridMap.Empty;
														GetGridMap(ref bodyMap, baseSets4, CandMaps[digit]);
														GetGridMap(ref elimMap, coverSets4, CandMaps[digit]);
														bodyMap &= elimMap;
														elimMap -= bodyMap;

														// Check the existence of fin.
														var finCellsMap = GridMap.Empty;
														if (finAndBodyCount == 4) // size == 4
														{
															goto CheckWhetherTheNumberOfIntersectionCellsIsNotZero;
														}

														// Get the fin mask.
														short finMask = (short)(
															baseMask & ~(1 << i | 1 << j | 1 << k | 1 << l) & Grid.MaxCandidatesMask);

														// Confirm all fin cells.
														foreach (int baseSet in baseSets4)
														{
															foreach (int x in finMask.GetAllSets())
															{
																int possibleFinCellOffset = RegionCells[baseSet][x];
																if (grid.Exists(
																	possibleFinCellOffset, digit) is true)
																{
																	finCellsMap.Add(possibleFinCellOffset);
																}
															}
														}

														// Check the number of fins is less than 4.
														if (finCellsMap.Count > 4)
														{
															continue;
														}

														// And all fins do not lie on any cover sets.
														var coverSetsMap = GridMap.Empty;
														foreach (int coverSet in coverSets3)
														{
															coverSetsMap |= RegionMaps[coverSet];
														}
														if (coverSetsMap.Overlaps(finCellsMap))
														{
															continue;
														}

														// Get intersection.
														foreach (int finCell in finCellsMap)
														{
															elimMap &= PeerMaps[finCell];
														}

													CheckWhetherTheNumberOfIntersectionCellsIsNotZero:
														// Check whether the number of intersection cells is not 0.
														if (elimMap.IsNotEmpty)
														{
															// Finned/Sashimi X-Wing found.
															// Check eliminations.
															var conclusions = new List<Conclusion>();
															foreach (int offset in elimMap)
															{
																conclusions.Add(
																	new Conclusion(Elimination, offset * 9 + digit));
															}
															if (conclusions.Count == 0)
															{
																continue;
															}

															// Eliminations does exist.
															// Check all highlight candidates.
															var candidateOffsets = new List<(int, int)>();
															foreach (int cell in bodyMap)
															{
																candidateOffsets.Add((0, cell * 9 + digit));
															}
															if (finCellsMap.IsNotEmpty)
															{
																foreach (int cell in finCellsMap)
																{
																	candidateOffsets.Add((1, cell * 9 + digit));
																}
															}

															// Check the fish is sashimi, normal finned or normal.
															bool? isSashimi = null;
															if (finCellsMap.IsNotEmpty)
															{
																isSashimi = true;
																int finCell = finCellsMap.SetAt(0);
																int block = finCell / 9 / 3 * 3 + finCell % 9 / 3;
																foreach (int cell in bodyMap)
																{
																	if (cell / 9 / 3 * 3 + cell % 9 / 3 == block
																		&& grid.Exists(cell, digit) is true)
																	{
																		isSashimi = false;
																		break;
																	}
																}
															}

															// Add to 'result'.
															accumulator.Add(
																new NormalFishTechniqueInfo(
																	conclusions,
																	views: new[]
																	{
																		new View(
																			cellOffsets: null,
																			candidateOffsets,
																			regionOffsets: new[]
																			{
																				(0, bs1), (0, bs2),
																				(0, bs3), (0, bs4),
																				(1, cs1), (1, cs2),
																				(1, cs3), (1, cs4)
																			},
																			links: null),
																		GetDirectView(
																			grid, digit, baseSets4,
																			coverSets4, searchRow, finCellsMap)
																	},
																	digit,
																	baseSets: baseSets4.ToArray(),
																	coverSets: coverSets4.ToArray(),
																	finCellOffsets: finCellsMap.ToArray(),
																	isSashimi));
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Get the direct fish view with the specified grid and the base sets.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="baseSets">The base sets.</param>
		/// <param name="coverSets">The cover sets.</param>
		/// <param name="searchRow">Indicates whether the current searcher searches row.</param>
		/// <param name="finCellsMap">The fins map.</param>
		/// <returns>The view.</returns>
		private static View GetDirectView(
			IReadOnlyGrid grid, int digit, ReadOnlySpan<int> baseSets, ReadOnlySpan<int> coverSets,
			bool searchRow, GridMap finCellsMap)
		{
			// Get the highlight cells (necessary).
			var cellOffsets = new List<(int, int)>();
			var candidateOffsets = finCellsMap.IsEmpty ? null : new List<(int, int)>();
			foreach (int baseSet in baseSets)
			{
				foreach (int cell in RegionMaps[baseSet])
				{
					switch (grid.Exists(cell, digit))
					{
						case true when finCellsMap[cell]:
						{
							cellOffsets.Add((1, cell));
							break;
						}
						case false:
						case null:
						{
							if (ValueMaps[digit].Any(c => RegionMaps[GetRegion(c, searchRow ? Column : Row)][cell]))
							{
								continue;
							}

							var baseMap = GridMap.Empty;
							foreach (int b in baseSets)
							{
								baseMap |= RegionMaps[b];
							}
							var coverMap = GridMap.Empty;
							foreach (int c in coverSets)
							{
								coverMap |= RegionMaps[c];
							}
							baseMap &= coverMap;
							if (baseMap[cell])
							{
								continue;
							}

							cellOffsets.Add((0, cell));
							break;
						}
						//default:
						//{
						//	// Don't forget this case.
						//	continue;
						//}
					}
				}
			}
			foreach (int cell in ValueMaps[digit])
			{
				cellOffsets.Add((2, cell));
			}

			foreach (int cell in finCellsMap)
			{
				candidateOffsets!.Add((1, cell * 9 + digit));
			}

			return new View(cellOffsets, candidateOffsets, null, null);
		}

		/// <summary>
		/// Record all cells in the all regions to a <see cref="GridMap"/> instance.
		/// </summary>
		/// <param name="map">(<see langword="ref"/> parameter) The map.</param>
		/// <param name="regionOffsets">All region offsets.</param>
		/// <param name="candMap">The candidate map.</param>
		private static void GetGridMap(ref GridMap map, ReadOnlySpan<int> regionOffsets, GridMap candMap)
		{
			foreach (int regionOffset in regionOffsets)
			{
				map |= RegionMaps[regionOffset];
			}

			map &= candMap;
		}
	}
}
