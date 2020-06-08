using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Data.GridMap.InitializeOption;

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
							var extraCells = new List<int>();
							int extraDigit = extraDigits.FindFirstSet();
							foreach (int cell in allCellsMap)
							{
								if ((grid.GetCandidateMask(cell) >> extraDigit & 1) != 0)
								{
									extraCells.Add(cell);
								}
							}

							var extraCellsMap = new GridMap(extraCells) & EmptyMap;
							if (extraCellsMap.IsEmpty)
							{
								continue;
							}

							// Get all eliminations and highlight candidates.
							var conclusions = new List<Conclusion>();
							var candidateOffsets = new List<(int, int)>();
							if (extraCellsMap.Count == 1)
							{
								// Type 1.
								foreach (int cell in allCellsMap)
								{
									if (cell == extraCells[0])
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
									continue;
								}

								accumulator.Add(
									new XrType1TechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												links: null)
										},
										cells: allCellsMap.ToArray(),
										digits: normalDigits.GetAllSets().ToArray()));
							}
							else
							{
								// Type 2.
								// Check eliminations.
								var elimMap = new GridMap(extraCells, ProcessPeersWithoutItself) & CandMaps[extraDigit];
								if (elimMap.IsEmpty)
								{
									continue;
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
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												links: null)
										},
										cells: allCellsMap.ToArray(),
										digits: normalDigits.GetAllSets().ToArray(),
										extraDigit));
							}
						}
						else
						{
							// Check type 3 or 4.
							for (int subsetSize = 2; subsetSize <= 8 - size; subsetSize++)
							{
								CheckType3Naked(accumulator, grid, allCellsMap, subsetSize, normalDigits, extraDigits);
							}

							CheckType14(accumulator, grid, allCellsMap, normalDigits, extraDigits);
						}
					}
				}
			}
		}

		/// <summary>
		/// Check type 1 or 4.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="allCellsMap">All cells map.</param>
		/// <param name="normalDigits">The normal digits.</param>
		/// <param name="extraDigits">The extra digits.</param>
		private void CheckType14(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap allCellsMap,
			short normalDigits, short extraDigits)
		{
			// Now check extra cells.
			var extraCells = new List<int>();
			foreach (int cell in allCellsMap)
			{
				foreach (int digit in extraDigits.GetAllSets())
				{
					if ((grid.GetCandidateMask(cell) >> digit & 1) != 0)
					{
						extraCells.Add(cell);
					}
				}
			}

			int extraCellsCount = extraCells.Count;
			if (extraCellsCount == 1)
			{
				// Type 1 found.
				// Check eliminations.
				var conclusions = new List<Conclusion>();
				int extraCell = extraCells[0];
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
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets,
								regionOffsets: null,
								links: null)
						},
						cells: allCellsMap.ToArray(),
						digits: normalDigits.GetAllSets().ToArray()));
			}
			else
			{

			}
		}

		/// <summary>
		/// Check type 3 naked subsets.
		/// </summary>
		/// <param name="accumulator">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="allCellsMap">All cells map.</param>
		/// <param name="size">The size to check.</param>
		/// <param name="normalDigits">The normal digits.</param>
		/// <param name="extraDigits">The extra digits.</param>
		private void CheckType3Naked(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap allCellsMap,
			int size, short normalDigits, short extraDigits)
		{
			foreach (int region in allCellsMap.Regions)
			{
				// Firstly, we should check all cells to iterate,
				// which are empty cells and not in the structure.
				var unavailableCellsMap = RegionMaps[region];
				foreach (int cell in RegionCells[region])
				{
					if (grid.GetStatus(cell) == Empty && !allCellsMap[cell])
					{
						unavailableCellsMap.Remove(cell);
					}
				}

				if (8 - unavailableCellsMap.Count < size)
				{
					// The number of last cells are not enough to form a light subset.
					continue;
				}

				// Get the mask.
				short unavailableCellsMask = 0;
				short index = 0;
				foreach (int cell in RegionCells[region])
				{
					if (unavailableCellsMap[cell])
					{
						unavailableCellsMask |= index;
					}

					index++;
				}

				// Now iterate on them.
				foreach (short mask in new BitCombinationGenerator(9, size - 1))
				{
					if ((mask & unavailableCellsMask) != 0)
					{
						continue;
					}

					short subsetMask = 0;
					var usedCellsMap = RegionMaps[region] - unavailableCellsMap;
					foreach (int cell in usedCellsMap)
					{
						subsetMask |= grid.GetCandidateMask(cell);
					}

					short extraMask = 0;
					foreach (int digit in extraDigits.GetAllSets())
					{
						extraMask |= (short)(1 << digit);
					}

					subsetMask |= extraMask;
					if (subsetMask.CountSet() != size)
					{
						continue;
					}

					// XR type 3 found.
					// Record all eliminations.
					var conclusions = new List<Conclusion>();
					var elimMap = RegionMaps[region] - unavailableCellsMap - usedCellsMap;
					foreach (int cell in elimMap)
					{
						foreach (int digit in extraDigits.GetAllSets())
						{
							if (grid.Exists(cell, digit) is true)
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
					foreach (int cell in allCellsMap)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add((extraMask >> digit & 1, cell * 9 + digit));
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
							cells: allCellsMap.ToArray(),
							digits: normalDigits.GetAllSets().ToArray(),
							extraCells: usedCellsMap.ToArray(),
							extraDigits: extraDigits.GetAllSets().ToArray(),
							region));
				}
			}
		}
	}
}
