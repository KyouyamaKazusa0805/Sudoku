using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using static Sudoku.Solving.Manual.Uniqueness.Rectangles.UniqueRectangleTechniqueSearcher;
using ArType1 = Sudoku.Solving.Manual.Uniqueness.Rectangles.AvoidableRectangleType1DetailData;
using ArType2 = Sudoku.Solving.Manual.Uniqueness.Rectangles.AvoidableRectangleType2DetailData;
using ArType3 = Sudoku.Solving.Manual.Uniqueness.Rectangles.AvoidableRectangleType3DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Encapsulates an avoidable rectangle (AR) technique searcher.
	/// </summary>
	public sealed class AvoidableRectangleTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<AvoidableRectangleTechniqueInfo>();

			foreach (int[] cells in TraversingTable)
			{
				if (cells.Any(c => grid.GetCellStatus(c) != CellStatus.Given))
				{
					// Avoidable rectangles require all cells are non-given ones.
					continue;
				}

				// Initalize the data.
				int[][] cellTriplets = new int[4][]
				{
					new[] { cells[1], cells[2], cells[3] }, // 0
					new[] { cells[0], cells[2], cells[3] }, // 1
					new[] { cells[0], cells[1], cells[3] }, // 2
					new[] { cells[0], cells[1], cells[2] }, // 3
				};
				int[][] cellPairs = new int[6][]
				{
					new[] { cells[2], cells[3] }, // 0, 1
					new[] { cells[1], cells[3] }, // 0, 2
					new[] { cells[1], cells[2] }, // 0, 3
					new[] { cells[0], cells[3] }, // 1, 2
					new[] { cells[0], cells[2] }, // 1, 3
					new[] { cells[0], cells[1] }, // 2, 3
				};

				CheckType15AndHidden(result, grid, cells, cellTriplets);
				CheckType23(result, grid, cells, cellPairs);
			}

			return result;
		}


		private static void CheckType15AndHidden(
			IList<AvoidableRectangleTechniqueInfo> result, Grid grid,
			int[] cells, int[][] cellTriplets)
		{
			for (int i = 0; i < 4; i++)
			{
				int[] cellTriplet = cellTriplets[i];
				short totalMask = 511;
				foreach (int cell in cellTriplet)
				{
					totalMask &= grid.GetMask(cell);
				}

				// The index is 'i', which also represents the index of the extra cell.
				int extraCell = cells[i];

				// Check all different value kinds are no more than 3.
				int totalMaskCount = totalMask.CountSet();
				if (totalMaskCount == 6)
				{
					// Pattern found:
					// abc abc
					// abc ab+
					// Now check the last cell has only two candidates and
					// they should be 'a' and 'b'.
					short extraCellMask = grid.GetCandidates(extraCell);
					short finalMask = (short)(totalMask & extraCellMask);
					if (extraCellMask.CountSet() == 7 && finalMask.CountSet() == 6)
					{
						// The extra cell is a bivalue cell and the final mask
						// has 2 different digits, which means the pattern should
						// be this:
						// abc abc
						// abc ab
						// Therefore, type 5 found.

						// Record all highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						short cellInTripletMask = grid.GetCandidatesReversal(cellTriplet[0]);
						var digits = (~extraCellMask & 511).GetAllSets();
						int? extraDigit = cellInTripletMask.GetAllSets()
							.FirstOrDefault(i => !digits.Contains(i));

						if (extraDigit == null)
						{
							continue;
						}

						int extraDigitReal = (int)extraDigit;
						foreach (int cell in cells)
						{
							foreach (int digit in digits)
							{
								if (grid.CandidateExists(cell, digit))
								{
									candidateOffsets.Add((0, cell * 9 + digit));
								}
							}

							if (grid.CandidateExists(cell, extraDigitReal))
							{
								candidateOffsets.Add((1, cell * 9 + extraDigitReal));
							}
						}

						// Record all eliminations.
						var conclusions = new List<Conclusion>();
						var elimMap = new GridMap(cellTriplet[0])
							& new GridMap(cellTriplet[1])
							& new GridMap(cellTriplet[2]);
						foreach (int cell in elimMap.Offsets)
						{
							if (grid.CandidateExists(cell, extraDigitReal))
							{
								conclusions.Add(
									new Conclusion(
										ConclusionType.Elimination, cell * 9 + extraDigitReal));
							}
						}

						// Check if worth.
						if (conclusions.Count == 0)
						{
							continue;
						}

						// Type 5.
						result.Add(
							new AvoidableRectangleTechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: null,
										linkMasks: null)
								},
								detailData: new ArType2(cells, digits.ToArray(), extraDigitReal)));
					}
				}
				else if (totalMaskCount == 7)
				{
					// Pattern found:
					// ab ab
					// ab ab+

					// Record all highlight candidates.
					var candidateOffsets = new List<(int, int)>();
					var digits = grid.GetCandidatesReversal(cellTriplet[0]).GetAllSets();
					foreach (int cell in cellTriplet)
					{
						foreach (int digit in digits)
						{
							if (grid.CandidateExists(cell, digit))
							{
								candidateOffsets.Add((0, cell * 9 + digit));
							}
						}
					}

					// Record all eliminations.
					var conclusions = new List<Conclusion>();
					foreach (int digit in grid.GetCandidatesReversal(extraCell).GetAllSets())
					{
						if (grid.CandidateExists(extraCell, digit) && digits.Contains(digit))
						{
							conclusions.Add(
								new Conclusion(
									ConclusionType.Elimination, extraCell * 9 + digit));
						}
					}

					// Check the number of candidates and eliminations.
					int elimCount = conclusions.Count;
					if (elimCount == 0)
					{
						continue;
					}

					// Type 1.
					result.Add(
						new AvoidableRectangleTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: null,
									linkMasks: null)
							},
							detailData: new ArType1(cells, digits.ToArray())));
				}

				// TODO: Check avoidable hidden rectangle.
			}
		}

		private static void CheckType23(
			IList<AvoidableRectangleTechniqueInfo> result, Grid grid,
			int[] cells, int[][] cellPairs)
		{
			// Traverse on 'cellPairs'.
			for (int i = 0; i < 6; i++)
			{
				int[] cellPair = cellPairs[i];
				short cellPairMask = 511;
				foreach (int cell in cellPair)
				{
					cellPairMask &= grid.GetMask(cell);
				}

				if (cellPairMask.CountSet() != 7)
				{
					continue;
				}

				// Pattern found:
				// ab ab
				// ?  ?
				// or pattern:
				// ab ?
				// ?  ab
				int[] extraCells = i switch
				{
					0 => new[] { cells[0], cells[1] },
					1 => new[] { cells[0], cells[2] },
					2 => new[] { cells[0], cells[3] }, // Diagnoal type.
					3 => new[] { cells[1], cells[2] }, // Diagnoal type.
					4 => new[] { cells[1], cells[3] },
					5 => new[] { cells[2], cells[3] },
					_ => throw new Exception("Impossible case.")
				};

				short extraCellMask = 511;
				foreach (int cell in extraCells)
				{
					extraCellMask &= grid.GetMask(cell);
				}
				short totalMask = (short)(extraCellMask & cellPairMask);
				var digits = grid.GetCandidatesReversal(cellPair[0]).GetAllSets();

				if (totalMask.CountSet() == 6)
				{
					// Type 2 / 5 found.
					// Record all highlight candidates.
					var candidateOffsets = new List<(int, int)>();
					foreach (int cell in cellPair)
					{
						foreach (int digit in digits)
						{
							if (grid.CandidateExists(cell, digit))
							{
								candidateOffsets.Add((0, cell * 9 + digit));
							}
						}
					}
					foreach (int cell in extraCells)
					{
						foreach (int digit in digits)
						{
							if (grid.CandidateExists(cell, digit))
							{
								candidateOffsets.Add((1, cell * 9 + digit));
							}
						}
					}

					// Check whether elimination cells exist.
					var (a, b) = (extraCells[0], extraCells[1]);
					var elimMap = new GridMap(a, false) & new GridMap(b, false);
					if (elimMap.Count == 0)
					{
						continue;
					}

					// Record all eliminations.
					int extraDigit = (~totalMask & 511).GetAllSets().First(i => !digits.Contains(i));
					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMap.Offsets)
					{
						if (grid.CandidateExists(cell, extraDigit))
						{
							conclusions.Add(
								new Conclusion(
									ConclusionType.Elimination, cell * 9 + extraDigit));
						}
					}

					if (conclusions.Count == 0)
					{
						continue;
					}

					// Check if the type number is 2 or 5.
					bool isType5 = i switch
					{
						0 => false,
						1 => false,
						4 => false,
						5 => false,
						2 => true,
						3 => true,
						_ => throw new Exception("Impossible case.")
					};

					// Type 2 / 5.
					result.Add(
						new AvoidableRectangleTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: null,
									linkMasks: null)
							},
							detailData: new ArType2(cells, digits.ToArray(), extraDigit)));
				}

				if (i != 2 && i != 3)
				{
					// Check type 3.
					CellUtils.IsSameRegion(extraCells[0], extraCells[1], out int[] regions);
					for (int size = 1; size <= 3; size++)
					{
						CheckType3Naked(result, grid, cells, digits, regions, size);
						CheckType3Hidden(result, grid, cells, extraCells, digits, regions, size);
					}
				}
			}
		}

		/// <summary>
		/// Check type 3 (with naked subset).
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="regions">All regions.</param>
		/// <param name="size">The size to check.</param>
		private static void CheckType3Naked(
			IList<AvoidableRectangleTechniqueInfo> result, Grid grid, int[] cells,
			IEnumerable<int> digits, int[] regions, int size)
		{
			for (int i = 0, length = regions.Length; i < length; i++)
			{
				int region = regions[i];
				int[] cellsToTraverse = GridMap.GetCellsIn(region);
				for (int i1 = 0; i1 < 10 - size; i1++)
				{
					int c1 = cellsToTraverse[i1];
					if (cells.Contains(c1) || grid.GetCellStatus(c1) != CellStatus.Empty)
					{
						continue;
					}

					short mask1 = grid.GetMask(c1);
					if (size == 1)
					{
						// Check light naked pair.
						short mask = (short)(~mask1 & 511);
						var allCells = new List<int>(cells) { c1 };
						short otherDigitMask = GetOtherDigitMask(
							grid, allCells, digits, out short digitKindsMask);
						if (mask.CountSet() == 2 && otherDigitMask == mask)
						{
							// Type 3 (+ naked) found.
							// Record all highlight candidates.
							var candidateOffsets = new List<(int, int)>();
							foreach (int cell in allCells)
							{
								for (int x = 0, temp = otherDigitMask; x < 9; x++, temp >>= 1)
								{
									if ((temp & 1) != 0 && grid.CandidateExists(cell, x))
									{
										candidateOffsets.Add((1, cell * 9 + x));
									}
								}
								for (int x = 0, temp = digitKindsMask & ~otherDigitMask; x < 9; x++, temp >>= 1)
								{
									if ((temp & 1) != 0 && grid.CandidateExists(cell, x))
									{
										candidateOffsets.Add((0, cell * 9 + x));
									}
								}
							}

							// Record all eliminations.
							var conclusions = new List<Conclusion>();
							for (int digit = 0, temp = otherDigitMask; digit < 9; digit++, temp >>= 1)
							{
								if ((temp & 1) != 0)
								{
									GridMap elimMap = default;
									for (int y = 0, count = 0; y < 5; y++)
									{
										int cell = allCells[y];
										if (grid.CandidateExists(cell, digit))
										{
											if (count++ == 0)
											{
												elimMap = new GridMap(cell, false);
											}
											else
											{
												elimMap &= new GridMap(cell, false);
											}
										}
									}

									foreach (int cell in elimMap.Offsets)
									{
										if (grid.CandidateExists(cell, digit))
										{
											conclusions.Add(
												new Conclusion(
													ConclusionType.Elimination, cell * 9 + digit));
										}
									}
								}
							}

							if (conclusions.Count == 0)
							{
								continue;
							}

							result.Add(
								new AvoidableRectangleTechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: null,
											candidateOffsets,
											regionOffsets: null,
											linkMasks: null)
									},
									detailData: new ArType3(
										cells,
										digits: digits.ToArray(),
										subsetDigits: otherDigitMask.GetAllSets().ToArray(),
										subsetCells: new[] { c1 },
										true)));
						}
					}
					else // size > 1
					{
						for (int i2 = i1 + 1; i2 < 11 - size; i2++)
						{
							int c2 = cellsToTraverse[i2];
							if (cells.Contains(c2) || grid.GetCellStatus(c2) != CellStatus.Empty)
							{
								continue;
							}

							short mask2 = grid.GetMask(c2);
							if (size == 2)
							{
								// Check light naked triple.
								short mask = (short)((~mask1 & 511) | (~mask2 & 511));
								var allCells = new List<int>(cells) { c1, c2 };
								short otherDigitMask = GetOtherDigitMask(
									grid, allCells, digits, out short digitKindsMask);
								if (mask.CountSet() == 3 && otherDigitMask == mask)
								{
									// Type 3 (+ naked) found.
									// Record all highlight candidates.
									var candidateOffsets = new List<(int, int)>();
									foreach (int cell in allCells)
									{
										for (int x = 0, temp = otherDigitMask; x < 9; x++, temp >>= 1)
										{
											if ((temp & 1) != 0 && grid.CandidateExists(cell, x))
											{
												candidateOffsets.Add((1, cell * 9 + x));
											}
										}
										for (int x = 0, temp = digitKindsMask & ~otherDigitMask; x < 9; x++, temp >>= 1)
										{
											if ((temp & 1) != 0 && grid.CandidateExists(cell, x))
											{
												candidateOffsets.Add((0, cell * 9 + x));
											}
										}
									}

									// Record all eliminations.
									var conclusions = new List<Conclusion>();
									for (int digit = 0, temp = otherDigitMask; digit < 9; digit++, temp >>= 1)
									{
										if ((temp & 1) != 0)
										{
											GridMap elimMap = default;
											for (int y = 0, count = 0; y < 6; y++)
											{
												int cell = allCells[y];
												if (grid.CandidateExists(cell, digit))
												{
													if (count++ == 0)
													{
														elimMap = new GridMap(cell, false);
													}
													else
													{
														elimMap &= new GridMap(cell, false);
													}
												}
											}

											foreach (int cell in elimMap.Offsets)
											{
												if (grid.CandidateExists(cell, digit))
												{
													conclusions.Add(
														new Conclusion(
															ConclusionType.Elimination, cell * 9 + digit));
												}
											}
										}
									}

									if (conclusions.Count == 0)
									{
										continue;
									}

									result.Add(
										new AvoidableRectangleTechniqueInfo(
											conclusions,
											views: new[]
											{
												new View(
													cellOffsets: null,
													candidateOffsets,
													regionOffsets: null,
													linkMasks: null)
											},
											detailData: new ArType3(
												cells,
												digits: digits.ToArray(),
												subsetDigits: otherDigitMask.GetAllSets().ToArray(),
												subsetCells: new[] { c1, c2 },
												true)));
								}
							}
							else // size == 3
							{
								for (int i3 = i2 + 1; i3 < 9; i3++)
								{
									int c3 = cellsToTraverse[i3];
									if (cells.Contains(c3) || grid.GetCellStatus(c3) != CellStatus.Empty)
									{
										continue;
									}

									// Check light naked quadruple.
									short mask3 = grid.GetMask(c3);
									short mask = (short)(((~mask1 & 511) | (~mask2 & 511) | (~mask3 & 511)) & 511);
									var allCells = new List<int>(cells) { c1, c2, c3 };
									short otherDigitMask = GetOtherDigitMask(
										grid, allCells, digits, out short digitKindsMask);
									if (mask.CountSet() == 4 && otherDigitMask == mask)
									{
										// Type 3 (+ naked) found.
										// Record all highlight candidates.
										var candidateOffsets = new List<(int, int)>();
										foreach (int cell in allCells)
										{
											for (int x = 0, temp = otherDigitMask; x < 9; x++, temp >>= 1)
											{
												if ((temp & 1) != 0 && grid.CandidateExists(cell, x))
												{
													candidateOffsets.Add((1, cell * 9 + x));
												}
											}
											for (int x = 0, temp = digitKindsMask & ~otherDigitMask; x < 9; x++, temp >>= 1)
											{
												if ((temp & 1) != 0 && grid.CandidateExists(cell, x))
												{
													candidateOffsets.Add((0, cell * 9 + x));
												}
											}
										}

										// Record all eliminations.
										var conclusions = new List<Conclusion>();
										for (int digit = 0, temp = otherDigitMask; digit < 9; digit++, temp >>= 1)
										{
											if ((temp & 1) != 0)
											{
												GridMap elimMap = default;
												for (int y = 0, count = 0; y < 7; y++)
												{
													int cell = allCells[y];
													if (grid.CandidateExists(cell, digit))
													{
														if (count++ == 0)
														{
															elimMap = new GridMap(cell, false);
														}
														else
														{
															elimMap &= new GridMap(cell, false);
														}
													}
												}

												foreach (int cell in elimMap.Offsets)
												{
													if (grid.CandidateExists(cell, digit))
													{
														conclusions.Add(
															new Conclusion(
																ConclusionType.Elimination, cell * 9 + digit));
													}
												}
											}
										}

										if (conclusions.Count == 0)
										{
											continue;
										}

										result.Add(
											new AvoidableRectangleTechniqueInfo(
												conclusions,
												views: new[]
												{
													new View(
														cellOffsets: null,
														candidateOffsets,
														regionOffsets: null,
														linkMasks: null)
												},
												detailData: new ArType3(
													cells,
													digits: digits.ToArray(),
													subsetDigits: otherDigitMask.GetAllSets().ToArray(),
													subsetCells: new[] { c1, c2, c3 },
													true)));
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Check type 3 (with hidden subset).
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="extraCells">All extra cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="regions">All regions.</param>
		/// <param name="size">The size to check.</param>
		private static void CheckType3Hidden(
			IList<AvoidableRectangleTechniqueInfo> result, Grid grid, int[] cells,
			int[] extraCells, IEnumerable<int> digits, int[] regions, int size)
		{
			for (int i = 0, length = regions.Length; i < length; i++)
			{
				int region = regions[i];
				for (int d1 = 0; d1 < 10 - size; d1++)
				{
					short mask1 = grid.GetDigitAppearingMask(d1, region);
					if (mask1 == 0 || !digits.Contains(d1))
					{
						continue;
					}

					for (int d2 = d1 + 1; d2 < 11 - size; d2++)
					{
						short mask2 = grid.GetDigitAppearingMask(d2, region);
						if (mask2 == 0 || !digits.Contains(d2))
						{
							continue;
						}

						if (size == 1)
						{
							// Check light hidden pair.
							short mask = (short)(mask1 | mask2);
							if (mask.CountSet() == 3)
							{
								// Type 3 (+ hidden) found.
								// Record all highlight candidates and eliminations.
								var candidateOffsets = new List<(int, int)>();
								var conclusions = new List<Conclusion>();
								var otherDigits = new List<int>();
								var otherCells = new List<int>();
								int[] subsetDigits = new[] { d1, d2 };
								int[] cellsToTraverse = GridMap.GetCellsIn(region);
								foreach (int cell in cells)
								{
									foreach (int digit in digits)
									{
										if (grid.CandidateExists(cell, digit))
										{
											candidateOffsets.Add((0, cell * 9 + digit));
										}
									}
								}
								for (int x = 0, temp = mask; x < 9; x++, temp >>= 1)
								{
									if ((temp & 1) != 0)
									{
										int cell = cellsToTraverse[x];
										if (!cells.Contains(cell))
										{
											otherCells.Add(cell);
											if (grid.CandidateExists(cell, d1))
											{
												candidateOffsets.Add((1, cell * 9 + d1));
											}
											if (grid.CandidateExists(cell, d2))
											{
												candidateOffsets.Add((1, cell * 9 + d2));
											}

											for (int elimDigit = 0; elimDigit < 9; elimDigit++)
											{
												if (!subsetDigits.Contains(elimDigit))
												{
													otherDigits.Add(elimDigit);
													if (grid.CandidateExists(cell, elimDigit))
													{
														conclusions.Add(
															new Conclusion(
																ConclusionType.Elimination, cell * 9 + elimDigit));
													}
												}
											}
										}
									}
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								// Type 3 (+ hidden).
								result.Add(
									new AvoidableRectangleTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												linkMasks: null)
										},
										detailData: new ArType3(
											cells,
											digits: digits.ToArray(),
											subsetDigits: subsetDigits,
											subsetCells: otherCells,
											isNaked: false)));
							}
						}
						else // size > 1
						{
							for (int d3 = d2 + 1; d3 < 12 - size; d3++)
							{
								short mask3 = grid.GetDigitAppearingMask(d3, region);
								if (mask3 == 0)
								{
									continue;
								}

								if (size == 2)
								{
									// Check light hidden triple.
									short mask = (short)((short)(mask1 | mask2) | mask3);
									if (mask.CountSet() == 4
										&& extraCells.All(c => grid.CandidateDoesNotExist(c, d3)))
									{
										// Type 3 (+ hidden) found.
										// Record all highlight candidates and eliminations.
										var candidateOffsets = new List<(int, int)>();
										var conclusions = new List<Conclusion>();
										var otherDigits = new List<int>();
										var otherCells = new List<int>();
										int[] subsetDigits = new[] { d1, d2, d3 };
										int[] cellsToTraverse = GridMap.GetCellsIn(region);
										foreach (int cell in cells)
										{
											foreach (int digit in digits)
											{
												if (grid.CandidateExists(cell, digit))
												{
													candidateOffsets.Add((0, cell * 9 + digit));
												}
											}
										}
										for (int x = 0, temp = mask; x < 9; x++, temp >>= 1)
										{
											if ((temp & 1) != 0)
											{
												int cell = cellsToTraverse[x];
												if (!cells.Contains(cell))
												{
													otherCells.Add(cell);
													if (grid.CandidateExists(cell, d1))
													{
														candidateOffsets.Add((1, cell * 9 + d1));
													}
													if (grid.CandidateExists(cell, d2))
													{
														candidateOffsets.Add((1, cell * 9 + d2));
													}

													for (int elimDigit = 0; elimDigit < 9; elimDigit++)
													{
														if (!subsetDigits.Contains(elimDigit))
														{
															otherDigits.Add(elimDigit);
															if (grid.CandidateExists(cell, elimDigit))
															{
																conclusions.Add(
																	new Conclusion(
																		ConclusionType.Elimination, cell * 9 + elimDigit));
															}
														}
													}
												}
											}
										}

										if (conclusions.Count == 0)
										{
											continue;
										}

										// Type 3 (+ hidden).
										result.Add(
											new AvoidableRectangleTechniqueInfo(
												conclusions,
												views: new[]
												{
													new View(
														cellOffsets: null,
														candidateOffsets,
														regionOffsets: null,
														linkMasks: null)
												},
												detailData: new ArType3(
													cells,
													digits: digits.ToArray(),
													subsetDigits: subsetDigits,
													subsetCells: otherCells,
													isNaked: false)));
									}
								}
								else // size == 3
								{
									for (int d4 = d3 + 1; d4 < 9; d4++)
									{
										short mask4 = grid.GetDigitAppearingMask(d4, region);
										if (mask4 == 0)
										{
											continue;
										}

										// Check light hidden quadruple.
										short mask = (short)((short)((short)(mask1 | mask2) | mask3) | mask4);
										if (mask.CountSet() == 5
											&& extraCells.All(c => grid.CandidateDoesNotExist(c, d3) && grid.CandidateDoesNotExist(c, d4)))
										{
											// Type 3 (+ hidden) found.
											// Record all highlight candidates and eliminations.
											var candidateOffsets = new List<(int, int)>();
											var conclusions = new List<Conclusion>();
											var otherDigits = new List<int>();
											var otherCells = new List<int>();
											int[] subsetDigits = new[] { d1, d2, d3, d4 };
											int[] cellsToTraverse = GridMap.GetCellsIn(region);
											foreach (int cell in cells)
											{
												foreach (int digit in digits)
												{
													if (grid.CandidateExists(cell, digit))
													{
														candidateOffsets.Add((0, cell * 9 + digit));
													}
												}
											}
											for (int x = 0, temp = mask; x < 9; x++, temp >>= 1)
											{
												if ((temp & 1) != 0)
												{
													int cell = cellsToTraverse[x];
													if (!cells.Contains(cell))
													{
														otherCells.Add(cell);
														if (grid.CandidateExists(cell, d1))
														{
															candidateOffsets.Add((1, cell * 9 + d1));
														}
														if (grid.CandidateExists(cell, d2))
														{
															candidateOffsets.Add((1, cell * 9 + d2));
														}

														for (int elimDigit = 0; elimDigit < 9; elimDigit++)
														{
															if (!subsetDigits.Contains(elimDigit))
															{
																otherDigits.Add(elimDigit);
																if (grid.CandidateExists(cell, elimDigit))
																{
																	conclusions.Add(
																		new Conclusion(
																			ConclusionType.Elimination, cell * 9 + elimDigit));
																}
															}
														}
													}
												}
											}

											if (conclusions.Count == 0)
											{
												continue;
											}

											// Type 3 (+ hidden).
											result.Add(
												new AvoidableRectangleTechniqueInfo(
													conclusions,
													views: new[]
													{
														new View(
															cellOffsets: null,
															candidateOffsets,
															regionOffsets: null,
															linkMasks: null)
													},
													detailData: new ArType3(
														cells,
														digits: digits.ToArray(),
														subsetDigits: subsetDigits,
														subsetCells: otherCells,
														isNaked: false)));
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
		/// Get other digit mask used in type 3 with naked subset.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="allCells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="digitKindsMask">(out parameter) The digit kind mask.</param>
		/// <returns>The result mask.</returns>
		private static short GetOtherDigitMask(
			Grid grid, IEnumerable<int> allCells, IEnumerable<int> digits,
			out short digitKindsMask)
		{
			digitKindsMask = 511;
			foreach (int cell in allCells)
			{
				digitKindsMask &= grid.GetMask(cell);
			}
			digitKindsMask = (short)(~digitKindsMask & 511);
			short tempMask = 0;
			foreach (int digit in digits)
			{
				tempMask |= (short)(1 << digit);
			}

			return (short)(digitKindsMask & ~tempMask);
		}
	}
}
