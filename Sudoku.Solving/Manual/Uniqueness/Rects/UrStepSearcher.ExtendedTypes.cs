using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	partial class UrStepSearcher
	{
		/// <summary>
		/// Check UR+2D.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void Check2D(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index)
		{
			//   ↓ corner1
			// (ab )  abx
			//  aby  (ab )  xy  *
			//         ↑ corner2
			if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
			{
				return;
			}

			int[] otherCells = otherCellsMap.ToArray();
			short o1 = grid.GetCandidates(otherCells[0]), o2 = grid.GetCandidates(otherCells[1]);
			short o = (short)(o1 | o2);
			if ((o.PopCount(), o1.PopCount(), o2.PopCount(), o1 & comparer, o2 & comparer) is
				not (4, <= 3, <= 3, not 0, not 0)
				|| (o & comparer) != comparer)
			{
				return;
			}

			short xyMask = (short)(o ^ comparer);
			int x = xyMask.FindFirstSet(), y = xyMask.GetNextSet(x);
			var inter = new Cells(otherCells).PeerIntersection - urCells;
			foreach (int possibleXyCell in inter)
			{
				if (grid.GetCandidates(possibleXyCell) != xyMask)
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
						conclusions.Add(new(ConclusionType.Elimination, cell, x));
					}
					if (grid.Exists(cell, y) is true)
					{
						conclusions.Add(new(ConclusionType.Elimination, cell, y));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<DrawingInfo>();
				foreach (int cell in urCells)
				{
					if (otherCellsMap.Contains(cell))
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new((comparer >> digit & 1) == 0 ? 1 : 0, cell * 9 + digit));
						}
					}
					else
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(0, cell * 9 + digit));
						}
					}
				}
				foreach (int digit in xyMask)
				{
					candidateOffsets.Add(new(1, possibleXyCell * 9 + digit));
				}

				unsafe
				{
					if (!_allowIncompleteUr && candidateOffsets.Count(&CheckHighlightType) != 8)
					{
						continue;
					}
				}

				accumulator.Add(
					new Ur2DOr3XStepInfo(
						conclusions,
						new View[]
						{
							new()
							{
								Cells = arMode ? GetHighlightCells(urCells) : null,
								Candidates = candidateOffsets
							}
						},
						arMode ? UrTypeCode.APlus2D_Upper : UrTypeCode.Plus2D_Upper,
						d1,
						d2,
						urCells,
						arMode,
						x,
						y,
						possibleXyCell,
						index));
			}
		}

		/// <summary>
		/// Check UR+2B/1SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void Check2B1SL(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index)
		{
			//   ↓ corner1, corner2
			// (ab )  (ab )
			//  |
			//  | a
			//  |
			//  abx    aby
			if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
			{
				return;
			}

			foreach (int cell in stackalloc[] { corner1, corner2 })
			{
				foreach (int otherCell in otherCellsMap)
				{
					if (!IsSameRegionCell(cell, otherCell, out int regions))
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
							if (!IsConjugatePair(digit, new() { cell, otherCell }, region))
							{
								continue;
							}

							int elimCell = (otherCellsMap - otherCell)[0];
							if (grid.Exists(otherCell, digit) is not true)
							{
								continue;
							}

							int elimDigit = (comparer ^ (1 << digit)).FindFirstSet();
							var conclusions = new List<Conclusion>();
							if (grid.Exists(elimCell, elimDigit) is true)
							{
								conclusions.Add(new(ConclusionType.Elimination, elimCell, elimDigit));
							}
							if (conclusions.Count == 0)
							{
								continue;
							}

							var candidateOffsets = new List<DrawingInfo>();
							foreach (int urCell in urCells)
							{
								if (urCell == corner1 || urCell == corner2)
								{
									int coveredRegions = new Cells { urCell, otherCell }.CoveredRegions;
									if ((coveredRegions >> region & 1) != 0)
									{
										foreach (int d in grid.GetCandidates(urCell))
										{
											candidateOffsets.Add(new(d == digit ? 1 : 0, urCell * 9 + d));
										}
									}
									else
									{
										foreach (int d in grid.GetCandidates(urCell))
										{
											candidateOffsets.Add(new(0, urCell * 9 + d));
										}
									}
								}
								else if (urCell == otherCell || urCell == elimCell)
								{
									if (grid.Exists(urCell, d1) is true)
									{
										if (urCell != elimCell || d1 != elimDigit)
										{
											candidateOffsets.Add(
												new(
													urCell == elimCell ? 0 : (d1 == digit ? 1 : 0),
													urCell * 9 + d1));
										}
									}
									if (grid.Exists(urCell, d2) is true)
									{
										if (urCell != elimCell || d2 != elimDigit)
										{
											candidateOffsets.Add(
												new(
													urCell == elimCell ? 0 : (d2 == digit ? 1 : 0),
													urCell * 9 + d2));
										}
									}
								}
							}

							if (!_allowIncompleteUr && candidateOffsets.Count != 7)
							{
								continue;
							}

							accumulator.Add(
								new UrPlusStepInfo(
									conclusions,
									new View[]
									{
										new()
										{
											Cells = arMode ? GetHighlightCells(urCells) : null,
											Candidates = candidateOffsets,
											Regions = new DrawingInfo[] { new(0, region) }
										}
									},
									UrTypeCode.Plus2B1SL,
									d1,
									d2,
									urCells,
									arMode,
									new ConjugatePair[] { new(cell, otherCell, digit) },
									index));
						}
					}
				}
			}
		}

		/// <summary>
		/// Check UR+2D/1SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void Check2D1SL(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index)
		{
			//   ↓ corner1
			// (ab )   aby
			//  |
			//  | a
			//  |
			//  abx   (ab )
			//          ↑ corner2
			if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
			{
				return;
			}

			foreach (int cell in stackalloc[] { corner1, corner2 })
			{
				foreach (int otherCell in otherCellsMap)
				{
					if (!IsSameRegionCell(cell, otherCell, out var regions))
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
							if (!IsConjugatePair(digit, new() { cell, otherCell }, region))
							{
								continue;
							}

							int elimCell = (otherCellsMap - otherCell)[0];
							if (grid.Exists(otherCell, digit) is not true)
							{
								continue;
							}

							var conclusions = new List<Conclusion>();
							if (grid.Exists(elimCell, digit) is true)
							{
								conclusions.Add(new(ConclusionType.Elimination, elimCell, digit));
							}
							if (conclusions.Count == 0)
							{
								continue;
							}

							var candidateOffsets = new List<DrawingInfo>();
							foreach (int urCell in urCells)
							{
								if (urCell == corner1 || urCell == corner2)
								{
									bool flag = false;
									foreach (int r in new Cells { urCell, otherCell }.CoveredRegions)
									{
										if (r == region)
										{
											flag = true;
											break;
										}
									}

									if (flag)
									{
										foreach (int d in grid.GetCandidates(urCell))
										{
											candidateOffsets.Add(new(d == digit ? 1 : 0, urCell * 9 + d));
										}
									}
									else
									{
										foreach (int d in grid.GetCandidates(urCell))
										{
											candidateOffsets.Add(new(0, urCell * 9 + d));
										}
									}
								}
								else if (urCell == otherCell || urCell == elimCell)
								{
									if (grid.Exists(urCell, d1) is true && (urCell != elimCell || d1 != digit))
									{
										candidateOffsets.Add(
											new(
												urCell == elimCell ? 0 : (d1 == digit ? 1 : 0),
												urCell * 9 + d1));
									}
									if (grid.Exists(urCell, d2) is true && (urCell != elimCell || d2 != digit))
									{
										candidateOffsets.Add(
											new(
												urCell == elimCell ? 0 : (d2 == digit ? 1 : 0),
												urCell * 9 + d2));
									}
								}
							}

							if (!_allowIncompleteUr && candidateOffsets.Count != 7)
							{
								continue;
							}

							accumulator.Add(
								new UrPlusStepInfo(
									conclusions,
									new View[]
									{
										new()
										{
											Cells = arMode ? GetHighlightCells(urCells) : null,
											Candidates = candidateOffsets,
											Regions = new DrawingInfo[] { new(0, region) }
										}
									},
									UrTypeCode.Plus2D1SL,
									d1,
									d2,
									urCells,
									arMode,
									new ConjugatePair[] { new(cell, otherCell, digit) },
									index));
						}
					}
				}
			}
		}

		/// <summary>
		/// Check UR+3X.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="cornerCell">The corner cell.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void Check3X(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index)
		{
			//   ↓ cornerCell
			// (ab )  abx
			//  aby   abz   xy  *
			// Note: 'z' is 'x' or 'y'.
			if (grid.GetCandidates(cornerCell) != comparer)
			{
				return;
			}

			int[] offsets = otherCellsMap.ToArray();
			int c1 = offsets[0], c2 = offsets[1], c3 = offsets[2];
			short m1 = grid.GetCandidates(c1), m2 = grid.GetCandidates(c2), m3 = grid.GetCandidates(c3);
			short mask = (short)((short)(m1 | m2) | m3);

			if (
				(
					mask.PopCount(), m1.PopCount(), m2.PopCount(), m3.PopCount(),
					m1 & comparer, m2 & comparer, m3 & comparer
				) is not (4, <= 3, <= 3, <= 3, not 0, not 0, not 0)
				|| (mask & comparer) != comparer)
			{
				return;
			}

			short xyMask = (short)(mask ^ comparer);
			int x = xyMask.FindFirstSet();
			int y = xyMask.GetNextSet(x);
			var inter = otherCellsMap.PeerIntersection - urCells;
			foreach (int possibleXyCell in inter)
			{
				if (grid.GetCandidates(possibleXyCell) != xyMask)
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
						conclusions.Add(new(ConclusionType.Elimination, cell, x));
					}
					if (grid.Exists(cell, y) is true)
					{
						conclusions.Add(new(ConclusionType.Elimination, cell, y));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<DrawingInfo>();
				foreach (int cell in urCells)
				{
					if (otherCellsMap.Contains(cell))
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new((comparer >> digit & 1) == 0 ? 1 : 0, cell * 9 + digit));
						}
					}
					else
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(0, cell * 9 + digit));
						}
					}
				}
				foreach (int digit in xyMask)
				{
					candidateOffsets.Add(new(1, possibleXyCell * 9 + digit));
				}
				unsafe
				{
					if (!_allowIncompleteUr && candidateOffsets.Count(&CheckHighlightType) != 8)
					{
						continue;
					}
				}

				accumulator.Add(
					new Ur2DOr3XStepInfo(
						conclusions,
						new View[]
						{
							new()
							{
								Cells = arMode ? GetHighlightCells(urCells) : null,
								Candidates = candidateOffsets
							}
						},
						arMode ? UrTypeCode.APlus3X_Upper : UrTypeCode.Plus3X_Upper,
						d1,
						d2,
						urCells,
						arMode,
						x,
						y,
						possibleXyCell,
						index));
			}
		}

		/// <summary>
		/// Check UR+3X/2SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="cornerCell">The corner cell.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void Check3X2SL(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index)
		{
			//   ↓ cornerCell
			// (ab )    abx
			//           |
			//           | b
			//       a   |
			//  aby-----abz
			if (grid.GetCandidates(cornerCell) != comparer)
			{
				return;
			}

			int abzCell = GetDiagonalCell(urCells, cornerCell);
			var adjacentCellsMap = otherCellsMap - abzCell;
			foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
			{
				int[] offsets = adjacentCellsMap.ToArray();
				int abxCell = offsets[0], abyCell = offsets[1];
				var map1 = new Cells { abzCell, abxCell };
				var map2 = new Cells { abzCell, abyCell };
				if (!IsConjugatePair(b, map1, map1.CoveredLine)
					|| !IsConjugatePair(a, map2, map2.CoveredLine))
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				if (grid.Exists(abxCell, a) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, abxCell, a));
				}
				if (grid.Exists(abyCell, b) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, abyCell, b));
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<DrawingInfo>();
				foreach (int digit in grid.GetCandidates(abxCell))
				{
					if ((digit == d1 || digit == d2) && digit != a)
					{
						candidateOffsets.Add(new(digit == b ? 1 : 0, abxCell * 9 + digit));
					}
				}
				foreach (int digit in grid.GetCandidates(abyCell))
				{
					if ((digit == d1 || digit == d2) && digit != b)
					{
						candidateOffsets.Add(new(digit == a ? 1 : 0, abyCell * 9 + digit));
					}
				}
				foreach (int digit in grid.GetCandidates(abzCell))
				{
					if (digit == a || digit == b)
					{
						candidateOffsets.Add(new(1, abzCell * 9 + digit));
					}
				}
				foreach (int digit in comparer)
				{
					candidateOffsets.Add(new(0, cornerCell * 9 + digit));
				}
				if (!_allowIncompleteUr && candidateOffsets.Count != 6)
				{
					continue;
				}

				accumulator.Add(
					new UrPlusStepInfo(
						conclusions,
						new View[]
						{
							new()
							{
								Cells = arMode ? GetHighlightCells(urCells) : null,
								Candidates = candidateOffsets,
								Regions = new DrawingInfo[]
								{
									new(0, map1.CoveredLine),
									new(1, map2.CoveredLine)
								}
							}
						},
						UrTypeCode.Plus3X2SL,
						d1,
						d2,
						urCells,
						arMode,
						new ConjugatePair[] { new(abxCell, abzCell, b), new(abyCell, abzCell, a) },
						index));
			}
		}

		/// <summary>
		/// Check UR+3N/2SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="cornerCell">The corner cell.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void Check3N2SL(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index)
		{
			//   ↓ cornerCell
			// (ab )-----abx
			//        a   |
			//            | b
			//            |
			//  aby      abz
			if (grid.GetCandidates(cornerCell) != comparer)
			{
				return;
			}

			// Step 1: Get the diagonal cell of 'cornerCell' and determine
			// the existence of strong link.
			int abzCell = GetDiagonalCell(urCells, cornerCell);
			var adjacentCellsMap = otherCellsMap - abzCell;
			int[] offsets = adjacentCellsMap.ToArray();
			int abxCell = offsets[0], abyCell = offsets[1];
			foreach (var (begin, end) in stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) })
			{
				var linkMap = new Cells { begin, abzCell };
				foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
				{
					if (!IsConjugatePair(b, linkMap, linkMap.CoveredLine))
					{
						continue;
					}

					// Step 2: Get the link cell that is adjacent to 'cornerCell'
					// and check the strong link.
					var secondLinkMap = new Cells { cornerCell, begin };
					if (!IsConjugatePair(a, secondLinkMap, secondLinkMap.CoveredLine))
					{
						continue;
					}

					// Step 3: Check eliminations.
					var conclusions = new List<Conclusion>();
					if (grid.Exists(end, a) is true)
					{
						conclusions.Add(new(ConclusionType.Elimination, end, a));
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					// Step 4: Check highlight candidates.
					var candidateOffsets = new List<DrawingInfo>();
					foreach (int d in comparer)
					{
						candidateOffsets.Add(new(d == a ? 1 : 0, cornerCell * 9 + d));
					}
					foreach (int d in stackalloc[] { d1, d2 })
					{
						if (grid.Exists(abzCell, d) is true)
						{
							candidateOffsets.Add(new(d == b ? 1 : 0, abzCell * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(begin))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(new(1, begin * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(end))
					{
						if ((d == d1 || d == d2) && d != a)
						{
							candidateOffsets.Add(new(0, end * 9 + d));
						}
					}
					if (!_allowIncompleteUr && candidateOffsets.Count != 7)
					{
						continue;
					}

					var conjugatePairs = new ConjugatePair[]
					{
						new(cornerCell, begin, a),
						new(begin, abzCell, b)
					};
					accumulator.Add(
						new UrPlusStepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Cells = arMode ? GetHighlightCells(urCells) : null,
									Candidates = candidateOffsets,
									Regions = new DrawingInfo[]
									{
										new(0, conjugatePairs[0].Line),
										new(1, conjugatePairs[1].Line)
									}
								}
							},
							UrTypeCode.Plus3N2SL,
							d1,
							d2,
							urCells,
							arMode,
							conjugatePairs,
							index));
				}
			}
		}

		/// <summary>
		/// Check UR+3U/2SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="cornerCell">The corner cell.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void Check3U2SL(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index)
		{
			//   ↓ cornerCell
			// (ab )-----abx
			//        a
			//
			//        b
			//  aby -----abz
			if (grid.GetCandidates(cornerCell) != comparer)
			{
				return;
			}

			int abzCell = GetDiagonalCell(urCells, cornerCell);
			var adjacentCellsMap = otherCellsMap - abzCell;
			int[] offsets = adjacentCellsMap.ToArray();
			int abxCell = offsets[0], abyCell = offsets[1];
			foreach (var (begin, end) in stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) })
			{
				var linkMap = new Cells { begin, abzCell };
				foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
				{
					if (!IsConjugatePair(b, linkMap, linkMap.CoveredLine))
					{
						continue;
					}

					var secondLinkMap = new Cells { cornerCell, end };
					if (!IsConjugatePair(a, secondLinkMap, secondLinkMap.CoveredLine))
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					if (grid.Exists(begin, a) is true)
					{
						conclusions.Add(new(ConclusionType.Elimination, begin, a));
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<DrawingInfo>();
					foreach (int d in comparer)
					{
						candidateOffsets.Add(new(d == a ? 1 : 0, cornerCell * 9 + d));
					}
					foreach (int d in grid.GetCandidates(begin))
					{
						if ((d == d1 || d == d2) && d != a)
						{
							candidateOffsets.Add(new(1, begin * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(end))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(new(d == a ? 1 : 0, end * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(abzCell))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(new(d == b ? 1 : 0, abzCell * 9 + d));
						}
					}
					if (!_allowIncompleteUr && candidateOffsets.Count != 7)
					{
						continue;
					}

					var conjugatePairs = new ConjugatePair[]
					{
						new(cornerCell, end, a),
						new(begin, abzCell, b)
					};
					accumulator.Add(
						new UrPlusStepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Cells = arMode ? GetHighlightCells(urCells) : null,
									Candidates = candidateOffsets,
									Regions = new DrawingInfo[]
									{
										new(0, conjugatePairs[0].Line),
										new(1, conjugatePairs[1].Line)
									}
								}
							},
							UrTypeCode.Plus3U2SL,
							d1,
							d2,
							urCells,
							arMode,
							conjugatePairs,
							index));
				}
			}
		}

		/// <summary>
		/// Check UR+3E/2SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="cornerCell">The corner cell.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void Check3E2SL(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index)
		{
			//   ↓ cornerCell
			// (ab )-----abx
			//        a
			//
			//        a
			//  aby -----abz
			if (grid.GetCandidates(cornerCell) != comparer)
			{
				return;
			}

			int abzCell = GetDiagonalCell(urCells, cornerCell);
			var adjacentCellsMap = otherCellsMap - abzCell;
			int[] offsets = adjacentCellsMap.ToArray();
			int abxCell = offsets[0], abyCell = offsets[1];
			foreach (var (begin, end) in stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) })
			{
				var linkMap = new Cells { begin, abzCell };
				foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
				{
					if (!IsConjugatePair(a, linkMap, linkMap.CoveredLine))
					{
						continue;
					}

					var secondLinkMap = new Cells { cornerCell, end };
					if (!IsConjugatePair(a, secondLinkMap, secondLinkMap.CoveredLine))
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					if (grid.Exists(abzCell, b) is true)
					{
						conclusions.Add(new(ConclusionType.Elimination, abzCell, b));
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<DrawingInfo>();
					foreach (int d in comparer)
					{
						candidateOffsets.Add(new(d == a ? 1 : 0, cornerCell * 9 + d));
					}
					foreach (int d in grid.GetCandidates(begin))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(new(d == a ? 1 : 0, begin * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(end))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(new(d == a ? 1 : 0, end * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(abzCell))
					{
						if ((d == d1 || d == d2) && d != b)
						{
							candidateOffsets.Add(new(d == a ? 1 : 0, abzCell * 9 + d));
						}
					}
					if (!_allowIncompleteUr && candidateOffsets.Count != 7)
					{
						continue;
					}

					var conjugatePairs = new ConjugatePair[]
					{
						new(cornerCell, end, a),
						new(begin, abzCell, a)
					};
					accumulator.Add(
						new UrPlusStepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Cells = arMode ? GetHighlightCells(urCells) : null,
									Candidates = candidateOffsets,
									Regions = new DrawingInfo[]
									{
										new(0, conjugatePairs[0].Line),
										new(1, conjugatePairs[1].Line)
									}
								}
							},
							UrTypeCode.Plus3E2SL,
							d1,
							d2,
							urCells,
							arMode,
							conjugatePairs,
							index));
				}
			}
		}

		/// <summary>
		/// Check UR+4X/3SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void Check4X3SL(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index)
		{
			//   ↓ corner1, corner2
			// (abx)-----(aby)
			//        a    |
			//             | b
			//        a    |
			//  abz ----- abw
			var link1Map = new Cells { corner1, corner2 };
			foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
			{
				if (!IsConjugatePair(a, link1Map, link1Map.CoveredLine))
				{
					continue;
				}

				int abwCell = GetDiagonalCell(urCells, corner1);
				int abzCell = (otherCellsMap - abwCell)[0];
				foreach (var (head, begin, end, extra) in
					stackalloc[] { (corner2, corner1, abzCell, abwCell), (corner1, corner2, abwCell, abzCell) })
				{
					var link2Map = new Cells { begin, end };
					if (!IsConjugatePair(b, link2Map, link2Map.CoveredLine))
					{
						continue;
					}

					var link3Map = new Cells { end, extra };
					if (!IsConjugatePair(a, link3Map, link3Map.CoveredLine))
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					if (grid.Exists(head, b) is true)
					{
						conclusions.Add(new(ConclusionType.Elimination, head, b));
					}
					if (grid.Exists(extra, b) is true)
					{
						conclusions.Add(new(ConclusionType.Elimination, extra, b));
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<DrawingInfo>();
					foreach (int d in grid.GetCandidates(head))
					{
						if ((d == d1 || d == d2) && d != b)
						{
							candidateOffsets.Add(new(1, head * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(extra))
					{
						if ((d == d1 || d == d2) && d != b)
						{
							candidateOffsets.Add(new(1, extra * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(begin))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(new(1, begin * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(end))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(new(1, end * 9 + d));
						}
					}
					if (!_allowIncompleteUr && (candidateOffsets.Count, conclusions.Count) != (6, 2))
					{
						continue;
					}

					var conjugatePairs = new ConjugatePair[]
					{
						new(head, begin, a),
						new(begin, end, b),
						new(end, extra, a)
					};
					accumulator.Add(
						new UrPlusStepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Cells = arMode ? GetHighlightCells(urCells) : null,
									Candidates = candidateOffsets,
									Regions = new DrawingInfo[]
									{
										new(0, conjugatePairs[0].Line),
										new(1, conjugatePairs[1].Line),
										new(0, conjugatePairs[2].Line)
									}
								}
							},
							UrTypeCode.Plus4X3SL,
							d1,
							d2,
							urCells,
							arMode,
							conjugatePairs,
							index));
				}
			}
		}

		/// <summary>
		/// Check UR+4C/3SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		[SkipLocalsInit]
		partial void Check4C3SL(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index)
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
			var innerMaps = (stackalloc Cells[2]);
			var link1Map = new Cells { corner1, corner2 };
			foreach (var (a, b) in stackalloc[] { (d1, d2), (d2, d1) })
			{
				if (!IsConjugatePair(a, link1Map, link1Map.CoveredLine))
				{
					continue;
				}

				int end = GetDiagonalCell(urCells, corner1);
				int extra = (otherCellsMap - end)[0];
				foreach (var (abx, aby, abw, abz) in
					stackalloc[] { (corner2, corner1, extra, end), (corner1, corner2, end, extra) })
				{
					var link2Map = new Cells { aby, abw };
					if (!IsConjugatePair(a, link2Map, link2Map.CoveredLine))
					{
						continue;
					}

					Cells link3Map1 = new() { abw, abz }, link3Map2 = new() { abx, abz };
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
							conclusions.Add(new(ConclusionType.Elimination, aby, b));
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<DrawingInfo>();
						foreach (int d in grid.GetCandidates(abx))
						{
							if (d == d1 || d == d2)
							{
								candidateOffsets.Add(new(i == 0 ? d == a ? 1 : 0 : 1, abx * 9 + d));
							}
						}
						foreach (int d in grid.GetCandidates(abz))
						{
							if (d == d1 || d == d2)
							{
								candidateOffsets.Add(new(d == b ? 1 : 0, abz * 9 + d));
							}
						}
						foreach (int d in grid.GetCandidates(aby))
						{
							if ((d == d1 || d == d2) && d != b)
							{
								candidateOffsets.Add(new(1, aby * 9 + d));
							}
						}
						foreach (int d in grid.GetCandidates(abw))
						{
							if (d == d1 || d == d2)
							{
								candidateOffsets.Add(new(1, abw * 9 + d));
							}
						}
						if (!_allowIncompleteUr && candidateOffsets.Count != 7)
						{
							continue;
						}

						int[] offsets = linkMap.ToArray();
						var conjugatePairs = new ConjugatePair[]
						{
							new(abx, aby, a),
							new(aby, abw, a),
							new(offsets[0], offsets[1], b)
						};
						accumulator.Add(
							new UrPlusStepInfo(
								conclusions,
								new View[]
								{
									new()
									{
										Cells = arMode ? GetHighlightCells(urCells) : null,
										Candidates = candidateOffsets,
										Regions = new DrawingInfo[]
										{
											new(0, conjugatePairs[0].Line),
											new(0, conjugatePairs[1].Line),
											new(1, conjugatePairs[2].Line)
										}
									}
								},
								UrTypeCode.Plus4C3SL,
								d1,
								d2,
								urCells,
								arMode,
								conjugatePairs,
								index));
					}
				}
			}
		}

		/// <summary>
		/// Check UR-XY-Wing, UR-XYZ-Wing and UR-WXYZ-Wing.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// (<see langword="in"/> parameter) The map of other cells during the current UR searching.
		/// </param>
		/// <param name="size">The size of the wing to search.</param>
		/// <param name="index">The index.</param>
		partial void CheckWing(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap,
			int size, int index)
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
			if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
			{
				return;
			}

			if (new Cells { corner1, corner2 }.AllSetsAreInOneRegion(out int region) && region < 9)
			{
				// Subtype 1.
				int[] offsets = otherCellsMap.ToArray();
				int otherCell1 = offsets[0], otherCell2 = offsets[1];
				short mask1 = grid.GetCandidates(otherCell1);
				short mask2 = grid.GetCandidates(otherCell2);
				short mask = (short)(mask1 | mask2);

				if (mask.PopCount() != 2 + size || (mask & comparer) != comparer
					|| mask1 == comparer || mask2 == comparer)
				{
					return;
				}

				var map = (PeerMaps[otherCell1] | PeerMaps[otherCell2]) & BivalueMap;
				if (map.Count < size)
				{
					return;
				}

				var testMap = new Cells { otherCell1, otherCell2 }.PeerIntersection;
				short extraDigitsMask = (short)(mask ^ comparer);
				int[] cells = map.ToArray();
				for (int i1 = 0, length = cells.Length; i1 < length - size + 1; i1++)
				{
					int c1 = cells[i1];
					short m1 = grid.GetCandidates(c1);
					if ((m1 & ~extraDigitsMask) == 0)
					{
						continue;
					}

					for (int i2 = i1 + 1; i2 < length - size + 2; i2++)
					{
						int c2 = cells[i2];
						short m2 = grid.GetCandidates(c2);
						if ((m2 & ~extraDigitsMask) == 0)
						{
							continue;
						}

						if (size == 2)
						{
							// Check XY-Wing.
							short m = (short)((short)(m1 | m2) ^ extraDigitsMask);
							if ((m.PopCount(), (m1 & m2).PopCount()) != (1, 1))
							{
								continue;
							}

							// Now check whether all cells found should see their corresponding
							// cells in UR structure ('otherCells1' or 'otherCells2').
							bool flag = true;
							foreach (int cell in stackalloc[] { c1, c2 })
							{
								int extraDigit = (grid.GetCandidates(cell) & ~m).FindFirstSet();
								if (!(testMap & CandMaps[extraDigit]).Contains(cell))
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
							var elimMap = new Cells { c1, c2 }.PeerIntersection & CandMaps[elimDigit];
							if (elimMap.IsEmpty)
							{
								continue;
							}

							foreach (int cell in elimMap)
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, elimDigit));
							}

							var candidateOffsets = new List<DrawingInfo>();
							foreach (int cell in urCells)
							{
								if (grid.GetStatus(cell) == CellStatus.Empty)
								{
									foreach (int digit in grid.GetCandidates(cell))
									{
										candidateOffsets.Add(
											new(
												true switch
												{
													_ when digit == elimDigit => otherCellsMap.Contains(cell) ? 2 : 0,
													_ when (extraDigitsMask >> digit & 1) != 0 => 1,
													_ => 0
												}, cell * 9 + digit));
									}
								}
							}
							foreach (int digit in grid.GetCandidates(c1))
							{
								candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c1 * 9 + digit));
							}
							foreach (int digit in grid.GetCandidates(c2))
							{
								candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c2 * 9 + digit));
							}
							unsafe
							{
								if (!_allowIncompleteUr && candidateOffsets.Count(&CheckHighlightType) != 8)
								{
									continue;
								}
							}

							accumulator.Add(
								new UrWithWingStepInfo(
									conclusions,
									new View[]
									{
										new()
										{
											Cells = arMode ? GetHighlightCells(urCells) : null,
											Candidates = candidateOffsets
										}
									},
									arMode ? UrTypeCode.AXyWing : UrTypeCode.XyWing,
									d1,
									d2,
									urCells,
									arMode,
									new[] { c1, c2 },
									extraDigitsMask.GetAllSets().ToArray(),
									otherCellsMap,
									index));
						}
						else // size > 2
						{
							for (int i3 = i2 + 1; i3 < length - size + 3; i3++)
							{
								int c3 = cells[i3];
								short m3 = grid.GetCandidates(c3);
								if ((m3 & ~extraDigitsMask) == 0)
								{
									continue;
								}

								if (size == 3)
								{
									// Check XYZ-Wing.
									short m = (short)(((short)(m1 | m2) | m3) ^ extraDigitsMask);
									if ((m.PopCount(), (m1 & m2 & m3).PopCount()) != (1, 1))
									{
										continue;
									}

									// Now check whether all cells found should see their corresponding
									// cells in UR structure ('otherCells1' or 'otherCells2').
									bool flag = true;
									foreach (int cell in stackalloc[] { c1, c2, c3 })
									{
										int extraDigit = (grid.GetCandidates(cell) & ~m).FindFirstSet();
										if (!(testMap & CandMaps[extraDigit]).Contains(cell))
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
									var elimMap = new Cells { c1, c2, c3 }.PeerIntersection & CandMaps[elimDigit];
									if (elimMap.IsEmpty)
									{
										continue;
									}

									foreach (int cell in elimMap)
									{
										conclusions.Add(new(ConclusionType.Elimination, cell, elimDigit));
									}

									var candidateOffsets = new List<DrawingInfo>();
									foreach (int cell in urCells)
									{
										if (grid.GetStatus(cell) == CellStatus.Empty)
										{
											foreach (int digit in grid.GetCandidates(cell))
											{
												candidateOffsets.Add(
													new((extraDigitsMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
											}
										}
									}
									foreach (int digit in grid.GetCandidates(c1))
									{
										candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c1 * 9 + digit));
									}
									foreach (int digit in grid.GetCandidates(c2))
									{
										candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c2 * 9 + digit));
									}
									foreach (int digit in grid.GetCandidates(c3))
									{
										candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c3 * 9 + digit));
									}
									unsafe
									{
										if (!_allowIncompleteUr && candidateOffsets.Count(&CheckHighlightType) != 8)
										{
											continue;
										}
									}

									accumulator.Add(
										new UrWithWingStepInfo(
											conclusions,
											new View[]
											{
												new()
												{
													Cells = arMode ? GetHighlightCells(urCells) : null,
													Candidates = candidateOffsets
												}
											},
											arMode ? UrTypeCode.AXyzWing : UrTypeCode.XyzWing,
											d1,
											d2,
											urCells,
											arMode,
											new[] { c1, c2, c3 },
											extraDigitsMask.GetAllSets().ToArray(),
											otherCellsMap,
											index));
								}
								else // size == 4
								{
									for (int i4 = i3 + 1; i4 < length; i4++)
									{
										int c4 = cells[i4];
										short m4 = grid.GetCandidates(c4);
										if ((m4 & ~extraDigitsMask) == 0)
										{
											continue;
										}

										// Check WXYZ-Wing.
										short m = (short)((short)((short)((short)(m1 | m2) | m3) | m4) ^ extraDigitsMask);
										if ((m.PopCount(), (m1 & m2 & m3 & m4).PopCount()) != (1, 1))
										{
											continue;
										}

										// Now check whether all cells found should see their corresponding
										// cells in UR structure ('otherCells1' or 'otherCells2').
										bool flag = true;
										foreach (int cell in stackalloc[] { c1, c2, c3, c4 })
										{
											int extraDigit = (grid.GetCandidates(cell) & ~m).FindFirstSet();
											if (!(testMap & CandMaps[extraDigit]).Contains(cell))
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
										var elimMap = new Cells { c1, c2, c3, c4 }.PeerIntersection & CandMaps[elimDigit];
										if (elimMap.IsEmpty)
										{
											continue;
										}

										foreach (int cell in elimMap)
										{
											conclusions.Add(new(ConclusionType.Elimination, cell, elimDigit));
										}

										var candidateOffsets = new List<DrawingInfo>();
										foreach (int cell in urCells)
										{
											if (grid.GetStatus(cell) == CellStatus.Empty)
											{
												foreach (int digit in grid.GetCandidates(cell))
												{
													candidateOffsets.Add(
														new(
															(extraDigitsMask >> digit & 1) != 0 ? 1 : 0,
															cell * 9 + digit));
												}
											}
										}
										foreach (int digit in grid.GetCandidates(c1))
										{
											candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c1 * 9 + digit));
										}
										foreach (int digit in grid.GetCandidates(c2))
										{
											candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c2 * 9 + digit));
										}
										foreach (int digit in grid.GetCandidates(c3))
										{
											candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c3 * 9 + digit));
										}
										foreach (int digit in grid.GetCandidates(c4))
										{
											candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c4 * 9 + digit));
										}
										unsafe
										{
											if (!_allowIncompleteUr && candidateOffsets.Count(&CheckHighlightType) != 8)
											{
												continue;
											}
										}

										accumulator.Add(
											new UrWithWingStepInfo(
												conclusions,
												new View[]
												{
													new()
													{
														Cells = arMode ? GetHighlightCells(urCells) : null,
														Candidates = candidateOffsets
													}
												},
												arMode ? UrTypeCode.AWxyzWing : UrTypeCode.WxyzWing,
												d1,
												d2,
												urCells,
												arMode,
												new[] { c1, c2, c3, c4 },
												extraDigitsMask.GetAllSets().ToArray(),
												otherCellsMap,
												index));
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
