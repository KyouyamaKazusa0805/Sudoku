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
					0 => (9, 1), 1 => (9, 1), 2 => (9, 1), 3 => (9, 1),
					4 => (9, 2), 5 => (9, 2),
					6 => (18, 1), 7 => (18, 1),
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
							continue;
						}

						// Now check extra cells and extra digits.
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
						if (!otherDigits.HasOnlyOneElement())
						{
							// TODO: Check BDP 3 digits type 3 or 4.
						}
						else
						{
							// Type 1 or 2 found.
							var extraCells = new List<int>();
							foreach (int cell in allCells)
							{
								if (otherDigits.Any(digit => grid.CandidateExists(cell, digit)))
								{
									extraCells.Add(cell);
								}
							}

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
