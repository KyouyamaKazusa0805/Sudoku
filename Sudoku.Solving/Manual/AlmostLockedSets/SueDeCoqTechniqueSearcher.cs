using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.AlmostLockedSets
{
	/// <summary>
	/// Encapsulates a sue de coq (SdC) technique searcher.
	/// </summary>
	public sealed class SueDeCoqTechniqueSearcher : AlmostLockedSetTechniqueSearcher
	{
		/// <summary>
		/// All region maps.
		/// </summary>
		private readonly GridMap[] _regionMaps;


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="regionMaps">All regions grid maps.</param>
		public SueDeCoqTechniqueSearcher(GridMap[] regionMaps) => _regionMaps = regionMaps;


		/// <inheritdoc/>
		public override int Priority => 50;


		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			(var emptyMap, _, _) = grid;

			var result = new List<SueDeCoqTechniqueInfo>();

			for (int nonBlock = 9; nonBlock < 27; nonBlock++)
			{
				for (int block = 0; block < 9; block++)
				{
					// Get all enumeration grid maps.
					var nonBlockMap = GridMap.CreateInstance(nonBlock);
					var blockMap = GridMap.CreateInstance(block);
					var interMap = nonBlockMap & blockMap & emptyMap;
					var unionMap = (nonBlockMap | blockMap) & emptyMap;

					// Get the number of empty cells in the specified intersection cells
					// and check the number is no less than 2.
					// If the number of empty cells is 0 or 1, the intersection cannot
					// be a part of any SdC structure.
					int emptyCellsCountInInter = interMap.Count;
					if (emptyCellsCountInInter < 2)
					{
						continue;
					}

					var interCells = emptyCellsCountInInter switch
					{
						2 => stackalloc[]
						{
							interMap.ElementAt(0),
							interMap.ElementAt(1)
						},
						3 => stackalloc[]
						{
							interMap.ElementAt(0),
							interMap.ElementAt(1),
							interMap.ElementAt(2)
						},
						_ => throw new Exception("Impossible case.")
					};

					// Get all iteration cases of intersection cells.
					var iterationInterCells = new List<int[]>();
					if (emptyCellsCountInInter == 2)
					{
						iterationInterCells.Add(new[] { interCells[0], interCells[1] });
					}
					else
					{
						iterationInterCells.AddRange(new int[4][]
						{
							new[] { interCells[0], interCells[1] },
							new[] { interCells[0], interCells[2] },
							new[] { interCells[1], interCells[2] },
							new[] { interCells[0], interCells[1], interCells[2] }
						});
					}

					foreach (int[] interEmptyCells in iterationInterCells)
					{
						var tempUnionMap = unionMap;

						// Get all kinds of cells in intersection cells.
						short mask = 0;
						foreach (int cell in interEmptyCells)
						{
							mask |= grid.GetCandidatesReversal(cell);
						}
						int kinds = mask.CountSet();

						// Get the number of cells to take.
						int takingCellsCount = kinds - emptyCellsCountInInter;

						// Check whether a SdC can be formed in the region.
						foreach (int cell in interEmptyCells)
						{
							// Remove all cells in intersections in this iteration.
							tempUnionMap[cell] = false;
						}
						tempUnionMap &= emptyMap;

						// Get all cells to traverse.
						if (tempUnionMap.Count < takingCellsCount)
						{
							// Empty cells are not enough.
							continue;
						}

						SearchSdcRecursively(
							result, grid, takingCellsCount, nonBlock, block,
							new List<int>(interEmptyCells), emptyMap, tempUnionMap.ToArray(),
							interCells, 0);
					}
				}
			}

			return result;
		}


		#region SdC utils
		private void SearchSdcRecursively(
			IList<SueDeCoqTechniqueInfo> result, Grid grid, int restCellsToTakeCount,
			int nonBlock, int block, List<int> takenCells, GridMap emptyMap,
			ReadOnlySpan<int> unionMapArray, ReadOnlySpan<int> interCells,
			int curIndexOfArray)
		{
			if (takenCells.Count > 9)
			{
				return;
			}

			if (restCellsToTakeCount <= 0)
			{
				// Now check whether all taken cells can be formed a SdC.
				if (CheckSdC(grid, takenCells, nonBlock, block, out var digitRegions))
				{
					// SdC found.
					// Check eliminations.
					var conclusions = new List<Conclusion>();
					foreach (var (digit, regions) in digitRegions)
					{
						var map = GridMap.Empty;
						foreach (int region in regions)
						{
							map |= _regionMaps[region];
						}
						map &= emptyMap;
						foreach (int takenCell in takenCells)
						{
							map[takenCell] = false;
						}

						foreach (int cell in map.Offsets)
						{
							if (grid.CandidateExists(cell, digit))
							{
								conclusions.Add(
									new Conclusion(ConclusionType.Elimination, cell * 9 + digit));
							}
						}
					}

					if (conclusions.Count == 0)
					{
						return;
					}

					// Get all highlight candidates.
					var candidateOffsets = new List<(int, int)>();
					var als1Cells = new HashSet<int>();
					var als1Digits = new HashSet<int>();
					var als2Cells = new HashSet<int>();
					var als2Digits = new HashSet<int>();
					var allDigits = new HashSet<int>();
					foreach (int cell in takenCells)
					{
						foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
						{
							allDigits.Add(digit);

							var regions = (
								from pair in digitRegions
								select pair).First(pair => pair._digit == digit)._region;
							if (regions.Count == 2)
							{
								candidateOffsets.Add((2, cell * 9 + digit));
							}
							else
							{
								int region = regions[0];
								if (region >= 9)
								{
									// Line candidate.
									candidateOffsets.Add((0, cell * 9 + digit));
									if (!interCells.Contains(cell))
									{
										als1Cells.Add(cell);
									}
									als1Digits.Add(digit);
								}
								else
								{
									// Block candidate.
									candidateOffsets.Add((1, cell * 9 + digit));
									if (!interCells.Contains(cell))
									{
										als2Cells.Add(cell);
									}
									als2Digits.Add(digit);
								}
							}
						}
					}

					short interMask = 0;
					foreach (int cell in interCells)
					{
						interMask |= grid.GetCandidatesReversal(cell);
					}

					result.AddIfDoesNotContain(
						new SueDeCoqTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: null,
									linkMasks: null)
							},
							als1Cells: als1Cells.ToArray(),
							als1Digits: als1Digits.ToArray(),
							als2Cells: als2Cells.ToArray(),
							als2Digits: als2Digits.ToArray(),
							interCells: interCells.ToArray(),
							interDigits: interMask.GetAllSets().ToArray()));

					return;
				}
				if (takenCells.Count == 9)
				{
					return;
				}
			}

			for (int i = curIndexOfArray, length = unionMapArray.Length; i < length; i++)
			{
				int cell = unionMapArray[i];
				if (takenCells.Contains(cell))
				{
					continue;
				}

				takenCells.Add(cell);

				SearchSdcRecursively(
					result, grid, restCellsToTakeCount - 1, nonBlock, block,
					takenCells, emptyMap, unionMapArray, interCells, curIndexOfArray + 1);

				takenCells.Remove(cell);
			}
		}

		private bool CheckSdC(
			Grid grid, IReadOnlyList<int> takenCells, int nonBlock, int block,
			[NotNullWhen(true)] out IReadOnlyList<(int _digit, IReadOnlyList<int> _region)>? digitRegions)
		{
			digitRegions = null;

			if (takenCells.Count < 4)
			{
				return false;
			}

			// Check the number of different digits and the same number of cells.
			short mask = 0;
			foreach (int takenCell in takenCells)
			{
				mask |= grid.GetCandidatesReversal(takenCell);
			}

			if (mask.CountSet() != takenCells.Count)
			{
				return false;
			}

			// Check the structure spanned two regions.
			bool all = false;
			foreach (int region in new[] { nonBlock, block })
			{
				var map = _regionMaps[region];
				if (takenCells.All(c => map[c]))
				{
					all = true;
					break;
				}
			}
			if (all)
			{
				return false;
			}

			// Check all digits can appear only once in a region of all cells.
			var tempList = new List<(int _digit, IReadOnlyList<int> _region)>();
			foreach (int digit in mask.GetAllSets())
			{
				var tempMap = default(GridMap);
				int i = 0;
				var cells = from cell in takenCells
							where grid.CandidateExists(cell, digit)
							select cell;
				foreach (int cell in cells)
				{
					if (i++ == 0)
					{
						tempMap = new GridMap(cell);
					}
					else
					{
						tempMap &= new GridMap(cell);
					}
				}

				if (cells.Count() == 1)
				{
					int cell = cells.First();
					var (r, c, b) = CellUtils.GetRegion(cell);
					if (r + 9 == nonBlock || c + 18 == nonBlock)
					{
						tempList.Add((digit, new[] { nonBlock }));
					}
					else if (b == block)
					{
						tempList.Add((digit, new[] { block }));
					}
				}
				else
				{
					int spanningCellsCount = tempMap.Count;
					if (spanningCellsCount < 9)
					{
						// We cannot find a region whose cells contains ones having
						// that candidate.
						return false;
					}
					else if (spanningCellsCount > 9)
					{
						// Two regions found.
						tempList.Add((digit, new[] { nonBlock, block }));
					}
					else // spanningCellsCount == 9
					{
						var z = new List<int>();
						short blockMask = tempMap.BlockMask;
						short rowMask = tempMap.RowMask;
						short columnMask = tempMap.ColumnMask;

						if (blockMask.CountSet() == 1) z.Add(blockMask.FindFirstSet());
						if (rowMask.CountSet() == 1) z.Add(rowMask.FindFirstSet() + 9);
						if (columnMask.CountSet() == 1) z.Add(columnMask.FindFirstSet() + 18);

						if (z.Count == 0)
						{
							return false;
						}

						tempList.Add((digit, z));
					}
				}
			}

			digitRegions = tempList;
			return true;
		}
		#endregion
	}
}
