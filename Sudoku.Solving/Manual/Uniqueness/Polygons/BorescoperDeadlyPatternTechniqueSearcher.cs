using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Utils;
using BdpType1 = Sudoku.Solving.Manual.Uniqueness.Polygons.BorescoperDeadlyPatternType1DetailData;
using BdpType2 = Sudoku.Solving.Manual.Uniqueness.Polygons.BorescoperDeadlyPatternType2DetailData;
using BdpType3 = Sudoku.Solving.Manual.Uniqueness.Polygons.BorescoperDeadlyPatternType3DetailData;
using BdpType4 = Sudoku.Solving.Manual.Uniqueness.Polygons.BorescoperDeadlyPatternType4DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Encapsulates a Borescoper's deadly pattern technique searcher.
	/// </summary>
	public sealed partial class BorescoperDeadlyPatternTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <inheritdoc/>
		public override int Priority => 53;


		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			(var emptyCells, _, _) = grid;
			if (emptyCells.Count < 7)
			{
				return Array.Empty<TechniqueInfo>();
			}

			var result = new List<BorescoperDeadlyPatternTechniqueInfo>();

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

					Check3Digits(result, grid, block, tempQuad, i);
					Check4Digits(result, grid, block, tempQuad, i);
				}
			}

			return result;
		}


		private static void Check3Digits(
			IList<BorescoperDeadlyPatternTechniqueInfo> result, Grid grid,
			int block, int[] quad, int i)
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
				if (triplet.Any(c => grid.GetCellStatus(c) != CellStatus.Empty))
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
					_ => throw new Exception("Impossible case.")
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
					if (grid.GetCellStatus(pair1[i1, 0]) != CellStatus.Empty
						|| grid.GetCellStatus(pair1[i1, 1]) != CellStatus.Empty)
					{
						continue;
					}

					for (int i2 = 0; i2 < 6; i2++)
					{
						if (grid.GetCellStatus(pair2[i2, 0]) != CellStatus.Empty
							|| grid.GetCellStatus(pair2[i2, 1]) != CellStatus.Empty)
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
							if (otherDigits.Any(digit => grid.CandidateExists(cell, digit)))
							{
								extraCells.Add(cell);
							}
						}

						if (!otherDigits.HasOnlyOneElement())
						{
							Check3DigitsType3Naked(
								result, grid, digits, digitsMask, allCells,
								extraCells);
							Check3DigitsType4(
								result, grid, block, digits, digitsMask,
								allCells, pair1, pair2, triplet);
							// TODO: Check BDP 3 digits type 3 with hidden subests.
						}
						else
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
										conclusions.Add(
											new Conclusion(
												ConclusionType.Elimination, extraCell * 9 + digit));
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
									new BorescoperDeadlyPatternTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												linkMasks: null)
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
								var elimMap = GridMap.CreateInstance(extraCells);
								if (elimMap.Count == 0)
								{
									continue;
								}

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
									new BorescoperDeadlyPatternTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												linkMasks: null)
										},
										detailData: new BdpType2(
											cells: allCells,
											digits: digits.ToArray(),
											extraDigit)));
							}
						}
					}
				}
			}
		}

		private static void Check3DigitsType3Naked(
			IList<BorescoperDeadlyPatternTechniqueInfo> result, Grid grid, IEnumerable<int> digits,
			short digitsMask, IReadOnlyList<int> allCells, IReadOnlyList<int> extraCells)
		{
			var regions = new GridMap(extraCells).CoveredRegions;
			if (!regions.Any())
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
						int c1 = RegionUtils.GetCellOffset(region, i1);
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
							foreach (int cell in GridMap.GetCellsIn(region))
							{
								if (extraCells.Contains(cell) && cell != c1)
								{
									continue;
								}

								foreach (int digit in
									((short)(grid.GetCandidatesReversal(cell) & mask)).GetAllSets())
								{
									conclusions.Add(
										new Conclusion(
											ConclusionType.Elimination, cell * 9 + digit));
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
								new BorescoperDeadlyPatternTechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: null,
											candidateOffsets,
											regionOffsets: null,
											linkMasks: null)
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
								int c2 = RegionUtils.GetCellOffset(region, i2);
								if (extraCells.Contains(c2))
								{
									continue;
								}

								short m2 = grid.GetCandidatesReversal(c2);
								if (size == 3)
								{
									// TODO: Check naked triple.
								}
								else // size > 3
								{
									for (int i3 = i2 + 1; i3 < 12 - size; i3++)
									{
										int c3 = RegionUtils.GetCellOffset(region, i3);
										if (extraCells.Contains(c3))
										{
											continue;
										}

										short m3 = grid.GetCandidatesReversal(c3);
										if (size == 4)
										{
											// TODO: Check naked quadruple.
										}
										else // size == 5
										{
											for (int i4 = 0; i4 < 9; i4++)
											{
												int c4 = RegionUtils.GetCellOffset(region, i4);
												if (extraCells.Contains(c4))
												{
													continue;
												}

												short m4 = grid.GetCandidatesReversal(c4);
												
												// TODO: Check naked quintuple.
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

		private static void Check3DigitsType4(
			IList<BorescoperDeadlyPatternTechniqueInfo> result, Grid grid, int block,
			IEnumerable<int> digits, short digitMask, IReadOnlyList<int> allCells, int[,] pair1,
			int[,] pair2, int[] triplet)
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
				if (mask.CountSet() <= 3)
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
				if (grid.CandidateExists(cell, elimDigit))
				{
					conclusions.Add(
						new Conclusion(ConclusionType.Elimination, cell * 9 + elimDigit));
				}
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
				new BorescoperDeadlyPatternTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: null,
							candidateOffsets,
							regionOffsets: new[] { (0, block )},
							linkMasks: null)
					},
					detailData: new BdpType4(
						cells: allCells,
						digits: digits.ToArray(),
						region: block)));
		}

		private void Check4Digits(
			IList<BorescoperDeadlyPatternTechniqueInfo> result, Grid grid,
			int block, int[] quad, int i)
		{
			// TODO: Check BDP 4 digits type 1, 2, 3, 4 and locked type.
		}


		private static void RecordPairs(int block, int region, int[,] pair, int increment, int i)
		{
			for (int z = 0, cur = 0; z < 9; z++)
			{
				int cell = RegionUtils.GetCellOffset(region, z);
				(_, _, int b) = CellUtils.GetRegion(cell);
				if (block == b)
				{
					continue;
				}

				(pair[cur, 0], pair[cur, 1]) = i switch
				{
					0 => (cell, cell + increment),
					1 => region >= 18 && region < 27 ? (cell - increment, cell) : (cell, cell + increment),
					2 => region >= 9 && region < 18 ? (cell - increment, cell) : (cell, cell + increment),
					3 => (cell - increment, cell),
					_ => throw new Exception("Impossible case.")
				};
				cur++;
			}
		}
	}
}
