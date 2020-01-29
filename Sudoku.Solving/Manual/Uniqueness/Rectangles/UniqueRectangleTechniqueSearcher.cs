using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using UrType1 = Sudoku.Solving.Manual.Uniqueness.Rectangles.UniqueRectangleType1DetailData;
using UrType2 = Sudoku.Solving.Manual.Uniqueness.Rectangles.UniqueRectangleType2DetailData;
using UrType4 = Sudoku.Solving.Manual.Uniqueness.Rectangles.UniqueRectangleType4DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Encapsulates a unique rectangle technique searcher.
	/// </summary>
	public sealed partial class UniqueRectangleTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// Indicates whether the solver should check incompleted URs.
		/// </summary>
		private readonly bool _checkIncompleted;


		/// <summary>
		/// Initializes an instance with the checking option.
		/// </summary>
		/// <param name="checkIncompletedUniquenessPatterns">
		/// Indicates whether the solver should check incompleted URs.
		/// </param>
		public UniqueRectangleTechniqueSearcher(bool checkIncompletedUniquenessPatterns) =>
			_checkIncompleted = checkIncompletedUniquenessPatterns;


		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<UniquenessTechniqueInfo>();

			foreach (int[] cells in TraversingTable)
			{
				// Check if one cell of all is not empty.
				if (cells.Any(c => grid.GetCellStatus(c) != CellStatus.Empty))
				{
					continue;
				}

				// Initalize the data.
				var cellTriplets = new int[4][]
				{
					new[] { cells[1], cells[2], cells[3] }, // 0
					new[] { cells[0], cells[2], cells[3] }, // 1
					new[] { cells[0], cells[1], cells[3] }, // 2
					new[] { cells[0], cells[1], cells[2] }, // 3
				};
				var cellPairs = new int[6][]
				{
					new[] { cells[2], cells[3] }, // 0, 1
					new[] { cells[1], cells[3] }, // 0, 2
					new[] { cells[1], cells[2] }, // 0, 3
					new[] { cells[0], cells[3] }, // 1, 2
					new[] { cells[0], cells[2] }, // 1, 3
					new[] { cells[0], cells[1] }, // 2, 3
				};

				#region Type 1,5 check
				// Traverse on 'cellTriplets'.
				// Check type 1 / 5.
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
						short extraCellMask = (short)(grid.GetMask(extraCell) & 511);
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
							int cellInTripletMask = ~grid.GetMask(cellTriplet[0]) & 511;
							var digits = (~extraCellMask & 511).GetAllSets();
							int extraDigit = cellInTripletMask
								.GetAllSets().First(i => !digits.Contains(i));
							foreach (int cell in cells)
							{
								foreach (int digit in digits)
								{
									if (grid.CandidateExists(cell, digit))
									{
										candidateOffsets.Add((0, cell * 9 + digit));
									}
								}

								if (grid.CandidateExists(cell, extraDigit))
								{
									candidateOffsets.Add((1, cell * 9 + extraDigit));
								}
							}

							// Record all eliminations.
							var conclusions = new List<Conclusion>();
							var elimMap = new GridMap(cellTriplet[0])
								& new GridMap(cellTriplet[1])
								& new GridMap(cellTriplet[2]);
							foreach (int cell in elimMap.Offsets)
							{
								if (grid.CandidateExists(cell, extraDigit))
								{
									conclusions.Add(
										new Conclusion(
											ConclusionType.Elimination, cell * 9 + extraDigit));
								}
							}

							// Check if worth.
							if (conclusions.Count == 0
								|| !_checkIncompleted && candidateOffsets.Count != 11)
							{
								continue;
							}

							// Type 5.
							result.Add(
								new UniqueRectangleTechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: null,
											candidateOffsets,
											regionOffsets: null,
											linkMasks: null)
									},
									detailData: new UrType2(
										cells,
										digits: digits.ToArray(),
										extraDigit,
										isType5: true)));
						}
					}
					else if (totalMaskCount == 7)
					{
						// Pattern found:
						// ab ab
						// ab ab+

						// Record all highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						var digits = (~grid.GetMask(cellTriplet[0]) & 511).GetAllSets();
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
						foreach (int digit in (~grid.GetMask(extraCell) & 511).GetAllSets())
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
						if (!_checkIncompleted && (candidateOffsets.Count != 6 || elimCount != 2)
							|| _checkIncompleted && elimCount == 0)
						{
							continue;
						}

						// Type 1.
						result.Add(
							new UniqueRectangleTechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: null,
										linkMasks: null)
								},
								detailData: new UrType1(cells, digits.ToArray())));
					}
					else
					{
						// Neither type 1 nor 5.
						continue;
					}
				}
				#endregion

				#region Type 2,4,5,6 check
				// Traverse on 'cellPairs'.
				// Check type 2 / 4 / 5 / 6.
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
						0 => new[] { cells[0], cells[1] }, // Type 2.
						1 => new[] { cells[0], cells[2] }, // Type 2.
						2 => new[] { cells[0], cells[3] }, // Type 5.
						3 => new[] { cells[1], cells[2] }, // Type 5.
						4 => new[] { cells[1], cells[3] }, // Type 2.
						5 => new[] { cells[2], cells[3] }, // Type 2.
						_ => throw new Exception("Impossible case.")
					};

					short extraCellMask = 511;
					foreach (int cell in extraCells)
					{
						extraCellMask &= grid.GetMask(cell);
					}
					short totalMask = (short)(extraCellMask & cellPairMask);
					var digits = (~grid.GetMask(cellPair[0]) & 511).GetAllSets();
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
						var elimMap = new GridMap(a) { [a] = false } & new GridMap(b) { [b] = false };
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

						if (conclusions.Count == 0 || !_checkIncompleted && candidateOffsets.Count != 8)
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
							new UniqueRectangleTechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: null,
										linkMasks: null)
								},
								detailData: new UrType2(cells, digits.ToArray(), extraDigit, isType5)));
					}

					// Then check type 4 / 6.
					if (i == 3 || i == 4)
					{
						// TODO: Check type 6.
					}
					else
					{
						// Check type 4.
						// Get region.
						var sameRegions = new List<int>();
						var ((r1, c1, b1), (r2, c2, b2)) =
							(CellUtils.GetRegion(extraCells[0]), CellUtils.GetRegion(extraCells[1]));
						if (r1 == r2) sameRegions.Add(r1 + 9);
						if (c1 == c2) sameRegions.Add(c1 + 18);
						if (b1 == b2) sameRegions.Add(b1);

						foreach (int regionOffset in sameRegions)
						{
							if (digits.All(d => grid.HasDigitValue(d, regionOffset)))
							{
								continue;
							}

							short maskComparer = 0;
							for (int temp = 0; temp < 9; temp++)
							{
								maskComparer += (short)(
									extraCells.Contains(RegionUtils.GetCellOffset(regionOffset, temp))
										? 1
										: 0);

								if (temp != 8)
								{
									maskComparer <<= 1;
								}
							}
							maskComparer.ReverseBits();
							maskComparer = (short)(maskComparer >> 7 & 511);

							var digitsArray = digits.ToArray();
							for (int index = 0; index < 2; index++)
							{
								int digit = digitsArray[index];
								int elimDigit = index == 0 ? digitsArray[1] : digitsArray[0];
								short mask = grid.GetDigitAppearingMask(digit, regionOffset);
								if (mask.CountSet() == 2 && mask == maskComparer)
								{
									// Type 4 found.
									// Record all highlight candidates.
									var candidateOffsets = new List<(int, int)>();
									foreach (int cell in cellPair)
									{
										foreach (int d in digits)
										{
											if (grid.CandidateExists(cell, d))
											{
												candidateOffsets.Add((0, cell * 9 + d));
											}
										}
									}
									foreach (int cell in extraCells)
									{
										if (grid.CandidateExists(cell, digit))
										{
											candidateOffsets.Add((1, cell * 9 + digit));
										}
									}

									// Record all eliminations.
									var conclusions = new List<Conclusion>();
									foreach (int cell in extraCells)
									{
										if (grid.CandidateExists(cell, elimDigit))
										{
											conclusions.Add(
												new Conclusion(
													ConclusionType.Elimination, cell * 9 + elimDigit));
										}
									}

									int elimCount = conclusions.Count;
									if (elimCount == 0
										|| !_checkIncompleted && (candidateOffsets.Count != 6 || elimCount != 2))
									{
										continue;
									}

									// Type 4.
									result.Add(
										new UniqueRectangleTechniqueInfo(
											conclusions,
											views: new[]
											{
												new View(
													cellOffsets: null,
													candidateOffsets,
													regionOffsets: new[] { (0, regionOffset) },
													linkMasks: null)
											},
											detailData: new UrType4(
												cells,
												digits: digitsArray,
												conjugatePair: new ConjugatePair(
													from: extraCells[0],
													to: extraCells[1],
													digit))));
								}
							}
						}
					}
				}
				#endregion
			}

			return result;
		}
	}
}
