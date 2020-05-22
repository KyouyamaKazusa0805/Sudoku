using System;
using System.Collections.Generic;
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
using UlType1 = Sudoku.Solving.Manual.Uniqueness.Loops.UlType1DetailData;
using UlType2 = Sudoku.Solving.Manual.Uniqueness.Loops.UlType2DetailData;
using UlType3 = Sudoku.Solving.Manual.Uniqueness.Loops.UlType3DetailData;
using UlType4 = Sudoku.Solving.Manual.Uniqueness.Loops.UlType4DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Encapsulates a <b>unique loop</b> (UL) technique searcher.
	/// In fact the unique loop can also search for URs.
	/// </summary>
	[TechniqueDisplay("Unique Loop")]
	public sealed class UlTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 46;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			for (int cell = 0; cell < 81; cell++)
			{
				if (grid.IsBivalueCell(cell, out short mask))
				{
					int d1 = mask.GetNextSet(-1);
					int d2 = mask.GetNextSet(d1);
					int[] digits = new[] { d1, d2 };

					var tempLoop = new List<int>();
					var loops = new List<List<int>>();

					CheckForLoopsRecursively(grid, cell, d1, d2, tempLoop, 2, 0, -1, loops);

					// Check loop finished.
					foreach (var loop in loops)
					{
						// Potential loop found. Check it.
						if (IsValidLoop(grid, loop))
						{
							// This is a unique loop.
							// Get cells with more than two candidates.
							var extraCells = new List<int>(2);
							foreach (int loopCell in loop)
							{
								if (grid.GetCandidatesReversal(loopCell).CountSet() > 2)
								{
									extraCells.Add(loopCell);
								}
							}

							if (extraCells.Count == 1)
							{
								// Type 1 found.
								int extraCell = extraCells[0];

								// Record all eliminations.
								var conclusions = new List<Conclusion>();
								if (!grid[extraCell, d1])
								{
									conclusions.Add(new Conclusion(Elimination, extraCell, d1));
								}
								if (!grid[extraCell, d2])
								{
									conclusions.Add(new Conclusion(Elimination, extraCell, d2));
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								// Record all highlight candidates.
								var candidateOffsets = new List<(int, int)>();
								foreach (int loopCell in loop)
								{
									if (loopCell == extraCell)
									{
										// Skip the extra cell in the loop.
										continue;
									}

									candidateOffsets.Add((0, loopCell * 9 + d1));
									candidateOffsets.Add((0, loopCell * 9 + d2));
								}

								// UL type 1.
								accumulator.AddIfDoesNotContain(
									new UlTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												links: null)
										},
										detailData: new UlType1(
											cells: loop,
											digits)));
							}
							else if (extraCells.Count > 2)
							{
								// Type 2 (has more than 2 extra cells) found.
								short extraDigitMask = 0;
								foreach (int extraCell in extraCells)
								{
									extraDigitMask |= grid.GetCandidatesReversal(extraCell);
								}
								extraDigitMask &= (short)~((1 << d1) | (1 << d2));
								int extraDigit = extraDigitMask.FindFirstSet();

								// Record all eliminations.
								var conclusions = new List<Conclusion>();
								var elimMap = new GridMap(extraCells, GridMap.InitializeOption.ProcessPeersWithoutItself);
								foreach (int elimCell in elimMap.Offsets)
								{
									if (!(grid.Exists(elimCell, extraDigit) is true))
									{
										continue;
									}

									conclusions.Add(new Conclusion(Elimination, elimCell, extraDigit));
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								// Record all highlight candidates.
								var candidateOffsets = new List<(int, int)>();
								foreach (int loopCell in loop)
								{
									candidateOffsets.Add((0, loopCell * 9 + d1));
									candidateOffsets.Add((0, loopCell * 9 + d2));
								}
								foreach (int extraCell in extraCells)
								{
									candidateOffsets.Add((1, extraCell * 9 + extraDigit));
								}

								// UL type 2.
								accumulator.AddIfDoesNotContain(
									new UlTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												links: null)
										},
										detailData: new UlType2(
											cells: loop,
											digits,
											extraDigit)));
							}
							else
							{
								if (extraCells.Count != 2)
								{
									// None of type 2 to 4 patterns can satisfy the condition.
									continue;
								}

								var regions = new GridMap(extraCells).CoveredRegions;
								bool hasSameRegion = regions.Any();
								int c1 = extraCells[0];
								int c2 = extraCells[1];
								short extraDigitMask = (short)(grid.GetCandidatesReversal(c1)
									| grid.GetCandidatesReversal(c2));
								extraDigitMask &= (short)~((1 << d1) | (1 << d2));
								int count = extraDigitMask.CountSet();
								if (count == 1)
								{
									CheckType2(
										accumulator, grid, extraDigitMask.FindFirstSet(),
										extraCells, digits, loop);
								}
								else if (count >= 2)
								{
									if (!hasSameRegion)
									{
										// Extra cells lie on different regions,
										// neither type 3 nor 4.
										continue;
									}

									for (int size = 2; size <= 4; size++)
									{
										CheckType3Naked(
											accumulator, grid, extraDigitMask, digits,
											loop, regions, size);
										CheckType3Hidden(
											accumulator, grid, extraCells, digits,
											loop, regions, size);
									}
								}

								if (!hasSameRegion)
								{
									continue;
								}

								CheckType4(accumulator, grid, extraCells, digits, regions, loop);
							}
						}
					}
				}
			}
		}


		/// <summary>
		/// Check for type 2 (with two extra cells).
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="extraDigit">The extra digit.</param>
		/// <param name="extraCells">All extra cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="loop">The loop.</param>
		private static void CheckType2(
			IBag<TechniqueInfo> result, IReadOnlyGrid grid, int extraDigit,
			IReadOnlyList<int> extraCells, int[] digits, IReadOnlyList<int> loop)
		{
			// Record all eliminations.
			var conclusions = new List<Conclusion>();
			var elimMap = new GridMap(extraCells, GridMap.InitializeOption.ProcessPeersWithoutItself);

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
				return;
			}

			// Record all highlight candidates.
			var candidateOffsets = new List<(int, int)>();
			foreach (int cell in loop)
			{
				candidateOffsets.Add((0, cell * 9 + digits[0]));
				candidateOffsets.Add((0, cell * 9 + digits[1]));
			}
			foreach (int extraCell in extraCells)
			{
				candidateOffsets.Add((1, extraCell * 9 + extraDigit));
			}

			// UL type 2.
			result.AddIfDoesNotContain(
				new UlTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: null,
							candidateOffsets,
							regionOffsets: null,
							links: null)
					},
					detailData: new UlType2(loop, digits, extraDigit)));
		}

		/// <summary>
		/// Check type 3 (with naked subsets).
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="extraDigits">The extra digits.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="loop">The loop.</param>
		/// <param name="regions">All regions.</param>
		/// <param name="size">The size.</param>
		private void CheckType3Naked(
			IBag<TechniqueInfo> result, IReadOnlyGrid grid, short extraDigits,
			int[] digits, IReadOnlyList<int> loop, IEnumerable<int> regions, int size)
		{
			foreach (int region in regions)
			{
				int[] cells = RegionCells[region];
				for (int i1 = 0; i1 < 11 - size; i1++)
				{
					int c1 = cells[i1];
					if (grid.GetStatus(c1) != Empty || loop.Contains(c1))
					{
						continue;
					}

					short m1 = grid.GetCandidatesReversal(c1);
					if (size == 2)
					{
						// Check naked pair.
						short mask = (short)(m1 | extraDigits);
						if (mask.CountSet() != 2)
						{
							continue;
						}

						// Naked pair found.
						// Get all eliminations.
						var conclusions = new List<Conclusion>();
						foreach (int cell in cells)
						{
							if (cell == c1 || loop.Contains(cell))
							{
								continue;
							}

							foreach (int digit in extraDigits.GetAllSets())
							{
								if (!(grid.Exists(cell, digit) is true))
								{
									continue;
								}

								conclusions.Add(new Conclusion(Elimination, cell, digit));
							}
						}

						if (conclusions.Count == 0)
						{
							continue;
						}

						// UL type 3.
						// Get all highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						foreach (int cell in loop)
						{
							for (int digit = 0; digit < 9; digit++)
							{
								candidateOffsets.Add((digits.Contains(digit) ? 0 : 1, cell * 9 + digit));
							}
						}
						foreach (int digit in m1.GetAllSets())
						{
							candidateOffsets.Add((1, c1 * 9 + digit));
						}

						// UL type 3 (with naked subset).
						result.AddIfDoesNotContain(
							new UlTechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: new[] { (0, region) },
										links: null)
								},
								detailData: new UlType3(
									cells: loop,
									digits,
									subsetDigits: mask.GetAllSets().ToArray(),
									subsetCells: new[] { c1 },
									isNaked: true)));
					}
					else // size > 2
					{
						for (int i2 = i1 + 1; i2 < 12 - size; i2++)
						{
							int c2 = cells[i2];
							if (grid.GetStatus(c2) != Empty || loop.Contains(c2))
							{
								continue;
							}

							short m2 = grid.GetCandidatesReversal(c2);
							if (size == 3)
							{
								// Check naked triple.
								short mask = (short)((short)(m1 | m2) | extraDigits);
								if (mask.CountSet() != 3)
								{
									continue;
								}

								// Naked pair found.
								// Get all eliminations.
								var conclusions = new List<Conclusion>();
								foreach (int cell in cells)
								{
									if (cell == c1 || cell == c2 || loop.Contains(cell))
									{
										continue;
									}

									foreach (int digit in extraDigits.GetAllSets())
									{
										if (!(grid.Exists(cell, digit) is true))
										{
											continue;
										}

										conclusions.Add(new Conclusion(Elimination, cell, digit));
									}
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								// UL type 3.
								// Get all highlight candidates.
								var candidateOffsets = new List<(int, int)>();
								foreach (int cell in loop)
								{
									for (int digit = 0; digit < 9; digit++)
									{
										candidateOffsets.Add((digits.Contains(digit) ? 0 : 1, cell * 9 + digit));
									}
								}
								foreach (int digit in m1.GetAllSets())
								{
									candidateOffsets.Add((1, c1 * 9 + digit));
								}
								foreach (int digit in m2.GetAllSets())
								{
									candidateOffsets.Add((1, c2 * 9 + digit));
								}

								// UL type 3 (with naked subset).
								result.AddIfDoesNotContain(
									new UlTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: new[] { (0, region) },
												links: null)
										},
										detailData: new UlType3(
											cells: loop,
											digits,
											subsetDigits: mask.GetAllSets().ToArray(),
											subsetCells: new[] { c1, c2 },
											isNaked: true)));
							}
							else // size == 4
							{
								for (int i3 = i2 + 1; i3 < 9; i3++)
								{
									int c3 = cells[i3];
									if (grid.GetStatus(c3) != Empty || loop.Contains(c3))
									{
										continue;
									}

									short m3 = grid.GetCandidatesReversal(c3);

									// Check naked quadruple.
									short mask = (short)((short)((short)(m1 | m2) | m3) | extraDigits);
									if (mask.CountSet() != 4)
									{
										continue;
									}

									// Naked pair found.
									// Get all eliminations.
									var conclusions = new List<Conclusion>();
									foreach (int cell in cells)
									{
										if (cell == c1 || cell == c2 || cell == c3 || loop.Contains(cell))
										{
											continue;
										}

										foreach (int digit in extraDigits.GetAllSets())
										{
											if (!(grid.Exists(cell, digit) is true))
											{
												continue;
											}

											conclusions.Add(new Conclusion(Elimination, cell, digit));
										}
									}

									if (conclusions.Count == 0)
									{
										continue;
									}

									// UL type 3.
									// Get all highlight candidates.
									var candidateOffsets = new List<(int, int)>();
									foreach (int cell in loop)
									{
										for (int digit = 0; digit < 9; digit++)
										{
											candidateOffsets.Add((digits.Contains(digit) ? 0 : 1, cell * 9 + digit));
										}
									}
									foreach (int digit in m1.GetAllSets())
									{
										candidateOffsets.Add((1, c1 * 9 + digit));
									}
									foreach (int digit in m2.GetAllSets())
									{
										candidateOffsets.Add((1, c2 * 9 + digit));
									}
									foreach (int digit in m3.GetAllSets())
									{
										candidateOffsets.Add((1, c3 * 9 + digit));
									}

									// UL type 3 (with naked subset).
									result.AddIfDoesNotContain(
										new UlTechniqueInfo(
											conclusions,
											views: new[]
											{
												new View(
													cellOffsets: null,
													candidateOffsets,
													regionOffsets: new[] { (0, region) },
													links: null)
											},
											detailData: new UlType3(
												cells: loop,
												digits,
												subsetDigits: mask.GetAllSets().ToArray(),
												subsetCells: new[] { c1, c2, c3 },
												isNaked: true)));
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Check type 3 (with hidden subsets).
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="extraCells">The extra cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="loop">The loop.</param>
		/// <param name="regions">All regions.</param>
		/// <param name="size">The size.</param>
		private void CheckType3Hidden(
			IBag<TechniqueInfo> result, IReadOnlyGrid grid, IReadOnlyList<int> extraCells,
			int[] digits, IReadOnlyList<int> loop, IEnumerable<int> regions, int size)
		{
			foreach (int region in regions)
			{
				for (int d1 = 0; d1 < 10 - size; d1++)
				{
					if (!digits.Contains(d1))
					{
						continue;
					}

					short m1 = grid.GetDigitAppearingMask(d1, region);
					if (m1 == 0)
					{
						continue;
					}

					for (int d2 = d1 + 1; d2 < 11 - size; d2++)
					{
						if (!digits.Contains(d2))
						{
							continue;
						}

						short m2 = grid.GetDigitAppearingMask(d2, region);
						if (m2 == 0)
						{
							continue;
						}

						if (size == 2)
						{
							// Check hidden pair.
							short mask = (short)(m1 | m2);
							if (mask.CountSet() == 3)
							{
								// Hidden pair found.
								// Record all elimination cells.
								var elimCells = new List<int>();
								foreach (int pos in mask.GetAllSets())
								{
									int cell = RegionCells[region][pos];
									if (loop.Contains(cell))
									{
										continue;
									}

									elimCells.Add(cell);
								}

								// Record all eliminations.
								var subsetCells = new List<int>();
								var conclusions = new List<Conclusion>();
								foreach (int cell in elimCells)
								{
									if (!loop.Contains(cell))
									{
										subsetCells.Add(cell);
									}

									for (int digit = 0; digit < 9; digit++)
									{
										if (!loop.Contains(cell)
											&& !digits.Contains(digit)
											&& grid.Exists(cell, digit) is true)
										{
											conclusions.Add(new Conclusion(Elimination, cell, digit));
										}
									}
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								// Record all highlight candidates.
								var candidateOffsets = new List<(int, int)>();
								foreach (int cell in loop)
								{
									if (extraCells.Contains(cell))
									{
										candidateOffsets.Add((1, cell * 9 + digits[0]));
										candidateOffsets.Add((1, cell * 9 + digits[1]));
									}
									else
									{
										candidateOffsets.Add((0, cell * 9 + digits[0]));
										candidateOffsets.Add((0, cell * 9 + digits[1]));
									}
								}
								foreach (int cell in RegionCells[region])
								{
									if (loop.Contains(cell))
									{
										continue;
									}

									if (grid.Exists(cell, d1) is true)
									{
										candidateOffsets.Add((1, cell * 9 + d1));
									}
									if (grid.Exists(cell, d2) is true)
									{
										candidateOffsets.Add((1, cell * 9 + d2));
									}
								}

								// Type 3 (with hidden subset).
								result.AddIfDoesNotContain(
									new UlTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: new[] { (0, region) },
												links: null)
										},
										detailData: new UlType3(
											cells: loop,
											digits,
											subsetDigits: new[] { d1, d2 },
											subsetCells,
											isNaked: false)));
							}
						}
						else // size > 2
						{
							for (int d3 = d2 + 1; d3 < 12 - size; d3++)
							{
								short m3 = grid.GetDigitAppearingMask(d3, region);
								if (m3 == 0)
								{
									continue;
								}

								bool condition(int p) => loop.Contains(RegionCells[region][p]);
								if (size == 3)
								{
									// Check hidden triple.
									short mask = (short)((short)(m1 | m2) | m3);
									if (mask.CountSet() == 4
										&& m3.GetAllSets().All(p => !condition(p)))
									{
										// Hidden pair found.
										// Record all elimination cells.
										var elimCells = new List<int>();
										foreach (int pos in mask.GetAllSets())
										{
											int cell = RegionCells[region][pos];
											if (loop.Contains(cell))
											{
												continue;
											}

											elimCells.Add(cell);
										}

										// Record all eliminations.
										var subsetCells = new List<int>();
										var conclusions = new List<Conclusion>();
										foreach (int cell in elimCells)
										{
											if (!loop.Contains(cell))
											{
												subsetCells.Add(cell);
											}

											for (int digit = 0; digit < 9; digit++)
											{
												if (!loop.Contains(cell)
													&& !digits.Contains(digit)
													&& grid.Exists(cell, digit) is true)
												{
													conclusions.Add(new Conclusion(Elimination, cell, digit));
												}
											}
										}

										if (conclusions.Count == 0)
										{
											continue;
										}

										// Record all highlight candidates.
										var candidateOffsets = new List<(int, int)>();
										foreach (int cell in loop)
										{
											if (extraCells.Contains(cell))
											{
												candidateOffsets.Add((1, cell * 9 + digits[0]));
												candidateOffsets.Add((1, cell * 9 + digits[1]));
											}
											else
											{
												candidateOffsets.Add((0, cell * 9 + digits[0]));
												candidateOffsets.Add((0, cell * 9 + digits[1]));
											}
										}
										foreach (int cell in RegionCells[region])
										{
											if (loop.Contains(cell))
											{
												continue;
											}

											if (grid.Exists(cell, d1) is true)
											{
												candidateOffsets.Add((1, cell * 9 + d1));
											}
											if (grid.Exists(cell, d2) is true)
											{
												candidateOffsets.Add((1, cell * 9 + d2));
											}
											if (grid.Exists(cell, d3) is true)
											{
												candidateOffsets.Add((1, cell * 9 + d3));
											}
										}

										// Type 3 (with hidden subset).
										result.AddIfDoesNotContain(
											new UlTechniqueInfo(
												conclusions,
												views: new[]
												{
													new View(
														cellOffsets: null,
														candidateOffsets,
														regionOffsets: new[] { (0, region) },
														links: null)
												},
												detailData: new UlType3(
													cells: loop,
													digits,
													subsetDigits: new[] { d1, d2, d3 },
													subsetCells,
													isNaked: false)));
									}
								}
								else // size > 3
								{
									for (int d4 = d3 + 1; d4 < 9; d4++)
									{
										short m4 = grid.GetDigitAppearingMask(d4, region);
										if (m4 == 0)
										{
											continue;
										}

										// Check hidden quadruple.
										short mask = (short)((short)((short)(m1 | m2) | m3) | m4);
										if (mask.CountSet() == 5
											&& m3.GetAllSets().All(p => !condition(p))
											&& m4.GetAllSets().All(p => !condition(p)))
										{
											// Hidden pair found.
											// Record all elimination cells.
											var elimCells = new List<int>();
											foreach (int pos in mask.GetAllSets())
											{
												int cell = RegionCells[region][pos];
												if (loop.Contains(cell))
												{
													continue;
												}

												elimCells.Add(cell);
											}

											// Record all eliminations.
											var subsetCells = new List<int>();
											var conclusions = new List<Conclusion>();
											foreach (int cell in elimCells)
											{
												if (!loop.Contains(cell))
												{
													subsetCells.Add(cell);
												}

												for (int digit = 0; digit < 9; digit++)
												{
													if (!loop.Contains(cell)
														&& !digits.Contains(digit)
														&& grid.Exists(cell, digit) is true)
													{
														conclusions.Add(new Conclusion(Elimination, cell, digit));
													}
												}
											}

											if (conclusions.Count == 0)
											{
												continue;
											}

											// Record all highlight candidates.
											var candidateOffsets = new List<(int, int)>();
											foreach (int cell in loop)
											{
												if (extraCells.Contains(cell))
												{
													candidateOffsets.Add((1, cell * 9 + digits[0]));
													candidateOffsets.Add((1, cell * 9 + digits[1]));
												}
												else
												{
													candidateOffsets.Add((0, cell * 9 + digits[0]));
													candidateOffsets.Add((0, cell * 9 + digits[1]));
												}
											}
											foreach (int cell in RegionCells[region])
											{
												if (loop.Contains(cell))
												{
													continue;
												}

												if (grid.Exists(cell, d1) is true)
												{
													candidateOffsets.Add((1, cell * 9 + d1));
												}
												if (grid.Exists(cell, d2) is true)
												{
													candidateOffsets.Add((1, cell * 9 + d2));
												}
												if (grid.Exists(cell, d3) is true)
												{
													candidateOffsets.Add((1, cell * 9 + d3));
												}
												if (grid.Exists(cell, d4) is true)
												{
													candidateOffsets.Add((1, cell * 9 + d4));
												}
											}

											// Type 3 (with hidden subset).
											result.AddIfDoesNotContain(
												new UlTechniqueInfo(
													conclusions,
													views: new[]
													{
														new View(
															cellOffsets: null,
															candidateOffsets,
															regionOffsets: new[] { (0, region) },
															links: null)
													},
													detailData: new UlType3(
														cells: loop,
														digits,
														subsetDigits: new[] { d1, d2, d3, d4 },
														subsetCells,
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
		/// Check type 4.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="extraCells">The extra cells.</param>
		/// <param name="digits">The digits.</param>
		/// <param name="regions">All regions.</param>
		/// <param name="loop">The loop.</param>
		private void CheckType4(
			IBag<TechniqueInfo> result, IReadOnlyGrid grid, IReadOnlyList<int> extraCells,
			int[] digits, IEnumerable<int> regions, IReadOnlyList<int> loop)
		{
			foreach (int region in regions)
			{
				for (int i = 0; i < 2; i++)
				{
					int digit = digits[i];
					short mask = grid.GetDigitAppearingMask(digit, region);
					if (mask.CountSet() == 2 && mask.GetAllSets().All(i => loop.Contains(RegionCells[region][i])))
					{
						// Type 4 found.
						int elimDigit = i == 0 ? digits[1] : digits[0];

						// Record all eliminations.
						var conclusions = new List<Conclusion>();
						foreach (int cell in extraCells)
						{
							if (!(grid.Exists(cell, elimDigit) is true))
							{
								continue;
							}

							conclusions.Add(
								new Conclusion(Elimination, cell, elimDigit));
						}

						if (conclusions.Count == 0)
						{
							continue;
						}

						// Record all highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						foreach (int cell in loop)
						{
							if (extraCells.Contains(cell))
							{
								candidateOffsets.Add((1, cell * 9 + digit));
							}
							else
							{
								candidateOffsets.Add((0, cell * 9 + digits[0]));
								candidateOffsets.Add((0, cell * 9 + digits[1]));
							}
						}

						// Type 4.
						result.AddIfDoesNotContain(
							new UlTechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: new[] { (0, region) },
										links: null)
								},
								detailData: new UlType4(
									cells: loop,
									digits,
									conjugatePair: new ConjugatePair(extraCells[0], extraCells[1], digit))));
					}
				}
			}
		}


		/// <summary>
		/// Check whether the loop is valid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="loop">The loop to check.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		private static bool IsValidLoop(IReadOnlyGrid grid, IList<int> loop)
		{
			var visitedOdd = new HashSet<int>();
			var visitedEven = new HashSet<int>();

			bool isOdd = false;
			foreach (int cell in loop)
			{
				for (int regionType = 0; regionType < 3; regionType++)
				{
					int region = stackalloc[]
					{
						GetRegion(cell, RegionLabel.Block),
						GetRegion(cell, RegionLabel.Row),
						GetRegion(cell, RegionLabel.Column)
					}[regionType];
					if (isOdd)
					{
						if (visitedOdd.Contains(region))
						{
							return false;
						}
						else
						{
							visitedOdd.Add(region);
						}
					}
					else
					{
						if (visitedEven.Contains(region))
						{
							return false;
						}
						else
						{
							visitedEven.Add(region);
						}
					}
				}

				isOdd = !isOdd;
			}

			// All regions must have been visited once with each parity (or never).
			return visitedOdd.All(c => visitedEven.Contains(c))
				&& visitedEven.All(c => visitedOdd.Contains(c));
		}

		/// <summary>
		/// Check the validity of the unique loop recursively.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="cell">The cell to check.</param>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <param name="loop">The loop.</param>
		/// <param name="allowedExtraCellsCount">The number of allowed extra cells.</param>
		/// <param name="exDigitsMask">The extra digits mask.</param>
		/// <param name="lastRegionType">The last region type.</param>
		/// <param name="loops">All loops.</param>
		private static void CheckForLoopsRecursively(
			IReadOnlyGrid grid, int cell, int d1, int d2, IList<int> loop,
			int allowedExtraCellsCount, short exDigitsMask,
			int lastRegionType, IList<List<int>> loops)
		{
			loop.Add(cell);
			for (int regionType = 0; regionType < 3; regionType++)
			{
				if (regionType == lastRegionType)
				{
					continue;
				}

				int region = stackalloc[]
				{
					GetRegion(cell, RegionLabel.Block),
					GetRegion(cell, RegionLabel.Row),
					GetRegion(cell, RegionLabel.Column)
				}[regionType];
				for (int pos = 0; pos < 9; pos++)
				{
					int nextCell = RegionCells[region][pos];
					if (grid.GetStatus(nextCell) != Empty)
					{
						continue;
					}

					// loop.Count == 4 is for URs.
					if (loop[0] == nextCell && loop.Count >= 6)
					{
						// The loop is closed. Now save as a copy.
						loops.Add(new List<int>(loop));
					}
					else if (!loop.Contains(nextCell) && !grid[nextCell, d1] && !grid[nextCell, d2])
					{
						short nextCellMask = grid.GetCandidatesReversal(nextCell);
						exDigitsMask |= nextCellMask;
						exDigitsMask &= (short)~((1 << d1) | (1 << d2));
						int digitsCount = nextCellMask.CountSet();

						// We can continue if:
						// (1) The cell has exactly two digits of the loop.
						// (2) The cell has one extra digit, the same as all previous cells
						// with an extra digit (for type 2 only).
						// (3) The cell has extra digits and the maximum number of cells
						// with extra digits, 2, is not reached.
						if (digitsCount != 2 && exDigitsMask.CountSet() != 1 && allowedExtraCellsCount <= 0)
						{
							continue;
						}

						int newAllowedExtraCellCount = allowedExtraCellsCount;
						if (digitsCount > 2)
						{
							newAllowedExtraCellCount--;
						}

						CheckForLoopsRecursively(
							grid, nextCell, d1, d2, loop, newAllowedExtraCellCount,
							exDigitsMask, regionType, loops);
					}
				}
			}

			// Backtracking.
			loop.RemoveLastElement();
		}
	}
}
