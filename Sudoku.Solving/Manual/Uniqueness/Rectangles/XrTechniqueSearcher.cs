using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Utils;
using static Sudoku.GridProcessings;
using XrType1 = Sudoku.Solving.Manual.Uniqueness.Rectangles.XrType1DetailData;
using XrType2 = Sudoku.Solving.Manual.Uniqueness.Rectangles.XrType2DetailData;
using XrType3 = Sudoku.Solving.Manual.Uniqueness.Rectangles.XrType3DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Encapsulates an <b>extended rectangle</b> (XR) technique searcher.
	/// </summary>
	[TechniqueDisplay("Extended Rectangle")]
	public sealed class XrTechniqueSearcher : RectangleTechniqueSearcher
	{
		/// <summary>
		/// The table of regions to traverse.
		/// </summary>
		private static readonly int[,] Regions = new int[18, 2]
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


		/// <summary>
		/// The static constructor of this class.
		/// </summary>
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
						var positions = mask.GetAllSets();
						var allCellsMap = GridMap.Empty;
						var pairs = new List<(int, int)>();
						foreach (int pos in positions)
						{
							int c1 = RegionUtils.GetCellOffset(r1, pos);
							int c2 = RegionUtils.GetCellOffset(r2, pos);
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
							if ((grid.GetCandidates(l) | grid.GetCandidates(r)).CountSet() > 7)
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
							m1 |= grid.GetCandidatesReversal(l);
							m2 |= grid.GetCandidatesReversal(r);
						}

						int resultMask = m1 | m2;
						var normalDigits = new List<int>();
						var extraDigits = new List<int>();
						var digits = resultMask.GetAllSets();
						foreach (int digit in digits)
						{
							int count = 0;
							foreach (var (l, r) in pairs)
							{
								if (((grid.GetCandidates(l) | grid.GetCandidates(r)) >> digit & 1) == 0)
								{
									// Both two cells contain same digit.
									count++;
								}
							}

							if (count >= 2)
							{
								// This candidate must be in the structure.
								normalDigits.Add(digit);
							}
							else
							{
								// This candidate must be the extra digit.
								extraDigits.Add(digit);
							}
						}

						if (normalDigits.Count != size)
						{
							// The number of normal digits are not enough.
							continue;
						}

						if (resultMask.CountSet() == size + 1)
						{
							// Possible type 1 or 2 found.
							// Now check extra cells.
							var extraCells = new List<int>();
							foreach (int cell in allCellsMap.Offsets)
							{
								if ((grid.GetCandidates(cell) >> extraDigits[0] & 1) == 0)
								{
									extraCells.Add(cell);
								}
							}

							var extraCellsMap = new GridMap(extraCells);
							if (extraCellsMap.Offsets.All(c => grid.GetCellStatus(c) != CellStatus.Empty))
							{
								continue;
							}

							// Get all eliminations and highlight candidates.
							int extraDigit = extraDigits[0];
							var conclusions = new List<Conclusion>();
							var candidateOffsets = new List<(int, int)>();
							if (extraCellsMap.Count == 1)
							{
								// Type 1.
								foreach (int cell in allCellsMap.Offsets)
								{
									if (cell == extraCells[0])
									{
										foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
										{
											if (digit == extraDigit)
											{
												continue;
											}

											conclusions.Add(
												new Conclusion(ConclusionType.Elimination, cell, digit));
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

								if (conclusions.Count == 0)
								{
									continue;
								}

								accumulator.Add(
									new XrTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												links: null)
										},
										detailData: new XrType1(
											cells: allCellsMap.ToArray(),
											digits: normalDigits)));
							}
							else
							{
								// Type 2.
								// Check eliminations.
								var elimMap = new GridMap(extraCells, GridMap.InitializeOption.ProcessPeersWithoutItself);
								foreach (int cell in elimMap.Offsets)
								{
									if (!(grid.Exists(cell, extraDigit) is true))
									{
										continue;
									}

									conclusions.Add(
										new Conclusion(
											ConclusionType.Elimination, cell, extraDigit));
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								// Record all highlight candidates.
								foreach (int cell in allCellsMap.Offsets)
								{
									foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
									{
										candidateOffsets.Add(
											(digit == extraDigit ? 1 : 0, cell * 9 + digit));
									}
								}

								accumulator.Add(
									new XrTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												links: null)
										},
										detailData: new XrType2(
											cells: allCellsMap.ToArray(),
											digits: normalDigits,
											extraDigit: extraDigit)));
							}
						}
						else
						{
							// Check type 3 or 4.
							for (int subsetSize = 2; subsetSize <= 8 - size; subsetSize++)
							{
								CheckType3Naked(
									accumulator, grid, allCellsMap, subsetSize, r1, r2, pairs,
									normalDigits, extraDigits);
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
			IReadOnlyList<int> normalDigits, IReadOnlyList<int> extraDigits)
		{
			// Now check extra cells.
			var extraCells = new List<int>();
			foreach (int cell in allCellsMap.Offsets)
			{
				foreach (int digit in extraDigits)
				{
					if ((grid.GetCandidates(cell) >> digit & 1) == 0)
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
				foreach (int digit in normalDigits)
				{
					if (!(grid.Exists(extraCell, digit) is true))
					{
						continue;
					}

					conclusions.Add(
						new Conclusion(ConclusionType.Elimination, extraCell, digit));
				}

				if (conclusions.Count == 0)
				{
					return;
				}

				// Record all highlight candidates.
				var candidateOffsets = new List<(int, int)>();
				foreach (int cell in allCellsMap.Offsets)
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

				accumulator.Add(
					new XrTechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets,
								regionOffsets: null,
								links: null)
						},
						detailData: new XrType1(
							cells: allCellsMap.ToArray(),
							digits: normalDigits)));
			}
			else
			{
				// TODO: Check XR type 4.
			}
		}

		/// <summary>
		/// Check type 3 naked subsets.
		/// </summary>
		/// <param name="accumulator">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="allCellsMap">All cells map.</param>
		/// <param name="size">The size to check.</param>
		/// <param name="r1">The region 1.</param>
		/// <param name="r2">The region 2.</param>
		/// <param name="pairs">The pairs.</param>
		/// <param name="normalDigits">The normal digits.</param>
		/// <param name="extraDigits">The extra digits.</param>
		private void CheckType3Naked(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, GridMap allCellsMap,
			int size, int r1, int r2, IReadOnlyList<(int, int)> pairs, IReadOnlyList<int> normalDigits,
			IReadOnlyList<int> extraDigits)
		{
			foreach (int region in GetRegions(r1, r2, pairs))
			{
				// Firstly, we should check all cells to iterate,
				// which are empty cells and not in the structure.
				var unavailableCellsMap = GridMap.CreateInstance(region);
				foreach (int cell in RegionCells[region])
				{
					if (grid.GetCellStatus(cell) == CellStatus.Empty && !allCellsMap[cell])
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
					var usedCellsMap = GridMap.CreateInstance(region) - unavailableCellsMap;
					foreach (int cell in usedCellsMap.Offsets)
					{
						subsetMask |= grid.GetCandidatesReversal(cell);
					}

					short extraMask = 0;
					foreach (int digit in extraDigits)
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
					var elimMap = GridMap.CreateInstance(region) - unavailableCellsMap - usedCellsMap;
					foreach (int cell in elimMap.Offsets)
					{
						foreach (int digit in extraDigits)
						{
							if (!(grid.Exists(cell, digit) is true))
							{
								continue;
							}

							conclusions.Add(
								new Conclusion(ConclusionType.Elimination, cell, digit));
						}
					}

					if (conclusions.Count == 0)
					{
						continue;
					}

					// Record all highlight candidates.
					var candidateOffsets = new List<(int, int)>();
					foreach (int cell in allCellsMap.Offsets)
					{
						foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
						{
							candidateOffsets.Add((extraMask >> digit & 1, cell * 9 + digit));
						}
					}

					accumulator.Add(
						new XrTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: new[] { (0, region) },
									links: null)
							},
							detailData: new XrType3(
								cells: allCellsMap.ToArray(),
								digits: normalDigits,
								subsetCells: usedCellsMap.ToArray(),
								subsetDigits: extraDigits,
								isNaked: true)));
				}
			}
		}


		/// <summary>
		/// Get all regions to iterate (used for type 3).
		/// </summary>
		/// <param name="r1">The region 1.</param>
		/// <param name="r2">The region 2.</param>
		/// <param name="pairs">The pairs.</param>
		/// <returns>All regions.</returns>
		private static IEnumerable<int> GetRegions(int r1, int r2, IReadOnlyList<(int, int)> pairs)
		{
			foreach (var (l, r) in pairs)
			{
				foreach (int region in new GridMap { l, r }.CoveredRegions)
				{
					yield return region;
				}
			}

			yield return r1;
			yield return r2;
		}
	}
}
