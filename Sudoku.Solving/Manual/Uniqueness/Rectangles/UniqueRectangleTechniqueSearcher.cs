using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using ArType1 = Sudoku.Solving.Manual.Uniqueness.Rectangles.AvoidableRectangleType1DetailData;
using ArType2 = Sudoku.Solving.Manual.Uniqueness.Rectangles.AvoidableRectangleType2DetailData;
using ArType3 = Sudoku.Solving.Manual.Uniqueness.Rectangles.AvoidableRectangleType3DetailData;
using UrType1 = Sudoku.Solving.Manual.Uniqueness.Rectangles.UniqueRectangleType1DetailData;
using UrType2Or5 = Sudoku.Solving.Manual.Uniqueness.Rectangles.UniqueRectangleType2DetailData;
using UrType3 = Sudoku.Solving.Manual.Uniqueness.Rectangles.UniqueRectangleType3DetailData;
using UrType4 = Sudoku.Solving.Manual.Uniqueness.Rectangles.UniqueRectangleType4DetailData;
using UrType6 = Sudoku.Solving.Manual.Uniqueness.Rectangles.UniqueRectangleType6DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Encapsulates a unique rectangle technique searcher.
	/// </summary>
	public sealed partial class UniqueRectangleTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// Indicates whether the solver should check incompleted URs.
		/// </summary>
		private readonly bool _checkIncompleted;


		/// <summary>
		/// Initializes an instance with the checking option.
		/// </summary>
		/// <param name="checkIncompletedUniquenessPatterns">
		/// Indicates whether the solver should check incompleted URs.
		/// </param>
		public UniqueRectangleTechniqueSearcher(bool checkIncompletedUniquenessPatterns) =>
			_checkIncompleted = checkIncompletedUniquenessPatterns;


		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<RectangleTechniqueInfo>();

			foreach (bool urMode in new[] { true, false })
			{
				foreach (int[] cells in TraversingTable)
				{
					if (urMode && cells.Any(c => grid.GetCellStatus(c) != CellStatus.Empty)
						|| !urMode && cells.Any(c => grid.GetCellStatus(c) == CellStatus.Given))
					{
						continue;
					}

					// Initalize the data.
					int[][] cellTriplets = new int[4][]
					{
						new[] { cells[1], cells[2], cells[3] }, // 0
						new[] { cells[0], cells[2], cells[3] }, // 1
						new[] { cells[0], cells[1], cells[3] }, // 2
						new[] { cells[0], cells[1], cells[2] }, // 3
					};
					int[][] cellPairs = new int[6][]
					{
						new[] { cells[2], cells[3] }, // 0, 1
						new[] { cells[1], cells[3] }, // 0, 2
						new[] { cells[1], cells[2] }, // 0, 3
						new[] { cells[0], cells[3] }, // 1, 2
						new[] { cells[0], cells[2] }, // 1, 3
						new[] { cells[0], cells[1] }, // 2, 3
					};

					CheckType15AndHidden(result, grid, cells, cellTriplets, urMode);
					CheckType23456(result, grid, cells, cellPairs, urMode);
				}
			}
			
			return result;
		}

		#region Unique rectangle utils
		/// <summary>
		/// Check basic type and generalized locked candidates type.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="cells">All UR cells.</param>
		/// <param name="cellTriplets">Cell triplets to use.</param>
		/// <param name="urMode">
		/// Indicates whether the current searching is for UR. <see langword="true"/>
		/// is for UR, <see langword="false"/> is for AR.
		/// </param>
		private void CheckType15AndHidden(
			IList<RectangleTechniqueInfo> result, Grid grid,
			int[] cells, int[][] cellTriplets, bool urMode)
		{
			// Traverse on 'cellTriplets'.
			for (int i = 0; i < 4; i++)
			{
				int[] cellTriplet = cellTriplets[i];
				short totalMask = 511;
				foreach (int cell in cellTriplet)
				{
					totalMask &= grid.GetMask(cell);
				}

				// The index is 'i', which also represents the index of the extra cell.
				int extraCell = cells[i];

				// Check all different value kinds are no more than 3.
				int totalMaskCount = totalMask.CountSet();
				if (totalMaskCount == 6)
				{
					// Pattern found:
					// abc abc
					// abc ab+
					// Now check the last cell has only two candidates and
					// they should be 'a' and 'b'.
					short extraCellMask = grid.GetCandidates(extraCell);
					short finalMask = (short)(totalMask & extraCellMask);
					if (extraCellMask.CountSet() == 7 && finalMask.CountSet() == 6)
					{
						// The extra cell is a bivalue cell and the final mask
						// has 2 different digits, which means the pattern should
						// be this:
						// abc abc
						// abc ab
						// Therefore, type 5 found.
						if (!urMode && (
							grid.GetCellStatus(extraCell) == CellStatus.Empty
							|| cellTriplet.Any(cell => grid.GetCellStatus(cell) != CellStatus.Empty)))
						{
							continue;
						}

						// Record all highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						short cellInTripletMask = grid.GetCandidatesReversal(cellTriplet[0]);
						var digits = (~extraCellMask & 511).GetAllSets();
						int? extraDigit = cellInTripletMask.GetAllSets()
							.FirstOrDefault(i => !digits.Contains(i));

						if (extraDigit is null)
						{
							continue;
						}

						int extraDigitReal = (int)extraDigit;
						foreach (int cell in cells)
						{
							if (urMode
								|| !urMode && grid.GetCellStatus(cell) == CellStatus.Empty)
							{
								foreach (int digit in digits)
								{
									if (grid.CandidateExists(cell, digit))
									{
										candidateOffsets.Add((0, cell * 9 + digit));
									}
								}
							}

							if (grid.CandidateExists(cell, extraDigitReal))
							{
								candidateOffsets.Add((1, cell * 9 + extraDigitReal));
							}
						}

						// Record all eliminations.
						var conclusions = new List<Conclusion>();
						var elimMap = new GridMap(cellTriplet[0])
							& new GridMap(cellTriplet[1])
							& new GridMap(cellTriplet[2]);
						foreach (int cell in elimMap.Offsets)
						{
							if (grid.CandidateExists(cell, extraDigitReal))
							{
								conclusions.Add(
									new Conclusion(
										ConclusionType.Elimination, cell * 9 + extraDigitReal));
							}
						}

						// Check if worth.
						if (conclusions.Count == 0
							|| urMode && !_checkIncompleted && candidateOffsets.Count != 11)
						{
							continue;
						}

						// Type 5.
						result.Add(urMode
							? (RectangleTechniqueInfo)new UniqueRectangleTechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: null,
										linkMasks: null)
								},
								detailData: new UrType2Or5(cells, digits.ToArray(), extraDigitReal, true))
							: new AvoidableRectangleTechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets:
											new List<(int, int)>(
												from cell in cells
												select (0, cell)),
										candidateOffsets,
										regionOffsets: null,
										linkMasks: null)
								},
								detailData: new ArType2(cells, digits.ToArray(), extraDigitReal)));
					}
				}
				else if (totalMaskCount == 7)
				{
					// Pattern found:
					// ab ab
					// ab ab+
					if (!urMode && cellTriplet.Any(cell => grid.GetCellStatus(cell) == CellStatus.Empty))
					{
						continue;
					}

					// Record all highlight candidates.
					var candidateOffsets = new List<(int, int)>();
					var digits = grid.GetCandidatesReversal(cellTriplet[0]).GetAllSets();
					if (urMode)
					{
						foreach (int cell in cellTriplet)
						{
							foreach (int digit in digits)
							{
								if (grid.CandidateExists(cell, digit))
								{
									candidateOffsets.Add((0, cell * 9 + digit));
								}
							}
						}
					}

					// Record all eliminations.
					var conclusions = new List<Conclusion>();
					foreach (int digit in grid.GetCandidatesReversal(extraCell).GetAllSets())
					{
						if (grid.CandidateExists(extraCell, digit) && digits.Contains(digit))
						{
							conclusions.Add(
								new Conclusion(
									ConclusionType.Elimination, extraCell * 9 + digit));
						}
					}

					// Check the number of candidates and eliminations.
					int elimCount = conclusions.Count;
					if (elimCount == 0
						|| urMode && !_checkIncompleted && (candidateOffsets.Count != 6 || elimCount != 2))
					{
						continue;
					}

					// Type 1.
					result.Add(urMode
						? (RectangleTechniqueInfo)new UniqueRectangleTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: null,
									linkMasks: null)
							},
							detailData: new UrType1(cells, digits.ToArray()))
						: new AvoidableRectangleTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets:
										new List<(int, int)>(
											from cell in cells
											select (0, cell)),
									candidateOffsets: null,
									regionOffsets: null,
									linkMasks: null)
							},
							detailData: new ArType1(cells, digits.ToArray())));
				}

				// Check hidden rectangle.
				var span = i switch
				{
					0 => stackalloc[] { cells[1], cells[3], cells[2], cells[3] },
					1 => stackalloc[] { cells[0], cells[2], cells[2], cells[3] },
					2 => stackalloc[] { cells[0], cells[1], cells[0], cells[2] },
					3 => stackalloc[] { cells[0], cells[1], cells[1], cells[3] },
					_ => throw new Exception("Impossible case.")
				};
				CellUtils.IsSameRegion(span[0], span[1], out int[] regions1);
				CellUtils.IsSameRegion(span[2], span[3], out int[] regions2);
				int elimCell = cells[3 - i];
				static bool predicate(int region) => region >= 9;
				CheckHiddenRectangle(
					result, grid, new[] { regions1.First(predicate), regions2.First(predicate) },
					span, elimCell, cellTriplet, extraCell, cells, urMode);
			}
		}

		/// <summary>
		/// Check hidden rectangle.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="regions">All regions used.</param>
		/// <param name="conjugatePairsSeries">All conjugate pairs used.</param>
		/// <param name="elimCell">The cell whose candidate will be eliminated.</param>
		/// <param name="cellTriple">Cell triple.</param>
		/// <param name="extraCell">The extra cell.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="urMode">
		/// Indicates whether the current searching is for UR. <see langword="true"/>
		/// is for UR, <see langword="false"/> is for AR.
		/// </param>
		private void CheckHiddenRectangle(
			IList<RectangleTechniqueInfo> result, Grid grid, int[] regions,
			Span<int> conjugatePairsSeries, int elimCell, int[] cellTriple,
			int extraCell, int[] cells, bool urMode)
		{
			for (int digit = 0; digit < 9; digit++)
			{
				if (!urMode && (
					grid.GetCellStatus(extraCell) == CellStatus.Empty
					|| cellTriple.Any(cell => grid.GetCellStatus(cell) != CellStatus.Empty)))
				{
					continue;
				}

				short mask1 = grid.GetDigitAppearingMask(digit, regions[0]);
				short mask2 = grid.GetDigitAppearingMask(digit, regions[1]);
				if (mask1 == 0 || mask2 == 0)
				{
					continue;
				}

				var list = new HashSet<int>();
				for (int i = 0, temp = mask1, region = regions[0]; i < 9; i++, temp >>= 1)
				{
					if ((temp & 1) != 0)
					{
						list.Add(RegionUtils.GetCellOffset(region, i));
					}
				}
				for (int i = 0, temp = mask2, region = regions[1]; i < 9; i++, temp >>= 1)
				{
					if ((temp & 1) != 0)
					{
						list.Add(RegionUtils.GetCellOffset(region, i));
					}
				}

				if (list.Count == 3 && list.All(c => cells.Contains(c) && c != extraCell))
				{
					short bivalueMask = grid.GetCandidates(extraCell);
					if (bivalueMask.CountSet() == 7 && (bivalueMask >> digit & 1) == 0)
					{
						// Hidden rectangle found.
						// Get elimination digit.
						int elimDigit = (~(bivalueMask | (short)(1 << digit))).FindFirstSet();
						int[] digits = new[] { digit, elimDigit };

						// Record all highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						foreach (int cell in cellTriple)
						{
							if (grid.CandidateExists(cell, digit))
							{
								candidateOffsets.Add((1, cell * 9 + digit));
							}
							if (grid.CandidateExists(cell, elimDigit))
							{
								candidateOffsets.Add((0, cell * 9 + elimDigit));
							}
						}
						foreach (int d in digits)
						{
							if (grid.CandidateExists(extraCell, d))
							{
								candidateOffsets.Add((0, extraCell * 9 + d));
							}
						}
						
						// Record all eliminations.
						var conclusions = new List<Conclusion>();
						if (grid.CandidateExists(elimCell, elimDigit))
						{
							conclusions.Add(
								new Conclusion(ConclusionType.Elimination, elimCell * 9 + elimDigit));
						}

						if (conclusions.Count == 0
							|| urMode && !_checkIncompleted && candidateOffsets.Count != 8)
						{
							continue;
						}

						// Hidden rectangle.
						result.Add(urMode
							? (RectangleTechniqueInfo)new HiddenRectangleTechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets:
											new List<(int, int)>(
												from r in regions select (0, r)),
										linkMasks: null)
								},
								cells,
								digits,
								conjugatePairs: new[]
								{
									new ConjugatePair(conjugatePairsSeries[0], conjugatePairsSeries[1], digit),
									new ConjugatePair(conjugatePairsSeries[2], conjugatePairsSeries[3], digit)
								})
							: new HiddenAvoidableRectangleTechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets:
											new List<(int, int)>(
												from cell in cells select (0, cell)),
										candidateOffsets,
										regionOffsets:
											new List<(int, int)>(
												from r in regions select (0, r)),
										linkMasks: null)
								},
								cells,
								digits,
								conjugatePairs: new[]
								{
									new ConjugatePair(conjugatePairsSeries[0], conjugatePairsSeries[1], digit),
									new ConjugatePair(conjugatePairsSeries[2], conjugatePairsSeries[3], digit)
								}));
					}
				}
			}
		}

		/// <summary>
		/// Check type 2 to 6.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="cellPairs">Cell pairs.</param>
		/// <param name="urMode">
		/// Indicates whether the current searching is for UR. <see langword="true"/>
		/// is for UR, <see langword="false"/> is for AR.
		/// </param>
		private void CheckType23456(
			IList<RectangleTechniqueInfo> result, Grid grid,
			int[] cells, int[][] cellPairs, bool urMode)
		{
			// Traverse on 'cellPairs'.
			for (int i = 0; i < 6; i++)
			{
				int[] cellPair = cellPairs[i];
				short cellPairMask = 511;
				foreach (int cell in cellPair)
				{
					cellPairMask &= grid.GetMask(cell);
				}

				if (cellPairMask.CountSet() != 7)
				{
					continue;
				}

				// Pattern found:
				// ab ab
				// ?  ?
				// or pattern:
				// ab ?
				// ?  ab
				int[] extraCells = i switch
				{
					0 => new[] { cells[0], cells[1] },
					1 => new[] { cells[0], cells[2] },
					2 => new[] { cells[0], cells[3] }, // Diagnoal type.
					3 => new[] { cells[1], cells[2] }, // Diagnoal type.
					4 => new[] { cells[1], cells[3] },
					5 => new[] { cells[2], cells[3] },
					_ => throw new Exception("Impossible case.")
				};

				short extraCellMask = 511;
				foreach (int cell in extraCells)
				{
					extraCellMask &= grid.GetMask(cell);
				}
				short totalMask = (short)(extraCellMask & cellPairMask);
				var digits = grid.GetCandidatesReversal(cellPair[0]).GetAllSets();

				if (totalMask.CountSet() == 6)
				{
					if (!urMode && cellPair.Any(cell => grid.GetCellStatus(cell) == CellStatus.Empty))
					{
						continue;
					}

					// Type 2 / 5 found.
					// Record all highlight candidates.
					var candidateOffsets = new List<(int, int)>();
					foreach (int cell in cellPair)
					{
						foreach (int digit in digits)
						{
							if (urMode
								|| !urMode && grid.GetCellStatus(cell) == CellStatus.Empty)
							{
								if (grid.CandidateExists(cell, digit))
								{
									candidateOffsets.Add((0, cell * 9 + digit));
								}
							}
						}
					}
					
					foreach (int cell in extraCells)
					{
						foreach (int digit in digits)
						{
							if (grid.CandidateExists(cell, digit))
							{
								candidateOffsets.Add((1, cell * 9 + digit));
							}
						}
					}

					// Check whether elimination cells exist.
					var (a, b) = (extraCells[0], extraCells[1]);
					var elimMap = new GridMap(a, false) & new GridMap(b, false);
					if (elimMap.Count == 0)
					{
						continue;
					}

					// Record all eliminations.
					int extraDigit = (~totalMask & 511).GetAllSets().First(i => !digits.Contains(i));
					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMap.Offsets)
					{
						if (grid.CandidateExists(cell, extraDigit))
						{
							conclusions.Add(
								new Conclusion(
									ConclusionType.Elimination, cell * 9 + extraDigit));
						}
					}

					if (conclusions.Count == 0
						|| urMode && !_checkIncompleted && candidateOffsets.Count != 8)
					{
						continue;
					}

					// Check if the type number is 2 or 5.
					bool isType5 = i switch
					{
						0 => false,
						1 => false,
						4 => false,
						5 => false,
						2 => true,
						3 => true,
						_ => throw new Exception("Impossible case.")
					};

					// Type 2 / 5.
					result.Add(urMode
						? (RectangleTechniqueInfo)new UniqueRectangleTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: null,
									linkMasks: null)
							},
							detailData: new UrType2Or5(cells, digits.ToArray(), extraDigit, isType5))
						: new AvoidableRectangleTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets:
										new List<(int, int)>(
											from cell in cells
											select (0, cell)),
									candidateOffsets,
									regionOffsets: null,
									linkMasks: null)
							},
							detailData: new ArType2(cells, digits.ToArray(), extraDigit)));
				}

				// Then check type 4 / 6.
				if (i == 2 || i == 3)
				{
					if (urMode)
					{
						CheckType6(result, grid, cells, cellPair, extraCells, digits);
					}
				}
				else
				{
					if (urMode)
					{
						// Check type 4.
						CheckType4(result, grid, cells, cellPair, extraCells, digits);
					}

					// Check type 3.
					if (!urMode && (
						cellPair.Any(cell => grid.GetCellStatus(cell) == CellStatus.Empty)
						|| extraCells.Any(cell => grid.GetCellStatus(cell) != CellStatus.Empty)))
					{
						continue;
					}

					CellUtils.IsSameRegion(extraCells[0], extraCells[1], out int[] regions);
					for (int size = 1; size <= 3; size++)
					{
						CheckType3Naked(result, grid, cells, digits, regions, size, urMode);
						CheckType3Hidden(result, grid, cells, extraCells, digits, regions, size, urMode);
					}
				}
			}
		}

		/// <summary>
		/// Check type 3 (with naked subset).
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="regions">All regions.</param>
		/// <param name="size">The size to check.</param>
		/// <param name="urMode">
		/// Indicates whether the current searching is for UR. <see langword="true"/>
		/// is for UR, <see langword="false"/> is for AR.
		/// </param>
		private void CheckType3Naked(
			IList<RectangleTechniqueInfo> result, Grid grid, int[] cells,
			IEnumerable<int> digits, int[] regions, int size, bool urMode)
		{
			for (int i = 0, length = regions.Length; i < length; i++)
			{
				int region = regions[i];
				int[] cellsToTraverse = GridMap.GetCellsIn(region);
				for (int i1 = 0; i1 < 10 - size; i1++)
				{
					int c1 = cellsToTraverse[i1];
					if (cells.Contains(c1) || grid.GetCellStatus(c1) != CellStatus.Empty)
					{
						continue;
					}

					short mask1 = grid.GetMask(c1);
					if (size == 1)
					{
						// Check light naked pair.
						short mask = (short)(~mask1 & 511);
						var allCells = new List<int>(cells) { c1 };
						short otherDigitMask = GetOtherDigitMask(
							grid, allCells, digits, out short digitKindsMask);
						if (mask.CountSet() == 2 && otherDigitMask == mask)
						{
							// Type 3 (+ naked) found.
							// Record all highlight candidates.
							var candidateOffsets = new List<(int, int)>();
							foreach (int cell in allCells)
							{
								for (int x = 0, temp = otherDigitMask; x < 9; x++, temp >>= 1)
								{
									if ((temp & 1) != 0 && grid.CandidateExists(cell, x))
									{
										candidateOffsets.Add((1, cell * 9 + x));
									}
								}
								if (urMode
									|| !urMode && grid.GetCellStatus(cell) == CellStatus.Empty)
								{
									for (int x = 0, temp = digitKindsMask & ~otherDigitMask; x < 9; x++, temp >>= 1)
									{
										if ((temp & 1) != 0 && grid.CandidateExists(cell, x))
										{
											candidateOffsets.Add((0, cell * 9 + x));
										}
									}
								}
							}

							// Record all eliminations.
							var conclusions = new List<Conclusion>();
							for (int digit = 0, temp = otherDigitMask; digit < 9; digit++, temp >>= 1)
							{
								if ((temp & 1) != 0)
								{
									GridMap elimMap = default;
									for (int y = 0, count = 0; y < 5; y++)
									{
										int cell = allCells[y];
										if (grid.CandidateExists(cell, digit))
										{
											if (count++ == 0)
											{
												elimMap = new GridMap(cell, false);
											}
											else
											{
												elimMap &= new GridMap(cell, false);
											}
										}
									}

									foreach (int cell in elimMap.Offsets)
									{
										if (grid.CandidateExists(cell, digit))
										{
											conclusions.Add(
												new Conclusion(
													ConclusionType.Elimination, cell * 9 + digit));
										}
									}
								}
							}

							if (conclusions.Count == 0
								|| urMode && !_checkIncompleted && candidateOffsets.Count(c =>
								{
									var (type, _) = c;
									return type == 0;
								}) != 8)
							{
								continue;
							}

							result.Add(urMode
								? (RectangleTechniqueInfo)new UniqueRectangleTechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: null,
											candidateOffsets,
											regionOffsets: null,
											linkMasks: null)
									},
									detailData: new UrType3(
										cells,
										digits: digits.ToArray(),
										subsetDigits: otherDigitMask.GetAllSets().ToArray(),
										subsetCells: new[] { c1 },
										true))
								: new AvoidableRectangleTechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets:
												new List<(int, int)>(
													from cell in cells
													select (0, cell)),
											candidateOffsets,
											regionOffsets: null,
											linkMasks: null)
									},
									detailData: new ArType3(
										cells,
										digits: digits.ToArray(),
										subsetDigits: otherDigitMask.GetAllSets().ToArray(),
										subsetCells: new[] { c1 },
										true)));
						}
					}
					else // size > 1
					{
						for (int i2 = i1 + 1; i2 < 11 - size; i2++)
						{
							int c2 = cellsToTraverse[i2];
							if (cells.Contains(c2) || grid.GetCellStatus(c2) != CellStatus.Empty)
							{
								continue;
							}

							short mask2 = grid.GetMask(c2);
							if (size == 2)
							{
								// Check light naked triple.
								short mask = (short)((~mask1 & 511) | (~mask2 & 511));
								var allCells = new List<int>(cells) { c1, c2 };
								short otherDigitMask = GetOtherDigitMask(
									grid, allCells, digits, out short digitKindsMask);
								if (mask.CountSet() == 3 && otherDigitMask == mask)
								{
									// Type 3 (+ naked) found.
									// Record all highlight candidates.
									var candidateOffsets = new List<(int, int)>();
									foreach (int cell in allCells)
									{
										for (int x = 0, temp = otherDigitMask; x < 9; x++, temp >>= 1)
										{
											if ((temp & 1) != 0 && grid.CandidateExists(cell, x))
											{
												candidateOffsets.Add((1, cell * 9 + x));
											}
										}
										if (urMode
											|| !urMode && grid.GetCellStatus(cell) == CellStatus.Empty)
										{
											for (int x = 0, temp = digitKindsMask & ~otherDigitMask; x < 9; x++, temp >>= 1)
											{
												if ((temp & 1) != 0 && grid.CandidateExists(cell, x))
												{
													candidateOffsets.Add((0, cell * 9 + x));
												}
											}
										}
									}

									// Record all eliminations.
									var conclusions = new List<Conclusion>();
									for (int digit = 0, temp = otherDigitMask; digit < 9; digit++, temp >>= 1)
									{
										if ((temp & 1) != 0)
										{
											GridMap elimMap = default;
											for (int y = 0, count = 0; y < 6; y++)
											{
												int cell = allCells[y];
												if (grid.CandidateExists(cell, digit))
												{
													if (count++ == 0)
													{
														elimMap = new GridMap(cell, false);
													}
													else
													{
														elimMap &= new GridMap(cell, false);
													}
												}
											}

											foreach (int cell in elimMap.Offsets)
											{
												if (grid.CandidateExists(cell, digit))
												{
													conclusions.Add(
														new Conclusion(
															ConclusionType.Elimination, cell * 9 + digit));
												}
											}
										}
									}

									if (conclusions.Count == 0
										|| urMode && !_checkIncompleted && candidateOffsets.Count(c =>
										{
											var (type, _) = c;
											return type == 0;
										}) != 8)
									{
										continue;
									}

									result.Add(urMode
										? (RectangleTechniqueInfo)new UniqueRectangleTechniqueInfo(
											conclusions,
											views: new[]
											{
												new View(
													cellOffsets: null,
													candidateOffsets,
													regionOffsets: null,
													linkMasks: null)
											},
											detailData: new UrType3(
												cells,
												digits: digits.ToArray(),
												subsetDigits: otherDigitMask.GetAllSets().ToArray(),
												subsetCells: new[] { c1, c2 },
												true))
										: new AvoidableRectangleTechniqueInfo(
											conclusions,
											views: new[]
											{
												new View(
													cellOffsets:
														new List<(int, int)>(
															from cell in cells
															select (0, cell)),
													candidateOffsets,
													regionOffsets: null,
													linkMasks: null)
											},
											detailData: new ArType3(
												cells,
												digits: digits.ToArray(),
												subsetDigits: otherDigitMask.GetAllSets().ToArray(),
												subsetCells: new[] { c1, c2 },
												true)));
								}
							}
							else // size == 3
							{
								for (int i3 = i2 + 1; i3 < 9; i3++)
								{
									int c3 = cellsToTraverse[i3];
									if (cells.Contains(c3) || grid.GetCellStatus(c3) != CellStatus.Empty)
									{
										continue;
									}

									// Check light naked quadruple.
									short mask3 = grid.GetMask(c3);
									short mask = (short)(((~mask1 & 511) | (~mask2 & 511) | (~mask3 & 511)) & 511);
									var allCells = new List<int>(cells) { c1, c2, c3 };
									short otherDigitMask = GetOtherDigitMask(
										grid, allCells, digits, out short digitKindsMask);
									if (mask.CountSet() == 4 && otherDigitMask == mask)
									{
										// Type 3 (+ naked) found.
										// Record all highlight candidates.
										var candidateOffsets = new List<(int, int)>();
										foreach (int cell in allCells)
										{
											for (int x = 0, temp = otherDigitMask; x < 9; x++, temp >>= 1)
											{
												if ((temp & 1) != 0 && grid.CandidateExists(cell, x))
												{
													candidateOffsets.Add((1, cell * 9 + x));
												}
											}
											if (urMode
												|| !urMode && grid.GetCellStatus(cell) == CellStatus.Empty)
											{
												for (int x = 0, temp = digitKindsMask & ~otherDigitMask; x < 9; x++, temp >>= 1)
												{
													if ((temp & 1) != 0 && grid.CandidateExists(cell, x))
													{
														candidateOffsets.Add((0, cell * 9 + x));
													}
												}
											}
										}

										// Record all eliminations.
										var conclusions = new List<Conclusion>();
										for (int digit = 0, temp = otherDigitMask; digit < 9; digit++, temp >>= 1)
										{
											if ((temp & 1) != 0)
											{
												GridMap elimMap = default;
												for (int y = 0, count = 0; y < 7; y++)
												{
													int cell = allCells[y];
													if (grid.CandidateExists(cell, digit))
													{
														if (count++ == 0)
														{
															elimMap = new GridMap(cell, false);
														}
														else
														{
															elimMap &= new GridMap(cell, false);
														}
													}
												}

												foreach (int cell in elimMap.Offsets)
												{
													if (grid.CandidateExists(cell, digit))
													{
														conclusions.Add(
															new Conclusion(
																ConclusionType.Elimination, cell * 9 + digit));
													}
												}
											}
										}

										if (conclusions.Count == 0
											|| urMode && !_checkIncompleted && candidateOffsets.Count(c =>
											{
												var (type, _) = c;
												return type == 0;
											}) != 8)
										{
											continue;
										}

										result.Add(urMode
											? (RectangleTechniqueInfo)new UniqueRectangleTechniqueInfo(
												conclusions,
												views: new[]
												{
													new View(
														cellOffsets: null,
														candidateOffsets,
														regionOffsets: null,
														linkMasks: null)
												},
												detailData: new UrType3(
													cells,
													digits: digits.ToArray(),
													subsetDigits: otherDigitMask.GetAllSets().ToArray(),
													subsetCells: new[] { c1, c2, c3 },
													true))
											: new AvoidableRectangleTechniqueInfo(
												conclusions,
												views: new[]
												{
													new View(
														cellOffsets:
															new List<(int, int)>(
																from cell in cells
																select (0, cell)),
														candidateOffsets,
														regionOffsets: null,
														linkMasks: null)
												},
												detailData: new ArType3(
													cells,
													digits: digits.ToArray(),
													subsetDigits: otherDigitMask.GetAllSets().ToArray(),
													subsetCells: new[] { c1, c2, c3 },
													true)));
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Check type 3 (with hidden subset).
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="extraCells">All extra cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="regions">All regions.</param>
		/// <param name="size">The size to check.</param>
		/// <param name="urMode">
		/// Indicates whether the current searching is for UR. <see langword="true"/>
		/// is for UR, <see langword="false"/> is for AR.
		/// </param>
		private void CheckType3Hidden(
			IList<RectangleTechniqueInfo> result, Grid grid, int[] cells,
			int[] extraCells, IEnumerable<int> digits, int[] regions, int size,
			bool urMode)
		{
			for (int i = 0, length = regions.Length; i < length; i++)
			{
				int region = regions[i];
				for (int d1 = 0; d1 < 10 - size; d1++)
				{
					short mask1 = grid.GetDigitAppearingMask(d1, region);
					if (mask1 == 0 || !digits.Contains(d1))
					{
						continue;
					}

					for (int d2 = d1 + 1; d2 < 11 - size; d2++)
					{
						short mask2 = grid.GetDigitAppearingMask(d2, region);
						if (mask2 == 0 || !digits.Contains(d2))
						{
							continue;
						}

						if (size == 1)
						{
							// Check light hidden pair.
							short mask = (short)(mask1 | mask2);
							if (mask.CountSet() == 3)
							{
								// Type 3 (+ hidden) found.
								// Record all highlight candidates and eliminations.
								var candidateOffsets = new List<(int, int)>();
								var conclusions = new List<Conclusion>();
								var otherDigits = new List<int>();
								var otherCells = new List<int>();
								int[] subsetDigits = new[] { d1, d2 };
								int[] cellsToTraverse = GridMap.GetCellsIn(region);
								foreach (int cell in cells)
								{
									foreach (int digit in digits)
									{
										if (urMode
											|| !urMode && grid.GetCellStatus(cell) == CellStatus.Empty)
										{
											if (grid.CandidateExists(cell, digit))
											{
												candidateOffsets.Add((0, cell * 9 + digit));
											}
										}
									}
								}
								for (int x = 0, temp = mask; x < 9; x++, temp >>= 1)
								{
									if ((temp & 1) != 0)
									{
										int cell = cellsToTraverse[x];
										if (!cells.Contains(cell))
										{
											otherCells.Add(cell);
											if (grid.CandidateExists(cell, d1))
											{
												candidateOffsets.Add((1, cell * 9 + d1));
											}
											if (grid.CandidateExists(cell, d2))
											{
												candidateOffsets.Add((1, cell * 9 + d2));
											}

											for (int elimDigit = 0; elimDigit < 9; elimDigit++)
											{
												if (!subsetDigits.Contains(elimDigit))
												{
													otherDigits.Add(elimDigit);
													if (grid.CandidateExists(cell, elimDigit))
													{
														conclusions.Add(
															new Conclusion(
																ConclusionType.Elimination, cell * 9 + elimDigit));
													}
												}
											}
										}
									}
								}

								if (conclusions.Count == 0
									|| urMode && !_checkIncompleted && candidateOffsets.Count(c =>
									{
										var (type, _) = c;
										return type == 0;
									}) != 8)
								{
									continue;
								}

								// Type 3 (+ hidden).
								result.Add(urMode
									? (RectangleTechniqueInfo)new UniqueRectangleTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												linkMasks: null)
										},
										detailData: new UrType3(
											cells,
											digits: digits.ToArray(),
											subsetDigits: subsetDigits,
											subsetCells: otherCells,
											isNaked: false))
									: new AvoidableRectangleTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets:
													new List<(int, int)>(
														from cell in cells
														select (0, cell)),
												candidateOffsets,
												regionOffsets: null,
												linkMasks: null)
										},
										detailData: new ArType3(
											cells,
											digits: digits.ToArray(),
											subsetDigits: subsetDigits,
											subsetCells: otherCells,
											isNaked: false)));
							}
						}
						else // size > 1
						{
							for (int d3 = d2 + 1; d3 < 12 - size; d3++)
							{
								short mask3 = grid.GetDigitAppearingMask(d3, region);
								if (mask3 == 0)
								{
									continue;
								}

								if (size == 2)
								{
									// Check light hidden triple.
									short mask = (short)((short)(mask1 | mask2) | mask3);
									if (mask.CountSet() == 4
										&& extraCells.All(c => grid.CandidateDoesNotExist(c, d3)))
									{
										// Type 3 (+ hidden) found.
										// Record all highlight candidates and eliminations.
										var candidateOffsets = new List<(int, int)>();
										var conclusions = new List<Conclusion>();
										var otherDigits = new List<int>();
										var otherCells = new List<int>();
										int[] subsetDigits = new[] { d1, d2, d3 };
										int[] cellsToTraverse = GridMap.GetCellsIn(region);
										foreach (int cell in cells)
										{
											foreach (int digit in digits)
											{
												if (urMode
													|| !urMode && grid.GetCellStatus(cell) == CellStatus.Empty)
												{
													if (grid.CandidateExists(cell, digit))
													{
														candidateOffsets.Add((0, cell * 9 + digit));
													}
												}
											}
										}
										for (int x = 0, temp = mask; x < 9; x++, temp >>= 1)
										{
											if ((temp & 1) != 0)
											{
												int cell = cellsToTraverse[x];
												if (!cells.Contains(cell))
												{
													otherCells.Add(cell);
													if (grid.CandidateExists(cell, d1))
													{
														candidateOffsets.Add((1, cell * 9 + d1));
													}
													if (grid.CandidateExists(cell, d2))
													{
														candidateOffsets.Add((1, cell * 9 + d2));
													}

													for (int elimDigit = 0; elimDigit < 9; elimDigit++)
													{
														if (!subsetDigits.Contains(elimDigit))
														{
															otherDigits.Add(elimDigit);
															if (grid.CandidateExists(cell, elimDigit))
															{
																conclusions.Add(
																	new Conclusion(
																		ConclusionType.Elimination, cell * 9 + elimDigit));
															}
														}
													}
												}
											}
										}

										if (conclusions.Count == 0
											|| urMode && !_checkIncompleted && candidateOffsets.Count(c =>
											{
												var (type, _) = c;
												return type == 0;
											}) != 8)
										{
											continue;
										}

										// Type 3 (+ hidden).
										result.Add(urMode
											? (RectangleTechniqueInfo)new UniqueRectangleTechniqueInfo(
												conclusions,
												views: new[]
												{
													new View(
														cellOffsets: null,
														candidateOffsets,
														regionOffsets: null,
														linkMasks: null)
												},
												detailData: new UrType3(
													cells,
													digits: digits.ToArray(),
													subsetDigits: subsetDigits,
													subsetCells: otherCells,
													isNaked: false))
											: new AvoidableRectangleTechniqueInfo(
												conclusions,
												views: new[]
												{
													new View(
														cellOffsets:
															new List<(int, int)>(
																from cell in cells
																select (0, cell)),
														candidateOffsets,
														regionOffsets: null,
														linkMasks: null)
												},
												detailData: new ArType3(
													cells,
													digits: digits.ToArray(),
													subsetDigits: subsetDigits,
													subsetCells: otherCells,
													isNaked: false)));
									}
								}
								else // size == 3
								{
									for (int d4 = d3 + 1; d4 < 9; d4++)
									{
										short mask4 = grid.GetDigitAppearingMask(d4, region);
										if (mask4 == 0)
										{
											continue;
										}

										// Check light hidden quadruple.
										short mask = (short)((short)((short)(mask1 | mask2) | mask3) | mask4);
										if (mask.CountSet() == 5
											&& extraCells.All(c => grid.CandidateDoesNotExist(c, d3) && grid.CandidateDoesNotExist(c, d4)))
										{
											// Type 3 (+ hidden) found.
											// Record all highlight candidates and eliminations.
											var candidateOffsets = new List<(int, int)>();
											var conclusions = new List<Conclusion>();
											var otherDigits = new List<int>();
											var otherCells = new List<int>();
											int[] subsetDigits = new[] { d1, d2, d3, d4 };
											int[] cellsToTraverse = GridMap.GetCellsIn(region);
											foreach (int cell in cells)
											{
												foreach (int digit in digits)
												{
													if (urMode
														|| !urMode && grid.GetCellStatus(cell) == CellStatus.Empty)
													{
														if (grid.CandidateExists(cell, digit))
														{
															candidateOffsets.Add((0, cell * 9 + digit));
														}
													}
												}
											}
											for (int x = 0, temp = mask; x < 9; x++, temp >>= 1)
											{
												if ((temp & 1) != 0)
												{
													int cell = cellsToTraverse[x];
													if (!cells.Contains(cell))
													{
														otherCells.Add(cell);
														if (grid.CandidateExists(cell, d1))
														{
															candidateOffsets.Add((1, cell * 9 + d1));
														}
														if (grid.CandidateExists(cell, d2))
														{
															candidateOffsets.Add((1, cell * 9 + d2));
														}

														for (int elimDigit = 0; elimDigit < 9; elimDigit++)
														{
															if (!subsetDigits.Contains(elimDigit))
															{
																otherDigits.Add(elimDigit);
																if (grid.CandidateExists(cell, elimDigit))
																{
																	conclusions.Add(
																		new Conclusion(
																			ConclusionType.Elimination, cell * 9 + elimDigit));
																}
															}
														}
													}
												}
											}

											if (conclusions.Count == 0
												|| urMode && !_checkIncompleted && candidateOffsets.Count(c =>
												{
													var (type, _) = c;
													return type == 0;
												}) != 8)
											{
												continue;
											}

											// Type 3 (+ hidden).
											result.Add(urMode
												? (RectangleTechniqueInfo)new UniqueRectangleTechniqueInfo(
													conclusions,
													views: new[]
													{
														new View(
															cellOffsets: null,
															candidateOffsets,
															regionOffsets: null,
															linkMasks: null)
													},
													detailData: new UrType3(
														cells,
														digits: digits.ToArray(),
														subsetDigits: subsetDigits,
														subsetCells: otherCells,
														isNaked: false))
												: new AvoidableRectangleTechniqueInfo(
													conclusions,
													views: new[]
													{
														new View(
															cellOffsets:
																new List<(int, int)>(
																	from cell in cells
																	select (0, cell)),
															candidateOffsets,
															regionOffsets: null,
															linkMasks: null)
													},
													detailData: new ArType3(
														cells,
														digits: digits.ToArray(),
														subsetDigits: subsetDigits,
														subsetCells: otherCells,
														isNaked: false)));
										}
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Check type 6.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="cellPair">The cell pair.</param>
		/// <param name="extraCells">All extra cells.</param>
		/// <param name="digits">All digits.</param>
		private void CheckType6(
			IList<RectangleTechniqueInfo> result, Grid grid, int[] cells,
			int[] cellPair, int[] extraCells, IEnumerable<int> digits)
		{
			var ((r1, c1, _), (r2, c2, _)) =
				(CellUtils.GetRegion(cellPair[0]), CellUtils.GetRegion(cellPair[1]));
			r1 += 9; // 0..9 => 9..18
			r2 += 9;
			c1 += 18; // 0..9 => 18..27
			c2 += 18;
			short mask1 = GetRegionAppearingMask(r1, new[] { cellPair[0], extraCells[0] });
			short mask2 = GetRegionAppearingMask(r2, new[] { cellPair[1], extraCells[1] });
			short mask3 = GetRegionAppearingMask(c1, new[] { cellPair[0], extraCells[1] });
			short mask4 = GetRegionAppearingMask(c2, new[] { cellPair[1], extraCells[0] });
			int[] digitsArray = digits.ToArray();
			for (int i = 0; i < 2; i++)
			{
				int digit = digitsArray[i];
				int otherDigit = digitsArray[i == 0 ? 1 : 0];

				// Check whether row conjugate pairs form X-Wing.
				short r1Mask = grid.GetDigitAppearingMask(digit, r1);
				short r2Mask = grid.GetDigitAppearingMask(digit, r2);
				short c1Mask = grid.GetDigitAppearingMask(digit, c1);
				short c2Mask = grid.GetDigitAppearingMask(digit, c2);
				if (mask1 == r1Mask && r1Mask.CountSet() == 2
					&& mask2 == r2Mask && r2Mask.CountSet() == 2)
				{
					// Type 6 found.
					var conclusions = new List<Conclusion>();
					foreach (int cell in cellPair)
					{
						if (grid.GetCellStatus(cell) == CellStatus.Empty)
						{
							conclusions.Add(new Conclusion(ConclusionType.Assignment, cell * 9 + digit));
						}
					}

					var candidateOffsets = new List<(int, int)>();
					foreach (int cell in cellPair)
					{
						if (grid.CandidateExists(cell, digit))
						{
							candidateOffsets.Add((1, cell * 9 + digit));
						}
						if (grid.CandidateExists(cell, otherDigit))
						{
							candidateOffsets.Add((0, cell * 9 + otherDigit));
						}
					}
					foreach (int cell in extraCells)
					{
						if (grid.CandidateExists(cell, digit))
						{
							candidateOffsets.Add((1, cell * 9 + digit));
						}
						if (grid.CandidateExists(cell, otherDigit))
						{
							candidateOffsets.Add((0, cell * 9 + otherDigit));
						}
					}

					int elimCount = conclusions.Count;
					if (elimCount == 0
						|| !_checkIncompleted && (elimCount != 2 || candidateOffsets.Count != 8))
					{
						continue;
					}

					// Type 6.
					result.Add(
						new UniqueRectangleTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: new[] { (0, r1), (0, r2) },
									linkMasks: null)
							},
							detailData: new UrType6(
								cells,
								digits: digitsArray,
								conjugatePairs: new[]
								{
									new ConjugatePair(cellPair[0], extraCells[0], digit),
									new ConjugatePair(cellPair[1], extraCells[1], digit)
								})));
				}

				// Check whether column conjugate pairs form X-Wing.
				if (mask3 == c1Mask && c1Mask.CountSet() == 2
					&& mask4 == c2Mask && c2Mask.CountSet() == 2)
				{
					// Type 6 found.
					var conclusions = new List<Conclusion>();
					foreach (int cell in cellPair)
					{
						if (grid.GetCellStatus(cell) == CellStatus.Empty)
						{
							conclusions.Add(new Conclusion(ConclusionType.Assignment, cell * 9 + digit));
						}
					}

					var candidateOffsets = new List<(int, int)>();
					foreach (int cell in cellPair)
					{
						if (grid.CandidateExists(cell, digit))
						{
							candidateOffsets.Add((1, cell * 9 + digit));
						}
						if (grid.CandidateExists(cell, otherDigit))
						{
							candidateOffsets.Add((0, cell * 9 + otherDigit));
						}
					}
					foreach (int cell in extraCells)
					{
						if (grid.CandidateExists(cell, digit))
						{
							candidateOffsets.Add((1, cell * 9 + digit));
						}
						if (grid.CandidateExists(cell, otherDigit))
						{
							candidateOffsets.Add((0, cell * 9 + otherDigit));
						}
					}

					int elimCount = conclusions.Count;
					if (elimCount == 0
						|| !_checkIncompleted && (elimCount != 2 || candidateOffsets.Count != 8))
					{
						continue;
					}

					// Type 6.
					result.Add(
						new UniqueRectangleTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: new[] { (0, c1), (0, c2) },
									linkMasks: null)
							},
							detailData: new UrType6(
								cells,
								digits: digitsArray,
								conjugatePairs: new[]
								{
									new ConjugatePair(cellPair[0], extraCells[1], digit),
									new ConjugatePair(extraCells[0], cellPair[1], digit)
								})));
				}
			}
		}

		/// <summary>
		/// Check type 4.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="cellPair">The cell pair.</param>
		/// <param name="extraCells">All extra cells.</param>
		/// <param name="digits">All digits.</param>
		private void CheckType4(
			IList<RectangleTechniqueInfo> result, Grid grid, int[] cells,
			int[] cellPair, int[] extraCells, IEnumerable<int> digits)
		{
			// Get region.
			var sameRegions = new List<int>();
			var ((r1, c1, b1), (r2, c2, b2)) =
				(CellUtils.GetRegion(extraCells[0]), CellUtils.GetRegion(extraCells[1]));
			if (r1 == r2) sameRegions.Add(r1 + 9); // 0..9 => 9..18
			if (c1 == c2) sameRegions.Add(c1 + 18); // 0..9 => 18..27
			if (b1 == b2) sameRegions.Add(b1); // 0..9

			foreach (int regionOffset in sameRegions)
			{
				if (digits.All(d => grid.HasDigitValue(d, regionOffset)))
				{
					continue;
				}

				short maskComparer = GetRegionAppearingMask(regionOffset, extraCells);
				int[] digitsArray = digits.ToArray();
				for (int index = 0; index < 2; index++)
				{
					int digit = digitsArray[index];
					int elimDigit = index == 0 ? digitsArray[1] : digitsArray[0];
					short mask = grid.GetDigitAppearingMask(digit, regionOffset);
					if (mask.CountSet() == 2 && mask == maskComparer)
					{
						// Type 4 found.
						// Record all highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						foreach (int cell in cellPair)
						{
							foreach (int d in digits)
							{
								if (grid.CandidateExists(cell, d))
								{
									candidateOffsets.Add((0, cell * 9 + d));
								}
							}
						}
						foreach (int cell in extraCells)
						{
							if (grid.CandidateExists(cell, digit))
							{
								candidateOffsets.Add((1, cell * 9 + digit));
							}
						}

						// Record all eliminations.
						var conclusions = new List<Conclusion>();
						foreach (int cell in extraCells)
						{
							if (grid.CandidateExists(cell, elimDigit))
							{
								conclusions.Add(
									new Conclusion(
										ConclusionType.Elimination, cell * 9 + elimDigit));
							}
						}

						int elimCount = conclusions.Count;
						if (elimCount == 0
							|| !_checkIncompleted && (candidateOffsets.Count != 6 || elimCount != 2))
						{
							continue;
						}

						// Type 4.
						result.Add(
							new UniqueRectangleTechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: new[] { (0, regionOffset) },
										linkMasks: null)
								},
								detailData: new UrType4(
									cells,
									digits: digitsArray,
									conjugatePair: new ConjugatePair(extraCells[0], extraCells[1], digit))));
					}
				}
			}
		}


		/// <summary>
		/// Get other digit mask used in type 3 with naked subset.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="allCells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="digitKindsMask">(out parameter) The digit kind mask.</param>
		/// <returns>The result mask.</returns>
		private static short GetOtherDigitMask(
			Grid grid, IEnumerable<int> allCells, IEnumerable<int> digits,
			out short digitKindsMask)
		{
			digitKindsMask = 511;
			foreach (int cell in allCells)
			{
				digitKindsMask &= grid.GetMask(cell);
			}
			digitKindsMask = (short)(~digitKindsMask & 511);
			short tempMask = 0;
			foreach (int digit in digits)
			{
				tempMask |= (short)(1 << digit);
			}

			return (short)(digitKindsMask & ~tempMask);
		}

		/// <summary>
		/// Get the appearing mask in a region.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <param name="cells">All cells to check.</param>
		/// <returns>The result mask.</returns>
		private static short GetRegionAppearingMask(int region, int[] cells)
		{
			short mask = 0;
			for (int temp = 0; temp < 9; temp++)
			{
				mask += (short)(cells.Contains(RegionUtils.GetCellOffset(region, temp)) ? 1 : 0);

				if (temp != 8)
				{
					mask <<= 1;
				}
			}
			mask.ReverseBits();
			return (short)(mask >> 7 & 511);
		}
		#endregion
	}
}
