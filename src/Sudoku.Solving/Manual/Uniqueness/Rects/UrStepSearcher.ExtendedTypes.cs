using System;
using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Solving.Manual.Extensions;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

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
			if ((PopCount((uint)o), PopCount((uint)o1), PopCount((uint)o2), o1 & comparer, o2 & comparer) is
				not (4, <= 3, <= 3, not 0, not 0)
				|| (o & comparer) != comparer)
			{
				return;
			}

			short xyMask = (short)(o ^ comparer);
			int x = TrailingZeroCount(xyMask), y = xyMask.GetNextSet(x);
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

				if (IsIncompleteUr(candidateOffsets))
				{
					return;
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
						arMode ? Technique.ArPlus2D : Technique.UrPlus2D,
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

							int elimDigit = TrailingZeroCount(comparer ^ (1 << digit));
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

							if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
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
									Technique.UrPlus2B1SL,
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

							if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
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
									Technique.UrPlus2D1SL,
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

			if
			(
				(
					PopCount((uint)mask), PopCount((uint)m1), PopCount((uint)m2), PopCount((uint)m3),
					m1 & comparer, m2 & comparer, m3 & comparer
				) is not (4, <= 3, <= 3, <= 3, not 0, not 0, not 0)
				|| (mask & comparer) != comparer
			)
			{
				return;
			}

			short xyMask = (short)(mask ^ comparer);
			int x = TrailingZeroCount(xyMask), y = xyMask.GetNextSet(x);
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
				if (IsIncompleteUr(candidateOffsets))
				{
					return;
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
						arMode ? Technique.ArPlus3X : Technique.UrPlus3X,
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
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 6)
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
						Technique.UrPlus3X2SL,
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
					if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
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
							Technique.UrPlus3N2SL,
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
					if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
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
							Technique.UrPlus3U2SL,
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
					if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
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
							Technique.UrPlus3E2SL,
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
					if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Count) != (6, 2))
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
							Technique.UrPlus4X3SL,
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
						if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
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
								Technique.UrPlus4C3SL,
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

				if (PopCount((uint)mask) != 2 + size || (mask & comparer) != comparer
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
							if ((PopCount((uint)m), PopCount((uint)(m1 & m2))) != (1, 1))
							{
								continue;
							}

							// Now check whether all cells found should see their corresponding
							// cells in UR structure ('otherCells1' or 'otherCells2').
							bool flag = true;
							foreach (int cell in stackalloc[] { c1, c2 })
							{
								int extraDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~m);
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
							int elimDigit = TrailingZeroCount(m);
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
												digit == elimDigit
												? otherCellsMap.Contains(cell) ? 2 : 0
												: (extraDigitsMask >> digit & 1) != 0 ? 1 : 0,
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
							if (IsIncompleteUr(candidateOffsets))
							{
								return;
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
									arMode ? Technique.ArXyWing : Technique.UrXyWing,
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
									if ((PopCount((uint)m), PopCount((uint)(m1 & m2 & m3))) != (1, 1))
									{
										continue;
									}

									// Now check whether all cells found should see their corresponding
									// cells in UR structure ('otherCells1' or 'otherCells2').
									bool flag = true;
									foreach (int cell in stackalloc[] { c1, c2, c3 })
									{
										int extraDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~m);
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
									int elimDigit = TrailingZeroCount(m);
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
									if (IsIncompleteUr(candidateOffsets))
									{
										return;
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
											arMode ? Technique.ArXyzWing : Technique.UrXyzWing,
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
										if ((PopCount((uint)m), PopCount((uint)(m1 & m2 & m3 & m4))) != (1, 1))
										{
											continue;
										}

										// Now check whether all cells found should see their corresponding
										// cells in UR structure ('otherCells1' or 'otherCells2').
										bool flag = true;
										foreach (int cell in stackalloc[] { c1, c2, c3, c4 })
										{
											int extraDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~m);
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
										int elimDigit = TrailingZeroCount(m);
										var elimMap =
											new Cells { c1, c2, c3, c4 }.PeerIntersection & CandMaps[elimDigit];
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
										if (IsIncompleteUr(candidateOffsets))
										{
											return;
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
												arMode ? Technique.ArWxyzWing : Technique.UrWxyzWing,
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

		/// <summary>
		/// Check UR+SdC.
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
		partial void CheckSdc(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer,
			int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index)
		{
			//           |   xyz
			//  ab+ ab+  | abxyz abxyz
			//           |   xyz
			// ----------+------------
			// (ab)(ab)  |
			//  ↑ corner1, corner2
			bool notSatisfiedType3 = false;
			short mergedMaskInOtherCells = 0;
			foreach (int cell in otherCellsMap)
			{
				short currentMask = grid.GetCandidates(cell);
				mergedMaskInOtherCells |= currentMask;
				if ((currentMask & comparer) == 0
					|| currentMask == comparer || arMode && grid.GetStatus(cell) != CellStatus.Empty)
				{
					notSatisfiedType3 = true;
					break;
				}
			}

			if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer || notSatisfiedType3
				|| (mergedMaskInOtherCells & comparer) != comparer)
			{
				return;
			}

			// Check whether the corners spanned two blocks. If so, UR+SdC can't be found.
			short blockMaskInOtherCells = otherCellsMap.BlockMask;
			if (blockMaskInOtherCells == 0 || (blockMaskInOtherCells & blockMaskInOtherCells - 1) != 0)
			{
				return;
			}

			short otherDigitsMask = (short)(mergedMaskInOtherCells & ~comparer);
			byte line = (byte)otherCellsMap.CoveredLine;
			byte block = (byte)TrailingZeroCount(otherCellsMap.CoveredRegions & ~(1 << line));
			var (a, _, _, d) = IntersectionMaps[(line, block)];
			var list = new List<Cells>(4);
			foreach (bool cannibalMode in stackalloc[] { false, true })
			{
				foreach (byte otherBlock in d)
				{
					var emptyCellsInInterMap = RegionMaps[otherBlock] & RegionMaps[line] & EmptyMap;
					if (emptyCellsInInterMap.Count < 2)
					{
						// The intersection needs at least two empty cells.
						continue;
					}

					Cells b = RegionMaps[otherBlock] - RegionMaps[line], c = a & b;

					list.Clear();
					int[] offsets = emptyCellsInInterMap.ToArray();
					switch (emptyCellsInInterMap.Count)
					{
						case 2:
						{
							list.Add(new() { offsets[0], offsets[1] });

							break;
						}
						case 3:
						{
							int i = offsets[0], j = offsets[1], k = offsets[2];
							list.Add(new() { i, j });
							list.Add(new() { j, k });
							list.Add(new() { i, k });
							list.Add(new() { i, j, k });

							break;
						}
					}

					// Iterate on each intersection combination.
					foreach (var currentInterMap in list)
					{
						short selectedInterMask = 0;
						foreach (int cell in currentInterMap)
						{
							selectedInterMask |= grid.GetCandidates(cell);
						}
						if (PopCount((uint)selectedInterMask) <= currentInterMap.Count + 1)
						{
							// The intersection combination is an ALS or a normal subset,
							// which is invalid in SdCs.
							continue;
						}

						var blockMap = (b | c - currentInterMap) & EmptyMap;
						var lineMap = (a & EmptyMap) - otherCellsMap;

						// Iterate on the number of the cells that should be selected in block.
						for (int i = 1; i <= blockMap.Count - 1; i++)
						{
							// Iterate on each combination in block.
							foreach (int[] selectedCellsInBlock in blockMap.ToArray().GetSubsets(i))
							{
								bool flag = false;
								foreach (int digit in otherDigitsMask)
								{
									foreach (int cell in selectedCellsInBlock)
									{
										if (grid.Exists(cell, digit) is true)
										{
											flag = true;
											break;
										}
									}
								}
								if (flag)
								{
									continue;
								}

								var currentBlockMap = new Cells(selectedCellsInBlock);
								Cells elimMapBlock = Cells.Empty, elimMapLine = Cells.Empty;

								// Get the links of the block.
								short blockMask = 0;
								foreach (int cell in selectedCellsInBlock)
								{
									blockMask |= grid.GetCandidates(cell);
								}

								// Get the elimination map in the block.
								foreach (int digit in blockMask)
								{
									elimMapBlock |= CandMaps[digit];
								}
								elimMapBlock &= blockMap - currentBlockMap;

								foreach (int digit in otherDigitsMask)
								{
									elimMapLine |= CandMaps[digit];
								}
								elimMapLine &= lineMap - currentInterMap;

								checkGeneralizedSdc(
									accumulator, grid, arMode, cannibalMode, d1, d2, urCells,
									line, otherBlock, otherDigitsMask, blockMask, selectedInterMask,
									otherDigitsMask, elimMapLine, elimMapBlock, otherCellsMap, currentBlockMap,
									currentInterMap, i, 0, index);
							}
						}
					}
				}
			}

			static void checkGeneralizedSdc(
				IList<UrStepInfo> accumulator, in SudokuGrid grid, bool arMode, bool cannibalMode, int digit1,
				int digit2, int[] urCells, int line, int block, short lineMask, short blockMask,
				short selectedInterMask, short otherDigitsMask, in Cells elimMapLine, in Cells elimMapBlock,
				in Cells currentLineMap, in Cells currentBlockMap, in Cells currentInterMap, int i, int j,
				int index)
			{
				short maskOnlyInInter = (short)(selectedInterMask & ~(blockMask | lineMask));
				short maskIsolated = (short)(
					cannibalMode ? (lineMask & blockMask & selectedInterMask) : maskOnlyInInter
				);
				if (
					!cannibalMode && (
						(blockMask & lineMask) != 0
						|| maskIsolated != 0 && (maskIsolated == 0 || (maskIsolated & maskIsolated - 1) != 0)
					) || cannibalMode && (maskIsolated == 0 || (maskIsolated & maskIsolated - 1) != 0)
				)
				{
					return;
				}

				var elimMapIsolated = Cells.Empty;
				int digitIsolated = TrailingZeroCount(maskIsolated);
				if (digitIsolated != Constants.InvalidFirstSet)
				{
					elimMapIsolated =
						(cannibalMode ? currentBlockMap | currentLineMap : currentInterMap)
						* CandMaps[digitIsolated] & EmptyMap;
				}

				if (currentInterMap.Count + i + j + 1 ==
					PopCount((uint)blockMask) + PopCount((uint)lineMask) + PopCount((uint)maskOnlyInInter)
					&& (!elimMapBlock.IsEmpty || !elimMapLine.IsEmpty || !elimMapIsolated.IsEmpty))
				{
					// Check eliminations.
					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMapBlock)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							if ((blockMask >> digit & 1) != 0)
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, digit));
							}
						}
					}
					foreach (int cell in elimMapLine)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							if ((lineMask >> digit & 1) != 0)
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, digit));
							}
						}
					}
					foreach (int cell in elimMapIsolated)
					{
						conclusions.Add(new(ConclusionType.Elimination, cell, digitIsolated));
					}
					if (conclusions.Count == 0)
					{
						return;
					}

					// Record highlight candidates and cells.
					var candidateOffsets = new List<DrawingInfo>();
					foreach (int cell in urCells)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								new((otherDigitsMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
						}
					}
					foreach (int cell in currentBlockMap)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								new(!cannibalMode && digit == digitIsolated ? 3 : 2, cell * 9 + digit));
						}
					}
					foreach (int cell in currentInterMap)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								new(
									digitIsolated == digit ? 3 : (otherDigitsMask >> digit & 1) != 0 ? 1 : 2,
									cell * 9 + digit));
						}
					}

					accumulator.Add(
						new UrWithSdcStepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Cells = arMode ? GetHighlightCells(urCells) : null,
									Candidates = candidateOffsets,
									Regions = new DrawingInfo[] { new(0, block), new(2, line) }
								}
							},
							digit1,
							digit2,
							urCells,
							arMode,
							index,
							block,
							line,
							blockMask,
							lineMask,
							selectedInterMask,
							cannibalMode,
							maskIsolated,
							currentBlockMap,
							currentLineMap,
							currentInterMap));
				}
			}
		}

		/// <summary>
		/// Check UR+Guardian.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="index">The index.</param>
		partial void CheckGuardian(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, short comparer,
			int d1, int d2, int index)
		{
			var cells = new Cells(urCells);

			if ((grid.GetCandidates(urCells[0]) & comparer) != comparer
				|| (grid.GetCandidates(urCells[1]) & comparer) != comparer
				|| (grid.GetCandidates(urCells[2]) & comparer) != comparer
				|| (grid.GetCandidates(urCells[3]) & comparer) != comparer)
			{
				// Guardian type shouldn't be incomplete.
				return;
			}

			// Iterate on two regions used.
			foreach (int[] regionCombination in cells.Regions.GetAllSets().GetSubsets(2))
			{
				var regionCells = RegionMaps[regionCombination[0]] | RegionMaps[regionCombination[1]];
				if ((regionCells & cells) != cells)
				{
					// The regions must contain all 4 UR cells.
					continue;
				}

				var guardian1 = regionCells - cells & CandMaps[d1];
				var guardian2 = regionCells - cells & CandMaps[d2];
				if (!(guardian1.IsEmpty ^ guardian2.IsEmpty))
				{
					// Only one digit can contain guardians.
					continue;
				}

				int guardianDigit = -1;
				Cells? targetElimMap = null, targetGuardianMap = null;
				if (!guardian1.IsEmpty && (guardian1.PeerIntersection & CandMaps[d1]) is { IsEmpty: false } a)
				{
					targetElimMap = a;
					guardianDigit = d1;
					targetGuardianMap = guardian1;
				}
				else if (!guardian2.IsEmpty && (guardian2.PeerIntersection & CandMaps[d2]) is { IsEmpty: false } b)
				{
					targetElimMap = b;
					guardianDigit = d2;
					targetGuardianMap = guardian2;
				}

				if (targetElimMap is not { } elimMap || targetGuardianMap is not { } guardianMap
					|| guardianDigit == -1)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int cell in elimMap)
				{
					conclusions.Add(new(ConclusionType.Elimination, cell, d1));
				}

				var candidateOffsets = new List<DrawingInfo>();
				foreach (int cell in urCells)
				{
					candidateOffsets.Add(new(0, cell * 9 + d1));
					candidateOffsets.Add(new(0, cell * 9 + d2));
				}
				foreach (int cell in guardianMap)
				{
					candidateOffsets.Add(new(1, cell * 9 + guardianDigit));
				}

				accumulator.Add(
					new UrWithGuardianStepInfo(
						conclusions,
						new View[]
						{
							new()
							{
								Candidates = candidateOffsets,
								Regions = new DrawingInfo[]
								{
									new(0, regionCombination[0]),
									new(0, regionCombination[1])
								}
							}
						},
						d1,
						d2,
						urCells,
						guardianDigit,
						guardianMap,
						index));
			}
		}

		/// <summary>
		/// Check AR+Hidden single.
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
		partial void CheckHiddenSingleAvoidable(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer,
			int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index)
		{
			// ↓corner1
			// a   | aby  -  -
			// abx | a    -  b
			//     | -    -  -
			//       ↑corner2(cell 'a')
			// There's only one cell can be filled with the digit 'b' besides the cell 'aby'.

			if (grid.GetStatus(corner1) != CellStatus.Modifiable
				|| grid.GetStatus(corner2) != CellStatus.Modifiable
				|| grid[corner1] != grid[corner2]
				|| grid[corner1] != d1 && grid[corner1] != d2)
			{
				return;
			}

			// Get the base digit ('a') and the other digit ('b').
			// Here 'b' is the digit that we should check the possible hidden single.
			int baseDigit = grid[corner1], otherDigit = baseDigit == d1 ? d2 : d1;
			var cellsThatTwoOtherCellsBothCanSee = otherCellsMap.PeerIntersection & CandMaps[otherDigit];

			// Iterate on two cases (because we holds two other cells,
			// and both those two cells may contain possible elimination).
			for (int i = 0; i < 2; i++)
			{
				var (baseCell, anotherCell) = i == 0
					? (otherCellsMap[0], otherCellsMap[1])
					: (otherCellsMap[1], otherCellsMap[0]);

				// Iterate on each region type.
				for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
				{
					int region = baseCell.ToRegion(label);

					// If the region doesn't overlap with the specified region, just skip it.
					if ((cellsThatTwoOtherCellsBothCanSee & RegionMaps[region]).IsEmpty)
					{
						continue;
					}

					var otherCellsToCheck = RegionMaps[region] & CandMaps[otherDigit] & PeerMaps[anotherCell];
					int sameRegions = (otherCellsToCheck + anotherCell).CoveredRegions;
					foreach (int sameRegion in sameRegions)
					{
						// Check whether all possible positions of the digit 'b' in this region only
						// lies in the given cells above ('cellsThatTwoOtherCellsBothCanSee').
						if ((RegionMaps[sameRegion] - anotherCell & CandMaps[otherDigit]) != otherCellsToCheck)
						{
							continue;
						}

						// Possible hidden single found.
						// If the elimination doesn't exist, just skip it.
						if (grid.Exists(baseCell, otherDigit) is not true)
						{
							continue;
						}

						var cellOffsets = new List<DrawingInfo>();
						foreach (int cell in urCells)
						{
							cellOffsets.Add(new(0, cell));
						}

						var candidateOffsets = new List<DrawingInfo> { new(0, anotherCell * 9 + otherDigit) };
						foreach (int cell in otherCellsToCheck)
						{
							candidateOffsets.Add(new(1, cell * 9 + otherDigit));
						}

						accumulator.Add(
							new ArWithHiddenSingleStepInfo(
								new Conclusion[] { new(ConclusionType.Elimination, baseCell, otherDigit) },
								new View[]
								{
									new()
									{
										Cells = cellOffsets,
										Candidates = candidateOffsets,
										Regions = new DrawingInfo[] { new(0, sameRegion) }
									}
								},
								d1,
								d2,
								urCells,
								baseCell,
								anotherCell,
								sameRegion,
								index));
					}
				}
			}
		}
	}
}
