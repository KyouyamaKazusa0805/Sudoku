using System;
using System.Collections.Generic;
using System.Numerics;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	partial class UsStepSearcher
	{
		/// <summary>
		/// Check type 1.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="mask">The mask.</param>
		partial void CheckType1(IList<StepInfo> accumulator, in SudokuGrid grid, in Cells pattern, short mask)
		{
			if (PopCount((uint)mask) != 5)
			{
				return;
			}

			foreach (int[] digits in mask.GetAllSets().GetSubsets(4))
			{
				short digitsMask = 0;
				foreach (int digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				int extraDigit = TrailingZeroCount(mask & ~digitsMask);
				var extraDigitMap = CandMaps[extraDigit] & pattern;
				if (extraDigitMap.Count != 1)
				{
					continue;
				}

				int elimCell = extraDigitMap[0];
				short cellMask = grid.GetCandidates(elimCell);
				short elimMask = (short)(cellMask & ~(1 << extraDigit));
				if (elimMask == 0)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int digit in elimMask)
				{
					conclusions.Add(new(ConclusionType.Elimination, elimCell, digit));
				}

				var candidateOffsets = new List<DrawingInfo>();
				foreach (int digit in digits)
				{
					foreach (int cell in new Cells(pattern) { ~elimCell } & CandMaps[digit])
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
					}
				}

				accumulator.Add(
					new UsType1StepInfo(
						conclusions,
						new View[] { new() { Candidates = candidateOffsets } },
						pattern,
						digitsMask,
						elimCell * 9 + extraDigit
					)
				);
			}
		}

		/// <summary>
		/// Check type 2.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="mask">The mask.</param>
		partial void CheckType2(IList<StepInfo> accumulator, in Cells pattern, short mask)
		{
			if (PopCount((uint)mask) != 5)
			{
				return;
			}

			foreach (int[] digits in mask.GetAllSets().GetSubsets(4))
			{
				short digitsMask = 0;
				foreach (int digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				int extraDigit = TrailingZeroCount(mask & ~digitsMask);
				var elimMap = pattern % CandMaps[extraDigit];
				if (elimMap.IsEmpty)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int cell in elimMap)
				{
					conclusions.Add(new(ConclusionType.Elimination, cell, extraDigit));
				}

				var candidateOffsets = new List<DrawingInfo>();
				foreach (int digit in digits)
				{
					foreach (int cell in CandMaps[digit] & pattern)
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
					}
				}
				foreach (int cell in CandMaps[extraDigit] & pattern)
				{
					candidateOffsets.Add(new(1, cell * 9 + extraDigit));
				}

				accumulator.Add(
					new UsType2StepInfo(
						conclusions,
						new View[] { new() { Candidates = candidateOffsets } },
						pattern,
						digitsMask,
						extraDigit
					)
				);
			}
		}

		/// <summary>
		/// Check type 3.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="mask">The mask.</param>
		partial void CheckType3(IList<StepInfo> accumulator, in SudokuGrid grid, in Cells pattern, short mask)
		{
			foreach (int[] digits in mask.GetAllSets().GetSubsets(4))
			{
				short digitsMask = 0;
				foreach (int digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				short extraDigitsMask = (short)(mask & ~digitsMask);
				var tempMap = Cells.Empty;
				foreach (int digit in extraDigitsMask)
				{
					tempMap |= CandMaps[digit];
				}
				if (tempMap.InOneRegion)
				{
					continue;
				}

				foreach (int region in tempMap.CoveredRegions)
				{
					int[] allCells = ((RegionMaps[region] & EmptyMap) - pattern).ToArray();
					for (int size = PopCount((uint)extraDigitsMask) - 1, count = allCells.Length; size < count; size++)
					{
						foreach (int[] cells in allCells.GetSubsets(size))
						{
							short tempMask = 0;
							foreach (int cell in cells)
							{
								tempMask |= grid.GetCandidates(cell);
							}

							if (PopCount((uint)tempMask) != size + 1 || (tempMask & extraDigitsMask) != extraDigitsMask)
							{
								continue;
							}

							var cellsMap = new Cells(cells);
							var conclusions = new List<Conclusion>();
							foreach (int digit in tempMask)
							{
								foreach (int cell in (allCells - cellsMap) & CandMaps[digit])
								{
									conclusions.Add(new(ConclusionType.Elimination, cell, digit));
								}
							}
							if (conclusions.Count == 0)
							{
								continue;
							}

							var candidateOffsets = new List<DrawingInfo>();
							foreach (int cell in pattern)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(
										new((tempMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit)
									);
								}
							}
							foreach (int cell in cells)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(new(1, cell * 9 + digit));
								}
							}

							accumulator.Add(
								new UsType3StepInfo(
									conclusions,
									new View[]
									{
										new()
										{
											Candidates = candidateOffsets,
											Regions = new DrawingInfo[] { new(0, region) }
										}
									},
									pattern,
									digitsMask,
									extraDigitsMask,
									cells
								)
							);
						}
					}
				}
			}
		}

		/// <summary>
		/// Check type 4.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="mask">The mask.</param>
		partial void CheckType4(IList<StepInfo> accumulator, in SudokuGrid grid, in Cells pattern, short mask)
		{
			foreach (int[] digits in mask.GetAllSets().GetSubsets(4))
			{
				short digitsMask = 0;
				foreach (int digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				short extraDigitsMask = (short)(mask & ~digitsMask);
				var tempMap = Cells.Empty;
				foreach (int digit in extraDigitsMask)
				{
					tempMap |= CandMaps[digit];
				}
				if (tempMap.InOneRegion)
				{
					continue;
				}

				foreach (int region in tempMap.CoveredRegions)
				{
					int d1 = -1, d2 = -1, count = 0;
					var compareMap = RegionMaps[region] & pattern;
					foreach (int digit in digits)
					{
						if ((compareMap | RegionMaps[region] & CandMaps[digit]) == compareMap)
						{
							switch (count++)
							{
								case 0:
								{
									d1 = digit;
									break;
								}
								case 1:
								{
									d2 = digit;
									goto Finally;
								}
							}
						}
					}

				Finally:
					short comparer = (short)(1 << d1 | 1 << d2);
					short otherDigitsMask = (short)(digitsMask & ~comparer);
					var conclusions = new List<Conclusion>();
					foreach (int digit in otherDigitsMask)
					{
						foreach (int cell in compareMap & CandMaps[digit])
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<DrawingInfo>();
					foreach (int cell in pattern - compareMap)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(0, cell * 9 + digit));
						}
					}
					foreach (int cell in compareMap & CandMaps[d1])
					{
						candidateOffsets.Add(new(1, cell * 9 + d1));
					}
					foreach (int cell in compareMap & CandMaps[d2])
					{
						candidateOffsets.Add(new(1, cell * 9 + d2));
					}

					accumulator.Add(
						new UsType4StepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Candidates = candidateOffsets,
									Regions = new DrawingInfo[] { new(0, region) }
								}
							},
							pattern,
							digitsMask,
							d1,
							d2,
							compareMap
						)
					);
				}
			}
		}
	}
}
