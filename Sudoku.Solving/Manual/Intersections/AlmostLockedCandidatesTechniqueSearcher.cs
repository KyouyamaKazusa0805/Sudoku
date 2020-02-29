using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using Intersection = System.ValueTuple<int, int, Sudoku.Data.GridMap, Sudoku.Data.GridMap>;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Encapsulates an almost locked candidates (ALC) technique searcher.
	/// </summary>
	public sealed class AlmostLockedCandidatesTechniqueSearcher : IntersectionTechniqueSearcher
	{
		/// <summary>
		/// Indicates the searcher will check almost locked quadruple (ALQ).
		/// </summary>
		private readonly bool _checkAlq;

		/// <summary>
		/// All intersection series.
		/// </summary>
		private readonly Intersection[,] _intersection;


		/// <summary>
		/// Initializes an instance with the intersection table.
		/// </summary>
		/// <param name="intersection">The intersection table.</param>
		/// <param name="checkAlq">
		/// Indicates whether the searcher should check almost locked quadruple.
		/// </param>
		public AlmostLockedCandidatesTechniqueSearcher(
			Intersection[,] intersection, bool checkAlq) =>
			(_intersection, _checkAlq) = (intersection, checkAlq);


		/// <inheritdoc/>
		public override int Priority => 45;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			for (int size = 2; size <= (_checkAlq ? 4 : 3); size++)
			{
				AccumulateAllBySize(accumulator, grid, size);
			}
		}

		/// <summary>
		/// Take all by size.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="size">The size.</param>
		/// <returns>The result.</returns>
		private void AccumulateAllBySize(IBag<TechniqueInfo> result, IReadOnlyGrid grid, int size)
		{
			for (int i = 0; i < 18; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					var (baseSet, coverSet, left, right) = _intersection[i, j];
					var intersection = left & right;
					if (intersection.Offsets.All(o => grid.GetCellStatus(o) != CellStatus.Empty))
					{
						continue;
					}

					// Process for 2 cases.
					Process(grid, result, size, baseSet, coverSet, left, right, intersection);
					Process(grid, result, size, coverSet, baseSet, right, left, intersection);
				}
			}
		}


		/// <summary>
		/// Process the calculation.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="result">The result.</param>
		/// <param name="size">The size.</param>
		/// <param name="baseSet">The base set.</param>
		/// <param name="coverSet">The cover set.</param>
		/// <param name="left">The left grid map.</param>
		/// <param name="right">The right grid map.</param>
		/// <param name="intersection">The intersection.</param>
		private static void Process(
			IReadOnlyGrid grid, IBag<TechniqueInfo> result, int size,
			int baseSet, int coverSet, GridMap left, GridMap right, GridMap intersection)
		{
			GridMap a = left ^ intersection, b = right ^ intersection;
			int[] aCells = a.ToArray();
			for (int i1 = 0; i1 < 8 - size; i1++)
			{
				int c1 = aCells[i1];
				short mask1 = grid.GetCandidatesReversal(c1);
				if (size == 2)
				{
					// Check almost locked pair.
					var digits = mask1.GetAllSets();
					if (mask1.CountSet() != 2 || digits.Any(digit => grid.HasDigitValue(digit, coverSet)))
					{
						continue;
					}

					var maskList = new List<short>();
					foreach (int digit in digits)
					{
						maskList.Add(grid.GetDigitAppearingMask(digit, coverSet, b));
					}

					short ahsMask = 0;
					foreach (short mask in maskList)
					{
						ahsMask |= mask;
					}
					if (ahsMask.CountSet() != 1)
					{
						continue;
					}

					// Almost locked pair found.
					int ahsCell = RegionUtils.GetCellOffset(coverSet, ahsMask.FindFirstSet());

					// Record all highlight candidates.
					var candidateOffsets = new List<(int, int)>();
					foreach (int digit in digits)
					{
						if (grid.CandidateExists(c1, digit))
						{
							candidateOffsets.Add((0, c1 * 9 + digit));
						}
					}
					foreach (int cell in intersection.Offsets)
					{
						foreach (int digit in digits)
						{
							if (grid.CandidateExists(cell, digit))
							{
								candidateOffsets.Add((0, cell * 9 + digit));
							}
						}
					}
					foreach (int digit in digits)
					{
						if (grid.CandidateExists(ahsCell, digit))
						{
							candidateOffsets.Add((0, ahsCell * 9 + digit));
						}
					}

					// Record all eliminations.
					var conclusions = new List<Conclusion>();
					foreach (int aCell in a.Offsets)
					{
						if (aCell == c1)
						{
							continue;
						}

						foreach (int digit in digits)
						{
							if (grid.CandidateExists(aCell, digit))
							{
								conclusions.Add(
									new Conclusion(ConclusionType.Elimination, aCell, digit));
							}
						}
					}

					for (int digit = 0, temp = (short)(511 & (short)~mask1); digit < 9; digit++, temp >>= 1)
					{
						if ((temp & 1) != 0 && grid.CandidateExists(ahsCell, digit))
						{
							conclusions.Add(
								new Conclusion(ConclusionType.Elimination, ahsCell, digit));
						}
					}

					if (conclusions.Count == 0)
					{
						continue;
					}

					int[] cells = new[] { c1, ahsCell };
					var valueCells = from cell in cells
									 where grid.GetCellStatus(cell) != CellStatus.Empty
									 select (0, cell);
					result.Add(
						new AlmostLockedCandidatesTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: valueCells.Any() ? valueCells.ToList() : null,
									candidateOffsets,
									regionOffsets: new[] { (0, baseSet), (1, coverSet) },
									linkMasks: null)
							},
							digits: digits.ToArray(),
							baseCells: new[] { c1 },
							targetCells: new[] { ahsCell },
							hasValueCell: valueCells.Any()));
				}
				else // size > 2
				{
					for (int i2 = i1 + 1; i2 < 9 - size; i2++)
					{
						int c2 = aCells[i2];
						short mask2 = grid.GetCandidatesReversal(c2);
						if (size == 3)
						{
							// Check almost locked triple.
							short m = (short)(mask1 | mask2);
							var digits = m.GetAllSets();
							if (m.CountSet() != 3 || digits.Any(digit => grid.HasDigitValue(digit, coverSet)))
							{
								continue;
							}

							var maskList = new List<short>();
							foreach (int digit in digits)
							{
								maskList.Add(grid.GetDigitAppearingMask(digit, coverSet, b));
							}
							short ahsMask = 0;
							foreach (short mask in maskList)
							{
								ahsMask |= mask;
							}

							if (ahsMask.CountSet() != 2)
							{
								continue;
							}

							// Almost locked pair found.
							int[] ahsCellPositions = ahsMask.GetAllSets().ToArray();
							int ahsCell1 = RegionUtils.GetCellOffset(coverSet, ahsCellPositions[0]);
							int ahsCell2 = RegionUtils.GetCellOffset(coverSet, ahsCellPositions[1]);

							// Record all highlight candidates.
							var candidateOffsets = new List<(int, int)>();
							foreach (int digit in digits)
							{
								if (grid.CandidateExists(c1, digit))
								{
									candidateOffsets.Add((0, c1 * 9 + digit));
								}
								if (grid.CandidateExists(c2, digit))
								{
									candidateOffsets.Add((0, c2 * 9 + digit));
								}
							}
							foreach (int cell in intersection.Offsets)
							{
								foreach (int digit in digits)
								{
									if (grid.CandidateExists(cell, digit))
									{
										candidateOffsets.Add((0, cell * 9 + digit));
									}
								}
							}
							foreach (int digit in digits)
							{
								if (grid.CandidateExists(ahsCell1, digit))
								{
									candidateOffsets.Add((0, ahsCell1 * 9 + digit));
								}
								if (grid.CandidateExists(ahsCell2, digit))
								{
									candidateOffsets.Add((0, ahsCell2 * 9 + digit));
								}
							}

							// Record all eliminations.
							var conclusions = new List<Conclusion>();
							foreach (int aCell in a.Offsets)
							{
								if (aCell == c1 || aCell == c2)
								{
									continue;
								}

								foreach (int digit in digits)
								{
									if (grid.CandidateExists(aCell, digit))
									{
										conclusions.Add(
											new Conclusion(ConclusionType.Elimination, aCell, digit));
									}
								}
							}

							for (int digit = 0, temp = (short)(511 & (short)~m); digit < 9; digit++, temp >>= 1)
							{
								if ((temp & 1) != 0)
								{
									if (grid.CandidateExists(ahsCell1, digit))
									{
										conclusions.Add(
											new Conclusion(
												ConclusionType.Elimination, ahsCell1, digit));
									}
									if (grid.CandidateExists(ahsCell2, digit))
									{
										conclusions.Add(
											new Conclusion(
												ConclusionType.Elimination, ahsCell2, digit));
									}
								}
							}

							if (conclusions.Count == 0)
							{
								continue;
							}

							int[] cells = new[] { c1, c2, ahsCell1, ahsCell2 };
							var valueCells = from cell in cells
											 where grid.GetCellStatus(cell) != CellStatus.Empty
											 select (0, cell);
							result.Add(
								new AlmostLockedCandidatesTechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: valueCells.Any() ? valueCells.ToList() : null,
											candidateOffsets,
											regionOffsets: new[] { (0, baseSet), (1, coverSet) },
											linkMasks: null)
									},
									digits: digits.ToArray(),
									baseCells: new[] { c1, c2 },
									targetCells: new[] { ahsCell1, ahsCell2 },
									hasValueCell: valueCells.Any()));
						}
						else // size == 4
						{
							for (int i3 = i2 + 1; i3 < 6; i3++)
							{
								int c3 = aCells[i3];
								short mask3 = grid.GetCandidatesReversal(c3);

								// Check almost locked quadruple.
								short m = (short)((short)(mask1 | mask2) | mask3);
								var digits = m.GetAllSets();
								if (m.CountSet() != 4 || digits.Any(digit => grid.HasDigitValue(digit, coverSet)))
								{
									continue;
								}

								var maskList = new List<short>();
								foreach (int digit in digits)
								{
									maskList.Add(grid.GetDigitAppearingMask(digit, coverSet, b));
								}

								short ahsMask = 0;
								foreach (short mask in maskList)
								{
									ahsMask |= mask;
								}

								if (ahsMask.CountSet() != 3)
								{
									continue;
								}

								// Almost locked pair found.
								int[] ahsCellPositions = ahsMask.GetAllSets().ToArray();
								int ahsCell1 = RegionUtils.GetCellOffset(coverSet, ahsCellPositions[0]);
								int ahsCell2 = RegionUtils.GetCellOffset(coverSet, ahsCellPositions[1]);
								int ahsCell3 = RegionUtils.GetCellOffset(coverSet, ahsCellPositions[2]);

								// Record all highlight candidates.
								var candidateOffsets = new List<(int, int)>();
								foreach (int digit in digits)
								{
									if (grid.CandidateExists(c1, digit))
									{
										candidateOffsets.Add((0, c1 * 9 + digit));
									}
									if (grid.CandidateExists(c2, digit))
									{
										candidateOffsets.Add((0, c2 * 9 + digit));
									}
									if (grid.CandidateExists(c3, digit))
									{
										candidateOffsets.Add((0, c3 * 9 + digit));
									}
								}
								foreach (int cell in intersection.Offsets)
								{
									foreach (int digit in digits)
									{
										if (grid.CandidateExists(cell, digit))
										{
											candidateOffsets.Add((0, cell * 9 + digit));
										}
									}
								}
								foreach (int digit in digits)
								{
									if (grid.CandidateExists(ahsCell1, digit))
									{
										candidateOffsets.Add((0, ahsCell1 * 9 + digit));
									}
									if (grid.CandidateExists(ahsCell2, digit))
									{
										candidateOffsets.Add((0, ahsCell2 * 9 + digit));
									}
									if (grid.CandidateExists(ahsCell3, digit))
									{
										candidateOffsets.Add((0, ahsCell3 * 9 + digit));
									}
								}

								// Record all eliminations.
								var conclusions = new List<Conclusion>();
								foreach (int aCell in a.Offsets)
								{
									if (aCell == c1 || aCell == c2 || aCell == c3)
									{
										continue;
									}

									foreach (int digit in digits)
									{
										if (grid.CandidateExists(aCell, digit))
										{
											conclusions.Add(
												  new Conclusion(ConclusionType.Elimination, aCell, digit));
										}
									}
								}

								for (int digit = 0, temp = (short)(511 & (short)~m); digit < 9; digit++, temp >>= 1)
								{
									if ((temp & 1) != 0)
									{
										if (grid.CandidateExists(ahsCell1, digit))
										{
											conclusions.Add(
												new Conclusion(
													ConclusionType.Elimination, ahsCell1, digit));
										}
										if (grid.CandidateExists(ahsCell2, digit))
										{
											conclusions.Add(
												new Conclusion(
													ConclusionType.Elimination, ahsCell2, digit));
										}
										if (grid.CandidateExists(ahsCell3, digit))
										{
											conclusions.Add(
												new Conclusion(
													ConclusionType.Elimination, ahsCell3, digit));
										}
									}
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								int[] cells = new[] { c1, c2, c3, ahsCell1, ahsCell2, ahsCell3 };
								var valueCells = from cell in cells
												 where grid.GetCellStatus(cell) != CellStatus.Empty
												 select (0, cell);
								result.Add(
									new AlmostLockedCandidatesTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: valueCells.Any() ? valueCells.ToList() : null,
												candidateOffsets,
												regionOffsets: new[] { (0, baseSet), (1, coverSet) },
												linkMasks: null)
										},
										digits: digits.ToArray(),
										baseCells: new[] { c1, c2, c3 },
										targetCells: new[] { ahsCell1, ahsCell2, ahsCell3 },
										hasValueCell: valueCells.Any()));
							}
						}
					}
				}
			}
		}
	}
}
