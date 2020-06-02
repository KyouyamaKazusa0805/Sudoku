using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static System.Algorithms;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Data.GridMap.InitializeOption;
using static Sudoku.Solving.Manual.Uniqueness.Rects.UrTypeCode;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	partial class UrTechniqueSearcher
	{
		partial void CheckType1(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, GridMap otherCellsMap)
		{
			//   ↓ cornerCell
			// (abc) ab
			//  ab   ab

			// Get the summary mask.
			short mask = 0;
			foreach (int cell in otherCellsMap)
			{
				mask |= grid.GetCandidates(cell);
			}

			if (mask != comparer)
			{
				return;
			}

			// Type 1 found. Now check elimination.
			var conclusions = new List<Conclusion>();
			if (grid.Exists(cornerCell, d1) is true)
			{
				conclusions.Add(new Conclusion(Elimination, cornerCell, d1));
			}
			if (grid.Exists(cornerCell, d2) is true)
			{
				conclusions.Add(new Conclusion(Elimination, cornerCell, d2));
			}
			if (conclusions.Count == 0)
			{
				return;
			}

			var candidateOffsets = new List<(int, int)>();
			foreach (int cell in otherCellsMap)
			{
				foreach (int digit in grid.GetCandidates(cell).GetAllSets())
				{
					candidateOffsets.Add((0, cell * 9 + digit));
				}
			}
			if (!_allowIncompletedUr && (candidateOffsets.Count != 6 || conclusions.Count != 2))
			{
				return;
			}

			accumulator.Add(
				new UrType1TechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: arMode ? GetHighlightCells(urCells) : null,
							candidateOffsets: arMode ? null : candidateOffsets,
							regionOffsets: null,
							links: null)
					},
					digit1: d1,
					digit2: d2,
					cells: urCells,
					isAr: arMode));
		}

		partial void CheckType2(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap)
		{
			//   ↓ corner1 and corner2
			// (abc) (abc)
			//  ab    ab

			// Get the summary mask.
			short mask = 0;
			foreach (int cell in otherCellsMap)
			{
				mask |= grid.GetCandidates(cell);
			}

			if (mask != comparer)
			{
				return;
			}

			int extraMask = (grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) ^ comparer;
			if (extraMask.CountSet() != 1)
			{
				return;
			}

			// Type 2 or 5 found. Now check elimination.
			int extraDigit = extraMask.FindFirstSet();
			var elimMap = new GridMap(stackalloc[] { corner1, corner2 }, ProcessPeersWithoutItself) & CandMaps[extraDigit];
			if (elimMap.IsEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int cell in elimMap)
			{
				conclusions.Add(new Conclusion(Elimination, cell, extraDigit));
			}

			var candidateOffsets = new List<(int, int)>();
			foreach (int cell in urCells)
			{
				if (grid.GetStatus(cell) == Empty)
				{
					foreach (int digit in grid.GetCandidates(cell).GetAllSets())
					{
						candidateOffsets.Add((digit == extraDigit ? 1 : 0, cell * 9 + digit));
					}
				}
			}
			if (!_allowIncompletedUr && candidateOffsets.Count(CheckHighlightType) != 8)
			{
				return;
			}

			bool isType5 = !new GridMap { corner1, corner2 }.AllSetsAreInOneRegion(out _);
			accumulator.Add(
				new UrType2TechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: arMode ? GetHighlightCells(urCells) : null,
							candidateOffsets,
							regionOffsets: null,
							links: null)
					},
					typeCode: isType5 ? Type5 : Type2,
					digit1: d1,
					digit2: d2,
					cells: urCells,
					isAr: arMode,
					extraDigit: extraDigit));
		}

		partial void CheckType3Naked(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap)
		{
			//  ↓ corner1, corner2
			// (ab ) (ab )
			//  abx   aby
			if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer
				|| otherCellsMap.Any(c =>
				{
					short mask = grid.GetCandidates(c);
					return (mask & comparer) == 0 || mask == comparer || arMode && grid.GetStatus(c) != Empty;
				}))
			{
				return;
			}

			short mask = 0;
			foreach (int cell in otherCellsMap)
			{
				mask |= grid.GetCandidates(cell);
			}
			if ((mask & comparer) != comparer)
			{
				return;
			}

			short otherDigitsMask = (short)(mask ^ comparer);
			foreach (int region in otherCellsMap.CoveredRegions)
			{
				if ((ValueMaps[d1] | ValueMaps[d2]).Overlaps(RegionMaps[region]))
				{
					return;
				}

				var iterationMap = (RegionMaps[region] & EmptyMap) - otherCellsMap;
				for (int size = otherDigitsMask.CountSet() - 1; size < iterationMap.Count; size++)
				{
					foreach (int[] iteratedCells in GetCombinationsOfArray(iterationMap.ToArray(), size))
					{
						short tempMask = 0;
						foreach (int cell in iteratedCells)
						{
							tempMask |= grid.GetCandidates(cell);
						}
						if ((tempMask & comparer) != 0 || tempMask.CountSet() - 1 != size
							|| (tempMask & otherDigitsMask) != otherDigitsMask)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int digit in tempMask.GetAllSets())
						{
							foreach (int cell in (iterationMap - new GridMap(iteratedCells)) & CandMaps[digit])
							{
								conclusions.Add(new Conclusion(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var cellOffsets = new List<(int, int)>();
						foreach (int cell in urCells)
						{
							if (grid.GetStatus(cell) != Empty)
							{
								cellOffsets.Add((0, cell));
							}
						}

						var candidateOffsets = new List<(int, int)>();
						foreach (int cell in urCells)
						{
							if (grid.GetStatus(cell) == Empty)
							{
								foreach (int digit in grid.GetCandidates(cell).GetAllSets())
								{
									candidateOffsets.Add(((tempMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
								}
							}
						}
						foreach (int cell in iteratedCells)
						{
							foreach (int digit in grid.GetCandidates(cell).GetAllSets())
							{
								candidateOffsets.Add((1, cell * 9 + digit));
							}
						}

						accumulator.Add(
							new UrType3TechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: arMode ? cellOffsets : null,
										candidateOffsets,
										regionOffsets: new[] { (0, region) },
										links: null)
								},
								digit1: d1,
								digit2: d2,
								cells: urCells,
								extraDigits: otherDigitsMask.GetAllSets().ToArray(),
								extraCells: iteratedCells,
								region,
								isNaked: true,
								isAr: arMode));
					}
				}
			}
		}

		partial void CheckType4(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap)
		{
			//  ↓ corner1, corner2
			// (ab ) ab
			//  abx  aby
			if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
			{
				return;
			}

			foreach (int region in otherCellsMap.CoveredRegions)
			{
				if (region < 9)
				{
					// Process the case in lines.
					continue;
				}

				foreach (int digit in stackalloc[] { d1, d2 })
				{
					if (!IsConjugatePair(digit, otherCellsMap, region))
					{
						continue;
					}

					// Yes, Type 4 found.
					// Now check elimination.
					int elimDigit = (comparer ^ (1 << digit)).FindFirstSet();
					var elimMap = otherCellsMap & CandMaps[elimDigit];
					if (elimMap.IsEmpty)
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMap)
					{
						conclusions.Add(new Conclusion(Elimination, cell, elimDigit));
					}

					var candidateOffsets = new List<(int, int)>();
					foreach (int cell in urCells)
					{
						if (grid.GetStatus(cell) != Empty)
						{
							continue;
						}

						if (otherCellsMap[cell])
						{
							// Cells that contain the eliminations.
							void record(int d)
							{
								if (d != elimDigit && grid.Exists(cell, d) is true)
								{
									candidateOffsets.Add((1, cell * 9 + d));
								}
							}

							record(d1);
							record(d2);
						}
						else
						{
							// Corner1 and corner2.
							foreach (int d in grid.GetCandidates(cell).GetAllSets())
							{
								candidateOffsets.Add((0, cell * 9 + d));
							}
						}
					}

					if (!_allowIncompletedUr && (candidateOffsets.Count != 6 || conclusions.Count != 2))
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
							typeCode: Type4,
							digit1: d1,
							digit2: d2,
							cells: urCells,
							conjugatePairs: new[] { new ConjugatePair(otherCellsMap.SetAt(0), otherCellsMap.SetAt(1), digit) },
							isAr: arMode));
				}
			}
		}

		partial void CheckType5(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, GridMap otherCellsMap)
		{
			//  ↓ cornerCell
			// (ab ) abc
			//  abc  abc
			if (grid.GetCandidates(cornerCell) != comparer)
			{
				return;
			}

			// Get the summary mask.
			short mask = 0;
			foreach (int cell in otherCellsMap)
			{
				mask |= grid.GetCandidates(cell);
			}

			int extraMask = mask ^ comparer;
			if (extraMask.CountSet() != 1)
			{
				return;
			}

			// Type 5 found. Now check elimination.
			int extraDigit = extraMask.FindFirstSet();
			var conclusions = new List<Conclusion>();
			var cellsThatContainsExtraDigit = otherCellsMap & CandMaps[extraDigit];
			if (cellsThatContainsExtraDigit.HasOnlyOneElement())
			{
				return;
			}

			foreach (int cell in cellsThatContainsExtraDigit.PeerIntersection & CandMaps[extraDigit])
			{
				conclusions.Add(new Conclusion(Elimination, cell, extraDigit));
			}
			if (conclusions.Count == 0)
			{
				return;
			}

			var candidateOffsets = new List<(int, int)>();
			foreach (int cell in urCells)
			{
				if (grid.GetStatus(cell) != Empty)
				{
					continue;
				}

				foreach (int digit in grid.GetCandidates(cell).GetAllSets())
				{
					candidateOffsets.Add((digit == extraDigit ? 1 : 0, cell * 9 + digit));
				}
			}
			if (!_allowIncompletedUr && candidateOffsets.Count(CheckHighlightType) != 8)
			{
				return;
			}

			accumulator.Add(
				new UrType2TechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: arMode ? GetHighlightCells(urCells) : null,
							candidateOffsets,
							regionOffsets: null,
							links: null)
					},
					typeCode: Type5,
					digit1: d1,
					digit2: d2,
					cells: urCells,
					isAr: arMode,
					extraDigit: extraDigit));
		}

		partial void CheckType6(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap)
		{
			//  ↓ corner1
			// (ab )  aby
			//  abx  (ab)
			//        ↑corner2
			if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
			{
				return;
			}

			int o1 = otherCellsMap.SetAt(0), o2 = otherCellsMap.SetAt(1);
			int r1 = GetRegion(corner1, Row), c1 = GetRegion(corner1, Column);
			int r2 = GetRegion(corner2, Row), c2 = GetRegion(corner2, Column);
			foreach (int digit in stackalloc[] { d1, d2 })
			{
				foreach (var (region1, region2) in stackalloc[] { (r1, r2), (c1, c2) })
				{
					gather(region1 >= 9 && region1 < 18, digit, region1, region2);
				}
			}

			void gather(bool isRow, int digit, int region1, int region2)
			{
				if ((!isRow
					|| !IsConjugatePair(digit, new GridMap { corner1, o1 }, region1)
					|| !IsConjugatePair(digit, new GridMap { corner2, o2 }, region2))
					&& (isRow
					|| !IsConjugatePair(digit, new GridMap { corner1, o2 }, region1)
					|| !IsConjugatePair(digit, new GridMap { corner2, o1 }, region2)))
				{
					return;
				}

				// Check eliminations.
				var elimMap = otherCellsMap & CandMaps[digit];
				if (elimMap.IsEmpty)
				{
					return;
				}

				var conclusions = new List<Conclusion>();
				foreach (int cell in elimMap)
				{
					conclusions.Add(new Conclusion(Elimination, cell, digit));
				}

				var candidateOffsets = new List<(int, int)>();
				foreach (int cell in urCells)
				{
					if (otherCellsMap[cell])
					{
						void record(int d)
						{
							if (d != digit && grid.Exists(cell, d) is true)
							{
								candidateOffsets.Add((0, cell * 9 + d));
							}
						}

						record(d1);
						record(d2);
					}
					else
					{
						foreach (int d in grid.GetCandidates(cell).GetAllSets())
						{
							candidateOffsets.Add((d == digit ? 1 : 0, cell * 9 + d));
						}
					}
				}

				if (!_allowIncompletedUr && (candidateOffsets.Count != 6 || conclusions.Count != 2))
				{
					return;
				}

				accumulator.Add(
					new UrPlusTechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: arMode ? GetHighlightCells(urCells) : null,
								candidateOffsets,
								regionOffsets: new[] { (0, region1), (0, region2) },
								links: null)
						},
						typeCode: Type6,
						digit1: d1,
						digit2: d2,
						cells: urCells,
						conjugatePairs: new[]
						{
							new ConjugatePair(corner1, isRow ? o1 : o2, digit),
							new ConjugatePair(corner2, isRow ? o2 : o1, digit)
						},
						isAr: false));
			}
		}

		partial void CheckHidden(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, GridMap otherCellsMap)
		{
			//  ↓ cornerCell
			// (ab ) abx
			//  aby  abz
			if (grid.GetCandidates(cornerCell) != comparer)
			{
				return;
			}

			int abzCell = GetDiagonalCell(urCells, cornerCell);
			var adjacentCellsMap = new GridMap(otherCellsMap) { [abzCell] = false };
			int r = GetRegion(abzCell, Row), c = GetRegion(abzCell, Column);

			foreach (int digit in stackalloc[] { d1, d2 })
			{
				int abxCell = adjacentCellsMap.SetAt(0);
				int abyCell = adjacentCellsMap.SetAt(1);
				var map1 = new GridMap { abzCell, abxCell };
				var map2 = new GridMap { abzCell, abyCell };
				if (!IsConjugatePair(digit, map1, map1.CoveredLine) || !IsConjugatePair(digit, map2, map2.CoveredLine))
				{
					continue;
				}

				// Hidden UR found. Now check eliminations.
				int elimDigit = (comparer ^ (1 << digit)).FindFirstSet();
				if (!(grid.Exists(abzCell, elimDigit) is true))
				{
					continue;
				}

				var candidateOffsets = new List<(int, int)>();
				foreach (int cell in urCells)
				{
					if (grid.GetStatus(cell) != Empty)
					{
						continue;
					}

					if (otherCellsMap[cell])
					{
						void record(int d)
						{
							if ((cell != abzCell || d != elimDigit) && grid.Exists(cell, d) is true)
							{
								candidateOffsets.Add((d != elimDigit ? 1 : 0, cell * 9 + d));
							}
						}

						record(d1);
						record(d2);
					}
					else
					{
						foreach (int d in grid.GetCandidates(cell).GetAllSets())
						{
							candidateOffsets.Add((0, cell * 9 + d));
						}
					}
				}

				if (!_allowIncompletedUr && candidateOffsets.Count != 7)
				{
					continue;
				}

				accumulator.Add(
					new HiddenUrTechniqueInfo(
						conclusions: new[] { new Conclusion(Elimination, abzCell, elimDigit) },
						views: new[]
						{
							new View(
								cellOffsets: arMode ? GetHighlightCells(urCells) : null,
								candidateOffsets,
								regionOffsets: new[] { (0, r), (0, c) },
								links: null)
						},
						digit1: d1,
						digit2: d2,
						cells: urCells,
						conjugatePairs: new[]
						{
							new ConjugatePair(abzCell, abxCell, digit),
							new ConjugatePair(abzCell, abyCell, digit),
						},
						isAr: arMode));
			}
		}
	}
}
