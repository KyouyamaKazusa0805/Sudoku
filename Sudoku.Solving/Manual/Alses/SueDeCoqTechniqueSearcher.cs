using System;
using System.Collections.Generic;
using System.Externals;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates a sue de coq (SdC) technique searcher.
	/// </summary>
	public sealed class SueDeCoqTechniqueSearcher : AlmostLockedSetTechniqueSearcher
	{
		/// <summary>
		/// The corresponding line regions to iterate on.
		/// </summary>
		private static readonly int[][] NonblockTable = new int[9][]
		{
			new[] { 9, 10, 11, 18, 19, 20 },
			new[] { 9, 10, 11, 21, 22, 23 },
			new[] { 9, 10, 11, 24, 25, 26 },
			new[] { 12, 13, 14, 18, 19, 20 },
			new[] { 12, 13, 14, 21, 22, 23 },
			new[] { 12, 13, 14, 24, 25, 26 },
			new[] { 15, 16, 17, 18, 19, 20 },
			new[] { 15, 16, 17, 21, 22, 23 },
			new[] { 15, 16, 17, 24, 25, 26 },
		};

		/// <summary>
		/// Indicates all combinations (6 cells) to take.
		/// </summary>
		private static readonly IReadOnlyList<long>[] TakingCombinations6;

		/// <summary>
		/// Indicates all combinations (7 cells) to take.
		/// </summary>
		private static readonly IReadOnlyList<long>[] TakingCombinations7;


		/// <summary>
		/// All region maps.
		/// </summary>
		private readonly GridMap[] _regionMaps;


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="regionMaps">All regions grid maps.</param>
		public SueDeCoqTechniqueSearcher(GridMap[] regionMaps) =>
			_regionMaps = regionMaps;


		/// <summary>
		/// The static initializer of this class.
		/// </summary>
		static SueDeCoqTechniqueSearcher()
		{
			static IReadOnlyList<long>[] Z(int m)
			{
				var temp = new List<long>[m + 1];
				temp[0] = null!; // Only be a placeholder.
				for (int i = 1; i <= m; i++)
				{
					temp[i] = new List<long>(new BitCombinationGenerator(m, i));
				}

				return temp;
			}

			TakingCombinations6 = Z(6);
			TakingCombinations7 = Z(7);
		}


		/// <inheritdoc/>
		public override int Priority => 50;

		
		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			(var emptyMap, _, _) = grid;
			if (emptyMap.Count < 4)
			{
				return;
			}

			for (int block = 0; block < 9; block++)
			{
				foreach (int nonblock in NonblockTable[block])
				{
					// Get all enumeration grid maps.
					var nonblockMap = _regionMaps[nonblock] & emptyMap;
					var blockMap = _regionMaps[block] & emptyMap;
					var interMap = nonblockMap & blockMap;

					// Get the number of empty cells in the specified intersection cells
					// and check the number is no less than 2.
					// If the number of empty cells is 0 or 1, the intersection cannot
					// be a part of any SdC structure.
					int emptyCellsCountInInter = interMap.Count;
					if (emptyCellsCountInInter < 2)
					{
						continue;
					}

					// Get all empty cells in intersection.
					var interCells = emptyCellsCountInInter switch
					{
						2 => stackalloc[]
						{
							interMap.SetBitAt(0),
							interMap.SetBitAt(1)
						},
						3 => stackalloc[]
						{
							interMap.SetBitAt(0),
							interMap.SetBitAt(1),
							interMap.SetBitAt(2)
						},
						_ => throw Throwing.ImpossibleCase
					};

					// Get all iteration cases in intersection cells.
					var iterationInterCells = new List<int[]>();
					switch (emptyCellsCountInInter)
					{
						case 2:
						{
							iterationInterCells.Add(new[] { interCells[0], interCells[1] });
							break;
						}
						case 3:
						{
							iterationInterCells.AddRange(new int[4][]
							{
								new[] { interCells[0], interCells[1], interCells[2] },
								new[] { interCells[0], interCells[1] },
								new[] { interCells[0], interCells[2] },
								new[] { interCells[1], interCells[2] },
							});
							break;
						}
					}

					// Now we can iterate on those cases.
					var unionMap = nonblockMap | blockMap;
					foreach (int[] interEmptyCells in iterationInterCells)
					{
						int count = interEmptyCells.Length;
						var tempUnionMap = unionMap;

						// Get all kinds of cells in intersection cells.
						short interMask = 0;
						foreach (int cell in interEmptyCells)
						{
							interMask |= grid.GetCandidatesReversal(cell);
						}

						// Get the number of cells to take.
						int rankInInter = interMask.CountSet() - emptyCellsCountInInter;
						if (rankInInter < 2)
						{
							// These empty cells in intersection will form
							// a normal subset or an ALS.
							// If these cells form a subset, why we will use
							// other cells to iterate?
							// If these cells form an ALS, we may only find
							// at most a cell to form a subset (not an SdC).
							continue;
						}

						// Check whether an SdC can be formed in the region.
						foreach (int cell in interEmptyCells)
						{
							// Remove all cells in intersections in this iteration.
							tempUnionMap[cell] = false;
						}

						// Check whether the number of all empty cells in two
						// regions are enough to take or even form an SdC.
						if (tempUnionMap.Count < rankInInter)
						{
							// Empty cells are not enough.
							continue;
						}

						var takenInterMap = new GridMap((IEnumerable<int>)interEmptyCells);
						var blockTakingList = new List<int>(
							(_regionMaps[block] - new GridMap((IEnumerable<int>)interEmptyCells)).Offsets);
						var nonblockTakingList = new List<int>(
							(_regionMaps[nonblock] - new GridMap((IEnumerable<int>)interEmptyCells)).Offsets);
						for (int blockTakenCellsCount = 1;
							blockTakenCellsCount <= 8 - count;
							blockTakenCellsCount++)
						{
							TakeAllByInterCount(
								accumulator, grid, blockTakenCellsCount, count, interMask, interEmptyCells,
								block, nonblock, interMap, takenInterMap, tempUnionMap, blockTakingList,
								nonblockTakingList, count == 2 ? TakingCombinations7 : TakingCombinations6);
						}
					}
				}
			}
		}

		private void TakeAllByInterCount(
			IBag<TechniqueInfo> result, IReadOnlyGrid grid, int blockTakenCellsCount,
			int count, short interMask, int[] interEmptyCells, int block, int nonblock,
			GridMap interMap, GridMap takenInterMap, GridMap tempUnionMap,
			IReadOnlyList<int> blockTakingList, IReadOnlyList<int> nonblockTakingList,
			IReadOnlyList<long>[] takingCombinations)
		{
			foreach (byte blockCombination in takingCombinations[blockTakenCellsCount])
			{
				var takenCellsInBlockMap = GridMap.Empty;
				short blockMask = 0;
				foreach (int i in blockCombination.GetAllSets())
				{
					int cell = blockTakingList[i];
					if (grid.GetCellStatus(cell) != CellStatus.Empty)
					{
						goto Label_NextBlock;
					}

					takenCellsInBlockMap[cell] = true;
					blockMask |= grid.GetCandidatesReversal(cell);
				}

				int nonblockTakenCellsCount =
					(blockMask | interMask).CountSet() - blockTakenCellsCount - count;
				if (nonblockTakenCellsCount == 0)
				{
					continue;
				}

				foreach (byte nonblockCombination in takingCombinations[nonblockTakenCellsCount])
				{
					var takenCellsInNonblockMap = GridMap.Empty;
					short nonblockMask = 0;
					foreach (int i in nonblockCombination.GetAllSets())
					{
						int cell = nonblockTakingList[i];
						if (grid.GetCellStatus(cell) != CellStatus.Empty || interMap[cell])
						{
							goto Label_NextNonblock;
						}

						takenCellsInNonblockMap[cell] = true;
						nonblockMask |= grid.GetCandidatesReversal(cell);
					}

					if ((takenCellsInBlockMap & takenCellsInNonblockMap).IsNotEmpty)
					{
						// They got a same cell (in intersection).
						continue;
					}

					// Iterate on two collections to sum up all kinds of digits.
					short mask = (short)((short)(blockMask | nonblockMask) | interMask);

					// Check the kinds of digits.
					int digitsKindsCount = count + takenCellsInBlockMap.Count + takenCellsInNonblockMap.Count;
					if (mask.CountSet() != digitsKindsCount)
					{
						continue;
					}

					// Now check whether all digits lie on less than two regions.
					var digits = mask.GetAllSets();
					var digitRegions = new Dictionary<int, IEnumerable<int>>();
					foreach (int digit in digits)
					{
						var tempMap = GridMap.Empty;
						foreach (int cell in takenCellsInBlockMap.Offsets)
						{
							if (grid.CandidateExists(cell, digit))
							{
								tempMap[cell] = true;
							}
						}
						foreach (int cell in takenCellsInNonblockMap.Offsets)
						{
							if (grid.CandidateExists(cell, digit))
							{
								tempMap[cell] = true;
							}
						}
						foreach (int cell in interEmptyCells)
						{
							if (grid.CandidateExists(cell, digit))
							{
								tempMap[cell] = true;
							}
						}

						if (tempMap.Count == 1)
						{
							var (r, c, b) = CellUtils.GetRegion(tempMap.SetBitAt(0));
							if (b == block)
							{
								digitRegions.Add(digit, new[] { block });
							}
							else if (r + 9 == nonblock || c + 18 == nonblock)
							{
								digitRegions.Add(digit, new[] { nonblock });
							}
							else
							{
								goto Label_NextNonblock;
							}
						}
						else
						{
							var coveredRegions = tempMap.CoveredRegions;
							if (!coveredRegions.Any())
							{
								goto Label_NextNonblock;
							}

							digitRegions.Add(digit, coveredRegions);
						}
					}

					// Check whether the selected cells form a subset.
					var allTakenCellsMap = takenCellsInBlockMap | takenCellsInNonblockMap | takenInterMap;
					if (allTakenCellsMap.IsCoveredOneRegion(out _))
					{
						continue;
					}

					// Each digit lies on only one region,
					// which means an SdC has already forms.
					// Now check eliminations.
					var conclusions = new List<Conclusion>();
					var elimUnion = tempUnionMap - allTakenCellsMap;
					foreach (int digit in digits)
					{
						foreach (int region in digitRegions[digit])
						{
							foreach (int cell in (elimUnion & _regionMaps[region]).Offsets)
							{
								if (grid.CandidateExists(cell, digit))
								{
									conclusions.AddIfDoesNotContain(
										new Conclusion(ConclusionType.Elimination, cell, digit));
								}
							}
						}
					}

					if (conclusions.Count == 0)
					{
						continue;
					}

					// Record all highlight candidates.
					var candidateOffsets = new List<(int, int)>();
					foreach (int cell in allTakenCellsMap.Offsets)
					{
						short tempMask = grid.GetCandidatesReversal(cell);
						foreach (int digit in tempMask.GetAllSets())
						{
							int cand = cell * 9 + digit;
							if (digitRegions[digit].Count() == 2)
							{
								candidateOffsets.Add((2, cand));
							}
							else if (takenCellsInBlockMap[cell])
							{
								candidateOffsets.Add((0, cand));
							}
							else
							{
								candidateOffsets.Add((1, cand));
							}
						}
					}

					result.AddIfDoesNotContain(
						new SueDeCoqTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: new[] { (0, nonblock), (0, block) },
									linkMasks: null)
							},
							als1Cells: new List<int>(takenCellsInBlockMap.Offsets),
							als1Digits: new List<int>(blockMask.GetAllSets()),
							als2Cells: new List<int>(takenCellsInNonblockMap.Offsets),
							als2Digits: new List<int>(nonblockMask.GetAllSets()),
							interCells: new List<int>(takenInterMap.Offsets),
							interDigits: new List<int>(interMask.GetAllSets())));

				Label_NextNonblock:;
				}

			Label_NextBlock:;
			}
		}
	}
}
