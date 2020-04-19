using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Utils;
using static Sudoku.Data.GridMap.InitializeOption;
using static Sudoku.Solving.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	partial class UrTechniqueSearcher
	{
		partial void Check2D(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap)
		{
			//   ↓ corner1
			// (ab )  abx
			//  aby  (ab )  xy  *
			//         ↑ corner2
			if ((grid.GetCandidatesReversal(corner1) | grid.GetCandidatesReversal(corner2)) != comparer)
			{
				return;
			}

			int[] otherCells = otherCellsMap.ToArray();
			short o1 = grid.GetCandidatesReversal(otherCells[0]);
			short o2 = grid.GetCandidatesReversal(otherCells[1]);
			short o = (short)(o1 | o2);
			if (o.CountSet() != 4 || o1.CountSet() > 3 || o2.CountSet() > 3
				|| (o & comparer) != comparer || (o1 & comparer) == 0 || (o2 & comparer) == 0)
			{
				return;
			}

			short xyMask = (short)(o ^ comparer);
			int x = xyMask.FindFirstSet();
			int y = xyMask.GetNextSet(x);
			var inter = new GridMap(otherCells, ProcessPeersWithoutItself) - new GridMap(urCells);
			foreach (int possibleXyCell in inter.Offsets)
			{
				if (grid.GetCandidatesReversal(possibleXyCell) != xyMask)
				{
					continue;
				}

				// 'xy' found.
				// Now check eliminations.
				var elimMap = inter & new GridMap(possibleXyCell, false);
				var conclusions = new List<Conclusion>();
				foreach (int cell in elimMap.Offsets)
				{
					if (grid.Exists(cell, x) is true)
					{
						conclusions.Add(new Conclusion(Elimination, cell, x));
					}
					if (grid.Exists(cell, y) is true)
					{
						conclusions.Add(new Conclusion(Elimination, cell, y));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<(int, int)>();
				foreach (int cell in urCells)
				{
					if (otherCellsMap[cell])
					{
						foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
						{
							candidateOffsets.Add(((comparer >> digit & 1) == 0 ? 1 : 0, cell * 9 + digit));
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
				foreach (int digit in xyMask.GetAllSets())
				{
					candidateOffsets.Add((1, possibleXyCell * 9 + digit));
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
						typeName: "+ 2D",
						typeCode: 8,
						digit1: d1,
						digit2: d2,
						cells: urCells,
						x,
						y,
						xyCell: possibleXyCell,
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
					if (region < 9)
					{
						continue;
					}

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

						int elimDigit = (comparer ^ (1 << digit)).FindFirstSet();
						var conclusions = new List<Conclusion>();
						if (grid.Exists(elimCell, elimDigit) is true)
						{
							conclusions.Add(new Conclusion(Elimination, elimCell, elimDigit));
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<(int, int)>();
						foreach (int urCell in urCells)
						{
							if (urCell == corner1 || urCell == corner2)
							{
								if (new GridMap(stackalloc[] { urCell, sameRegionCell }).CoveredRegions.Any(r => r == region))
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
							else if (urCell == sameRegionCell || urCell == elimCell)
							{
								void record(int d)
								{
									if (grid.Exists(urCell, d) is true)
									{
										if (urCell == elimCell && d == elimDigit)
										{
											return;
										}

										candidateOffsets.Add((urCell == elimCell ? 0 : d == digit ? 1 : 0, urCell * 9 + d));
									}
								}

								record(d1);
								record(d2);
							}
						}

						if (!_allowUncompletedUr && candidateOffsets.Count != 7)
						{
							continue;
						}

						accumulator.Add(
							new UrPlusTechniqueInfo(
								conclusions,
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
					if (region < 9)
					{
						continue;
					}

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

						var conclusions = new List<Conclusion>();
						if (grid.Exists(elimCell, digit) is true)
						{
							conclusions.Add(new Conclusion(Elimination, elimCell, digit));
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<(int, int)>();
						foreach (int urCell in urCells)
						{
							if (urCell == corner1 || urCell == corner2)
							{
								if (new GridMap(stackalloc[] { urCell, sameRegionCell }).CoveredRegions.Any(r => r == region))
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
							else if (urCell == sameRegionCell || urCell == elimCell)
							{
								void record(int d)
								{
									if (grid.Exists(urCell, d) is true)
									{
										if (urCell == elimCell && d == digit)
										{
											return;
										}

										candidateOffsets.Add((urCell == elimCell ? 0 : d == digit ? 1 : 0, urCell * 9 + d));
									}
								}

								record(d1);
								record(d2);
							}
						}

						if (!_allowUncompletedUr && candidateOffsets.Count != 7)
						{
							continue;
						}

						accumulator.Add(
							new UrPlusTechniqueInfo(
								conclusions,
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

		partial void Check3X(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, GridMap otherCellsMap)
		{
			//   ↓ cornerCell
			// (ab )  abx
			//  aby   abz   xy  *
			// Note: 'z' is 'x' or 'y'.
			if (grid.GetCandidatesReversal(cornerCell) != comparer)
			{
				return;
			}

			int c1 = otherCellsMap.SetAt(0);
			int c2 = otherCellsMap.SetAt(1);
			int c3 = otherCellsMap.SetAt(2);
			short m1 = grid.GetCandidatesReversal(c1);
			short m2 = grid.GetCandidatesReversal(c2);
			short m3 = grid.GetCandidatesReversal(c3);
			short mask = (short)((short)(m1 | m2) | m3);
			if (mask.CountSet() != 4 || m1.CountSet() > 3 || m2.CountSet() > 3 || m3.CountSet() > 3
				|| (mask & comparer) != comparer
				|| (m1 & comparer) == 0 || (m2 & comparer) == 0 || (m3 & comparer) == 0)
			{
				return;
			}

			short xyMask = (short)(mask ^ comparer);
			int x = xyMask.FindFirstSet();
			int y = xyMask.GetNextSet(x);
			var inter = new GridMap(otherCellsMap.Offsets, ProcessPeersWithoutItself) - new GridMap(urCells);
			foreach (int possibleXyCell in inter.Offsets)
			{
				if (grid.GetCandidatesReversal(possibleXyCell) != xyMask)
				{
					continue;
				}

				// Possible XY cell found.
				// Now check eliminations.
				var conclusions = new List<Conclusion>();
				foreach (int cell in (inter & new GridMap(possibleXyCell, false)).Offsets)
				{
					if (grid.Exists(cell, x) is true)
					{
						conclusions.Add(new Conclusion(Elimination, cell, x));
					}
					if (grid.Exists(cell, y) is true)
					{
						conclusions.Add(new Conclusion(Elimination, cell, y));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<(int, int)>();
				foreach (int cell in urCells)
				{
					if (otherCellsMap[cell])
					{
						foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
						{
							candidateOffsets.Add(((comparer >> digit & 1) == 0 ? 1 : 0, cell * 9 + digit));
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
				foreach (int digit in xyMask.GetAllSets())
				{
					candidateOffsets.Add((1, possibleXyCell * 9 + digit));
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
						typeName: "+ 3X",
						typeCode: 9,
						digit1: d1,
						digit2: d2,
						cells: urCells,
						x,
						y,
						xyCell: possibleXyCell,
						isAr: arMode));
			}
		}

		partial void Check3X2SL(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, GridMap otherCellsMap)
		{
			//   ↓ cornerCell
			// (ab )    abx
			//           |
			//           | b
			//       a   |
			//  aby-----abz
			if (grid.GetCandidatesReversal(cornerCell) != comparer)
			{
				return;
			}

			int abzCell = GetDiagonalCell(urCells, cornerCell);
			var adjacentCellsMap = new GridMap(otherCellsMap) { [abzCell] = false };
			var (r, c, _) = CellUtils.GetRegion(abzCell);
			r += 9; c += 18;

			foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
			{
				int abxCell = adjacentCellsMap.SetAt(0);
				int abyCell = adjacentCellsMap.SetAt(1);
				var map1 = new GridMap(stackalloc[] { abzCell, abxCell });
				var map2 = new GridMap(stackalloc[] { abzCell, abyCell });
				if (!IsConjugatePair(grid, b, map1, map1.CoveredLine)
					|| !IsConjugatePair(grid, a, map2, map2.CoveredLine))
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				if (grid.Exists(abxCell, a) is true)
				{
					conclusions.Add(new Conclusion(Elimination, abxCell, a));
				}
				if (grid.Exists(abyCell, b) is true)
				{
					conclusions.Add(new Conclusion(Elimination, abyCell, b));
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<(int, int)>();
				foreach (int digit in grid.GetCandidatesReversal(abxCell).GetAllSets())
				{
					if (digit != d1 && digit != d2 || digit == a)
					{
						continue;
					}

					candidateOffsets.Add((digit == b ? 1 : 0, abxCell * 9 + digit));
				}
				foreach (int digit in grid.GetCandidatesReversal(abyCell).GetAllSets())
				{
					if (digit != d1 && digit != d2 || digit == b)
					{
						continue;
					}

					candidateOffsets.Add((digit == a ? 1 : 0, abyCell * 9 + digit));
				}
				foreach (int digit in grid.GetCandidatesReversal(abzCell).GetAllSets())
				{
					if (digit != a && digit != b)
					{
						continue;
					}
					candidateOffsets.Add((1, abzCell * 9 + digit));
				}
				foreach (int digit in comparer.GetAllSets())
				{
					candidateOffsets.Add((0, cornerCell * 9 + digit));
				}
				if (!_allowUncompletedUr && candidateOffsets.Count != 6)
				{
					continue;
				}

				accumulator.Add(
					new UrPlusTechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: arMode ? GetHighlightCells(urCells) : null,
								candidateOffsets,
								regionOffsets: new[] { (0, map1.CoveredLine), (1, map2.CoveredLine) },
								links: null)
						},
						typeName: "+ 3X / 2SL",
						typeCode: 10,
						digit1: d1,
						digit2: d2,
						cells: urCells,
						conjugatePairs: new[]
						{
							new ConjugatePair(abxCell, abzCell, b),
							new ConjugatePair(abyCell, abzCell, a)
						},
						isAr: arMode));
			}
		}


	}
}
