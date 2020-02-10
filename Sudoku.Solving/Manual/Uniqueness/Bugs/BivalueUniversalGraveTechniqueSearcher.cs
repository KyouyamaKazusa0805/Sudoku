using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Runtime;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Utils;
using BugMultiple = Sudoku.Solving.Manual.Uniqueness.Bugs.BivalueUniversalGraveMultipleTrueCandidatesTechniqueInfo;
using BugType1 = Sudoku.Solving.Manual.Uniqueness.Bugs.BivalueUniversalGraveTechniqueInfo;
using BugType2 = Sudoku.Solving.Manual.Uniqueness.Bugs.BivalueUniversalGraveType2TechniqueInfo;
using BugType3 = Sudoku.Solving.Manual.Uniqueness.Bugs.BivalueUniversalGraveType3TechniqueInfo;
using BugType4 = Sudoku.Solving.Manual.Uniqueness.Bugs.BivalueUniversalGraveType4TechniqueInfo;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Encapsulates a bivalue universal grave (BUG) technique searcher.
	/// </summary>
	public sealed class BivalueUniversalGraveTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// All region maps.
		/// </summary>
		private readonly GridMap[] _regionMaps;


		/// <summary>
		/// Initializes an instance with the region maps.
		/// </summary>
		/// <param name="regionMaps"></param>
		public BivalueUniversalGraveTechniqueSearcher(GridMap[] regionMaps) =>
			_regionMaps = regionMaps;


		/// <inheritdoc/>
		/// <exception cref="WrongHandlingException">
		/// Throws when the number of true candidates is naught.
		/// </exception>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var checker = new BugChecker(grid);
			var trueCandidates = checker.TrueCandidates;
			if (trueCandidates.Count == 0)
			{
				return Array.Empty<UniquenessTechniqueInfo>();
			}

			var result = new List<UniquenessTechniqueInfo>();
			int trueCandidatesCount = trueCandidates.Count;
			switch (trueCandidatesCount)
			{
				case 0:
				{
					throw new WrongHandlingException(grid);
				}
				case 1:
				{
					// BUG + 1 found.
					result.Add(
						new BugType1(
							conclusions: new[] { new Conclusion(ConclusionType.Assignment, trueCandidates[0]) },
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets: new[] { (0, trueCandidates[0]) },
									regionOffsets: null,
									linkMasks: null)
							}));
					break;
				}
				default:
				{
					if (CheckSingleDigit(trueCandidates))
					{
						CheckType2(result, grid, trueCandidates);
					}
					else
					{
						CheckMultiple(result, grid, trueCandidates);
						CheckType4(result, grid, trueCandidates);
						for (int size = 2; size <= 5; size++)
						{
							CheckType3Naked(result, grid, trueCandidates, size);
							//if (size != 2)
							//{
							//	// BUG type 3 with a hidden pair does not exist.
							//	CheckType3Hidden(result, grid, trueCandidates, size);
							//}
						}
					}

					break;
				}
			}

			return result;
		}


		#region BUG utils
		/// <summary>
		/// Check type 3 (with hidden subsets).
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		/// <param name="size">The size.</param>
		[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		private void CheckType3Hidden(
			IList<UniquenessTechniqueInfo> result, Grid grid,
			IReadOnlyList<int> trueCandidates, int size)
		{
			// Check whether all true candidates lie on a same region.
			var candsGroupByCell = from cand in trueCandidates group cand by cand / 9;
			var trueCandidateCells = from candGroupByCell in candsGroupByCell
									 select candGroupByCell.Key;
			int trueCandidateCellsCount = 0;
			var map = default(GridMap);
			foreach (int cell in trueCandidateCells)
			{
				if (trueCandidateCellsCount++ == 0)
				{
					map = new GridMap(cell);
				}
				else
				{
					map &= new GridMap(cell);
				}
			}
			if (map.Count != 9 || trueCandidateCellsCount >= size)
			{
				return;
			}

			foreach (var candGroupByCell in candsGroupByCell)
			{
				// Get the region.
				int region = default;
				for (int i = 0; i < 27; i++)
				{
					if (_regionMaps[i] == map)
					{
						region = i;
						break;
					}
				}

				var trueCandidateDigits = (
					from cand in trueCandidates
					orderby cand
					select cand % 9).Distinct();
				short maskInTrueCandidateCells = 0;
				foreach (int cand in trueCandidates)
				{
					maskInTrueCandidateCells |= (short)(1 << cand % 9);
				}
				maskInTrueCandidateCells = (short)(~maskInTrueCandidateCells & 511);

				for (int d1 = 0; d1 < 10 - size; d1++)
				{
					if (grid.HasDigitValue(d1, region) || trueCandidateDigits.Contains(d1))
					{
						continue;
					}

					short mask1 = grid.GetDigitAppearingMask(d1, region);
					for (int d2 = d1 + 1; d2 < 11 - size; d2++)
					{
						if (grid.HasDigitValue(d2, region) || trueCandidateDigits.Contains(d2))
						{
							continue;
						}

						short mask2 = grid.GetDigitAppearingMask(d2, region);
						for (int d3 = d2 + 1; d3 < 12 - size; d3++)
						{
							if (grid.HasDigitValue(d3, region) || trueCandidateDigits.Contains(d3))
							{
								continue;
							}

							short mask3 = grid.GetDigitAppearingMask(d3, region);
							if (size == 3)
							{
								// Check hidden triple.
								short mask = (short)((short)(mask1 | mask2) | mask3);
								if (mask.CountSet() - trueCandidateCellsCount == 2)
								{
									// Hidden triple found.
									int[] digits = new[] { d1, d2, d3 };

									// Record all lighlight candidates.
									var cells = new List<int>();
									var candidateOffsets = new List<(int, int)>(
										from cand in trueCandidates select (0, cand));
									foreach (int cell in map.Offsets)
									{
										foreach (int digit in digits)
										{
											if (grid.CandidateExists(cell, digit))
											{
												candidateOffsets.Add((1, cell * 9 + digit));
											}
										}

										cells.Add(cell);
									}

									// Record eliminations.
									var conclusions = new List<Conclusion>();
									foreach (int cell in trueCandidateCells)
									{
										map[cell] = false;
									}
									foreach (int cell in map.Offsets)
									{
										for (int digit = 0; digit < 9; digit++)
										{
											if (digits.Contains(digit))
											{
												continue;
											}

											if (grid.CandidateExists(cell, digit))
											{
												conclusions.Add(
													new Conclusion(
														ConclusionType.Elimination, cell * 9 + digit));
											}
										}
									}

									if (conclusions.Count == 0)
									{
										continue;
									}

									// Hidden triple.
									result.Add(
										new BugType3(
											conclusions,
											views: new[]
											{
												new View(
													cellOffsets: null,
													candidateOffsets,
													regionOffsets: new[] { (0, region) },
													linkMasks: null)
											},
											trueCandidates,
											digits,
											cells,
											isNaked: false));
								}
							}
							else // size > 3
							{
								for (int d4 = d3 + 1; d4 < 13 - size; d4++)
								{
									if (grid.HasDigitValue(d4, region)
										|| trueCandidateDigits.Contains(d4))
									{
										continue;
									}

									short mask4 = grid.GetDigitAppearingMask(d4, region);
									if (size == 4)
									{
										// TODO: Check hidden quadruple.

									}
									else // size == 5
									{
										for (int d5 = d4 + 1; d5 < 9; d5++)
										{
											if (grid.HasDigitValue(d5, region)
												|| trueCandidateDigits.Contains(d5))
											{
												continue;
											}

											short mask5 = grid.GetDigitAppearingMask(d5, region);

											// TODO: Check hidden quintuple.

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
		/// Check type 3 (with naked subsets).
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		/// <param name="size">The size.</param>
		private void CheckType3Naked(
			IList<UniquenessTechniqueInfo> result, Grid grid,
			IReadOnlyList<int> trueCandidates, int size)
		{
			// Check whether all true candidates lie on a same region.
			var candsGroupByCell = from cand in trueCandidates group cand by cand / 9;
			var trueCandidateCells = from candGroupByCell in candsGroupByCell
									 select candGroupByCell.Key;
			int trueCandidateCellsCount = 0;
			var map = default(GridMap);
			foreach (int cell in trueCandidateCells)
			{
				if (trueCandidateCellsCount++ == 0)
				{
					map = new GridMap(cell);
				}
				else
				{
					map &= new GridMap(cell);
				}
			}
			if (map.Count != 9)
			{
				return;
			}

			foreach (var candGroupByCell in candsGroupByCell)
			{
				// Get the region.
				int region = default;
				for (int i = 0; i < 27; i++)
				{
					if (_regionMaps[i] == map)
					{
						region = i;
						break;
					}
				}

				int[] cells = GridMap.GetCellsIn(region);
				if (cells.Count(c => grid.GetCellStatus(c) == CellStatus.Empty)
					- trueCandidateCellsCount <= size - 1)
				{
					continue;
				}

				short maskInTrueCandidateCells = 0;
				foreach (int cand in trueCandidates)
				{
					maskInTrueCandidateCells |= (short)(1 << cand % 9);
				}

				for (int i1 = 0; i1 < 11 - size; i1++)
				{
					int c1 = RegionUtils.GetCellOffset(region, i1);
					short mask1 = grid.GetCandidatesReversal(c1);
					if (size == 2)
					{
						// Check naked pair.
						short mask = (short)(mask1 | maskInTrueCandidateCells);
						if (mask.CountSet() != 2)
						{
							continue;
						}

						// Naked pair found.
						var digits = mask.GetAllSets();

						// Record all eliminations.
						var conclusions = new List<Conclusion>();
						foreach (int cell in cells)
						{
							if (trueCandidateCells.Contains(cell) || c1 == cell)
							{
								continue;
							}

							foreach (int digit in digits)
							{
								if (grid.CandidateExists(cell, digit))
								{
									conclusions.Add(
										new Conclusion(
											ConclusionType.Elimination, cell * 9 + digit));
								}
							}
						}

						if (conclusions.Count == 0)
						{
							continue;
						}

						// Record all highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						foreach (int cand in candGroupByCell)
						{
							candidateOffsets.Add((0, cand));
						}
						var digitsInCell1 = grid.GetCandidatesReversal(c1).GetAllSets();
						foreach (int digit in digitsInCell1)
						{
							candidateOffsets.Add((1, c1 * 9 + digit));
						}

						// BUG type 3 (with naked pair).
						result.Add(
							new BugType3(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: new[] { (0, region) },
										linkMasks: null)
								},
								trueCandidates,
								digits: digits.ToArray(),
								cells: new[] { c1 },
								isNaked: true));
					}
					else // size > 2
					{
						for (int i2 = i1 + 1; i2 < 12 - size; i2++)
						{
							int c2 = RegionUtils.GetCellOffset(region, i2);
							short mask2 = grid.GetCandidatesReversal(c2);
							if (size == 3)
							{
								// Check naked triple.
								short mask = (short)((short)(mask1 | mask2) | maskInTrueCandidateCells);
								if (mask.CountSet() != 3)
								{
									continue;
								}

								// Naked triple found.
								var digits = mask.GetAllSets();

								// Record all eliminations.
								var conclusions = new List<Conclusion>();
								foreach (int cell in cells)
								{
									if (trueCandidateCells.Contains(cell) || c1 == cell || c2 == cell)
									{
										continue;
									}

									foreach (int digit in digits)
									{
										if (grid.CandidateExists(cell, digit))
										{
											conclusions.Add(
												new Conclusion(
													ConclusionType.Elimination, cell * 9 + digit));
										}
									}
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								// Record all highlight candidates.
								var candidateOffsets = new List<(int, int)>();
								foreach (int cand in candGroupByCell)
								{
									candidateOffsets.Add((0, cand));
								}
								foreach (int digit in grid.GetCandidatesReversal(c1).GetAllSets())
								{
									candidateOffsets.Add((1, c1 * 9 + digit));
								}
								foreach (int digit in grid.GetCandidatesReversal(c2).GetAllSets())
								{
									candidateOffsets.Add((1, c2 * 9 + digit));
								}

								// BUG type 3 (with naked triple).
								result.Add(
									new BugType3(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: new[] { (0, region) },
												linkMasks: null)
										},
										trueCandidates,
										digits: digits.ToArray(),
										cells: new[] { c1, c2 },
										isNaked: true));
							}
							else // size > 3
							{
								for (int i3 = i2 + 1; i3 < 13 - size; i3++)
								{
									int c3 = RegionUtils.GetCellOffset(region, i3);
									short mask3 = grid.GetCandidatesReversal(c3);
									if (size == 4)
									{
										// Check naked quadruple.
										short mask = (short)((short)((short)(mask1 | mask2) | mask3) | maskInTrueCandidateCells);
										if (mask.CountSet() != 4)
										{
											continue;
										}

										// Naked quadruple found.
										var digits = mask.GetAllSets();

										// Record all eliminations.
										var conclusions = new List<Conclusion>();
										foreach (int cell in cells)
										{
											if (trueCandidateCells.Contains(cell)
												|| c1 == cell || c2 == cell || c3 == cell)
											{
												continue;
											}

											foreach (int digit in digits)
											{
												if (grid.CandidateExists(cell, digit))
												{
													conclusions.Add(
														new Conclusion(
															ConclusionType.Elimination, cell * 9 + digit));
												}
											}
										}

										if (conclusions.Count == 0)
										{
											continue;
										}

										// Record all highlight candidates.
										var candidateOffsets = new List<(int, int)>();
										foreach (int cand in candGroupByCell)
										{
											candidateOffsets.Add((0, cand));
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

										// BUG type 3 (with naked quadruple).
										result.Add(
											new BugType3(
												conclusions,
												views: new[]
												{
													new View(
														cellOffsets: null,
														candidateOffsets,
														regionOffsets: new[] { (0, region) },
														linkMasks: null)
												},
												trueCandidates,
												digits: digits.ToArray(),
												cells: new[] { c1, c2, c3 },
												isNaked: true));
									}
									else // size == 5
									{
										for (int i4 = i3 + 1; i4 < 9; i4++)
										{
											int c4 = RegionUtils.GetCellOffset(region, i4);
											short mask4 = grid.GetCandidatesReversal(c4);

											// Check naked quintuple.
											short mask = (short)((short)((short)((short)
												(mask1 | mask2) | mask3) | mask4) | maskInTrueCandidateCells);
											if (mask.CountSet() != 5)
											{
												continue;
											}

											// Naked quintuple found.
											var digits = mask.GetAllSets();

											// Record all eliminations.
											var conclusions = new List<Conclusion>();
											foreach (int cell in cells)
											{
												if (trueCandidateCells.Contains(cell)
													|| c1 == cell || c2 == cell || c3 == cell || c4 == cell)
												{
													continue;
												}

												foreach (int digit in digits)
												{
													if (grid.CandidateExists(cell, digit))
													{
														conclusions.Add(
															new Conclusion(
																ConclusionType.Elimination, cell * 9 + digit));
													}
												}
											}

											if (conclusions.Count == 0)
											{
												continue;
											}

											// Record all highlight candidates.
											var candidateOffsets = new List<(int, int)>();
											foreach (int cand in candGroupByCell)
											{
												candidateOffsets.Add((0, cand));
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

											// BUG type 3 (with naked quintuple).
											result.Add(
												new BugType3(
													conclusions,
													views: new[]
													{
														new View(
															cellOffsets: null,
															candidateOffsets,
															regionOffsets: new[] { (0, region) },
															linkMasks: null)
													},
													trueCandidates,
													digits: digits.ToArray(),
													cells: new[] { c1, c2, c3, c4 },
													isNaked: true));
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
		/// <param name="trueCandidates">All true candidates.</param>
		private static void CheckType4(
			IList<UniquenessTechniqueInfo> result, Grid grid, IReadOnlyList<int> trueCandidates)
		{
			// Conjugate pairs should lie on two cells.
			var candsGroupByCell = from cand in trueCandidates group cand by cand / 9;
			if (candsGroupByCell.Count() != 2)
			{
				return;
			}

			// Check two cell has same region.
			var cells = new List<int>();
			foreach (var candGroupByCell in candsGroupByCell)
			{
				cells.Add(candGroupByCell.Key);
			}
			if (!CellUtils.IsSameRegion(cells.ElementAt(0), cells.ElementAt(1), out int[] regions))
			{
				return;
			}

			// Check for each region.
			foreach (int region in regions)
			{
				// Add up all digits.
				var digits = new HashSet<int>();
				foreach (var candGroupByCell in candsGroupByCell)
				{
					foreach (int cand in candGroupByCell)
					{
						digits.Add(cand % 9);
					}
				}

				// Check whether exists a conjugate pair in this region.
				for (int conjuagtePairDigit = 0; conjuagtePairDigit < 9; conjuagtePairDigit++)
				{
					// Check whether forms a conjugate pair.
					short mask = grid.GetDigitAppearingMask(conjuagtePairDigit, region);
					if (mask.CountSet() != 2)
					{
						continue;
					}

					// Check whether the conjuagte pair lies on current two cells.
					int c1 = RegionUtils.GetCellOffset(region, mask.GetSetBitIndex(1));
					int c2 = RegionUtils.GetCellOffset(region, mask.GetSetBitIndex(2));
					if (c1 != cells[0] || c2 != cells[1])
					{
						continue;
					}

					// Check whether all digits contain that digit.
					if (digits.Contains(conjuagtePairDigit))
					{
						continue;
					}

					// BUG type 4 found.
					// Now add up all eliminations.
					var conclusions = new List<Conclusion>();
					foreach (var candGroupByCell in candsGroupByCell)
					{
						int cell = candGroupByCell.Key;
						short digitMask = 0;
						foreach (int cand in candGroupByCell)
						{
							digitMask |= (short)(1 << cand % 9);
						}

						// Bitwise not.
						digitMask = (short)(~digitMask & 511);
						foreach (int d in digitMask.GetAllSets())
						{
							if (conjuagtePairDigit == d || grid.CandidateDoesNotExist(cell, d))
							{
								continue;
							}

							conclusions.Add(
								new Conclusion(
									ConclusionType.Elimination, cell * 9 + d));
						}
					}

					// Check eliminations.
					if (conclusions.Count == 0)
					{
						continue;
					}

					// BUG type 4.
					result.Add(
						new BugType4(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets:
										new List<(int, int)>(from cand in trueCandidates select (0, cand))
										{
											(1, c1 * 9 + conjuagtePairDigit),
											(1, c2 * 9 + conjuagtePairDigit)
										},
									regionOffsets: new[] { (0, region) },
									linkMasks: null)
							},
							digits.ToList(),
							cells,
							conjugatePair: new ConjugatePair(c1, c2, conjuagtePairDigit)));
				}
			}
		}

		/// <summary>
		/// Check BUG+n.
		/// </summary>
		/// <param name="result">The result list.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		private static void CheckMultiple(
			IList<UniquenessTechniqueInfo> result, Grid grid, IReadOnlyList<int> trueCandidates)
		{
			if (trueCandidates.Count > 18)
			{
				return;
			}

			var digits = new List<int>();
			foreach (int cand in trueCandidates)
			{
				int digit = cand % 9;
				if (!digits.Contains(digit))
				{
					digits.Add(digit);
				}
			}

			if (digits.Count >= 3)
			{
				return;
			}

			var digitsDic = new Dictionary<int, int>();
			foreach (int cand in trueCandidates)
			{
				int digit = cand % 9;
				if (digitsDic.TryGetValue(digit, out _))
				{
					digitsDic[digit]++;
				}
				else
				{
					digitsDic.Add(digit, 1);
				}
			}

			if (digitsDic.All(pair => pair.Value >= 2))
			{
				return;
			}

			if (digitsDic.Count == 2)
			{
				var map = default(FullGridMap);
				for (int i = 0; i < trueCandidates.Count; i++)
				{
					int cand = trueCandidates[i];
					if (i == 0)
					{
						map = new FullGridMap(cand);
					}
					else
					{
						map &= new FullGridMap(cand);
					}
				}

				if (map.Count == 0)
				{
					return;
				}


				// BUG + n found.
				// Check eliminations.
				var conclusions = new List<Conclusion>();
				foreach (int candidate in map.Offsets)
				{
					if (grid.CandidateExists(candidate / 9, candidate % 9))
					{
						conclusions.Add(
							new Conclusion(
								ConclusionType.Elimination, candidate));
					}
				}

				if (conclusions.Count == 0)
				{
					return;
				}

				// BUG + n.
				result.Add(
					new BugMultiple(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets:
									new List<(int, int)>(
										from cand in trueCandidates select (0, cand)),
								regionOffsets: null,
								linkMasks: null)
						},
						candidates: trueCandidates));
			}
			else
			{
				// Degenerated to BUG type 2.
				var map = default(GridMap);
				for (int i = 0; i < trueCandidates.Count; i++)
				{
					int cand = trueCandidates[i];
					if (i == 0)
					{
						map = new GridMap(cand);
					}
					else
					{
						map &= new GridMap(cand);
					}
				}

				if (map.Count != 0)
				{
					// BUG type 2 (or BUG + n, but special) found.
					// Check eliminations.
					var conclusions = new List<Conclusion>();
					int digit = trueCandidates[0] % 9;
					foreach (int cell in map.Offsets)
					{
						if (grid.CandidateExists(cell, digit))
						{
							conclusions.Add(
								new Conclusion(
									ConclusionType.Elimination, cell * 9 + digit));
						}
					}

					if (conclusions.Count == 0)
					{
						return;
					}

					// BUG type 2 (or BUG + n, but special).
					result.Add(
						new BugMultiple(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets:
										new List<(int, int)>(
											from cand in trueCandidates select (0, cand)),
									regionOffsets: null,
									linkMasks: null)
							},
							candidates: trueCandidates));
				}
			}
		}

		/// <summary>
		/// Check type 2.
		/// </summary>
		/// <param name="result">The result list.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		private static void CheckType2(
			IList<UniquenessTechniqueInfo> result, Grid grid, IReadOnlyList<int> trueCandidates)
		{
			var map = default(GridMap);
			int i = 0;
			foreach (int cand in trueCandidates)
			{
				if (i++ == 0)
				{
					map = new GridMap(cand / 9);
				}
				else
				{
					map &= new GridMap(cand / 9);
				}
			}

			if (map.Count == 0)
			{
				return;
			}

			// BUG type 2 found.
			// Check eliminations.
			var conclusions = new List<Conclusion>();
			int digit = trueCandidates[0] % 9;
			foreach (int cell in map.Offsets)
			{
				if (grid.CandidateExists(cell, digit))
				{
					conclusions.Add(
						new Conclusion(
							ConclusionType.Elimination, cell * 9 + digit));
				}
			}

			if (conclusions.Count == 0)
			{
				return;
			}

			// BUG type 2.
			result.Add(
				new BugType2(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: null,
							candidateOffsets:
								new List<(int, int)>(
									from cand in trueCandidates select (0, cand)),
							regionOffsets: null,
							linkMasks: null)
					},
					digit,
					cells: trueCandidates));
		}

		/// <summary>
		/// Check whether all candidates in the list has same digit value.
		/// </summary>
		/// <param name="list">The list of all true candidates.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		private static bool CheckSingleDigit(IReadOnlyList<int> list)
		{
			int i = 0;
			int comparer = default;
			foreach (int cand in list)
			{
				if (i++ == 0)
				{
					comparer = cand % 9;
					continue;
				}

				if (comparer != cand % 9)
				{
					return false;
				}
			}

			return true;
		}
		#endregion
	}
}
