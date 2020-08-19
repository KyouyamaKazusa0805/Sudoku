using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Data.GridMap.InitializationOption;
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
			if ((grid.GetCandidateMask(corner1) | grid.GetCandidateMask(corner2)) != comparer)
			{
				return;
			}

			int[] otherCells = otherCellsMap.ToArray();
			short o1 = grid.GetCandidateMask(otherCells[0]);
			short o2 = grid.GetCandidateMask(otherCells[1]);
			short o = (short)(o1 | o2);
			if (o.CountSet() != 4 || o1.CountSet() > 3 || o2.CountSet() > 3
				|| (o & comparer) != comparer || (o1 & comparer) == 0 || (o2 & comparer) == 0)
			{
				return;
			}

			short xyMask = (short)(o ^ comparer);
			int x = xyMask.FindFirstSet();
			int y = xyMask.GetNextSet(x);
			var inter = new GridMap(otherCells, ProcessPeersWithoutItself) - urCells;
			foreach (int possibleXyCell in inter)
			{
				if (grid.GetCandidateMask(possibleXyCell) != xyMask)
				{
					continue;
				}

				// 'xy' found.
				// Now check eliminations.
				var elimMap = inter & PeerMaps[possibleXyCell];
				var conclusions = new List<Conclusion>();
				foreach (int cell in elimMap)
				{
					if (grid.Exists(cell, x) is true)
					{
						conclusions.Add(new(Elimination, cell, x));
					}
					if (grid.Exists(cell, y) is true)
					{
						conclusions.Add(new(Elimination, cell, y));
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
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(((comparer >> digit & 1) == 0 ? 1 : 0, cell * 9 + digit));
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
				foreach (int digit in xyMask.GetAllSets())
				{
					candidateOffsets.Add((1, possibleXyCell * 9 + digit));
				}

				if (!_allowIncompleteUr && candidateOffsets.Count(CheckHighlightType) != 8)
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
						typeCode: arMode ? APlus2D : Plus2D,
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
			if ((grid.GetCandidateMask(corner1) | grid.GetCandidateMask(corner2)) != comparer)
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
						if (!IsConjugatePair(digit, new() { cell, sameRegionCell }, region))
						{
							continue;
						}

						int elimCell = new GridMap(otherCellsMap) { [sameRegionCell] = false }.SetAt(0);
						if (grid.Exists(sameRegionCell, digit) is not true)
						{
							continue;
						}

						int elimDigit = (comparer ^ (1 << digit)).FindFirstSet();
						var conclusions = new List<Conclusion>();
						if (grid.Exists(elimCell, elimDigit) is true)
						{
							conclusions.Add(new(Elimination, elimCell, elimDigit));
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
								if (new GridMap { urCell, sameRegionCell }.CoveredRegions.Contains(region))
								{
									foreach (int d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add((d == digit ? 1 : 0, urCell * 9 + d));
									}
								}
								else
								{
									foreach (int d in grid.GetCandidates(urCell))
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
										if (urCell != elimCell || d != elimDigit)
										{
											candidateOffsets.Add((
												urCell == elimCell ? 0 : (d == digit ? 1 : 0), urCell * 9 + d));
										}
									}
								}

								record(d1);
								record(d2);
							}
						}

						if (!_allowIncompleteUr && candidateOffsets.Count != 7)
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
			if ((grid.GetCandidateMask(corner1) | grid.GetCandidateMask(corner2)) != comparer)
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
						if (!IsConjugatePair(digit, new() { cell, sameRegionCell }, region))
						{
							continue;
						}

						int elimCell = new GridMap(otherCellsMap) { [sameRegionCell] = false }.SetAt(0);
						if (grid.Exists(sameRegionCell, digit) is not true)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						if (grid.Exists(elimCell, digit) is true)
						{
							conclusions.Add(new(Elimination, elimCell, digit));
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
								if (new GridMap { urCell, sameRegionCell }.CoveredRegions.Any(r => r == region))
								{
									foreach (int d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add((d == digit ? 1 : 0, urCell * 9 + d));
									}
								}
								else
								{
									foreach (int d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add((0, urCell * 9 + d));
									}
								}
							}
							else if (urCell == sameRegionCell || urCell == elimCell)
							{
								void record(int d)
								{
									if (grid.Exists(urCell, d) is true && (urCell != elimCell || d != digit))
									{
										candidateOffsets.Add((
											urCell == elimCell ? 0 : (d == digit ? 1 : 0), urCell * 9 + d));
									}
								}

								record(d1);
								record(d2);
							}
						}

						if (!_allowIncompleteUr && candidateOffsets.Count != 7)
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
			if (grid.GetCandidateMask(cornerCell) != comparer)
			{
				return;
			}

			int c1 = otherCellsMap.SetAt(0);
			int c2 = otherCellsMap.SetAt(1);
			int c3 = otherCellsMap.SetAt(2);
			short m1 = grid.GetCandidateMask(c1);
			short m2 = grid.GetCandidateMask(c2);
			short m3 = grid.GetCandidateMask(c3);
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
			var inter = otherCellsMap.PeerIntersection - urCells;
			foreach (int possibleXyCell in inter)
			{
				if (grid.GetCandidateMask(possibleXyCell) != xyMask)
				{
					continue;
				}

				// Possible XY cell found.
				// Now check eliminations.
				var conclusions = new List<Conclusion>();
				foreach (int cell in inter & PeerMaps[possibleXyCell])
				{
					if (grid.Exists(cell, x) is true)
					{
						conclusions.Add(new(Elimination, cell, x));
					}
					if (grid.Exists(cell, y) is true)
					{
						conclusions.Add(new(Elimination, cell, y));
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
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(((comparer >> digit & 1) == 0 ? 1 : 0, cell * 9 + digit));
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
				foreach (int digit in xyMask.GetAllSets())
				{
					candidateOffsets.Add((1, possibleXyCell * 9 + digit));
				}
				if (!_allowIncompleteUr && candidateOffsets.Count(CheckHighlightType) != 8)
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
						typeCode: arMode ? APlus3X : Plus3X,
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
			if (grid.GetCandidateMask(cornerCell) != comparer)
			{
				return;
			}

			int abzCell = GetDiagonalCell(urCells, cornerCell);
			var adjacentCellsMap = new GridMap(otherCellsMap) { [abzCell] = false };
			foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
			{
				int abxCell = adjacentCellsMap.SetAt(0);
				int abyCell = adjacentCellsMap.SetAt(1);
				var map1 = new GridMap { abzCell, abxCell };
				var map2 = new GridMap { abzCell, abyCell };
				if (!IsConjugatePair(b, map1, map1.CoveredLine)
					|| !IsConjugatePair(a, map2, map2.CoveredLine))
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				if (grid.Exists(abxCell, a) is true)
				{
					conclusions.Add(new(Elimination, abxCell, a));
				}
				if (grid.Exists(abyCell, b) is true)
				{
					conclusions.Add(new(Elimination, abyCell, b));
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<(int, int)>();
				foreach (int digit in grid.GetCandidates(abxCell))
				{
					if ((digit == d1 || digit == d2) && digit != a)
					{
						candidateOffsets.Add((digit == b ? 1 : 0, abxCell * 9 + digit));
					}
				}
				foreach (int digit in grid.GetCandidates(abyCell))
				{
					if ((digit == d1 || digit == d2) && digit != b)
					{
						candidateOffsets.Add((digit == a ? 1 : 0, abyCell * 9 + digit));
					}
				}
				foreach (int digit in grid.GetCandidates(abzCell))
				{
					if (digit == a || digit == b)
					{
						candidateOffsets.Add((1, abzCell * 9 + digit));
					}
				}
				foreach (int digit in comparer.GetAllSets())
				{
					candidateOffsets.Add((0, cornerCell * 9 + digit));
				}
				if (!_allowIncompleteUr && candidateOffsets.Count != 6)
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
							new(abxCell, abzCell, b),
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
			if (grid.GetCandidateMask(cornerCell) != comparer)
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
				var linkMap = new GridMap { begin, abzCell };
				foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
				{
					if (!IsConjugatePair(b, linkMap, linkMap.CoveredLine))
					{
						continue;
					}

					// Step 2: Get the link cell that is adjacent to 'cornerCell'
					// and check the strong link.
					var secondLinkMap = new GridMap { cornerCell, begin };
					if (!IsConjugatePair(a, secondLinkMap, secondLinkMap.CoveredLine))
					{
						continue;
					}

					// Step 3: Check eliminations.
					var conclusions = new List<Conclusion>();
					if (grid.Exists(end, a) is true)
					{
						conclusions.Add(new(Elimination, end, a));
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
						if (grid.Exists(abzCell, d) is true)
						{
							candidateOffsets.Add((d == b ? 1 : 0, abzCell * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(begin))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add((1, begin * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(end))
					{
						if ((d == d1 || d == d2) && d != a)
						{
							candidateOffsets.Add((0, end * 9 + d));
						}
					}
					if (!_allowIncompleteUr && candidateOffsets.Count != 7)
					{
						continue;
					}

					var conjugatePairs = new[]
					{
						new(cornerCell, begin, a),
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
			if (grid.GetCandidateMask(cornerCell) != comparer)
			{
				return;
			}

			int abzCell = GetDiagonalCell(urCells, cornerCell);
			var adjacentCellsMap = new GridMap(otherCellsMap) { [abzCell] = false };
			int abxCell = adjacentCellsMap.SetAt(0);
			int abyCell = adjacentCellsMap.SetAt(1);
			foreach (var (begin, end) in stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) })
			{
				var linkMap = new GridMap { begin, abzCell };
				foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
				{
					if (!IsConjugatePair(b, linkMap, linkMap.CoveredLine))
					{
						continue;
					}

					var secondLinkMap = new GridMap { cornerCell, end };
					if (!IsConjugatePair(a, secondLinkMap, secondLinkMap.CoveredLine))
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					if (grid.Exists(begin, a) is true)
					{
						conclusions.Add(new(Elimination, begin, a));
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
					foreach (int d in grid.GetCandidates(begin))
					{
						if ((d == d1 || d == d2) && d != a)
						{
							candidateOffsets.Add((1, begin * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(end))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add((d == a ? 1 : 0, end * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(abzCell))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add((d == b ? 1 : 0, abzCell * 9 + d));
						}
					}
					if (!_allowIncompleteUr && candidateOffsets.Count != 7)
					{
						continue;
					}

					var conjugatePairs = new[]
					{
						new(cornerCell, end, a),
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
			if (grid.GetCandidateMask(cornerCell) != comparer)
			{
				return;
			}

			int abzCell = GetDiagonalCell(urCells, cornerCell);
			var adjacentCellsMap = new GridMap(otherCellsMap) { [abzCell] = false };
			int abxCell = adjacentCellsMap.SetAt(0);
			int abyCell = adjacentCellsMap.SetAt(1);
			foreach (var (begin, end) in stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) })
			{
				var linkMap = new GridMap { begin, abzCell };
				foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
				{
					if (!IsConjugatePair(a, linkMap, linkMap.CoveredLine))
					{
						continue;
					}

					var secondLinkMap = new GridMap { cornerCell, end };
					if (!IsConjugatePair(a, secondLinkMap, secondLinkMap.CoveredLine))
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					if (grid.Exists(abzCell, b) is true)
					{
						conclusions.Add(new(Elimination, abzCell, b));
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
					foreach (int d in grid.GetCandidates(begin))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add((d == a ? 1 : 0, begin * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(end))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add((d == a ? 1 : 0, end * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(abzCell))
					{
						if ((d == d1 || d == d2) && d != b)
						{
							candidateOffsets.Add((d == a ? 1 : 0, abzCell * 9 + d));
						}
					}
					if (!_allowIncompleteUr && candidateOffsets.Count != 7)
					{
						continue;
					}

					var conjugatePairs = new[]
					{
						new(cornerCell, end, a),
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
			var link1Map = new GridMap { corner1, corner2 };
			foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
			{
				if (!IsConjugatePair(a, link1Map, link1Map.CoveredLine))
				{
					continue;
				}

				int abwCell = GetDiagonalCell(urCells, corner1);
				int abzCell = new GridMap(otherCellsMap) { [abwCell] = false }.SetAt(0);
				foreach (var (head, begin, end, extra) in stackalloc[] { (corner2, corner1, abzCell, abwCell), (corner1, corner2, abwCell, abzCell) })
				{
					var link2Map = new GridMap { begin, end };
					if (!IsConjugatePair(b, link2Map, link2Map.CoveredLine))
					{
						continue;
					}

					var link3Map = new GridMap { end, extra };
					if (!IsConjugatePair(a, link3Map, link3Map.CoveredLine))
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					if (grid.Exists(head, b) is true)
					{
						conclusions.Add(new(Elimination, head, b));
					}
					if (grid.Exists(extra, b) is true)
					{
						conclusions.Add(new(Elimination, extra, b));
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<(int, int)>();
					foreach (int d in grid.GetCandidates(head))
					{
						if ((d == d1 || d == d2) && d != b)
						{
							candidateOffsets.Add((1, head * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(extra))
					{
						if ((d == d1 || d == d2) && d != b)
						{
							candidateOffsets.Add((1, extra * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(begin))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add((1, begin * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(end))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add((1, end * 9 + d));
						}
					}
					if (!_allowIncompleteUr && (candidateOffsets.Count != 6 || conclusions.Count != 2))
					{
						continue;
					}

					var conjugatePairs = new[]
					{
						new(head, begin, a),
						new(begin, end, b),
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
			// Subtype 1:
			//   ↓ corner1, corner2
			// (abx)-----(aby)
			//        a    |
			//             | a
			//        b    |
			//  abz ----- abw
			//
			// Subtype 2:
			//   ↓ corner1, corner2
			// (abx)-----(aby)
			//   |    a    |
			//   | b       | a
			//   |         |
			//  abz       abw
			var innerMaps = (Span<GridMap>)stackalloc GridMap[2];
			var link1Map = new GridMap { corner1, corner2 };
			foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
			{
				if (!IsConjugatePair(a, link1Map, link1Map.CoveredLine))
				{
					continue;
				}

				int end = GetDiagonalCell(urCells, corner1);
				int extra = new GridMap(otherCellsMap) { [end] = false }.SetAt(0);
				foreach (var (abx, aby, abw, abz) in
					stackalloc[] { (corner2, corner1, extra, end), (corner1, corner2, end, extra) })
				{
					var link2Map = new GridMap { aby, abw };
					if (!IsConjugatePair(a, link2Map, link2Map.CoveredLine))
					{
						continue;
					}

					GridMap link3Map1 = new() { abw, abz }, link3Map2 = new() { abx, abz };
					innerMaps[0] = link3Map1;
					innerMaps[1] = link3Map2;
					for (int i = 0; i < 2; i++)
					{
						var linkMap = innerMaps[i];
						if (!IsConjugatePair(b, link3Map1, link3Map1.CoveredLine))
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						if (grid.Exists(aby, b) is true)
						{
							conclusions.Add(new(Elimination, aby, b));
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<(int, int)>();
						foreach (int d in grid.GetCandidates(abx))
						{
							if (d == d1 || d == d2)
							{
								candidateOffsets.Add((i == 0 ? d == a ? 1 : 0 : 1, abx * 9 + d));
							}
						}
						foreach (int d in grid.GetCandidates(abz))
						{
							if (d == d1 || d == d2)
							{
								candidateOffsets.Add((d == b ? 1 : 0, abz * 9 + d));
							}
						}
						foreach (int d in grid.GetCandidates(aby))
						{
							if ((d == d1 || d == d2) && d != b)
							{
								candidateOffsets.Add((1, aby * 9 + d));
							}
						}
						foreach (int d in grid.GetCandidates(abw))
						{
							if (d == d1 || d == d2)
							{
								candidateOffsets.Add((1, abw * 9 + d));
							}
						}
						if (!_allowIncompleteUr && candidateOffsets.Count != 7)
						{
							continue;
						}

						var conjugatePairs = new[]
						{
							new(abx, aby, a),
							new(aby, abw, a),
							new ConjugatePair(linkMap.SetAt(0), linkMap.SetAt(1), b)
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
		}

		partial void CheckWing(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap,
			int size)
		{
			// Subtype 1:
			//     ↓ corner1
			//   (ab )  abxy  yz  xz
			//   (ab )  abxy  *
			//     ↑ corner2
			// Note that 'abxy' cells should be in the same region.
			//
			// Subtype 2:
			//     ↓ corner1
			//   (ab )  abx   xz
			//    aby  (ab )  *   yz
			//           ↑ corner2
			if ((grid.GetCandidateMask(corner1) | grid.GetCandidateMask(corner2)) != comparer)
			{
				return;
			}

			if (new GridMap { corner1, corner2 }.AllSetsAreInOneRegion(out int region) && region < 9)
			{
				// Subtype 1.
				int otherCell1 = otherCellsMap.SetAt(0), otherCell2 = otherCellsMap.SetAt(1);
				short mask1 = grid.GetCandidateMask(otherCell1);
				short mask2 = grid.GetCandidateMask(otherCell2);
				short mask = (short)(mask1 | mask2);
				if (mask.CountSet() != 2 + size || (mask & comparer) != comparer
					|| mask1 == comparer || mask2 == comparer)
				{
					return;
				}

				var map = (PeerMaps[otherCell1] | PeerMaps[otherCell2]) & BivalueMap;
				if (map.Count < size)
				{
					return;
				}

				var testMap = new GridMap(stackalloc[] { otherCell1, otherCell2 }, ProcessPeersWithoutItself);
				short extraDigitsMask = (short)(mask ^ comparer);
				int[] cells = map.ToArray();
				for (int i1 = 0, length = cells.Length; i1 < length - size + 1; i1++)
				{
					int c1 = cells[i1];
					short m1 = grid.GetCandidateMask(c1);
					if ((m1 & ~extraDigitsMask) == 0)
					{
						continue;
					}

					for (int i2 = i1 + 1; i2 < length - size + 2; i2++)
					{
						int c2 = cells[i2];
						short m2 = grid.GetCandidateMask(c2);
						if ((m2 & ~extraDigitsMask) == 0)
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

							// Now check whether all cells found should see their corresponding
							// cells in UR structure ('otherCells1' or 'otherCells2').
							bool flag = true;
							foreach (int cell in stackalloc[] { c1, c2 })
							{
								int extraDigit = (grid.GetCandidateMask(cell) & ~m).FindFirstSet();
								if (!(testMap & CandMaps[extraDigit])[cell])
								{
									flag = false;
									break;
								}
							}
							if (!flag)
							{
								continue;
							}

							// Now check eliminations.
							var conclusions = new List<Conclusion>();
							int elimDigit = m.FindFirstSet();
							var elimMap =
								new GridMap(stackalloc[] { c1, c2 }, ProcessPeersWithoutItself) & CandMaps[elimDigit];
							if (elimMap.IsEmpty)
							{
								continue;
							}

							foreach (int cell in elimMap)
							{
								conclusions.Add(new(Elimination, cell, elimDigit));
							}

							var candidateOffsets = new List<(int, int)>();
							foreach (int cell in urCells)
							{
								if (grid.GetStatus(cell) == Empty)
								{
									foreach (int digit in grid.GetCandidates(cell))
									{
										candidateOffsets.Add((
											true switch
											{
												_ when digit == elimDigit => otherCellsMap[cell] ? 2 : 0,
												_ when (extraDigitsMask >> digit & 1) != 0 => 1,
												_ => 0
											}, cell * 9 + digit));
									}
								}
							}
							foreach (int digit in grid.GetCandidates(c1))
							{
								candidateOffsets.Add((digit == elimDigit ? 2 : 1, c1 * 9 + digit));
							}
							foreach (int digit in grid.GetCandidates(c2))
							{
								candidateOffsets.Add((digit == elimDigit ? 2 : 1, c2 * 9 + digit));
							}
							if (!_allowIncompleteUr && candidateOffsets.Count(CheckHighlightType) != 8)
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
									typeCode: arMode ? AXyWing : XyWing,
									digit1: d1,
									digit2: d2,
									cells: urCells,
									extraCells: new[] { c1, c2 },
									extraDigits: extraDigitsMask.GetAllSets(),
									pivots: otherCellsMap,
									isAr: arMode));
						}
						else // size > 2
						{
							for (int i3 = i2 + 1; i3 < length - size + 3; i3++)
							{
								int c3 = cells[i3];
								short m3 = grid.GetCandidateMask(c3);
								if ((m3 & ~extraDigitsMask) == 0)
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

									// Now check whether all cells found should see their corresponding
									// cells in UR structure ('otherCells1' or 'otherCells2').
									bool flag = true;
									foreach (int cell in stackalloc[] { c1, c2, c3 })
									{
										int extraDigit = (grid.GetCandidateMask(cell) & ~m).FindFirstSet();
										if (!(testMap & CandMaps[extraDigit])[cell])
										{
											flag = false;
											break;
										}
									}
									if (!flag)
									{
										continue;
									}

									// Now check eliminations.
									var conclusions = new List<Conclusion>();
									int elimDigit = m.FindFirstSet();
									var elimMap =
										new GridMap(stackalloc[] { c1, c2, c3 }, ProcessPeersWithoutItself)
										& CandMaps[elimDigit];
									if (elimMap.IsEmpty)
									{
										continue;
									}

									foreach (int cell in elimMap)
									{
										conclusions.Add(new(Elimination, cell, elimDigit));
									}

									var candidateOffsets = new List<(int, int)>();
									foreach (int cell in urCells)
									{
										if (grid.GetStatus(cell) == Empty)
										{
											foreach (int digit in grid.GetCandidates(cell))
											{
												candidateOffsets.Add((
													(extraDigitsMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
											}
										}
									}
									foreach (int digit in grid.GetCandidates(c1))
									{
										candidateOffsets.Add((digit == elimDigit ? 2 : 1, c1 * 9 + digit));
									}
									foreach (int digit in grid.GetCandidates(c2))
									{
										candidateOffsets.Add((digit == elimDigit ? 2 : 1, c2 * 9 + digit));
									}
									foreach (int digit in grid.GetCandidates(c3))
									{
										candidateOffsets.Add((digit == elimDigit ? 2 : 1, c3 * 9 + digit));
									}
									if (!_allowIncompleteUr && candidateOffsets.Count(CheckHighlightType) != 8)
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
											typeCode: arMode ? AXyzWing : XyzWing,
											digit1: d1,
											digit2: d2,
											cells: urCells,
											extraCells: new[] { c1, c2, c3 },
											extraDigits: extraDigitsMask.GetAllSets(),
											pivots: otherCellsMap,
											isAr: arMode));
								}
								else // size == 4
								{
									for (int i4 = i3 + 1; i4 < length; i4++)
									{
										int c4 = cells[i4];
										short m4 = grid.GetCandidateMask(c4);
										if ((m4 & ~extraDigitsMask) == 0)
										{
											continue;
										}

										// Check WXYZ-Wing.
										short m = (short)((short)((short)((short)(m1 | m2) | m3) | m4) ^ extraDigitsMask);
										if (m.CountSet() != 1 || (m1 & m2 & m3 & m4).CountSet() != 1)
										{
											continue;
										}

										// Now check whether all cells found should see their corresponding
										// cells in UR structure ('otherCells1' or 'otherCells2').
										bool flag = true;
										foreach (int cell in stackalloc[] { c1, c2, c3, c4 })
										{
											int extraDigit = (grid.GetCandidateMask(cell) & ~m).FindFirstSet();
											if (!(testMap & CandMaps[extraDigit])[cell])
											{
												flag = false;
												break;
											}
										}
										if (!flag)
										{
											continue;
										}

										// Now check eliminations.
										var conclusions = new List<Conclusion>();
										int elimDigit = m.FindFirstSet();
										var elimMap =
											new GridMap(stackalloc[] { c1, c2, c3, c4 }, ProcessPeersWithoutItself)
											& CandMaps[elimDigit];
										if (elimMap.IsEmpty)
										{
											continue;
										}

										foreach (int cell in elimMap)
										{
											conclusions.Add(new(Elimination, cell, elimDigit));
										}

										var candidateOffsets = new List<(int, int)>();
										foreach (int cell in urCells)
										{
											if (grid.GetStatus(cell) == Empty)
											{
												foreach (int digit in grid.GetCandidates(cell))
												{
													candidateOffsets.Add((
														(extraDigitsMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
												}
											}
										}
										foreach (int digit in grid.GetCandidates(c1))
										{
											candidateOffsets.Add((digit == elimDigit ? 2 : 1, c1 * 9 + digit));
										}
										foreach (int digit in grid.GetCandidates(c2))
										{
											candidateOffsets.Add((digit == elimDigit ? 2 : 1, c2 * 9 + digit));
										}
										foreach (int digit in grid.GetCandidates(c3))
										{
											candidateOffsets.Add((digit == elimDigit ? 2 : 1, c3 * 9 + digit));
										}
										foreach (int digit in grid.GetCandidates(c4))
										{
											candidateOffsets.Add((digit == elimDigit ? 2 : 1, c4 * 9 + digit));
										}
										if (!_allowIncompleteUr && candidateOffsets.Count(CheckHighlightType) != 8)
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
												typeCode: arMode ? AWxyzWing : WxyzWing,
												digit1: d1,
												digit2: d2,
												cells: urCells,
												extraCells: new[] { c1, c2, c3, c4 },
												extraDigits: extraDigitsMask.GetAllSets(),
												pivots: otherCellsMap,
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
