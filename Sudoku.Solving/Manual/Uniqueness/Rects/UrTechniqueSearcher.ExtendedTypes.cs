using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.GridMap.InitializeOption;
using static Sudoku.Solving.ConclusionType;
using static Sudoku.Solving.Manual.Uniqueness.Rects.UrTypeCode;

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
						typeCode: Plus2D,
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
								typeCode: Plus2B1SL,
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
								typeCode: Plus2D1SL,
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
			var inter = new GridMap(otherCellsMap, ProcessPeersWithoutItself) - new GridMap(urCells);
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
						typeCode: Plus3X,
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
						typeCode: Plus3X2SL,
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

		partial void Check3N2SL(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, GridMap otherCellsMap)
		{
			//   ↓ cornerCell
			// (ab )-----abx
			//        a   |
			//            | b
			//            |
			//  aby      abz
			if (grid.GetCandidatesReversal(cornerCell) != comparer)
			{
				return;
			}

			// Step 1: Get the diagonal cell of 'cornerCell' and determine
			// the existence of strong link.
			int abzCell = GetDiagonalCell(urCells, cornerCell);
			var adjacentCellsMap = new GridMap(otherCellsMap) { [abzCell] = false };
			int abxCell = adjacentCellsMap.SetAt(0);
			int abyCell = adjacentCellsMap.SetAt(1);
			foreach (var (begin, end) in stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) })
			{
				var linkMap = new GridMap(stackalloc[] { begin, abzCell });
				foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
				{
					if (!IsConjugatePair(grid, b, linkMap, linkMap.CoveredLine))
					{
						continue;
					}

					// Step 2: Get the link cell that is adjacent to 'cornerCell'
					// and check the strong link.
					var secondLinkMap = new GridMap(stackalloc[] { cornerCell, begin });
					if (!IsConjugatePair(grid, a, secondLinkMap, secondLinkMap.CoveredLine))
					{
						continue;
					}

					// Step 3: Check eliminations.
					var conclusions = new List<Conclusion>();
					if (grid.Exists(end, a) is true)
					{
						conclusions.Add(new Conclusion(Elimination, end, a));
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					// Step 4: Check highlight candidates.
					var candidateOffsets = new List<(int, int)>();
					foreach (int d in comparer.GetAllSets())
					{
						candidateOffsets.Add((d == a ? 1 : 0, cornerCell * 9 + d));
					}
					foreach (int d in stackalloc[] { d1, d2 })
					{
						if (!(grid.Exists(abzCell, d) is true))
						{
							continue;
						}

						candidateOffsets.Add((d == b ? 1 : 0, abzCell * 9 + d));
					}
					foreach (int d in grid.GetCandidatesReversal(begin).GetAllSets())
					{
						if (d != d1 && d != d2)
						{
							continue;
						}

						candidateOffsets.Add((1, begin * 9 + d));
					}
					foreach (int d in grid.GetCandidatesReversal(end).GetAllSets())
					{
						if (d != d1 && d != d2 || d == a)
						{
							continue;
						}

						candidateOffsets.Add((0, end * 9 + d));
					}
					if (!_allowUncompletedUr && candidateOffsets.Count != 7)
					{
						continue;
					}

					var conjugatePairs = new[]
					{
						new ConjugatePair(cornerCell, begin, a),
						new ConjugatePair(begin, abzCell, b)
					};
					accumulator.Add(
						new UrPlusTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: arMode ? GetHighlightCells(urCells) : null,
									candidateOffsets,
									regionOffsets: new[] { (0, conjugatePairs[0].Line), (1, conjugatePairs[1].Line) },
									links: null)
							},
							typeCode: Plus3N2SL,
							digit1: d1,
							digit2: d2,
							cells: urCells,
							conjugatePairs,
							isAr: arMode));
				}
			}
		}

		partial void Check3U2SL(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, GridMap otherCellsMap)
		{
			//   ↓ cornerCell
			// (ab )-----abx
			//        a
			//
			//        b
			//  aby -----abz
			if (grid.GetCandidatesReversal(cornerCell) != comparer)
			{
				return;
			}

			int abzCell = GetDiagonalCell(urCells, cornerCell);
			var adjacentCellsMap = new GridMap(otherCellsMap) { [abzCell] = false };
			int abxCell = adjacentCellsMap.SetAt(0);
			int abyCell = adjacentCellsMap.SetAt(1);
			foreach (var (begin, end) in stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) })
			{
				var linkMap = new GridMap(stackalloc[] { begin, abzCell });
				foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
				{
					if (!IsConjugatePair(grid, b, linkMap, linkMap.CoveredLine))
					{
						continue;
					}

					var secondLinkMap = new GridMap(stackalloc[] { cornerCell, end });
					if (!IsConjugatePair(grid, a, secondLinkMap, secondLinkMap.CoveredLine))
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					if (grid.Exists(begin, a) is true)
					{
						conclusions.Add(new Conclusion(Elimination, begin, a));
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<(int, int)>();
					foreach (int d in comparer.GetAllSets())
					{
						candidateOffsets.Add((d == a ? 1 : 0, cornerCell * 9 + d));
					}
					foreach (int d in grid.GetCandidatesReversal(begin).GetAllSets())
					{
						if (d != d1 && d != d2 || d == a)
						{
							continue;
						}

						candidateOffsets.Add((1, begin * 9 + d));
					}
					foreach (int d in grid.GetCandidatesReversal(end).GetAllSets())
					{
						if (d != d1 && d != d2)
						{
							continue;
						}

						candidateOffsets.Add((d == a ? 1 : 0, end * 9 + d));
					}
					foreach (int d in grid.GetCandidatesReversal(abzCell).GetAllSets())
					{
						if (d != d1 && d != d2)
						{
							continue;
						}

						candidateOffsets.Add((d == b ? 1 : 0, abzCell * 9 + d));
					}
					if (!_allowUncompletedUr && candidateOffsets.Count != 7)
					{
						continue;
					}

					var conjugatePairs = new[]
					{
						new ConjugatePair(cornerCell, end, a),
						new ConjugatePair(begin, abzCell, b)
					};
					accumulator.Add(
						new UrPlusTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: arMode ? GetHighlightCells(urCells) : null,
									candidateOffsets,
									regionOffsets: new[] { (0, conjugatePairs[0].Line), (1, conjugatePairs[1].Line) },
									links: null)
							},
							typeCode: Plus3U2SL,
							digit1: d1,
							digit2: d2,
							cells: urCells,
							conjugatePairs,
							isAr: arMode));
				}
			}
		}

		partial void Check3E2SL(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, GridMap otherCellsMap)
		{
			//   ↓ cornerCell
			// (ab )-----abx
			//        a
			//
			//        a
			//  aby -----abz
			if (grid.GetCandidatesReversal(cornerCell) != comparer)
			{
				return;
			}

			int abzCell = GetDiagonalCell(urCells, cornerCell);
			var adjacentCellsMap = new GridMap(otherCellsMap) { [abzCell] = false };
			int abxCell = adjacentCellsMap.SetAt(0);
			int abyCell = adjacentCellsMap.SetAt(1);
			foreach (var (begin, end) in stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) })
			{
				var linkMap = new GridMap(stackalloc[] { begin, abzCell });
				foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
				{
					if (!IsConjugatePair(grid, a, linkMap, linkMap.CoveredLine))
					{
						continue;
					}

					var secondLinkMap = new GridMap(stackalloc[] { cornerCell, end });
					if (!IsConjugatePair(grid, a, secondLinkMap, secondLinkMap.CoveredLine))
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					if (grid.Exists(abzCell, b) is true)
					{
						conclusions.Add(new Conclusion(Elimination, abzCell, b));
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<(int, int)>();
					foreach (int d in comparer.GetAllSets())
					{
						candidateOffsets.Add((d == a ? 1 : 0, cornerCell * 9 + d));
					}
					foreach (int d in grid.GetCandidatesReversal(begin).GetAllSets())
					{
						if (d != d1 && d != d2)
						{
							continue;
						}

						candidateOffsets.Add((d == a ? 1 : 0, begin * 9 + d));
					}
					foreach (int d in grid.GetCandidatesReversal(end).GetAllSets())
					{
						if (d != d1 && d != d2)
						{
							continue;
						}

						candidateOffsets.Add((d == a ? 1 : 0, end * 9 + d));
					}
					foreach (int d in grid.GetCandidatesReversal(abzCell).GetAllSets())
					{
						if (d != d1 && d != d2 || d == b)
						{
							continue;
						}

						candidateOffsets.Add((d == a ? 1 : 0, abzCell * 9 + d));
					}
					if (!_allowUncompletedUr && candidateOffsets.Count != 7)
					{
						continue;
					}

					var conjugatePairs = new[]
					{
						new ConjugatePair(cornerCell, end, a),
						new ConjugatePair(begin, abzCell, a)
					};
					accumulator.Add(
						new UrPlusTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: arMode ? GetHighlightCells(urCells) : null,
									candidateOffsets,
									regionOffsets: new[] { (0, conjugatePairs[0].Line), (1, conjugatePairs[1].Line) },
									links: null)
							},
							typeCode: Plus3E2SL,
							digit1: d1,
							digit2: d2,
							cells: urCells,
							conjugatePairs,
							isAr: arMode));
				}
			}
		}

		partial void Check4X3SL(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap)
		{
			//   ↓ corner1, corner2
			// (abx)-----(aby)
			//        a    |
			//             | b
			//        a    |
			//  abz ----- abw
			var link1Map = new GridMap(stackalloc[] { corner1, corner2 });
			foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
			{
				if (!IsConjugatePair(grid, a, link1Map, link1Map.CoveredLine))
				{
					continue;
				}

				int abwCell = GetDiagonalCell(urCells, corner1);
				int abzCell = new GridMap(otherCellsMap) { [abwCell] = false }.SetAt(0);
				foreach (var (head, begin, end, extra) in stackalloc[] { (corner2, corner1, abzCell, abwCell), (corner1, corner2, abwCell, abzCell) })
				{
					var link2Map = new GridMap(stackalloc[] { begin, end });
					if (!IsConjugatePair(grid, b, link2Map, link2Map.CoveredLine))
					{
						continue;
					}

					var link3Map = new GridMap(stackalloc[] { end, extra });
					if (!IsConjugatePair(grid, a, link3Map, link3Map.CoveredLine))
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					if (grid.Exists(head, b) is true)
					{
						conclusions.Add(new Conclusion(Elimination, head, b));
					}
					if (grid.Exists(extra, b) is true)
					{
						conclusions.Add(new Conclusion(Elimination, head, b));
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<(int, int)>();
					foreach (int d in grid.GetCandidatesReversal(head).GetAllSets())
					{
						if (d != d1 && d != d2 || d == b)
						{
							continue;
						}

						candidateOffsets.Add((1, head * 9 + d));
					}
					foreach (int d in grid.GetCandidatesReversal(extra).GetAllSets())
					{
						if (d != d1 && d != d2 || d == b)
						{
							continue;
						}

						candidateOffsets.Add((1, extra * 9 + d));
					}
					foreach (int d in grid.GetCandidatesReversal(begin).GetAllSets())
					{
						if (d != d1 && d != d2)
						{
							continue;
						}

						candidateOffsets.Add((1, begin * 9 + d));
					}
					foreach (int d in grid.GetCandidatesReversal(end).GetAllSets())
					{
						if (d != d1 && d != d2)
						{
							continue;
						}

						candidateOffsets.Add((1, end * 9 + d));
					}
					if (!_allowUncompletedUr && (candidateOffsets.Count != 6 || conclusions.Count != 2))
					{
						continue;
					}

					var conjugatePairs = new[]
					{
						new ConjugatePair(head, begin, a),
						new ConjugatePair(begin, end, b),
						new ConjugatePair(end, extra, a)
					};
					accumulator.Add(
						new UrPlusTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: arMode ? GetHighlightCells(urCells) : null,
									candidateOffsets,
									regionOffsets: new[]
									{
										(0, conjugatePairs[0].Line),
										(1, conjugatePairs[1].Line),
										(0, conjugatePairs[2].Line)
									},
									links: null)
							},
							typeCode: Plus4X3SL,
							digit1: d1,
							digit2: d2,
							cells: urCells,
							conjugatePairs,
							isAr: arMode));
				}
			}
		}

		partial void Check4C3SL(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap)
		{
			//   ↓ corner1, corner2
			// (abx)-----(aby)
			//        a    |
			//             | a
			//        b    |
			//  abz ----- abw
			var link1Map = new GridMap(stackalloc[] { corner1, corner2 });
			foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
			{
				if (!IsConjugatePair(grid, a, link1Map, link1Map.CoveredLine))
				{
					continue;
				}

				int abwCell = GetDiagonalCell(urCells, corner1);
				int abzCell = new GridMap(otherCellsMap) { [abwCell] = false }.SetAt(0);
				foreach (var (head, begin, end, extra) in stackalloc[] { (corner2, corner1, abzCell, abwCell), (corner1, corner2, abwCell, abzCell) })
				{
					var link2Map = new GridMap(stackalloc[] { begin, end });
					if (!IsConjugatePair(grid, a, link2Map, link2Map.CoveredLine))
					{
						continue;
					}

					var link3Map = new GridMap(stackalloc[] { end, extra });
					if (!IsConjugatePair(grid, b, link3Map, link3Map.CoveredLine))
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					if (grid.Exists(begin, b) is true)
					{
						conclusions.Add(new Conclusion(Elimination, begin, b));
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<(int, int)>();
					foreach (int d in grid.GetCandidatesReversal(head).GetAllSets())
					{
						if (d != d1 && d != d2)
						{
							continue;
						}

						candidateOffsets.Add((d == a ? 1 : 0, head * 9 + d));
					}
					foreach (int d in grid.GetCandidatesReversal(extra).GetAllSets())
					{
						if (d != d1 && d != d2)
						{
							continue;
						}

						candidateOffsets.Add((d == b ? 1 : 0, extra * 9 + d));
					}
					foreach (int d in grid.GetCandidatesReversal(begin).GetAllSets())
					{
						if (d != d1 && d != d2 || d == b)
						{
							continue;
						}

						candidateOffsets.Add((1, begin * 9 + d));
					}
					foreach (int d in grid.GetCandidatesReversal(end).GetAllSets())
					{
						if (d != d1 && d != d2)
						{
							continue;
						}

						candidateOffsets.Add((1, end * 9 + d));
					}
					if (!_allowUncompletedUr && candidateOffsets.Count != 7)
					{
						continue;
					}

					var conjugatePairs = new[]
					{
						new ConjugatePair(head, begin, a),
						new ConjugatePair(begin, end, a),
						new ConjugatePair(end, extra, b)
					};
					accumulator.Add(
						new UrPlusTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: arMode ? GetHighlightCells(urCells) : null,
									candidateOffsets,
									regionOffsets: new[]
									{
										(0, conjugatePairs[0].Line),
										(0, conjugatePairs[1].Line),
										(1, conjugatePairs[2].Line)
									},
									links: null)
							},
							typeCode: Plus4C3SL,
							digit1: d1,
							digit2: d2,
							cells: urCells,
							conjugatePairs,
							isAr: arMode));
				}
			}
		}

		partial void CheckWing(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap,
			GridMap bivalueCells, int size)
		{
			// Subtype 1:
			//     ↓ corner1
			//   (ab )  abxy  yz  xz
			//   (ab )  abxy  *
			//     ↑ corner2
			// Note that 'abxy' cells should be in the same region.
			// Subtype 2:
			//     ↓ corner1
			//   (ab )  abx   xz
			//    aby  (ab )  *   yz
			//           ↑ corner2
			if ((grid.GetCandidatesReversal(corner1) | grid.GetCandidatesReversal(corner2)) != comparer)
			{
				return;
			}

			if (new GridMap(stackalloc[] { corner1, corner2 }).AllSetsAreInOneRegion(out int? region)
				&& region < 9)
			{
				// Subtype 1.
				short mask1 = grid.GetCandidatesReversal(otherCellsMap.SetAt(0));
				short mask2 = grid.GetCandidatesReversal(otherCellsMap.SetAt(1));
				short mask = (short)(mask1 | mask2);
				if (mask.CountSet() != 2 + size || (mask & comparer) != comparer
					|| mask1 == comparer || mask2 == comparer)
				{
					return;
				}

				var map = new GridMap(otherCellsMap, ProcessPeersWithoutItself) & bivalueCells;
				if (map.Count < size)
				{
					return;
				}

				short extraDigitsMask = (short)(mask ^ comparer);
				int[] cells = map.ToArray();
				for (int i1 = 0, length = cells.Length; i1 < length - size + 1; i1++)
				{
					int c1 = cells[i1];
					short m1 = grid.GetCandidatesReversal(c1);
					if ((m1 & extraDigitsMask) == 0)
					{
						continue;
					}

					for (int i2 = i1 + 1; i2 < length - size + 2; i2++)
					{
						int c2 = cells[i2];
						short m2 = grid.GetCandidatesReversal(c2);
						if ((m2 & extraDigitsMask) == 0)
						{
							continue;
						}

						if (size == 2)
						{
							// Check XY-Wing.
							short m = (short)((short)(m1 | m2) ^ extraDigitsMask);
							if (m.CountSet() != 1 || (m1 & m2).CountSet() != 1)
							{
								continue;
							}

							// Now check eliminations.
							var conclusions = new List<Conclusion>();
							int elimDigit = m.FindFirstSet();
							var elimMap = new GridMap(stackalloc[] { c1, c2 }, ProcessPeersWithoutItself);
							if (elimMap.IsEmpty)
							{
								continue;
							}

							foreach (int cell in elimMap.Offsets)
							{
								if (grid.Exists(cell, elimDigit) is true)
								{
									conclusions.Add(new Conclusion(Elimination, cell, elimDigit));
								}
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

								foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
								{
									candidateOffsets.Add((true switch
									{
										_ when digit == elimDigit => otherCellsMap[cell] ? 2 : 0,
										_ when (extraDigitsMask >> digit & 1) != 0 => 1,
										_ => 0
									}, cell * 9 + digit));
								}
							}
							foreach (int digit in grid.GetCandidatesReversal(c1).GetAllSets())
							{
								candidateOffsets.Add((digit == elimDigit ? 2 : 1, c1 * 9 + digit));
							}
							foreach (int digit in grid.GetCandidatesReversal(c2).GetAllSets())
							{
								candidateOffsets.Add((digit == elimDigit ? 2 : 1, c2 * 9 + digit));
							}
							if (!_allowUncompletedUr && candidateOffsets.Count(CheckHighlightType) != 8)
							{
								continue;
							}

							accumulator.Add(
								new UrWithWingTechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: arMode ? GetHighlightCells(urCells) : null,
											candidateOffsets,
											regionOffsets: null,
											links: null)
									},
									typeCode: XyWing,
									digit1: d1,
									digit2: d2,
									cells: urCells,
									extraCells: new[] { c1, c2 },
									extraDigits: extraDigitsMask.GetAllSets(),
									pivots: otherCellsMap.Offsets,
									isAr: arMode));
						}
						else // size > 2
						{
							for (int i3 = i2 + 1; i3 < length - size + 3; i3++)
							{
								int c3 = cells[i3];
								short m3 = grid.GetCandidatesReversal(c3);
								if ((m3 & extraDigitsMask) == 0)
								{
									continue;
								}

								if (size == 3)
								{
									// Check XYZ-Wing.
									short m = (short)(((short)(m1 | m2) | m3) ^ extraDigitsMask);
									if (m.CountSet() != 1 || (m1 & m2 & m3).CountSet() != 1)
									{
										continue;
									}

									// Now check eliminations.
									var conclusions = new List<Conclusion>();
									int elimDigit = m.FindFirstSet();
									var elimMap = new GridMap(stackalloc[] { c1, c2, c3 }, ProcessPeersWithoutItself);
									if (elimMap.IsEmpty)
									{
										continue;
									}

									foreach (int cell in elimMap.Offsets)
									{
										if (grid.Exists(cell, elimDigit) is true)
										{
											conclusions.Add(new Conclusion(Elimination, cell, elimDigit));
										}
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

										foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
										{
											candidateOffsets.Add((true switch
											{
												_ when digit == elimDigit => otherCellsMap[cell] ? 2 : 0,
												_ when (extraDigitsMask >> digit & 1) != 0 => 1,
												_ => 0
											}, cell * 9 + digit));
										}
									}
									foreach (int digit in grid.GetCandidatesReversal(c1).GetAllSets())
									{
										candidateOffsets.Add((digit == elimDigit ? 2 : 1, c1 * 9 + digit));
									}
									foreach (int digit in grid.GetCandidatesReversal(c2).GetAllSets())
									{
										candidateOffsets.Add((digit == elimDigit ? 2 : 1, c2 * 9 + digit));
									}
									foreach (int digit in grid.GetCandidatesReversal(c3).GetAllSets())
									{
										candidateOffsets.Add((digit == elimDigit ? 2 : 1, c3 * 9 + digit));
									}
									if (!_allowUncompletedUr && candidateOffsets.Count(CheckHighlightType) != 8)
									{
										continue;
									}

									accumulator.Add(
										new UrWithWingTechniqueInfo(
											conclusions,
											views: new[]
											{
												new View(
													cellOffsets: arMode ? GetHighlightCells(urCells) : null,
													candidateOffsets,
													regionOffsets: null,
													links: null)
											},
											typeCode: XyzWing,
											digit1: d1,
											digit2: d2,
											cells: urCells,
											extraCells: new[] { c1, c2, c3 },
											extraDigits: extraDigitsMask.GetAllSets(),
											pivots: otherCellsMap.Offsets,
											isAr: arMode));
								}
								else // size == 4
								{
									for (int i4 = i3 + 1; i4 < length; i4++)
									{
										int c4 = cells[i4];
										short m4 = grid.GetCandidatesReversal(c4);
										if ((m4 & extraDigitsMask) == 0)
										{
											continue;
										}

										// Check WXYZ-Wing.
										short m = (short)((short)((short)((short)(m1 | m2) | m3) | m4) ^ extraDigitsMask);
										if (m.CountSet() != 1 || (m1 & m2 & m3 & m4).CountSet() != 1)
										{
											continue;
										}

										// Now check eliminations.
										var conclusions = new List<Conclusion>();
										int elimDigit = m.FindFirstSet();
										var elimMap = new GridMap(stackalloc[] { c1, c2, c3, c4 }, ProcessPeersWithoutItself);
										if (elimMap.IsEmpty)
										{
											continue;
										}

										foreach (int cell in elimMap.Offsets)
										{
											if (grid.Exists(cell, elimDigit) is true)
											{
												conclusions.Add(new Conclusion(Elimination, cell, elimDigit));
											}
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

											foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
											{
												candidateOffsets.Add((true switch
												{
													_ when digit == elimDigit => otherCellsMap[cell] ? 2 : 0,
													_ when (extraDigitsMask >> digit & 1) != 0 => 1,
													_ => 0
												}, cell * 9 + digit));
											}
										}
										foreach (int digit in grid.GetCandidatesReversal(c1).GetAllSets())
										{
											candidateOffsets.Add((digit == elimDigit ? 2 : 1, c1 * 9 + digit));
										}
										foreach (int digit in grid.GetCandidatesReversal(c2).GetAllSets())
										{
											candidateOffsets.Add((digit == elimDigit ? 2 : 1, c2 * 9 + digit));
										}
										foreach (int digit in grid.GetCandidatesReversal(c3).GetAllSets())
										{
											candidateOffsets.Add((digit == elimDigit ? 2 : 1, c3 * 9 + digit));
										}
										foreach (int digit in grid.GetCandidatesReversal(c4).GetAllSets())
										{
											candidateOffsets.Add((digit == elimDigit ? 2 : 1, c4 * 9 + digit));
										}
										if (!_allowUncompletedUr && candidateOffsets.Count(CheckHighlightType) != 8)
										{
											continue;
										}

										accumulator.Add(
											new UrWithWingTechniqueInfo(
												conclusions,
												views: new[]
												{
													new View(
														cellOffsets: arMode ? GetHighlightCells(urCells) : null,
														candidateOffsets,
														regionOffsets: null,
														links: null)
												},
												typeCode: WxyzWing,
												digit1: d1,
												digit2: d2,
												cells: urCells,
												extraCells: new[] { c1, c2, c3, c4 },
												extraDigits: extraDigitsMask.GetAllSets(),
												pivots: otherCellsMap.Offsets,
												isAr: arMode));
									}
								}
							}
						}
					}
				}
			}
			else
			{
				// TODO: Finish processing Subtype 2.
			}
		}
	}
}
