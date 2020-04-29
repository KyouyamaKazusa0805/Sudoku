using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Data.CellStatus;
using static Sudoku.GridProcessings;
using static Sudoku.Solving.ConclusionType;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Encapsulates an <b>almost locked candidates</b> (ALC) technique searcher.
	/// </summary>
	[TechniqueDisplay("Almost Locked Candidates")]
	public sealed class AlcTechniqueSearcher : IntersectionTechniqueSearcher
	{
		/// <summary>
		/// Indicates the searcher will check almost locked quadruple (ALQ).
		/// </summary>
		private readonly bool _checkAlq;


		/// <summary>
		/// Initializes an instance with the intersection table.
		/// </summary>
		/// <param name="checkAlq">
		/// Indicates whether the searcher should check almost locked quadruple.
		/// </param>
		public AlcTechniqueSearcher(bool checkAlq) => _checkAlq = checkAlq;


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 45;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
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
			(var emptyMap, _, _, _) = grid;

			foreach (var ((baseSet, coverSet), (a, b, c)) in IntersectionMaps)
			{
				if (c.Overlaps(emptyMap))
				{
					// Process for 2 cases.
					Process(grid, result, size, baseSet, coverSet, a, b, c);
					Process(grid, result, size, coverSet, baseSet, b, a, c);
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
		/// <param name="a">The left grid map.</param>
		/// <param name="b">The right grid map.</param>
		/// <param name="c">The intersection.</param>
		private static void Process(
			IReadOnlyGrid grid, IBag<TechniqueInfo> result, int size,
			int baseSet, int coverSet, GridMap a, GridMap b, GridMap c)
		{
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
					int ahsCell = RegionCells[coverSet][ahsMask.FindFirstSet()];

					// Record all highlight candidates.
					var candidateOffsets = new List<(int, int)>();
					foreach (int digit in digits)
					{
						if (!(grid.Exists(c1, digit) is true))
						{
							continue;
						}

						candidateOffsets.Add((0, c1 * 9 + digit));
					}
					foreach (int cell in c.Offsets)
					{
						foreach (int digit in digits)
						{
							if (!(grid.Exists(cell, digit) is true))
							{
								continue;
							}

							candidateOffsets.Add((1, cell * 9 + digit));
						}
					}
					foreach (int digit in digits)
					{
						if (!(grid.Exists(ahsCell, digit) is true))
						{
							continue;
						}

						candidateOffsets.Add((0, ahsCell * 9 + digit));
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
							if (!(grid.Exists(aCell, digit) is true))
							{
								continue;
							}

							conclusions.Add(new Conclusion(Elimination, aCell, digit));
						}
					}

					for (int digit = 0, temp = (short)(511 & (short)~mask1); digit < 9; digit++, temp >>= 1)
					{
						if ((temp & 1) == 0 || !(grid.Exists(ahsCell, digit) is true))
						{
							continue;
						}

						conclusions.Add(new Conclusion(Elimination, ahsCell, digit));
					}

					if (conclusions.Count == 0)
					{
						continue;
					}

					int[] cells = new[] { c1, ahsCell };
					var valueCells = from cell in cells
									 where grid.GetStatus(cell) != Empty
									 select (0, cell);
					result.Add(
						new AlcTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: valueCells.Any() ? valueCells.ToList() : null,
									candidateOffsets,
									regionOffsets: new[] { (0, baseSet), (1, coverSet) },
									links: null)
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
							int ahsCell1 = RegionCells[coverSet][ahsCellPositions[0]];
							int ahsCell2 = RegionCells[coverSet][ahsCellPositions[1]];

							// Record all highlight candidates.
							var candidateOffsets = new List<(int, int)>();
							foreach (int digit in digits)
							{
								if (grid.Exists(c1, digit) is true)
								{
									candidateOffsets.Add((0, c1 * 9 + digit));
								}
								if (grid.Exists(c2, digit) is true)
								{
									candidateOffsets.Add((0, c2 * 9 + digit));
								}
							}
							foreach (int cell in c.Offsets)
							{
								foreach (int digit in digits)
								{
									if (!(grid.Exists(cell, digit) is true))
									{
										continue;
									}

									candidateOffsets.Add((1, cell * 9 + digit));
								}
							}
							foreach (int digit in digits)
							{
								if (grid.Exists(ahsCell1, digit) is true)
								{
									candidateOffsets.Add((0, ahsCell1 * 9 + digit));
								}
								if (grid.Exists(ahsCell2, digit) is true)
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
									if (!(grid.Exists(aCell, digit) is true))
									{
										continue;
									}

									conclusions.Add(new Conclusion(Elimination, aCell, digit));
								}
							}

							for (int digit = 0, temp = (short)(511 & (short)~m); digit < 9; digit++, temp >>= 1)
							{
								if ((temp & 1) == 0)
								{
									continue;
								}

								if (grid.Exists(ahsCell1, digit) is true)
								{
									conclusions.Add(new Conclusion(Elimination, ahsCell1, digit));
								}
								if (grid.Exists(ahsCell2, digit) is true)
								{
									conclusions.Add(new Conclusion(Elimination, ahsCell2, digit));
								}
							}

							if (conclusions.Count == 0)
							{
								continue;
							}

							int[] cells = new[] { c1, c2, ahsCell1, ahsCell2 };
							var valueCells = from cell in cells
											 where grid.GetStatus(cell) != Empty
											 select (0, cell);
							result.Add(
								new AlcTechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: valueCells.Any() ? valueCells.ToList() : null,
											candidateOffsets,
											regionOffsets: new[] { (0, baseSet), (1, coverSet) },
											links: null)
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
								int ahsCell1 = RegionCells[coverSet][ahsCellPositions[0]];
								int ahsCell2 = RegionCells[coverSet][ahsCellPositions[1]];
								int ahsCell3 = RegionCells[coverSet][ahsCellPositions[2]];

								// Record all highlight candidates.
								var candidateOffsets = new List<(int, int)>();
								foreach (int digit in digits)
								{
									if (grid.Exists(c1, digit) is true)
									{
										candidateOffsets.Add((0, c1 * 9 + digit));
									}
									if (grid.Exists(c2, digit) is true)
									{
										candidateOffsets.Add((0, c2 * 9 + digit));
									}
									if (grid.Exists(c3, digit) is true)
									{
										candidateOffsets.Add((0, c3 * 9 + digit));
									}
								}
								foreach (int cell in c.Offsets)
								{
									foreach (int digit in digits)
									{
										if (!(grid.Exists(cell, digit) is true))
										{
											continue;
										}

										candidateOffsets.Add((1, cell * 9 + digit));
									}
								}
								foreach (int digit in digits)
								{
									if (grid.Exists(ahsCell1, digit) is true)
									{
										candidateOffsets.Add((0, ahsCell1 * 9 + digit));
									}
									if (grid.Exists(ahsCell2, digit) is true)
									{
										candidateOffsets.Add((0, ahsCell2 * 9 + digit));
									}
									if (grid.Exists(ahsCell3, digit) is true)
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
										if (!(grid.Exists(aCell, digit) is true))
										{
											continue;
										}

										conclusions.Add(new Conclusion(Elimination, aCell, digit));
									}
								}

								for (int digit = 0, temp = (short)(511 & (short)~m); digit < 9; digit++, temp >>= 1)
								{
									if ((temp & 1) == 0)
									{
										continue;
									}

									if (grid.Exists(ahsCell1, digit) is true)
									{
										conclusions.Add(new Conclusion(Elimination, ahsCell1, digit));
									}
									if (grid.Exists(ahsCell2, digit) is true)
									{
										conclusions.Add(new Conclusion(Elimination, ahsCell2, digit));
									}
									if (grid.Exists(ahsCell3, digit) is true)
									{
										conclusions.Add(new Conclusion(Elimination, ahsCell3, digit));
									}
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								int[] cells = new[] { c1, c2, c3, ahsCell1, ahsCell2, ahsCell3 };
								var valueCells = from cell in cells
												 where grid.GetStatus(cell) != Empty
												 select (0, cell);
								result.Add(
									new AlcTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: valueCells.Any() ? valueCells.ToList() : null,
												candidateOffsets,
												regionOffsets: new[] { (0, baseSet), (1, coverSet) },
												links: null)
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
