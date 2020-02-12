using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using UlType1 = Sudoku.Solving.Manual.Uniqueness.Loops.UniqueLoopType1DetailData;
using UlType2 = Sudoku.Solving.Manual.Uniqueness.Loops.UniqueLoopType2DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Encapsulates a unique loop technique searcher.
	/// </summary>
	public sealed class UniqueLoopTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid) => TakeAllCore(grid);


		#region UL utils
		private static IReadOnlyList<UniqueLoopTechniqueInfo> TakeAllCore(Grid grid)
		{
			var result = new List<UniqueLoopTechniqueInfo>();

			for (int cell = 0; cell < 81; cell++)
			{
				if (grid.IsBivalueCell(cell))
				{
					short mask = grid.GetCandidatesReversal(cell);
					int d1 = mask.GetNextSetBit(-1);
					int d2 = mask.GetNextSetBit(d1);
					int[] digits = new[] { d1, d2 };

					var tempLoop = new List<int>();
					var loops = new List<List<int>>();

					CheckForLoopsRecursively(grid, cell, d1, d2, tempLoop, 2, 0, -1, loops);

					// Check loop finished.
					foreach (var loop in loops)
					{
						// Potential loop found. Check it.
						if (IsValidLoop(grid, loop))
						{
							// This is a unique loop.
							// Get cells with more than two candidates.
							var extraCells = new List<int>(2);
							foreach (int loopCell in loop)
							{
								if (grid.GetCandidatesReversal(loopCell).CountSet() > 2)
								{
									extraCells.Add(loopCell);
								}
							}

							if (extraCells.Count == 1)
							{
								// Type 1 found.
								int extraCell = extraCells[0];

								// Record all eiminations.
								var conclusions = new List<Conclusion>();
								if (!grid[extraCell, d1])
								{
									conclusions.Add(
										new Conclusion(
											ConclusionType.Elimination, extraCell * 9 + d1));
								}
								if (!grid[extraCell, d2])
								{
									conclusions.Add(
										new Conclusion(
											ConclusionType.Elimination, extraCell * 9 + d2));
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								// Record all highlight candidates.
								var candidateOffsets = new List<(int, int)>();
								foreach (int loopCell in loop)
								{
									if (loopCell == extraCell)
									{
										// Skip the extra cell in the loop.
										continue;
									}

									candidateOffsets.Add((0, loopCell * 9 + d1));
									candidateOffsets.Add((0, loopCell * 9 + d2));
								}

								// UL type 1.
								result.Add(
									new UniqueLoopTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												linkMasks: null)
										},
										detailData: new UlType1(
											cells: loop,
											digits)));
							}
							else if (extraCells.Count > 2)
							{
								// Type 2 (has more than 2 extra cells) found.
								short extraDigitMask = 0;
								foreach (int extraCell in extraCells)
								{
									extraDigitMask |= grid.GetCandidatesReversal(extraCell);
								}
								extraDigitMask &= (short)~((1 << d1) | (1 << d2));
								int extraDigit = extraDigitMask.FindFirstSet();

								// Record all eliminations.
								var conclusions = new List<Conclusion>();
								var elimMap = default(GridMap);
								for (int i = 0; i < extraCells.Count; i++)
								{
									if (i == 0)
									{
										elimMap = new GridMap(extraCells[i], false);
									}
									else
									{
										elimMap &= new GridMap(extraCells[i], false);
									}
								}
								foreach (int elimCell in elimMap.Offsets)
								{
									if (grid.CandidateExists(elimCell, extraDigit))
									{
										conclusions.Add(
											new Conclusion(
												ConclusionType.Elimination, elimCell * 9 + extraDigit));
									}
								}

								if (conclusions.Count == 0)
								{
									continue;
								}

								// Record all highlight candidates.
								var candidateOffsets = new List<(int, int)>();
								foreach (int loopCell in loop)
								{
									candidateOffsets.Add((0, loopCell * 9 + d1));
									candidateOffsets.Add((0, loopCell * 9 + d2));
								}
								foreach (int extraCell in extraCells)
								{
									candidateOffsets.Add((1, extraCell * 9 + extraDigit));
								}

								// UL type 2.
								result.Add(
									new UniqueLoopTechniqueInfo(
										conclusions,
										views: new[]
										{
											new View(
												cellOffsets: null,
												candidateOffsets,
												regionOffsets: null,
												linkMasks: null)
										},
										detailData: new UlType2(
											cells: loop,
											digits,
											extraDigit)));
							}
							else
							{
								if (extraCells.Count == 2)
								{
									int c1 = extraCells[0];
									int c2 = extraCells[1];
									short resultMask = (short)
										(grid.GetCandidatesReversal(c1) | grid.GetCandidatesReversal(c2));
									resultMask &= (short)~((1 << d1) | (1 << d2));
									int count = resultMask.CountSet();
									if (count == 1)
									{
										CheckType2(
											result, grid, resultMask.FindFirstSet(),
											extraCells, digits, loop);
									}
									else if (count >= 2)
									{
										// Type 3.
									}
								}

								// Type 4.
							}
						}
					}
				}
			}

			return result;
		}

		private static void CheckType2(
			IList<UniqueLoopTechniqueInfo> result, Grid grid,
			int extraDigit, IReadOnlyList<int> extraCells, int[] digits, IReadOnlyList<int> loop)
		{
			// Record all eliminations.
			var conclusions = new List<Conclusion>();
			var elimMap = default(GridMap);
			for (int i = 0; i < 2; i++)
			{
				int extraCell = extraCells[i];
				if (i == 0)
				{
					elimMap = new GridMap(extraCell, false);
				}
				else
				{
					elimMap &= new GridMap(extraCell, false);
				}
			}

			foreach (int cell in elimMap.Offsets)
			{
				if (grid.CandidateExists(cell, extraDigit))
				{
					conclusions.Add(new Conclusion(ConclusionType.Elimination, cell * 9 + extraDigit));
				}
			}

			if (conclusions.Count == 0)
			{
				return;
			}

			// UL type 2.
			// Record all highlight candidates.
			var candidateOffsets = new List<(int, int)>();
			foreach (int cell in loop)
			{
				candidateOffsets.Add((0, cell * 9 + digits[0]));
				candidateOffsets.Add((0, cell * 9 + digits[1]));
			}
			foreach (int extraCell in extraCells)
			{
				candidateOffsets.Add((1, extraCell * 9 + extraDigit));
			}

			result.Add(
				new UniqueLoopTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: null,
							candidateOffsets,
							regionOffsets: null,
							linkMasks: null)
					},
					detailData: new UlType2(loop, digits, extraDigit)));
		}

		private static bool IsValidLoop(Grid grid, IList<int> loop)
		{
			var visitedOdd = new HashSet<int>();
			var visitedEven = new HashSet<int>();

			bool isOdd = false;
			foreach (int cell in loop)
			{
				for (int regionType = 0; regionType < 3; regionType++)
				{
					var (r, c, b) = CellUtils.GetRegion(cell);
					int region = stackalloc[] { b, r + 9, c + 18 }[regionType];
					if (isOdd)
					{
						if (visitedOdd.Contains(region))
						{
							return false;
						}
						else
						{
							visitedOdd.Add(region);
						}
					}
					else
					{
						if (visitedEven.Contains(region))
						{
							return false;
						}
						else
						{
							visitedEven.Add(region);
						}
					}
				}

				isOdd = !isOdd;
			}

			// All regions must have been visited once with each parity (or never).
			return visitedOdd.All(c => visitedEven.Contains(c))
				&& visitedEven.All(c => visitedOdd.Contains(c));
		}

		private static void CheckForLoopsRecursively(
			Grid grid, int cell, int d1, int d2, IList<int> loop,
			int allowedExtraCellsCount, short exDigitsMask,
			int lastRegionType, IList<List<int>> loops)
		{
			loop.Add(cell);
			for (int regionType = 0; regionType < 3; regionType++)
			{
				if (regionType == lastRegionType)
				{
					continue;
				}

				var (r, c, b) = CellUtils.GetRegion(cell);
				int region = stackalloc[] { b, r + 9, c + 18 }[regionType];
				for (int pos = 0; pos < 9; pos++)
				{
					int nextCell = RegionUtils.GetCellOffset(region, pos);
					if (grid.GetCellStatus(nextCell) != CellStatus.Empty)
					{
						continue;
					}

					if (loop[0] == nextCell && loop.Count >= 4)
					{
						// The loop is closed. Now save as a copy.
						loops.Add(new List<int>(loop));
					}
					else if (!loop.Contains(nextCell) && !grid[nextCell, d1] && !grid[nextCell, d2])
					{
						short nextCellMask = grid.GetCandidatesReversal(nextCell);
						exDigitsMask |= nextCellMask;
						exDigitsMask &= (short)~((1 << d1) | (1 << d2));
						int digitsCount = nextCellMask.CountSet();

						// We can continue if:
						// (1) The cell has exactly two digits of the loop.
						// (2) The cell has one extra digit, the same as all previous cells
						// with an extra digit (for type 2 only).
						// (3) The cell has extra digits and the maximum number of cells
						// with extra digits, 2, is not reached.
						if (digitsCount != 2 && exDigitsMask.CountSet() != 1 && allowedExtraCellsCount <= 0)
						{
							continue;
						}

						int newAllowedExtraCellCount = allowedExtraCellsCount;
						if (digitsCount > 2)
						{
							newAllowedExtraCellCount--;
						}

						CheckForLoopsRecursively(
							grid, nextCell, d1, d2, loop, newAllowedExtraCellCount,
							exDigitsMask, regionType, loops);
					}
				}
			}

			// Roll back.
			loop.RemoveAt(loop.Count - 1);
		}
		#endregion
	}
}
