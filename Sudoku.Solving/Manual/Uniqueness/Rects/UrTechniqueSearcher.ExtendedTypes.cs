using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.GridMap.InitializeOption;
using static Sudoku.Solving.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	partial class UrTechniqueSearcher
	{
		partial void Check2DOr3X(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap)
		{
			//   ↓ corner1
			// (ab )    abx
			//  aby  (ab(x|y))  xy  *
			//           ↑ corner2
			short corner1Mask = grid.GetCandidatesReversal(corner1);
			short corner2Mask = grid.GetCandidatesReversal(corner2);
			short cornerMask = (short)(corner1Mask | corner2Mask);
			int abxyCellCandidatesCount = cornerMask.CountSet();
			if (abxyCellCandidatesCount != 2 && abxyCellCandidatesCount != 3
				|| abxyCellCandidatesCount == 2 && cornerMask != comparer
				|| abxyCellCandidatesCount == 3 && corner1Mask != comparer && corner2Mask != comparer)
			{
				// The result mask be 'abx', 'aby' or 'ab',
				// and one of two corner cells should contain only a and b.
				return;
			}

			short mask = 0;
			foreach (int cell in otherCellsMap.Offsets)
			{
				mask |= grid.GetCandidatesReversal(cell);
			}
			if (mask.CountSet() != 4 || (mask & comparer) != comparer)
			{
				// To ensure 'abx' and 'aby' contains both number a and b,
				// and hold four digits a, b, x and y.
				return;
			}

			short xyMask = (short)(mask ^ comparer);
			int x = xyMask.FindFirstSet(), y = xyMask.GetNextSet(x);
			var cellsThatBothCornerCanSeeMap =
				new GridMap(stackalloc[] { corner1, corner2 }, ProcessPeersWithoutItself)
				- new GridMap(urCells);
			foreach (int xyCell in cellsThatBothCornerCanSeeMap.Offsets)
			{
				if (grid.GetCandidatesReversal(xyCell) != xyMask)
				{
					continue;
				}

				// Now check eliminations.
				var conclusions = new List<Conclusion>();
				var elimMap = new GridMap(stackalloc[] { corner1, corner2, xyCell }, ProcessPeersWithoutItself);
				if (elimMap.Count == 0)
				{
					continue;
				}

				foreach (int cell in elimMap.Offsets)
				{
					void record(int value)
					{
						if (grid.Exists(cell, value) is true)
						{
							conclusions.Add(new Conclusion(Elimination, cell, value));
						}
					}

					record(x);
					record(y);
				}

				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<(int, int)>();
				foreach (int cell in urCells)
				{
					if (grid.GetCellStatus(cell) != Empty)
					{
						continue;
					}

					if (otherCellsMap[cell])
					{
						foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
						{
							candidateOffsets.Add((digit != d1 && digit != d2 ? 1 : 0, cell * 9 + digit));
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
				foreach (int digit in grid.GetCandidatesReversal(xyCell).GetAllSets())
				{
					candidateOffsets.Add((1, xyCell * 9 + digit));
				}

				if (!_allowUncompletedUr && candidateOffsets.Count(CheckHighlightType) != 8)
				{
					continue;
				}

				accumulator.Add(
					new Ur2DOr3XTechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: arMode ? GetHighlightCells(urCells) : null,
								candidateOffsets,
								regionOffsets: null,
								links: null)
						},
						typeName: abxyCellCandidatesCount switch
						{
							2 => "+ 2D",
							3 => "+ 3X",
							_ => throw Throwing.ImpossibleCase
						},
						typeCode: 8,
						digit1: d1,
						digit2: d2,
						cells: urCells,
						x: x,
						y: y,
						xyCell: xyCell,
						isAr: arMode));
			}
		}

		partial void Check2B1SL(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap)
		{
			//   ↓ corner1, corner2
			// (ab )  (ab )
			//  |
			//  | a
			//  |
			//  abx    aby
			if ((grid.GetCandidatesReversal(corner1) | grid.GetCandidatesReversal(corner2)) != comparer)
			{
				return;
			}

			foreach (int cell in stackalloc[] { corner1, corner2 })
			{
				int sameRegionCell = GetSameRegionCell(cell, otherCellsMap, out var regions);
				if (sameRegionCell == -1)
				{
					continue;
				}

				foreach (int region in regions)
				{
					foreach (int digit in stackalloc[] { d1, d2 })
					{
						if (!IsConjugatePair(grid, digit, new GridMap(stackalloc[] { cell, sameRegionCell }), region))
						{
							continue;
						}

						int elimCell = new GridMap(otherCellsMap) { [sameRegionCell] = false }.Offsets.First();
						if (!(grid.Exists(sameRegionCell, digit) is true))
						{
							continue;
						}

						int elimDigit = comparer ^ (1 << digit);
						var candidateOffsets = new List<(int, int)>();
						foreach (int urCell in urCells)
						{
							if (urCell == corner1 || urCell == corner2)
							{
								if (new GridMap(stackalloc[] { urCell, sameRegionCell }).AllSetsAreInOneRegion(out int? r)
									&& r == region)
								{
									foreach (int d in grid.GetCandidatesReversal(urCell).GetAllSets())
									{
										candidateOffsets.Add((d == digit ? 1 : 0, urCell * 9 + d));
									}
								}
								else
								{
									foreach (int d in grid.GetCandidatesReversal(urCell).GetAllSets())
									{
										candidateOffsets.Add((0, urCell * 9 + d));
									}
								}
							}
							else if (urCell == sameRegionCell)
							{
								foreach (int d in grid.GetCandidatesReversal(urCell).GetAllSets())
								{
									candidateOffsets.Add((d == digit ? 1 : 0, urCell * 9 + d));
								}
							}
							else // urCell == elimCell
							{
								foreach (int d in grid.GetCandidatesReversal(urCell).GetAllSets())
								{
									if (d == elimDigit)
									{
										continue;
									}

									candidateOffsets.Add((0, urCell * 9 + d));
								}
							}
						}

						if (!_allowUncompletedUr && candidateOffsets.Count != 7)
						{
							continue;
						}

						accumulator.Add(
							new UrPlusTechniqueInfo(
								conclusions: new[] { new Conclusion(Elimination, elimCell, digit) },
								views: new[]
								{
									new View(
										cellOffsets: arMode ? GetHighlightCells(urCells) : null,
										candidateOffsets,
										regionOffsets: new[] { (0, region) },
										links: null)
								},
								typeName: "UR + 2B / 1SL",
								typeCode: 9,
								digit1: d1,
								digit2: d2,
								cells: urCells,
								conjugatePairs: new[] { new ConjugatePair(cell, sameRegionCell, digit) },
								isAr: arMode));
					}
				}
			}
		}

		partial void Check2D1SL(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap)
		{
			//   ↓ corner1
			// (ab )   aby
			//  |
			//  | a
			//  |
			//  abx   (ab )
			//          ↑ corner2
			if ((grid.GetCandidatesReversal(corner1) | grid.GetCandidatesReversal(corner2)) != comparer)
			{
				return;
			}

			foreach (int cell in stackalloc[] { corner1, corner2 })
			{
				int sameRegionCell = GetSameRegionCell(cell, otherCellsMap, out var regions);
				if (sameRegionCell == -1)
				{
					continue;
				}

				foreach (int region in regions)
				{
					foreach (int digit in stackalloc[] { d1, d2 })
					{
						if (!IsConjugatePair(grid, digit, new GridMap(stackalloc[] { cell, sameRegionCell }), region))
						{
							continue;
						}

						int elimCell = new GridMap(otherCellsMap) { [sameRegionCell] = false }.Offsets.First();
						if (!(grid.Exists(sameRegionCell, digit) is true))
						{
							continue;
						}

						var candidateOffsets = new List<(int, int)>();
						foreach (int urCell in urCells)
						{
							if (urCell == corner1 || urCell == corner2)
							{
								if (new GridMap(stackalloc[] { urCell, sameRegionCell }).AllSetsAreInOneRegion(out int? r)
									&& r == region)
								{
									foreach (int d in grid.GetCandidatesReversal(urCell).GetAllSets())
									{
										candidateOffsets.Add((d == digit ? 1 : 0, urCell * 9 + d));
									}
								}
								else
								{
									foreach (int d in grid.GetCandidatesReversal(urCell).GetAllSets())
									{
										candidateOffsets.Add((0, urCell * 9 + d));
									}
								}
							}
							else if (urCell == sameRegionCell)
							{
								foreach (int d in grid.GetCandidatesReversal(urCell).GetAllSets())
								{
									candidateOffsets.Add((d == digit ? 1 : 0, urCell * 9 + d));
								}
							}
							else // urCell == elimCell
							{
								foreach (int d in grid.GetCandidatesReversal(urCell).GetAllSets())
								{
									if (d == digit)
									{
										continue;
									}

									candidateOffsets.Add((0, urCell * 9 + d));
								}
							}
						}

						if (!_allowUncompletedUr && candidateOffsets.Count != 7)
						{
							continue;
						}

						accumulator.Add(
							new UrPlusTechniqueInfo(
								conclusions: new[] { new Conclusion(Elimination, elimCell, digit) },
								views: new[]
								{
									new View(
										cellOffsets: arMode ? GetHighlightCells(urCells) : null,
										candidateOffsets,
										regionOffsets: new[] { (0, region) },
										links: null)
								},
								typeName: "UR + 2D / 1SL",
								typeCode: 9,
								digit1: d1,
								digit2: d2,
								cells: urCells,
								conjugatePairs: new[] { new ConjugatePair(cell, sameRegionCell, digit) },
								isAr: arMode));
					}
				}
			}
		}
	}
}
