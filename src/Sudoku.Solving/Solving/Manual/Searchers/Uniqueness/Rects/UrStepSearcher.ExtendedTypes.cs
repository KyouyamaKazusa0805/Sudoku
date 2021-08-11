using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	partial class UrStepSearcher
	{
		/// <summary>
		/// Check UR+2D.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// The map of other cells during the current UR searching.
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
			if (
				(
					TotalNumbersCount: PopCount((uint)o),
					OtherCell1NumbersCount: PopCount((uint)o1),
					OtherCell2NumbersCount: PopCount((uint)o2),
					OtherCell1Intersetion: o1 & comparer,
					OtherCell2Intersetion: o2 & comparer
				) is not (
					TotalNumbersCount: 4,
					OtherCell1NumbersCount: <= 3,
					OtherCell2NumbersCount: <= 3,
					OtherCell1Intersetion: not 0,
					OtherCell2Intersetion: not 0
				) || (o & comparer) != comparer
			)
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
						index
					)
				);
			}
		}

		/// <summary>
		/// Check UR+2B/1SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// The map of other cells during the current UR searching.
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

							int elimCell = new Cells(otherCellsMap) { ~otherCell }[0];
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
													urCell * 9 + d1
												)
											);
										}
									}
									if (grid.Exists(urCell, d2) is true)
									{
										if (urCell != elimCell || d2 != elimDigit)
										{
											candidateOffsets.Add(
												new(
													urCell == elimCell ? 0 : (d2 == digit ? 1 : 0),
													urCell * 9 + d2
												)
											);
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
									index
								)
							);
						}
					}
				}
			}
		}

		/// <summary>
		/// Check UR+2D/1SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// The map of other cells during the current UR searching.
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

							int elimCell = new Cells(otherCellsMap) { ~otherCell }[0];
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
												urCell * 9 + d1
											)
										);
									}
									if (grid.Exists(urCell, d2) is true && (urCell != elimCell || d2 != digit))
									{
										candidateOffsets.Add(
											new(
												urCell == elimCell ? 0 : (d2 == digit ? 1 : 0),
												urCell * 9 + d2
											)
										);
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
									index
								)
							);
						}
					}
				}
			}
		}

		/// <summary>
		/// Check UR+3X.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="cornerCell">The corner cell.</param>
		/// <param name="otherCellsMap">
		/// The map of other cells during the current UR searching.
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
					TotalNumbersCount: PopCount((uint)mask),
					Cell1NumbersCount: PopCount((uint)m1),
					Cell2NumbersCount: PopCount((uint)m2),
					Cell3NumbersCount: PopCount((uint)m3),
					Cell1Intersection: m1 & comparer,
					Cell2Intersection: m2 & comparer,
					Cell3Intersection: m3 & comparer
				) is not (
					TotalNumbersCount: 4,
					Cell1NumbersCount: <= 3,
					Cell2NumbersCount: <= 3,
					Cell3NumbersCount: <= 3,
					Cell1Intersection: not 0,
					Cell2Intersection: not 0,
					Cell3Intersection: not 0
				) || (mask & comparer) != comparer
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
						index
					)
				);
			}
		}

		/// <summary>
		/// Check UR+3X/2SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="cornerCell">The corner cell.</param>
		/// <param name="otherCellsMap">
		/// The map of other cells during the current UR searching.
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
			var adjacentCellsMap = new Cells(otherCellsMap) { ~abzCell };
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
						index
					)
				);
			}
		}

		/// <summary>
		/// Check UR+3N/2SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="cornerCell">The corner cell.</param>
		/// <param name="otherCellsMap">
		/// The map of other cells during the current UR searching.
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
			var adjacentCellsMap = new Cells(otherCellsMap) { ~abzCell };
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
							index
						)
					);
				}
			}
		}

		/// <summary>
		/// Check UR+3U/2SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="cornerCell">The corner cell.</param>
		/// <param name="otherCellsMap">
		/// The map of other cells during the current UR searching.
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
			var adjacentCellsMap = new Cells(otherCellsMap) { ~abzCell };
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
							index
						)
					);
				}
			}
		}

		/// <summary>
		/// Check UR+3E/2SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="cornerCell">The corner cell.</param>
		/// <param name="otherCellsMap">
		/// The map of other cells during the current UR searching.
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
			var adjacentCellsMap = new Cells(otherCellsMap) { ~abzCell };
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
							index
						)
					);
				}
			}
		}

		/// <summary>
		/// Check UR+4X/3SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// The map of other cells during the current UR searching.
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
				int abzCell = new Cells(otherCellsMap) { ~abwCell }[0];
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
							index
						)
					);
				}
			}
		}

		/// <summary>
		/// Check UR+4C/3SL.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// The map of other cells during the current UR searching.
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
				int extra = new Cells(otherCellsMap) { ~end }[0];
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
								index
							)
						);
					}
				}
			}
		}
	}
}
