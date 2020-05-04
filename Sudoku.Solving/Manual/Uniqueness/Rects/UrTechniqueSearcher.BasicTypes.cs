using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.GridMap.InitializeOption;
using static Sudoku.GridProcessings;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Solving.Manual.Uniqueness.Rects.UrTypeCode;
using Sudoku.Data.Collections;

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
			foreach (int cell in otherCellsMap.Offsets)
			{
				mask |= grid.GetCandidatesReversal(cell);
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
			foreach (int cell in otherCellsMap.Offsets)
			{
				foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
				{
					candidateOffsets.Add((0, cell * 9 + digit));
				}
			}
			if (!_allowUncompletedUr && (candidateOffsets.Count != 6 || conclusions.Count != 2))
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
			foreach (int cell in otherCellsMap.Offsets)
			{
				mask |= grid.GetCandidatesReversal(cell);
			}

			if (mask != comparer)
			{
				return;
			}

			int extraMask = (grid.GetCandidatesReversal(corner1) | grid.GetCandidatesReversal(corner2)) ^ comparer;
			if (extraMask.CountSet() != 1)
			{
				return;
			}

			// Type 2 or 5 found. Now check elimination.
			int extraDigit = extraMask.FindFirstSet();
			var conclusions = new List<Conclusion>();
			foreach (int cell in
				new GridMap(stackalloc[] { corner1, corner2 }, ProcessPeersWithoutItself).Offsets)
			{
				if (!(grid.Exists(cell, extraDigit) is true))
				{
					continue;
				}

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

				foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
				{
					candidateOffsets.Add((digit == extraDigit ? 1 : 0, cell * 9 + digit));
				}
			}
			if (!_allowUncompletedUr && candidateOffsets.Count(CheckHighlightType) != 8)
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
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap, int size)
		{
			//  ↓ corner1, corner2
			// (ab ) (ab )
			//  abx   aby
			if ((grid.GetCandidatesReversal(corner1) | grid.GetCandidatesReversal(corner2)) != comparer
				|| otherCellsMap.Offsets.Any(c =>
				{
					short mask = grid.GetCandidatesReversal(c);
					return (mask & comparer) == 0 || mask == comparer || arMode && grid.GetStatus(c) != Empty;
				}))
			{
				return;
			}

			short mask = 0;
			foreach (int cell in otherCellsMap.Offsets)
			{
				mask |= grid.GetCandidatesReversal(cell);
			}
			if ((mask & comparer) != comparer)
			{
				return;
			}

			bool determinator(int c) => grid.GetStatus(c) == Empty && !otherCellsMap[c];
			short otherDigitsMask = (short)(mask ^ comparer);
			foreach (int region in otherCellsMap.CoveredRegions)
			{
				if (region < 9)
				{
					// Process when the region is a line.
					continue;
				}

				if (grid.HasDigitValue(d1, region) || grid.HasDigitValue(d2, region))
				{
					return;
				}

				for (int i1 = 0; i1 < 10 - size; i1++)
				{
					int c1 = RegionCells[region][i1];
					if (!determinator(c1))
					{
						continue;
					}

					if (size == 2)
					{
						// Check light naked pair.
						if (mask.CountSet() != 4)
						{
							continue;
						}

						short cellMask = grid.GetCandidatesReversal(c1);
						short m1 = (short)((short)(mask | cellMask) ^ comparer);
						if (m1.CountSet() != 2 || cellMask != otherDigitsMask)
						{
							continue;
						}

						var extraDigits = m1.GetAllSets();
						if (extraDigits.All(d => (cellMask >> d & 1) == 0))
						{
							// All extra cells should contain at least one extra digit.
							continue;
						}

						// Type 3 found. Now check eliminations.
						var conclusions = new List<Conclusion>();
						var cells = (otherCellsMap | new GridMap { c1 }).Offsets;
						foreach (int digit in extraDigits)
						{
							foreach (int cell in new GridMap(
								from cell in cells
								where grid.Exists(cell, digit) is true
								select cell, ProcessPeersWithoutItself).Offsets)
							{
								if (!(grid.Exists(cell, digit) is true))
								{
									continue;
								}

								conclusions.Add(new Conclusion(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
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

							foreach (int d in grid.GetCandidatesReversal(cell).GetAllSets())
							{
								candidateOffsets.Add((d == d1 || d == d2 ? 0 : 1, cell * 9 + d));
							}
						}
						foreach (int d in grid.GetCandidatesReversal(c1).GetAllSets())
						{
							candidateOffsets.Add((1, c1 * 9 + d));
						}

						if (!_allowUncompletedUr && candidateOffsets.Count(CheckHighlightType) != 8)
						{
							continue;
						}

						accumulator.Add(
							new UrType3TechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: arMode ? GetHighlightCells(urCells) : null,
										candidateOffsets,
										regionOffsets: null,
										links: null)
								},
								digit1: d1,
								digit2: d2,
								cells: urCells,
								extraDigits: extraDigits.ToArray(),
								extraCells: new[] { c1 },
								region: region,
								isNaked: true,
								isAr: arMode));
					}
					else
					{
						for (int i2 = i1 + 1; i2 < 11 - size; i2++)
						{
							int c2 = RegionCells[region][i2];
							if (!determinator(c2))
							{
								continue;
							}

							if (size == 3)
							{
								// Check light naked triple.
								if (mask.CountSet() != 5)
								{
									continue;
								}

								short cellMask = (short)(
									grid.GetCandidatesReversal(c1) | grid.GetCandidatesReversal(c2));
								short m2 = (short)((short)(mask | cellMask) ^ comparer);
								if (m2.CountSet() != 3 || cellMask != otherDigitsMask)
								{
									continue;
								}

								var extraDigits = m2.GetAllSets();
								if (extraDigits.All(d => (cellMask >> d & 1) == 0))
								{
									// All extra cells should contain at least one extra digit.
									continue;
								}

								// Type 3 found. Now check eliminations.
								var conclusions = new List<Conclusion>();
								var cells = (otherCellsMap | new GridMap { c1 }).Offsets;
								foreach (int digit in extraDigits)
								{
									foreach (int cell in new GridMap(
										from cell in cells
										where grid.Exists(cell, digit) is true
										select cell, ProcessPeersWithoutItself).Offsets)
									{
										if (!(grid.Exists(cell, digit) is true))
										{
											continue;
										}

										conclusions.Add(new Conclusion(Elimination, cell, digit));
									}
								}
								if (conclusions.Count == 0)
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

									foreach (int d in grid.GetCandidatesReversal(cell).GetAllSets())
									{
										candidateOffsets.Add((d == d1 || d == d2 ? 0 : 1, cell * 9 + d));
									}
								}
								foreach (int d in grid.GetCandidatesReversal(c1).GetAllSets())
								{
									candidateOffsets.Add((1, c1 * 9 + d));
								}
								foreach (int d in grid.GetCandidatesReversal(c2).GetAllSets())
								{
									candidateOffsets.Add((1, c2 * 9 + d));
								}

								if (!_allowUncompletedUr && candidateOffsets.Count(CheckHighlightType) != 8)
								{
									continue;
								}

								accumulator.Add(
									new UrType3TechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: arMode ? GetHighlightCells(urCells) : null,
												candidateOffsets,
												regionOffsets: null,
												links: null)
										},
										digit1: d1,
										digit2: d2,
										cells: urCells,
										extraDigits: extraDigits.ToArray(),
										extraCells: new[] { c1, c2 },
										region: region,
										isNaked: true,
										isAr: arMode));
							}
							else // size == 4
							{
								for (int i3 = i2 + 1; i3 < 12 - size; i3++)
								{
									int c3 = RegionCells[region][i3];
									if (!determinator(c3))
									{
										continue;
									}

									// Check light naked quadruple.
									if (mask.CountSet() != 6)
									{
										continue;
									}

									short cellMask = (short)((short)(
										grid.GetCandidatesReversal(c1)
										| grid.GetCandidatesReversal(c2))
										| grid.GetCandidatesReversal(c3));
									short m3 = (short)((short)(mask | cellMask) ^ comparer);
									if (m3.CountSet() != 4 || cellMask != otherDigitsMask)
									{
										continue;
									}

									var extraDigits = m3.GetAllSets();
									if (extraDigits.All(d => (cellMask >> d & 1) == 0))
									{
										// All extra cells should contain at least one extra digit.
										continue;
									}

									// Type 3 found. Now check eliminations.
									var conclusions = new List<Conclusion>();
									var cells = (otherCellsMap | new GridMap { c1 }).Offsets;
									foreach (int digit in extraDigits)
									{
										foreach (int cell in new GridMap(
											from cell in cells
											where grid.Exists(cell, digit) is true
											select cell, ProcessPeersWithoutItself).Offsets)
										{
											if (!(grid.Exists(cell, digit) is true))
											{
												continue;
											}

											conclusions.Add(new Conclusion(Elimination, cell, digit));
										}
									}
									if (conclusions.Count == 0)
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

										foreach (int d in grid.GetCandidatesReversal(cell).GetAllSets())
										{
											candidateOffsets.Add((d == d1 || d == d2 ? 0 : 1, cell * 9 + d));
										}
									}
									foreach (int d in grid.GetCandidatesReversal(c1).GetAllSets())
									{
										candidateOffsets.Add((1, c1 * 9 + d));
									}
									foreach (int d in grid.GetCandidatesReversal(c2).GetAllSets())
									{
										candidateOffsets.Add((1, c2 * 9 + d));
									}
									foreach (int d in grid.GetCandidatesReversal(c3).GetAllSets())
									{
										candidateOffsets.Add((1, c3 * 9 + d));
									}

									if (!_allowUncompletedUr && candidateOffsets.Count(CheckHighlightType) != 8)
									{
										continue;
									}

									accumulator.Add(
										new UrType3TechniqueInfo(
											conclusions,
											views: new[]
											{
												new View(
													cellOffsets: arMode ? GetHighlightCells(urCells) : null,
													candidateOffsets,
													regionOffsets: null,
													links: null)
											},
											digit1: d1,
											digit2: d2,
											cells: urCells,
											extraDigits: extraDigits.ToArray(),
											extraCells: new[] { c1, c2, c3 },
											region: region,
											isNaked: true,
											isAr: arMode));
								}
							}
						}
					}
				}
			}
		}

		partial void CheckType3Hidden(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap, int size)
		{
			//  ↓ corner1, corner2
			// (ab ) (ab )
			//  abx   aby
			if ((grid.GetCandidatesReversal(corner1) | grid.GetCandidatesReversal(corner2)) != comparer
				|| otherCellsMap.Offsets.Any(c =>
				{
					short mask = grid.GetCandidatesReversal(c);
					return (mask & comparer) == 0 || mask == comparer || arMode && grid.GetStatus(c) != Empty;
				}))
			{
				return;
			}

			short mask = 0;
			foreach (int cell in otherCellsMap.Offsets)
			{
				mask |= grid.GetCandidatesReversal(cell);
			}
			if ((mask & comparer) != comparer)
			{
				return;
			}

			foreach (int region in otherCellsMap.CoveredRegions)
			{
				if (region < 9)
				{
					// Process when the region is a line.
					continue;
				}

				if (grid.HasDigitValue(d1, region) || grid.HasDigitValue(d2, region))
				{
					return;
				}

				if (size == 2)
				{
					// Check hidden pair.
					var totalMap = grid.GetDigitAppearingCells(d1, region) | grid.GetDigitAppearingCells(d2, region);
					if (totalMap.Count != 3)
					{
						continue;
					}

					// Now check eliminations.
					var conclusions = new List<Conclusion>();
					foreach (int cell in (totalMap - otherCellsMap).Offsets)
					{
						foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
						{
							if (!(grid.Exists(cell, digit) is true) || digit == d1 || digit == d2)
							{
								continue;
							}

							conclusions.Add(new Conclusion(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<(int, int)>();
					foreach (int cell in (new GridMap(urCells) | totalMap).Offsets)
					{
						if (grid.GetStatus(cell) != Empty)
						{
							continue;
						}

						if (totalMap[cell])
						{
							int id = otherCellsMap[cell] ? 0 : 1;
							void record(int digit)
							{
								if (grid.Exists(cell, digit) is true)
								{
									candidateOffsets.Add((id, cell * 9 + digit));
								}
							}

							record(d1);
							record(d2);
						}
						else
						{
							foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
							{
								candidateOffsets.Add((0, cell * 9 + digit));
							}
						}
					}
					if (!_allowUncompletedUr && candidateOffsets.Count(CheckHighlightType) != 8)
					{
						continue;
					}

					accumulator.Add(
						new UrType3TechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: arMode ? GetHighlightCells(urCells) : null,
									candidateOffsets,
									regionOffsets: new[] { (0, region) },
									links: null)
							},
							digit1: d1,
							digit2: d2,
							cells: urCells,
							extraDigits: new[] { d1, d2 },
							extraCells: (totalMap - otherCellsMap).ToArray(),
							region: region,
							isNaked: false,
							isAr: arMode));
				}
				else // size > 2
				{
					// Iterate on extra digits.
					for (int ed1 = 0; ed1 < 12 - size; ed1++)
					{
						if (grid.HasDigitValue(ed1, region))
						{
							continue;
						}

						var map1 = grid.GetDigitAppearingCells(ed1, region);
						if (map1.Overlaps(otherCellsMap))
						{
							// The extra digit cannot lie on the cell 'abx' or 'aby'.
							continue;
						}

						if (size == 3)
						{
							// Check hidden triple.
							var totalMap =
								grid.GetDigitAppearingCells(d1, region)
								| grid.GetDigitAppearingCells(d2, region)
								| grid.GetDigitAppearingCells(ed1, region);
							if (totalMap.Count != 4)
							{
								continue;
							}

							// Now check eliminations.
							var conclusions = new List<Conclusion>();
							foreach (int cell in (totalMap - otherCellsMap).Offsets)
							{
								foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
								{
									if (!(grid.Exists(cell, digit) is true)
										|| digit == d1 || digit == d2 || digit == ed1)
									{
										continue;
									}

									conclusions.Add(new Conclusion(Elimination, cell, digit));
								}
							}
							if (conclusions.Count == 0)
							{
								continue;
							}

							var candidateOffsets = new List<(int, int)>();
							foreach (int cell in (new GridMap(urCells) | totalMap).Offsets)
							{
								if (grid.GetStatus(cell) != Empty)
								{
									continue;
								}

								if (totalMap[cell])
								{
									int id = otherCellsMap[cell] ? 0 : 1;
									void record(int digit)
									{
										if (grid.Exists(cell, digit) is true)
										{
											candidateOffsets.Add((id, cell * 9 + digit));
										}
									}

									record(d1);
									record(d2);
								}
								else
								{
									foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
									{
										candidateOffsets.Add((0, cell * 9 + digit));
									}
								}
							}
							if (!_allowUncompletedUr && candidateOffsets.Count(CheckHighlightType) != 8)
							{
								continue;
							}

							accumulator.Add(
								new UrType3TechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: arMode ? GetHighlightCells(urCells) : null,
											candidateOffsets,
											regionOffsets: new[] { (0, region) },
											links: null)
									},
									digit1: d1,
									digit2: d2,
									cells: urCells,
									extraDigits: new[] { d1, d2, ed1 },
									extraCells: (totalMap - otherCellsMap).ToArray(),
									region: region,
									isNaked: false,
									isAr: arMode));
						}
						else // size == 4
						{
							for (int ed2 = ed1 + 1; ed2 < 9; ed2++)
							{
								if (grid.HasDigitValue(ed2, region))
								{
									continue;
								}

								var map2 = grid.GetDigitAppearingCells(ed2, region);
								if (map2.Overlaps(otherCellsMap))
								{
									// The extra digit cannot lie on the cell 'abx' or 'aby'.
									continue;
								}

								// Check hidden quadruple.
								var totalMap =
									grid.GetDigitAppearingCells(d1, region)
									| grid.GetDigitAppearingCells(d2, region)
									| grid.GetDigitAppearingCells(ed1, region)
									| grid.GetDigitAppearingCells(ed2, region);
								if (totalMap.Count != 5)
								{
									continue;
								}

								// Now check eliminations.
								var conclusions = new List<Conclusion>();
								foreach (int cell in (totalMap - otherCellsMap).Offsets)
								{
									foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
									{
										if (!(grid.Exists(cell, digit) is true)
											|| digit == d1 || digit == d2 || digit == ed1 || digit == ed2)
										{
											continue;
										}

										conclusions.Add(new Conclusion(Elimination, cell, digit));
									}
								}
								if (conclusions.Count == 0)
								{
									continue;
								}

								var candidateOffsets = new List<(int, int)>();
								foreach (int cell in (new GridMap(urCells) | totalMap).Offsets)
								{
									if (grid.GetStatus(cell) != Empty)
									{
										continue;
									}

									if (totalMap[cell])
									{
										int id = otherCellsMap[cell] ? 0 : 1;
										void record(int digit)
										{
											if (grid.Exists(cell, digit) is true)
											{
												candidateOffsets.Add((id, cell * 9 + digit));
											}
										}

										record(d1);
										record(d2);
									}
									else
									{
										foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
										{
											candidateOffsets.Add((0, cell * 9 + digit));
										}
									}
								}
								if (!_allowUncompletedUr && candidateOffsets.Count(CheckHighlightType) != 8)
								{
									continue;
								}

								accumulator.Add(
									new UrType3TechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: arMode ? GetHighlightCells(urCells) : null,
												candidateOffsets,
												regionOffsets: new[] { (0, region) },
												links: null)
										},
										digit1: d1,
										digit2: d2,
										cells: urCells,
										extraDigits: new[] { d1, d2, ed1, ed2 },
										extraCells: (totalMap - otherCellsMap).ToArray(),
										region: region,
										isNaked: false,
										isAr: arMode));
							}
						}
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
			if ((grid.GetCandidatesReversal(corner1) | grid.GetCandidatesReversal(corner2)) != comparer)
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
					if (!IsConjugatePair(grid, digit, otherCellsMap, region))
					{
						continue;
					}

					// Yes, Type 4 found.
					// Now check elimination.
					int elimDigit = (comparer ^ (1 << digit)).FindFirstSet();
					var conclusions = new List<Conclusion>();
					foreach (int cell in otherCellsMap.Offsets)
					{
						if (!(grid.Exists(cell, elimDigit) is true))
						{
							continue;
						}

						conclusions.Add(new Conclusion(Elimination, cell, elimDigit));
					}
					if (conclusions.Count == 0)
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
							foreach (int d in grid.GetCandidatesReversal(cell).GetAllSets())
							{
								candidateOffsets.Add((0, cell * 9 + d));
							}
						}
					}

					if (!_allowUncompletedUr && (candidateOffsets.Count != 6 || conclusions.Count != 2))
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
			if (grid.GetCandidatesReversal(cornerCell) != comparer)
			{
				return;
			}

			// Get the summary mask.
			short mask = 0;
			foreach (int cell in otherCellsMap.Offsets)
			{
				mask |= grid.GetCandidatesReversal(cell);
			}

			int extraMask = mask ^ comparer;
			if (extraMask.CountSet() != 1)
			{
				return;
			}

			// Type 5 found. Now check elimination.
			int extraDigit = extraMask.FindFirstSet();
			var conclusions = new List<Conclusion>();
			var cellsThatContainsExtraDigit = from cell in otherCellsMap.Offsets
											  where grid.Exists(cell, extraDigit) is true
											  select cell;
			if (cellsThatContainsExtraDigit.HasOnlyOneElement())
			{
				return;
			}

			foreach (int cell in new GridMap(cellsThatContainsExtraDigit, ProcessPeersWithoutItself).Offsets)
			{
				if (!(grid.Exists(cell, extraDigit) is true))
				{
					continue;
				}

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

				foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
				{
					candidateOffsets.Add((digit == extraDigit ? 1 : 0, cell * 9 + digit));
				}
			}
			if (!_allowUncompletedUr && candidateOffsets.Count(CheckHighlightType) != 8)
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
			if ((grid.GetCandidatesReversal(corner1) | grid.GetCandidatesReversal(corner2)) != comparer)
			{
				return;
			}

			int o1 = otherCellsMap.SetAt(0), o2 = otherCellsMap.SetAt(1);
			var (r1, c1, _) = Cell.GetRegion(corner1);
			var (r2, c2, _) = Cell.GetRegion(corner2);
			r1 += 9; r2 += 9; c1 += 18; c2 += 18;
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
					|| !IsConjugatePair(grid, digit, new GridMap { corner1, o1 }, region1)
					|| !IsConjugatePair(grid, digit, new GridMap { corner2, o2 }, region2))
					&& (isRow
					|| !IsConjugatePair(grid, digit, new GridMap { corner1, o2 }, region1)
					|| !IsConjugatePair(grid, digit, new GridMap { corner2, o1 }, region2)))
				{
					return;
				}

				// Check eliminations.
				var conclusions = new List<Conclusion>();
				foreach (int cell in otherCellsMap.Offsets)
				{
					if (!(grid.Exists(cell, digit) is true))
					{
						continue;
					}

					conclusions.Add(new Conclusion(Elimination, cell, digit));
				}
				if (conclusions.Count == 0)
				{
					return;
				}

				var candidateOffsets = new List<(int, int)>();
				foreach (int cell in urCells)
				{
					if (otherCellsMap[cell])
					{
						void record(int d)
						{
							if (d == digit || !(grid.Exists(cell, d) is true))
							{
								return;
							}

							candidateOffsets.Add((0, cell * 9 + d));
						}

						record(d1);
						record(d2);
					}
					else
					{
						foreach (int d in grid.GetCandidatesReversal(cell).GetAllSets())
						{
							candidateOffsets.Add((d == digit ? 1 : 0, cell * 9 + d));
						}
					}
				}

				if (!_allowUncompletedUr && (candidateOffsets.Count != 6 || conclusions.Count != 2))
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
			if (grid.GetCandidatesReversal(cornerCell) != comparer)
			{
				return;
			}

			int abzCell = GetDiagonalCell(urCells, cornerCell);
			var adjacentCellsMap = new GridMap(otherCellsMap) { [abzCell] = false };
			var (r, c, _) = Cell.GetRegion(abzCell);
			r += 9; c += 18;

			foreach (int digit in stackalloc[] { d1, d2 })
			{
				int abxCell = adjacentCellsMap.SetAt(0);
				int abyCell = adjacentCellsMap.SetAt(1);
				var map1 = new GridMap { abzCell, abxCell };
				var map2 = new GridMap { abzCell, abyCell };
				if (!IsConjugatePair(grid, digit, map1, map1.CoveredLine)
					|| !IsConjugatePair(grid, digit, map2, map2.CoveredLine))
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
							if (cell == abzCell && d == elimDigit)
							{
								return;
							}

							if (grid.Exists(cell, d) is true)
							{
								candidateOffsets.Add((d != elimDigit ? 1 : 0, cell * 9 + d));
							}
						}

						record(d1);
						record(d2);
					}
					else
					{
						foreach (int d in grid.GetCandidatesReversal(cell).GetAllSets())
						{
							candidateOffsets.Add((0, cell * 9 + d));
						}
					}
				}

				if (!_allowUncompletedUr && candidateOffsets.Count != 7)
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
