using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Data.GridMap.InitializeOption;
using BdpType1 = Sudoku.Solving.Manual.Uniqueness.Polygons.BdpType1DetailData;
using BdpType2 = Sudoku.Solving.Manual.Uniqueness.Polygons.BdpType2DetailData;
using BdpType3 = Sudoku.Solving.Manual.Uniqueness.Polygons.BdpType3DetailData;
using BdpType4 = Sudoku.Solving.Manual.Uniqueness.Polygons.BdpType4DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Encapsulates a <b>Borescoper's deadly pattern</b> technique searcher.
	/// </summary>
	[TechniqueDisplay("Borescoper's Deadly Pattern")]
	public sealed partial class BdpTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 53;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			if (EmptyMap.Count < 7)
			{
				return;
			}

			for (int block = 0; block < 9; block++)
			{
				for (int i = 0; i < 9; i++) // 9 cases.
				{
					int[] quad = Quads[i];
					int[] tempQuad = new int[4];
					for (int j = 0; j < 4; j++)
					{
						// Set all indices to cell offsets.
						tempQuad[j] = (block / 3 * 3 + quad[j] / 3) * 9 + block % 3 * 3 + quad[j] % 3;
					}

					Check3Digits(accumulator, grid, block, tempQuad, i);
					Check4Digits(accumulator, grid, block, tempQuad, i);
				}
			}
		}


		private static void Check3Digits(
			IBag<TechniqueInfo> result, IReadOnlyGrid grid, int block, int[] quad, int i)
		{
			int[][] triplets = new int[4][]
			{
				new[] { quad[0], quad[1], quad[2] }, // (0, 1) and (0, 2) is same region.
				new[] { quad[1], quad[0], quad[3] }, // (0, 1) and (1, 3) is same region.
				new[] { quad[2], quad[0], quad[3] }, // (0, 2) and (2, 3) is same region.
				new[] { quad[3], quad[1], quad[2] }, // (1, 3) and (2, 3) is same region.
			};
			for (int j = 0; j < 4; j++)
			{
				int[] triplet = triplets[j];
				if (triplet.Any(c => grid.GetStatus(c) != Empty))
				{
					continue;
				}

				int region1 = new GridMap { triplet[0], triplet[1] }.CoveredLine;
				int region2 = new GridMap { triplet[0], triplet[2] }.CoveredLine;
				int[,] pair1 = new int[6, 2], pair2 = new int[6, 2];
				(int incre1, int incre2) = i switch
				{
					0 => (9, 1),
					1 => (9, 1),
					2 => (9, 1),
					3 => (9, 1),
					4 => (9, 2),
					5 => (9, 2),
					6 => (18, 1),
					7 => (18, 1),
					8 => (18, 2),
					_ => throw Throwing.ImpossibleCase
				};
				if (region1 >= 9 && region1 < 18)
				{
					// 'region1' is a row and 'region2' is a column.
					RecordPairs(block, region1, pair1, incre1, j);
					RecordPairs(block, region2, pair2, incre2, j);
				}
				else
				{
					// 'region1' is a column and 'region2' is a row.
					RecordPairs(block, region1, pair1, incre2, j);
					RecordPairs(block, region2, pair2, incre1, j);
				}

				for (int i1 = 0; i1 < 6; i1++)
				{
					if (grid.GetStatus(pair1[i1, 0]) != Empty
						|| grid.GetStatus(pair1[i1, 1]) != Empty)
					{
						continue;
					}

					for (int i2 = 0; i2 < 6; i2++)
					{
						if (grid.GetStatus(pair2[i2, 0]) != Empty
							|| grid.GetStatus(pair2[i2, 1]) != Empty)
						{
							continue;
						}

						// Now check extra digit and its cell.
						short pair1Mask = (short)(
							grid.GetCandidatesReversal(pair1[i1, 0])
							& grid.GetCandidatesReversal(pair1[i1, 1]));
						short pair2Mask = (short)(
							grid.GetCandidatesReversal(pair2[i2, 0])
							& grid.GetCandidatesReversal(pair2[i2, 1]));
						short tripletMask = (short)((short)(
							grid.GetCandidatesReversal(triplet[0])
							& grid.GetCandidatesReversal(triplet[1]))
							& grid.GetCandidatesReversal(triplet[2]));
						short digitsMask = (short)((short)(pair1Mask | pair2Mask) | tripletMask);
						if (digitsMask.CountSet() != 3)
						{
							// If the structure is correct, all masks from three parts
							// will hold 3 digits at total. If we get the result mask,
							// the mask from all cells will contain so-called "other digits"
							// bits.
							continue;
						}

						bool predicate(int c)
						{
							var digits = grid.GetCandidatesReversal(c).GetAllSets();
							return digitsMask.GetAllSets().All(d => !digits.Contains(d));
						}
						if ((new[] { pair1[i1, 0], pair1[i1, 1] }).Any(predicate)
							|| new[] { pair2[i2, 0], pair2[i2, 1] }.Any(predicate)
							|| triplet.Any(predicate))
						{
							continue;
						}

						// Now check extra digits.
						var allCells = new List<int>(triplet)
						{
							pair1[i1, 0], pair1[i1, 1], pair2[i2, 0], pair2[i2, 1]
						};
						short totalMask = 0;
						foreach (int cell in allCells)
						{
							totalMask |= grid.GetCandidatesReversal(cell);
						}
						var digits = digitsMask.GetAllSets();
						short otherDigitsMask = (short)(totalMask ^ digitsMask);
						var otherDigits = otherDigitsMask.GetAllSets();

						// Find all extra cells.
						var extraCells = new List<int>();
						foreach (int cell in allCells)
						{
							if (otherDigits.Any(digit => grid.Exists(cell, digit) is true))
							{
								extraCells.Add(cell);
							}
						}

						if (otherDigits.HasOnlyOneElement())
						{
							// Type 1 or 2 found.
							// Now check whether type 1 or 2.
							if (extraCells.Count == 1)
							{
								// Type 1.
								// Check eliminations.
								var conclusions = new List<Conclusion>();
								int extraCell = extraCells[0];
								foreach (int digit in grid.GetCandidatesReversal(extraCell).GetAllSets())
								{
									if ((digitsMask >> digit & 1) != 0)
									{
										conclusions.Add(new Conclusion(Elimination, extraCell, digit));
									}
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								// Record all highlight candidates.
								var candidateOffsets = new List<(int, int)>();
								foreach (int cell in allCells)
								{
									if (cell == extraCell)
									{
										continue;
									}

									foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
									{
										candidateOffsets.Add((0, cell * 9 + digit));
									}
								}

								result.Add(
									new BdpTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												links: null)
										},
										detailData: new BdpType1(
											cells: allCells,
											digits: digits.ToArray())));
							}
							else
							{
								// Type 2.
								// Check eliminations.
								int extraDigit = otherDigits.First();
								var conclusions = new List<Conclusion>();
								var elimMap = new GridMap(extraCells, ProcessPeersWithoutItself);
								if (elimMap.IsEmpty)
								{
									continue;
								}

								foreach (int cell in elimMap.Offsets)
								{
									if (!(grid.Exists(cell, extraDigit) is true))
									{
										continue;
									}

									conclusions.Add(new Conclusion(Elimination, cell, extraDigit));
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								// Record all highlight candidates.
								var candidateOffsets = new List<(int, int)>();
								foreach (int cell in allCells)
								{
									foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
									{
										candidateOffsets.Add((digit == extraDigit ? 1 : 0, cell * 9 + digit));
									}
								}

								result.Add(
									new BdpTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												links: null)
										},
										detailData: new BdpType2(
											cells: allCells,
											digits: digits.ToArray(),
											extraDigit)));
							}
						}
						else
						{
							//Check3DigitsType3Naked(
							//	result, grid, digits, digitsMask, allCells,
							//	extraCells);
							//Check3DigitsType4(
							//	result, grid, block, digits, digitsMask,
							//	allCells, pair1, pair2, triplet);
							// TODO: Check BDP 3 digits type 3 with hidden subsets.
						}
					}
				}
			}
		}

		[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		private static void Check3DigitsType3Naked(
			IBag<TechniqueInfo> result, IReadOnlyGrid grid, IEnumerable<int> digits,
			short digitsMask, IReadOnlyList<int> allCells, IReadOnlyList<int> extraCells)
		{
			var regions = new GridMap(extraCells).CoveredRegions;
			if (regions.None())
			{
				return;
			}

			short extraCellsMask = 0;
			foreach (int extraCell in extraCells)
			{
				extraCellsMask |= grid.GetCandidatesReversal(extraCell);
			}
			foreach (int region in regions)
			{
				for (int size = 2; size <= 5; size++)
				{
					for (int i1 = 0; i1 < 10 - size; i1++)
					{
						int c1 = RegionCells[region][i1];
						if (extraCells.Contains(c1))
						{
							continue;
						}

						short m1 = grid.GetCandidatesReversal(c1);
						if (size == 2)
						{
							// Check naked pair.
							short mask = (short)(digitsMask ^ (m1 | extraCellsMask));
							if (mask.CountSet() != 2)
							{
								continue;
							}

							// Naked pair found.
							// Record all eliminations.
							var conclusions = new List<Conclusion>();
							foreach (int cell in RegionCells[region])
							{
								if (extraCells.Contains(cell) && cell != c1)
								{
									continue;
								}

								foreach (int digit in
									((short)(grid.GetCandidatesReversal(cell) & mask)).GetAllSets())
								{
									conclusions.Add(new Conclusion(Elimination, cell, digit));
								}
							}

							if (conclusions.Count == 0)
							{
								continue;
							}

							// Record all highlight candidates.
							var candidateOffsets = new List<(int, int)>();
							foreach (int cell in allCells)
							{
								if (extraCells.Contains(cell))
								{
									foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
									{
										if ((mask >> digit & 1) != 0)
										{
											candidateOffsets.Add((1, cell * 9 + digit));
										}
										else
										{
											candidateOffsets.Add((0, cell * 9 + digit));
										}
									}
								}
								else
								{
									foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
									{
										candidateOffsets.Add((0, cell * 9 + digit));
									}
								}
							}
							foreach (int digit in grid.GetCandidatesReversal(c1).GetAllSets())
							{
								candidateOffsets.Add((1, c1 * 9 + digit));
							}

							result.Add(
								new BdpTechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: null,
											candidateOffsets,
											regionOffsets: null,
											links: null)
									},
									detailData: new BdpType3(
										cells: allCells,
										digits: digits.ToArray(),
										subsetDigits: mask.GetAllSets().ToArray(),
										subsetCells: new List<int>(extraCells) { c1 },
										isNaked: true)));
						}
						else // size > 2
						{
							for (int i2 = i1 + 1; i2 < 11 - size; i2++)
							{
								int c2 = RegionCells[region][i2];
								if (extraCells.Contains(c2))
								{
									continue;
								}

								short m2 = grid.GetCandidatesReversal(c2);
								if (size == 3)
								{
									// Check naked triple.
									short mask = (short)(digitsMask ^ ((short)(m1 | m2) | extraCellsMask));
									if (mask.CountSet() != 3)
									{
										continue;
									}

									// Naked pair found.
									// Record all eliminations.
									var conclusions = new List<Conclusion>();
									foreach (int cell in RegionCells[region])
									{
										if (extraCells.Contains(cell) && cell != c1 && cell != c2)
										{
											continue;
										}

										foreach (int digit in
											((short)(grid.GetCandidatesReversal(cell) & mask)).GetAllSets())
										{
											conclusions.Add(new Conclusion(Elimination, cell, digit));
										}
									}

									if (conclusions.Count == 0)
									{
										continue;
									}

									// Record all highlight candidates.
									var candidateOffsets = new List<(int, int)>();
									foreach (int cell in allCells)
									{
										if (extraCells.Contains(cell))
										{
											foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
											{
												if ((mask >> digit & 1) != 0)
												{
													candidateOffsets.Add((1, cell * 9 + digit));
												}
												else
												{
													candidateOffsets.Add((0, cell * 9 + digit));
												}
											}
										}
										else
										{
											foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
											{
												candidateOffsets.Add((0, cell * 9 + digit));
											}
										}
									}
									foreach (int digit in grid.GetCandidatesReversal(c1).GetAllSets())
									{
										candidateOffsets.Add((1, c1 * 9 + digit));
									}
									foreach (int digit in grid.GetCandidatesReversal(c2).GetAllSets())
									{
										candidateOffsets.Add((1, c2 * 9 + digit));
									}

									result.Add(
										new BdpTechniqueInfo(
											conclusions,
											views: new[]
											{
												new View(
													cellOffsets: null,
													candidateOffsets,
													regionOffsets: null,
													links: null)
											},
											detailData: new BdpType3(
												cells: allCells,
												digits: digits.ToArray(),
												subsetDigits: mask.GetAllSets().ToArray(),
												subsetCells: new List<int>(extraCells) { c1, c2 },
												isNaked: true)));
								}
								else // size > 3
								{
									for (int i3 = i2 + 1; i3 < 12 - size; i3++)
									{
										int c3 = RegionCells[region][i3];
										if (extraCells.Contains(c3))
										{
											continue;
										}

										short m3 = grid.GetCandidatesReversal(c3);
										if (size == 4)
										{
											// Check naked quadruple.
											short mask = (short)(digitsMask ^ ((short)((short)(
												m1 | m2) | m3) | extraCellsMask));
											if (mask.CountSet() != 4)
											{
												continue;
											}

											// Naked pair found.
											// Record all eliminations.
											var conclusions = new List<Conclusion>();
											foreach (int cell in RegionCells[region])
											{
												if (extraCells.Contains(cell) && cell != c1
													&& cell != c2 && cell != c3)
												{
													continue;
												}

												foreach (int digit in
													((short)(grid.GetCandidatesReversal(cell) & mask)).GetAllSets())
												{
													conclusions.Add(new Conclusion(Elimination, cell, digit));
												}
											}

											if (conclusions.Count == 0)
											{
												continue;
											}

											// Record all highlight candidates.
											var candidateOffsets = new List<(int, int)>();
											foreach (int cell in allCells)
											{
												if (extraCells.Contains(cell))
												{
													foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
													{
														if ((mask >> digit & 1) != 0)
														{
															candidateOffsets.Add((1, cell * 9 + digit));
														}
														else
														{
															candidateOffsets.Add((0, cell * 9 + digit));
														}
													}
												}
												else
												{
													foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
													{
														candidateOffsets.Add((0, cell * 9 + digit));
													}
												}
											}
											foreach (int digit in grid.GetCandidatesReversal(c1).GetAllSets())
											{
												candidateOffsets.Add((1, c1 * 9 + digit));
											}
											foreach (int digit in grid.GetCandidatesReversal(c2).GetAllSets())
											{
												candidateOffsets.Add((1, c2 * 9 + digit));
											}
											foreach (int digit in grid.GetCandidatesReversal(c3).GetAllSets())
											{
												candidateOffsets.Add((1, c3 * 9 + digit));
											}

											result.Add(
												new BdpTechniqueInfo(
													conclusions,
													views: new[]
													{
														new View(
															cellOffsets: null,
															candidateOffsets,
															regionOffsets: null,
															links: null)
													},
													detailData: new BdpType3(
														cells: allCells,
														digits: digits.ToArray(),
														subsetDigits: mask.GetAllSets().ToArray(),
														subsetCells: new List<int>(extraCells) { c1, c2, c3 },
														isNaked: true)));
										}
										else // size == 5
										{
											for (int i4 = 0; i4 < 9; i4++)
											{
												int c4 = RegionCells[region][i4];
												if (extraCells.Contains(c4))
												{
													continue;
												}

												short m4 = grid.GetCandidatesReversal(c4);

												// Check naked quintuple.
												short mask = (short)(digitsMask ^ ((short)((short)(
												m1 | m2) | m3) | extraCellsMask));
												if (mask.CountSet() != 5)
												{
													continue;
												}

												// Naked pair found.
												// Record all eliminations.
												var conclusions = new List<Conclusion>();
												foreach (int cell in RegionCells[region])
												{
													if (extraCells.Contains(cell) && cell != c1
														&& cell != c2 && cell != c3 && cell != c4)
													{
														continue;
													}

													foreach (int digit in
														((short)(grid.GetCandidatesReversal(cell) & mask)).GetAllSets())
													{
														conclusions.Add(new Conclusion(Elimination, cell, digit));
													}
												}

												if (conclusions.Count == 0)
												{
													continue;
												}

												// Record all highlight candidates.
												var candidateOffsets = new List<(int, int)>();
												foreach (int cell in allCells)
												{
													if (extraCells.Contains(cell))
													{
														foreach (int digit in
															grid.GetCandidatesReversal(cell).GetAllSets())
														{
															if ((mask >> digit & 1) != 0)
															{
																candidateOffsets.Add((1, cell * 9 + digit));
															}
															else
															{
																candidateOffsets.Add((0, cell * 9 + digit));
															}
														}
													}
													else
													{
														foreach (int digit in
															grid.GetCandidatesReversal(cell).GetAllSets())
														{
															candidateOffsets.Add((0, cell * 9 + digit));
														}
													}
												}
												foreach (int digit in grid.GetCandidatesReversal(c1).GetAllSets())
												{
													candidateOffsets.Add((1, c1 * 9 + digit));
												}
												foreach (int digit in grid.GetCandidatesReversal(c2).GetAllSets())
												{
													candidateOffsets.Add((1, c2 * 9 + digit));
												}
												foreach (int digit in grid.GetCandidatesReversal(c3).GetAllSets())
												{
													candidateOffsets.Add((1, c3 * 9 + digit));
												}
												foreach (int digit in grid.GetCandidatesReversal(c4).GetAllSets())
												{
													candidateOffsets.Add((1, c4 * 9 + digit));
												}

												result.Add(
													new BdpTechniqueInfo(
														conclusions,
														views: new[]
														{
															new View(
																cellOffsets: null,
																candidateOffsets,
																regionOffsets: null,
																links: null)
														},
														detailData: new BdpType3(
															cells: allCells,
															digits: digits.ToArray(),
															subsetDigits: mask.GetAllSets().ToArray(),
															subsetCells: new List<int>(extraCells) { c1, c2, c3, c4 },
															isNaked: true)));
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

		[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		private static void Check3DigitsType4(
			IBag<TechniqueInfo> result, IReadOnlyGrid grid, int block,
			IEnumerable<int> digits, short digitMask, IReadOnlyList<int> allCells,
			int[,] pair1, int[,] pair2, int[] triplet)
		{
			// When we check type 4, we should be carefully when searching for triplets.
			// Triplet will not always contains a conjugate pair, but a
			// "conjugate triplet region".
			// Note that if the pairs has two conjugate pairs, two cells will form a
			// naked pair instead of other structures.
			short conjugatePairDigits = 0;
			foreach (int digit in digits)
			{
				short mask = grid.GetDigitAppearingMask(digit, block);
				int digitAppearingCount = mask.CountSet();
				if (digitAppearingCount == 0)
				{
					return;
				}

				if (digitAppearingCount <= 3)
				{
					conjugatePairDigits |= (short)(1 << digit);
				}
			}

			if (conjugatePairDigits.CountSet() != 2)
			{
				return;
			}

			// Now "conjugate region" forms. Check eliminations.
			int elimDigit = (digitMask ^ conjugatePairDigits).FindFirstSet();
			var conclusions = new List<Conclusion>();
			foreach (int cell in triplet)
			{
				if (!(grid.Exists(cell, elimDigit) is true))
				{
					continue;
				}

				conclusions.Add(new Conclusion(Elimination, cell, elimDigit));
			}

			if (conclusions.Count == 0)
			{
				return;
			}

			// Record all highlight candidates.
			var candidateOffsets = new List<(int, int)>();
			foreach (int cell in pair1)
			{
				foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
				{
					candidateOffsets.Add((0, cell * 9 + digit));
				}
			}
			foreach (int cell in pair2)
			{
				foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
				{
					candidateOffsets.Add((0, cell * 9 + digit));
				}
			}
			foreach (int cell in triplet)
			{
				foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
				{
					if (digit == elimDigit)
					{
						continue;
					}

					// Only highlight non-eliminated digit.
					candidateOffsets.Add((1, cell * 9 + digit));
				}
			}

			result.Add(
				new BdpTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: null,
							candidateOffsets,
							regionOffsets: new[] { (0, block )},
							links: null)
					},
					detailData: new BdpType4(
						cells: allCells,
						digits: digits.ToArray(),
						region: block)));
		}

		private void Check4Digits(
			IBag<TechniqueInfo> result, IReadOnlyGrid grid,
			int block, int[] quad, int i)
		{
			if (quad.Any(c => grid.GetStatus(c) != Empty))
			{
				return;
			}

			int[] regions = new[]
			{
				new GridMap { quad[0], quad[1] }.CoveredLine,
				new GridMap { quad[0], quad[2] }.CoveredLine,
				new GridMap { quad[1], quad[3] }.CoveredLine,
				new GridMap { quad[2], quad[3] }.CoveredLine,
				new GridMap(quad).CoveredRegions.First()
			};

			int region1 = regions[0], region2 = regions[1];
			int[,] pair1 = new int[6, 2], pair2 = new int[6, 2];
			(int incre1, int incre2) = i switch
			{
				0 => (9, 1),
				1 => (9, 1),
				2 => (9, 1),
				3 => (9, 1),
				4 => (9, 2),
				5 => (9, 2),
				6 => (18, 1),
				7 => (18, 1),
				8 => (18, 2),
				_ => throw Throwing.ImpossibleCase
			};
			if (region1 >= 9 && region1 < 18)
			{
				// 'region1' is a row and 'region2' is a column.
				RecordPairs(block, region1, pair1, incre1, 0);
				RecordPairs(block, region2, pair2, incre2, 0);
			}
			else
			{
				// 'region1' is a column and 'region2' is a row.
				RecordPairs(block, region1, pair1, incre2, 0);
				RecordPairs(block, region2, pair2, incre1, 0);
			}

			for (int i1 = 0; i1 < 6; i1++)
			{
				if (grid.GetStatus(pair1[i1, 0]) != Empty
					|| grid.GetStatus(pair1[i1, 1]) != Empty)
				{
					continue;
				}

				for (int i2 = 0; i2 < 6; i2++)
				{
					if (grid.GetStatus(pair2[i2, 0]) != Empty
						|| grid.GetStatus(pair2[i2, 1]) != Empty)
					{
						continue;
					}

					// Now check extra digit and its cell.
					short pair1Mask = (short)(
						grid.GetCandidatesReversal(pair1[i1, 0])
						& grid.GetCandidatesReversal(pair1[i1, 1]));
					short pair2Mask = (short)(
						grid.GetCandidatesReversal(pair2[i2, 0])
						& grid.GetCandidatesReversal(pair2[i2, 1]));
					short quadMask = (short)((short)((short)(
						grid.GetCandidatesReversal(quad[0])
						& grid.GetCandidatesReversal(quad[1]))
						& grid.GetCandidatesReversal(quad[2]))
						& grid.GetCandidatesReversal(quad[3]));
					short digitsMask = (short)((short)(pair1Mask | pair2Mask) | quadMask);
					int digitsCount = digitsMask.CountSet();
					if (digitsCount != 3 && digitsCount != 4)
					{
						// If the structure is correct, all masks from three parts
						// will hold 3 or 4 digits at total. If we get the result mask,
						// the mask from all cells will contain so-called "other digits"
						// bits.
						continue;
					}

					if (digitsCount == 3)
					{
						// TODO: Check BDP locked type.
					}
					else
					{
						// Now check extra digits.
						var allCells = new List<int>(quad)
						{
							pair1[i1, 0], pair1[i1, 1], pair2[i2, 0], pair2[i2, 1]
						};
						short totalMask = 0;
						foreach (int cell in allCells)
						{
							totalMask |= grid.GetCandidatesReversal(cell);
						}
						var digits = digitsMask.GetAllSets();
						short otherDigitsMask = (short)(totalMask ^ digitsMask);
						var otherDigits = otherDigitsMask.GetAllSets();

						// Find all extra cells.
						var extraCells = new List<int>();
						foreach (int cell in allCells)
						{
							if (!otherDigits.Any(digit => grid.Exists(cell, digit) is true))
							{
								continue;
							}

							extraCells.Add(cell);
						}

						if (otherDigits.HasOnlyOneElement())
						{
							// Type 1 or 2 found.
							// Now check whether type 1 or 2.
							if (extraCells.Count == 1)
							{
								// Type 1.
								// Check eliminations.
								var conclusions = new List<Conclusion>();
								int extraCell = extraCells[0];
								foreach (int digit in grid.GetCandidatesReversal(extraCell).GetAllSets())
								{
									if ((digitsMask >> digit & 1) != 0)
									{
										conclusions.Add(new Conclusion(Elimination, extraCell, digit));
									}
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								// Record all highlight candidates.
								var candidateOffsets = new List<(int, int)>();
								foreach (int cell in allCells)
								{
									if (cell == extraCell)
									{
										continue;
									}

									foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
									{
										candidateOffsets.Add((0, cell * 9 + digit));
									}
								}

								result.Add(
									new BdpTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												links: null)
										},
										detailData: new BdpType1(
											cells: allCells,
											digits: digits.ToArray())));
							}
							else
							{
								// Type 2.
								// Check eliminations.
								int extraDigit = otherDigits.First();
								var conclusions = new List<Conclusion>();
								var elimMap = new GridMap(extraCells, ProcessPeersWithoutItself);
								if (elimMap.IsEmpty)
								{
									continue;
								}

								foreach (int cell in elimMap.Offsets)
								{
									if (!(grid.Exists(cell, extraDigit) is true))
									{
										continue;
									}

									conclusions.Add(new Conclusion(Elimination, cell, extraDigit));
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								// Record all highlight candidates.
								var candidateOffsets = new List<(int, int)>();
								foreach (int cell in allCells)
								{
									foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
									{
										candidateOffsets.Add((digit == extraDigit ? 1 : 0, cell * 9 + digit));
									}
								}

								result.Add(
									new BdpTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												links: null)
										},
										detailData: new BdpType2(
											cells: allCells,
											digits: digits.ToArray(),
											extraDigit)));
							}
						}
						else
						{
							//Check4DigitsType3Naked(
							//	result, grid, digits, digitsMask, allCells,
							//	extraCells);
							//Check4DigitsType4(
							//	result, grid, block, digits, digitsMask,
							//	allCells, pair1, pair2, quad);
							// TODO: Check BDP 4 digits type 3 with hidden subsets.
						}
					}
				}
			}
		}

		[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
		private void Check4DigitsType3Naked(
			IBag<TechniqueInfo> result, IReadOnlyGrid grid, IEnumerable<int> digits,
			short digitsMask, IReadOnlyList<int> allCells, List<int> extraCells)
		{
			// TODO: Check BDP 4 digits type 3 with naked subsets.
		}

		[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
		private void Check4DigitsType4(
			IBag<TechniqueInfo> result, IReadOnlyGrid grid, int block,
			IEnumerable<int> digits, short digitsMask, IReadOnlyList<int> allCells,
			int[,] pair1, int[,] pair2, int[] quad)
		{
			// TODO: Check BDP 4 digits type 4.
		}


		private static void RecordPairs(int block, int region, int[,] pair, int increment, int i)
		{
			for (int z = 0, cur = 0; z < 9; z++)
			{
				int cell = RegionCells[region][z];
				if (block == GetRegion(cell, RegionLabel.Block))
				{
					continue;
				}

				(pair[cur, 0], pair[cur, 1]) = i switch
				{
					0 => (cell, cell + increment),
					1 => region >= 18 && region < 27 ? (cell - increment, cell) : (cell, cell + increment),
					2 => region >= 9 && region < 18 ? (cell - increment, cell) : (cell, cell + increment),
					3 => (cell - increment, cell),
					_ => throw Throwing.ImpossibleCase
				};
				cur++;
			}
		}
	}
}
