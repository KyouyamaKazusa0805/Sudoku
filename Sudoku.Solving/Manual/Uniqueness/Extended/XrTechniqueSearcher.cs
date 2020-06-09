using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static System.Algorithms;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	/// <summary>
	/// Encapsulates an <b>extended rectangle</b> technique searcher.
	/// </summary>
	[TechniqueDisplay("Extended Rectangle")]
	public sealed class XrTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// The table of regions to traverse.
		/// </summary>
		private static readonly int[,] Regions =
		{
			{ 9, 10 }, { 9, 11 }, { 10, 11 },
			{ 12, 13 }, { 12, 14 }, { 13, 14 },
			{ 15, 16 }, { 15, 17 }, { 16, 17 },
			{ 18, 19 }, { 18, 20 }, { 19, 20 },
			{ 21, 22 }, { 21, 23 }, { 22, 23 },
			{ 24, 25 }, { 24, 26 }, { 25, 26 }
		};

		/// <summary>
		/// All combinations.
		/// </summary>
		private static readonly IReadOnlyDictionary<int, IEnumerable<short>> Combinations;


		/// <include file='../../../../GlobalDocComments.xml' path='comments/staticConstructor'/>
		static XrTechniqueSearcher()
		{
			var list = new Dictionary<int, IEnumerable<short>>();
			for (int size = 3; size <= 7; size++)
			{
				var innerList = new List<short>();
				foreach (short mask in new BitCombinationGenerator(9, size))
				{
					// Check whether all cells are in same region.
					// If so, continue the loop immediately.
					short a = (short)(mask >> 6), b = (short)(mask >> 3 & 7), c = (short)(mask & 7);
					if (size == 3 && (a == 7 || b == 7 || c == 7))
					{
						continue;
					}

					innerList.Add(mask);
				}

				list.Add(size, innerList);
			}

			Combinations = list;
		}


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
			// Iterate on each region pair.
			for (int index = 0; index < 18; index++)
			{
				// Iterate on each size.
				var (r1, r2) = (Regions[index, 0], Regions[index, 1]);
				foreach (var (size, masks) in Combinations)
				{
					// Iterate on each combination.
					foreach (short mask in masks)
					{
						var allCellsMap = GridMap.Empty;
						var pairs = new List<(int, int)>();
						foreach (int pos in mask.GetAllSets())
						{
							int c1 = RegionCells[r1][pos], c2 = RegionCells[r2][pos];
							allCellsMap.Add(c1);
							allCellsMap.Add(c2);
							pairs.Add((c1, c2));
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

							// Get all eliminations and highlight candidates.
							var conclusions = new List<Conclusion>();
							var candidateOffsets = new List<(int, int)>();
							if (extraCellsMap.Count == 1)
							{
								CheckType1(
									accumulator, grid, allCellsMap, conclusions, candidateOffsets,
									extraCellsMap, normalDigits, extraDigit);
							}
							else
							{
								CheckType2(
									accumulator, grid, allCellsMap, conclusions, candidateOffsets,
									extraCellsMap, normalDigits, extraDigit);
							}
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
			}
		}

		private void CheckType1(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap allCellsMap, List<Conclusion> conclusions,
			List<(int, int)> candidateOffsets, GridMap extraCells, short normalDigits, int extraDigit)
		{
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
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap allCellsMap, List<Conclusion> conclusions,
			List<(int, int)> candidateOffsets, GridMap extraCells, short normalDigits, int extraDigit)
		{
			var elimMap = extraCells.PeerIntersection & CandMaps[extraDigit];
			if (elimMap.IsEmpty)
			{
				return;
			}

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
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap allCellsMap,
			short normalDigits, short extraDigits, GridMap extraCellsMap)
		{
			foreach (int region in extraCellsMap.CoveredRegions)
			{
				int[] otherCells = ((RegionMaps[region] & EmptyMap) - allCellsMap).ToArray();
				for (int size = 1; size < otherCells.Length; size++)
				{
					foreach (int[] cells in GetCombinationsOfArray(otherCells, size))
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

						var elimMap = (RegionMaps[region] & EmptyMap) - allCellsMap - new GridMap(cells);
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
						foreach (int cell in allCellsMap)
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
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap allCellsMap,
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
					if (!conjugateMask.IsPowerOfTwo())
					{
						break;
					}

					int conjugateDigit = conjugateMask.FindFirstSet();
					foreach (int region in extraCellsMap.CoveredRegions)
					{
						var map = RegionMaps[region] & allCellsMap & extraCellsMap;
						if (map != extraCellsMap || map != (CandMaps[conjugateDigit] & RegionMaps[region]))
						{
							continue;
						}

						short elimDigits = (short)(normalDigits & ~conjugateMask);
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

					break;
				}
			}
		}
	}
}
