using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Utils;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.GridMap.InitializeOption;
using static Sudoku.Solving.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Encapsulates an <b>unique rectangle</b> (UR) or
	/// <b>avoidable rectangle</b> (AR) technique searcher.
	/// </summary>
	[TechniqueDisplay("Unique Rectangle")]
	public sealed class UrTechniqueSearcher : RectangleTechniqueSearcher
	{
		/// <summary>
		/// Indicates whether the UR can be uncompleted. In other words,
		/// some of UR candidates can be removed before the pattern forms.
		/// </summary>
		private readonly bool _allowUncompletedUr;

		/// <summary>
		/// Indicates whether the searcher can search for extended URs.
		/// </summary>
		private readonly bool _searchExtended;


		/// <summary>
		/// Initializes an instance with the specified value indicating
		/// whether the structure can be uncompleted, and a value indicating
		/// whether the searcher can search for extended URs.
		/// </summary>
		/// <param name="allowUncompletedUr">
		/// A <see cref="bool"/> value indicating that.
		/// </param>
		/// <param name="searchExtended">A <see cref="bool"/> value indicating that.</param>
		public UrTechniqueSearcher(bool allowUncompletedUr, bool searchExtended) =>
			(_allowUncompletedUr, _searchExtended) = (allowUncompletedUr, searchExtended);


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 45;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			// Iterate on mode (whether use AR or UR mode to search).
			var tempList = new List<UrTechniqueInfo>();
			foreach (bool arMode in stackalloc[] { false, true })
			{
				// Iterate on each possible UR structure.
				foreach (int[] urCells in UrCellsList)
				{
					// Check preconditions.
					if (!checkPreconditions(grid, urCells, arMode))
					{
						continue;
					}

					// Get all candidates that all four cells appeared.
					short mask = 0;
					foreach (int urCell in urCells)
					{
						mask |= grid.GetCandidatesReversal(urCell);
					}

					// Iterate on each possible digit combination.
					int[] allDigitsInThem = mask.GetAllSets().ToArray();
					for (int i = 0, length = allDigitsInThem.Length; i < length - 1; i++)
					{
						int d1 = allDigitsInThem[i];
						for (int j = i + 1; j < length; j++)
						{
							int d2 = allDigitsInThem[j];

							// All possible UR patterns should contain at least one cell
							// that contains both 'd1' and 'd2'.
							short comparer = (short)(1 << d1 | 1 << d2);
							if (!arMode && urCells.All(c => (grid.GetCandidatesReversal(c) & comparer).CountSet() != 2))
							{
								continue;
							}

							// Iterate on each corner of four cells.
							for (int c1 = 0; c1 < 4; c1++)
							{
								int corner1 = urCells[c1];
								var otherCellsMap = new GridMap(urCells) { [corner1] = false };

								CheckType1(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap);
								CheckType5(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap);
								CheckHidden(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap);

								if (c1 == 3)
								{
									break;
								}

								for (int c2 = c1 + 1; c2 < 4; c2++)
								{
									int corner2 = urCells[c2];
									var tempOtherCellsMap = new GridMap(otherCellsMap) { [corner2] = false };

									CheckType2(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap);

									if (!arMode)
									{
										// Only UR mode searching.
										for (int size = 2; size <= 4; size++)
										{
											CheckType3Naked(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, size);
											CheckType3Hidden(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, size);
										}

										CheckType4(tempList, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap);

										if (c1 == 0 && c2 == 3 || c1 == 1 && c2 == 2)
										{
											// Diagonal type.
											CheckType6(tempList, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap);
										}
									}
								}
							}
						}
					}
				}
			}

			// Sort if worth.
			if (tempList.Count != 0)
			{
				tempList.Sort();
				accumulator.AddRange(tempList);
			}

			static bool checkPreconditions(IReadOnlyGrid grid, IEnumerable<int> urCells, bool arMode)
			{
				byte emptyCountWhenArMode = 0, modifiableCount = 0;
				foreach (int urCell in urCells)
				{
					switch (grid.GetCellStatus(urCell))
					{
						case Given:
						case Modifiable when !arMode:
						{
							return false;
						}
						case Empty when arMode:
						{
							emptyCountWhenArMode++;
							break;
						}
						case Modifiable:
						{
							modifiableCount++;
							break;
						}
					}
				}

				return modifiableCount != 4 && emptyCountWhenArMode != 4;
			}
		}

		private void CheckType1(
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
			if (!_allowUncompletedUr && candidateOffsets.Count != 6)
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
					typeName: "Type 1",
					typeCode: 1,
					digit1: d1,
					digit2: d2,
					cells: urCells,
					isAr: arMode));
		}

		private void CheckType2(
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
				if (grid.GetCellStatus(cell) != Empty)
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

			bool isType5 = !new GridMap(stackalloc[] { corner1, corner2 }).AllSetsAreInOneRegion(out _);
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
					typeName: $"Type {(isType5 ? "5" : "2")}",
					typeCode: isType5 ? 5 : 2,
					digit1: d1,
					digit2: d2,
					cells: urCells,
					isAr: arMode,
					extraDigit));
		}

		private void CheckType3Naked(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap, int size)
		{
			//  ↓ corner1, corner2
			// (ab ) (ab )
			//  abx   aby
			if ((grid.GetCandidatesReversal(corner1) | grid.GetCandidatesReversal(corner2)) != comparer)
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
				// To ensure 'abx' and 'aby' contains both number a and b.
				return;
			}

			bool determinator(int c) => grid.GetCellStatus(c) == Empty && !otherCellsMap[c];
			foreach (int region in otherCellsMap.CoveredRegions)
			{
				if (region < 9)
				{
					// Process when the region is a line.
					continue;
				}

				for (int i1 = 0; i1 < 10 - size; i1++)
				{
					int c1 = RegionUtils.GetCellOffset(region, i1);
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
						if (m1.CountSet() != 2)
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
						var cells = (otherCellsMap | new GridMap(stackalloc[] { c1 })).Offsets;
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
							if (grid.GetCellStatus(cell) != Empty)
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
								typeName: "Type 3",
								typeCode: 3,
								digit1: d1,
								digit2: d2,
								cells: urCells,
								extraDigits: extraDigits.ToArray(),
								extraCells: new[] { c1 },
								region,
								isNaked: true,
								isAr: arMode));
					}
					else
					{
						for (int i2 = i1 + 1; i2 < 11 - size; i2++)
						{
							int c2 = RegionUtils.GetCellOffset(region, i2);
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
								if (m2.CountSet() != 3)
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
								var cells = (otherCellsMap | new GridMap(stackalloc[] { c1 })).Offsets;
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
									if (grid.GetCellStatus(cell) != Empty)
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
										typeName: "Type 3",
										typeCode: 3,
										digit1: d1,
										digit2: d2,
										cells: urCells,
										extraDigits: extraDigits.ToArray(),
										extraCells: new[] { c1, c2 },
										region,
										isNaked: true,
										isAr: arMode));
							}
							else // size == 4
							{
								for (int i3 = i2 + 1; i3 < 12 - size; i3++)
								{
									int c3 = RegionUtils.GetCellOffset(region, i3);
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
									short m2 = (short)((short)(mask | cellMask) ^ comparer);
									if (m2.CountSet() != 4)
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
									var cells = (otherCellsMap | new GridMap(stackalloc[] { c1 })).Offsets;
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
										if (grid.GetCellStatus(cell) != Empty)
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
											typeName: "Type 3",
											typeCode: 3,
											digit1: d1,
											digit2: d2,
											cells: urCells,
											extraDigits: extraDigits.ToArray(),
											extraCells: new[] { c1, c2, c3 },
											region,
											isNaked: true,
											isAr: arMode));
								}
							}
						}
					}
				}
			}
		}

		private void CheckType3Hidden(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap, int size)
		{
			//  ↓ corner1, corner2
			// (ab ) (ab )
			//  abx   aby
			if ((grid.GetCandidatesReversal(corner1) | grid.GetCandidatesReversal(corner2)) != comparer)
			{
				return;
			}

			short tempMask = 0;
			foreach (int cell in otherCellsMap.Offsets)
			{
				tempMask |= grid.GetCandidatesReversal(cell);
			}
			if ((tempMask & comparer) != comparer)
			{
				// To ensure 'abx' and 'aby' contains both number a and b.
				return;
			}

			foreach (int region in otherCellsMap.CoveredRegions)
			{
				if (region < 9)
				{
					// Process when the region is a line.
					continue;
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
						if (grid.GetCellStatus(cell) != Empty)
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
							typeName: "Type 3",
							typeCode: 3,
							digit1: d1,
							digit2: d2,
							cells: urCells,
							extraDigits: new[] { d1, d2 },
							extraCells: (totalMap - otherCellsMap).ToArray(),
							region,
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
								if (grid.GetCellStatus(cell) != Empty)
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
									typeName: "Type 3",
									typeCode: 3,
									digit1: d1,
									digit2: d2,
									cells: urCells,
									extraDigits: new[] { d1, d2, ed1 },
									extraCells: (totalMap - otherCellsMap).ToArray(),
									region,
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
									if (grid.GetCellStatus(cell) != Empty)
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
										typeName: "Type 3",
										typeCode: 3,
										digit1: d1,
										digit2: d2,
										cells: urCells,
										extraDigits: new[] { d1, d2, ed1, ed2 },
										extraCells: (totalMap - otherCellsMap).ToArray(),
										region,
										isNaked: false,
										isAr: arMode));
							}
						}
					}
				}
			}
		}

		private void CheckType4(
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
						if (grid.GetCellStatus(cell) != Empty)
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
									regionOffsets: new[] { (0, region) },
									links: null)
							},
							typeName: "Type 4",
							typeCode: 4,
							digit1: d1,
							digit2: d2,
							cells: urCells,
							conjugatePairs: new[] { new ConjugatePair(otherCellsMap.SetAt(0), otherCellsMap.SetAt(1), digit) },
							isAr: arMode));
				}
			}
		}

		private void CheckType5(
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
			foreach (int cell in new GridMap(otherCellsMap.Offsets, ProcessPeersWithoutItself).Offsets)
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
				if (grid.GetCellStatus(cell) != Empty)
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
					typeName: "Type 5",
					typeCode: 5,
					digit1: d1,
					digit2: d2,
					cells: urCells,
					isAr: arMode,
					extraDigit));
		}

		private void CheckType6(
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
			var (r1, c1, _) = CellUtils.GetRegion(corner1);
			var (r2, c2, _) = CellUtils.GetRegion(corner2);
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
					|| !IsConjugatePair(grid, digit, new GridMap(stackalloc[] { corner1, o1 }), region1)
					|| !IsConjugatePair(grid, digit, new GridMap(stackalloc[] { corner2, o2 }), region2))
					&& (isRow
					|| !IsConjugatePair(grid, digit, new GridMap(stackalloc[] { corner1, o2 }), region1)
					|| !IsConjugatePair(grid, digit, new GridMap(stackalloc[] { corner2, o1 }), region2)))
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

				if (!_allowUncompletedUr && candidateOffsets.Count != 6)
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
						typeName: "Type 6",
						typeCode: 6,
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

		private void CheckHidden(
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
			var (r, c, _) = CellUtils.GetRegion(abzCell);
			r += 9; c += 18;

			foreach (int digit in stackalloc[] { d1, d2 })
			{
				int abxCell = adjacentCellsMap.SetAt(0);
				int abyCell = adjacentCellsMap.SetAt(1);
				var map1 = new GridMap(stackalloc[] { abzCell, abxCell });
				var map2 = new GridMap(stackalloc[] { abzCell, abyCell });
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
					if (grid.GetCellStatus(cell) != Empty)
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
						typeName: string.Empty,
						typeCode: 7,
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


		/// <summary>
		/// To determine whether the specified region forms a conjugate pair
		/// of the specified digit, and the cells where they contain the digit
		/// is same as the given map contains.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="map">The map.</param>
		/// <param name="region">The region.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsConjugatePair(
			IReadOnlyGrid grid, int digit, GridMap map, int region) =>
			grid.GetDigitAppearingCells(digit, region) == map;

		/// <summary>
		/// Check highlight type.
		/// </summary>
		/// <param name="pair">The pair.</param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool CheckHighlightType((int _id, int) pair) => pair._id == 0;

		/// <summary>
		/// Get a cell that cannot see each other.
		/// </summary>
		/// <param name="urCells">The UR cells.</param>
		/// <param name="cell">The current cell.</param>
		/// <returns>The diagonal cell.</returns>
		/// <exception cref="ArgumentException">
		/// Throws when the specified argument <paramref name="cell"/> is invalid.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetDiagonalCell(int[] urCells, int cell)
		{
			return true switch
			{
				_ when cell == urCells[0] => urCells[3],
				_ when cell == urCells[1] => urCells[2],
				_ when cell == urCells[2] => urCells[1],
				_ when cell == urCells[3] => urCells[0],
				_ => throw new ArgumentException("The cell is invalid.", nameof(cell))
			};
		}

		/// <summary>
		/// Get all highlight cells.
		/// </summary>
		/// <param name="urCells">The all UR cells used.</param>
		/// <returns>The list of highlight cells.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static IReadOnlyList<(int, int)> GetHighlightCells(int[] urCells) =>
			new List<(int, int)>(from cell in urCells select (0, cell));
	}
}
