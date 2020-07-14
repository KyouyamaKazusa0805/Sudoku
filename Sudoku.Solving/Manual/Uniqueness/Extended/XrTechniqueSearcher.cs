using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	/// <summary>
	/// Encapsulates an <b>extended rectangle</b> technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.XrType1))]
	[SearcherProperty(46)]
	public sealed partial class XrTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			foreach (var (allCellsMap, pairs, size) in Combinations)
			{
				if ((EmptyMap & allCellsMap) != allCellsMap)
				{
					continue;
				}

				// Check each pair.
				// Ensures all pairs should contains same digits
				// and the kind of digits must be greater than 2.
				bool checkKindsFlag = true;
				foreach (var (l, r) in pairs)
				{
					short tempMask = (short)(grid.GetCandidateMask(l) & grid.GetCandidateMask(r));
					if (tempMask == 0 || tempMask.IsPowerOfTwo())
					{
						checkKindsFlag = false;
						break;
					}
				}
				if (!checkKindsFlag)
				{
					// Failed to check.
					continue;
				}

				// Check the mask of cells from two regions.
				short m1 = 0, m2 = 0;
				foreach (var (l, r) in pairs)
				{
					m1 |= grid.GetCandidateMask(l);
					m2 |= grid.GetCandidateMask(r);
				}

				short resultMask = (short)(m1 | m2);
				short normalDigits = 0, extraDigits = 0;
				foreach (int digit in resultMask.GetAllSets())
				{
					int count = 0;
					foreach (var (l, r) in pairs)
					{
						if (((grid.GetCandidateMask(l) & grid.GetCandidateMask(r)) >> digit & 1) != 0)
						{
							// Both two cells contain same digit.
							count++;
						}
					}

					(count >= 2 ? ref normalDigits : ref extraDigits) |= (short)(1 << digit);
				}

				if (normalDigits.CountSet() != size)
				{
					// The number of normal digits are not enough.
					continue;
				}

				if (resultMask.CountSet() == size + 1)
				{
					// Possible type 1 or 2 found.
					// Now check extra cells.
					int extraDigit = extraDigits.FindFirstSet();
					var extraCellsMap = allCellsMap & CandMaps[extraDigit];
					if (extraCellsMap.IsEmpty)
					{
						continue;
					}

					if (extraCellsMap.Count == 1)
					{
						CheckType1(accumulator, grid, allCellsMap, extraCellsMap, normalDigits, extraDigit);
					}

					CheckType2(accumulator, grid, allCellsMap, extraCellsMap, normalDigits, extraDigit);
				}
				else
				{
					var extraCellsMap = GridMap.Empty;
					foreach (int cell in allCellsMap)
					{
						foreach (int digit in extraDigits.GetAllSets())
						{
							if (!grid[cell, digit])
							{
								extraCellsMap.Add(cell);
								break;
							}
						}
					}

					if (!extraCellsMap.AllSetsAreInOneRegion(out _))
					{
						continue;
					}

					CheckType3Naked(accumulator, grid, allCellsMap, normalDigits, extraDigits, extraCellsMap);
					CheckType14(accumulator, grid, allCellsMap, normalDigits, extraCellsMap);
				}
			}
		}

		private void CheckType1(
			IList<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap allCellsMap,
			GridMap extraCells, short normalDigits, int extraDigit)
		{
			var conclusions = new List<Conclusion>();
			var candidateOffsets = new List<(int, int)>();
			foreach (int cell in allCellsMap)
			{
				if (cell == extraCells.SetAt(0))
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						if (digit != extraDigit)
						{
							conclusions.Add(new Conclusion(Elimination, cell, digit));
						}
					}
				}
				else
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add((0, cell * 9 + digit));
					}
				}
			}

			if (conclusions.Count == 0)
			{
				return;
			}

			accumulator.Add(
				new XrType1TechniqueInfo(
					conclusions,
					views: new[] { new View(candidateOffsets) },
					cells: allCellsMap,
					digits: normalDigits));
		}

		private void CheckType2(
			IList<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap allCellsMap,
			GridMap extraCells, short normalDigits, int extraDigit)
		{
			var elimMap = extraCells.PeerIntersection & CandMaps[extraDigit];
			if (elimMap.IsEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			var candidateOffsets = new List<(int, int)>();
			foreach (int cell in elimMap)
			{
				conclusions.Add(new Conclusion(Elimination, cell, extraDigit));
			}

			foreach (int cell in allCellsMap)
			{
				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add((digit == extraDigit ? 1 : 0, cell * 9 + digit));
				}
			}

			accumulator.Add(
				new XrType2TechniqueInfo(
					conclusions,
					views: new[] { new View(candidateOffsets) },
					cells: allCellsMap,
					digits: normalDigits,
					extraDigit));
		}

		private void CheckType3Naked(
			IList<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap allCellsMap,
			short normalDigits, short extraDigits, GridMap extraCellsMap)
		{
			foreach (int region in extraCellsMap.CoveredRegions)
			{
				int[] otherCells = ((RegionMaps[region] & EmptyMap) - allCellsMap).ToArray();
				for (int size = 1; size < otherCells.Length; size++)
				{
					foreach (int[] cells in otherCells.GetSubsets(size))
					{
						short mask = 0;
						foreach (int cell in cells)
						{
							mask |= grid.GetCandidateMask(cell);
						}

						if ((mask & extraDigits) != extraDigits || mask.CountSet() != size + 1)
						{
							continue;
						}

						var elimMap = (RegionMaps[region] & EmptyMap) - allCellsMap - cells;
						if (elimMap.IsEmpty)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int digit in mask.GetAllSets())
						{
							foreach (int cell in elimMap & CandMaps[digit])
							{
								conclusions.Add(new Conclusion(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<(int, int)>();
						foreach (int cell in allCellsMap - extraCellsMap)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add((0, cell * 9 + digit));
							}
						}
						foreach (int cell in extraCellsMap)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(((mask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
							}
						}
						foreach (int cell in cells)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add((1, cell * 9 + digit));
							}
						}

						accumulator.Add(
							new XrType3TechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: new[] { (0, region) },
										links: null)
								},
								cells: allCellsMap,
								digits: normalDigits,
								extraCells: cells,
								extraDigits: mask,
								region));
					}
				}
			}
		}

		private void CheckType14(
			IList<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap allCellsMap,
			short normalDigits, GridMap extraCellsMap)
		{
			switch (extraCellsMap.Count)
			{
				case 1:
				{
					// Type 1 found.
					// Check eliminations.
					var conclusions = new List<Conclusion>();
					int extraCell = extraCellsMap.SetAt(0);
					foreach (int digit in normalDigits.GetAllSets())
					{
						if (grid.Exists(extraCell, digit) is true)
						{
							conclusions.Add(new Conclusion(Elimination, extraCell, digit));
						}
					}

					if (conclusions.Count == 0)
					{
						return;
					}

					// Record all highlight candidates.
					var candidateOffsets = new List<(int, int)>();
					foreach (int cell in allCellsMap)
					{
						if (cell == extraCell)
						{
							continue;
						}

						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add((0, cell * 9 + digit));
						}
					}

					accumulator.Add(
						new XrType1TechniqueInfo(
							conclusions,
							views: new[] { new View(candidateOffsets) },
							cells: allCellsMap,
							digits: normalDigits));

					break;
				}
				case 2:
				{
					// Type 4.
					short m1 = grid.GetCandidateMask(extraCellsMap.SetAt(0));
					short m2 = grid.GetCandidateMask(extraCellsMap.SetAt(1));
					short conjugateMask = (short)(m1 & m2 & normalDigits);
					if (conjugateMask == 0)
					{
						return;
					}

					foreach (int conjugateDigit in conjugateMask.GetAllSets())
					{
						foreach (int region in extraCellsMap.CoveredRegions)
						{
							var map = RegionMaps[region] & extraCellsMap;
							if (map != extraCellsMap || map != (CandMaps[conjugateDigit] & RegionMaps[region]))
							{
								continue;
							}

							short elimDigits = (short)(normalDigits & ~(1 << conjugateDigit));
							var conclusions = new List<Conclusion>();
							foreach (int digit in elimDigits.GetAllSets())
							{
								foreach (int cell in extraCellsMap & CandMaps[digit])
								{
									conclusions.Add(new Conclusion(Elimination, cell, digit));
								}
							}
							if (conclusions.Count == 0)
							{
								continue;
							}

							var candidateOffsets = new List<(int, int)>();
							foreach (int cell in allCellsMap - extraCellsMap)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add((0, cell * 9 + digit));
								}
							}
							foreach (int cell in extraCellsMap)
							{
								candidateOffsets.Add((1, cell * 9 + conjugateDigit));
							}

							accumulator.Add(
								new XrType4TechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: null,
											candidateOffsets,
											regionOffsets: new[] { (0, region) },
											links: null)
									},
									cells: allCellsMap,
									digits: normalDigits,
									conjugatePair: new ConjugatePair(extraCellsMap, conjugateDigit)));
						}
					}

					break;
				}
			}
		}
	}
}
