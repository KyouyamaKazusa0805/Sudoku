using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	partial class UsTechniqueSearcher
	{
		/// <summary>
		/// Check type 1.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="pattern">(<see langword="in"/> parameter) The pattern.</param>
		/// <param name="mask">The mask.</param>
		private static partial void CheckType1(
			IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap pattern, short mask)
		{
			if (mask.PopCount() != 5)
			{
				return;
			}

			foreach (int[] digits in mask.GetMaskSubsets(4))
			{
				short digitsMask = 0;
				foreach (int digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				int extraDigit = (mask & ~digitsMask).FindFirstSet();
				var extraDigitMap = CandMaps[extraDigit] & pattern;
				if (extraDigitMap.Count != 1)
				{
					continue;
				}

				int elimCell = extraDigitMap.First;
				short cellMask = grid.GetCandidateMask(elimCell);
				short elimMask = (short)(cellMask & ~(1 << extraDigit));
				if (elimMask == 0)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int digit in elimMask)
				{
					conclusions.Add(new(Elimination, elimCell, digit));
				}

				var candidateOffsets = new List<DrawingInfo>();
				foreach (int digit in digits)
				{
					foreach (int cell in new GridMap(pattern) { ~elimCell } & CandMaps[digit])
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
					}
				}

				accumulator.Add(
					new UsType1TechniqueInfo(
						conclusions,
						new View[] { new(candidateOffsets) },
						pattern,
						digitsMask,
						elimCell * 9 + extraDigit));
			}
		}

#pragma warning disable IDE0060
		/// <summary>
		/// Check type 2.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="pattern">(<see langword="in"/> parameter) The pattern.</param>
		/// <param name="mask">The mask.</param>
		private static partial void CheckType2(
			IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap pattern, short mask)
		{
			if (mask.PopCount() != 5)
			{
				return;
			}

			foreach (int[] digits in mask.GetMaskSubsets(4))
			{
				short digitsMask = 0;
				foreach (int digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				int extraDigit = (mask & ~digitsMask).FindFirstSet();
				var elimMap = (CandMaps[extraDigit] & pattern).PeerIntersection & CandMaps[extraDigit];
				if (elimMap.IsEmpty)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int cell in elimMap)
				{
					conclusions.Add(new(Elimination, cell, extraDigit));
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
					new UsType2TechniqueInfo(
						conclusions,
						new View[] { new(candidateOffsets) },
						pattern,
						digitsMask,
						extraDigit));
			}
		}
#pragma warning restore IDE0060

		/// <summary>
		/// Check type 3.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="pattern">(<see langword="in"/> parameter) The pattern.</param>
		/// <param name="mask">The mask.</param>
		private static partial void CheckType3(
			IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap pattern, short mask)
		{
			foreach (int[] digits in mask.GetMaskSubsets(4))
			{
				short digitsMask = 0;
				foreach (int digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				short extraDigitsMask = (short)(mask & ~digitsMask);
				var tempMap = GridMap.Empty;
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
					for (int size = extraDigitsMask.PopCount() - 1, count = allCells.Length; size < count; size++)
					{
						foreach (int[] cells in allCells.GetSubsets(size))
						{
							short tempMask = 0;
							foreach (int cell in cells)
							{
								tempMask |= grid.GetCandidateMask(cell);
							}

							if (tempMask.PopCount() != size + 1 || (tempMask & extraDigitsMask) != extraDigitsMask)
							{
								continue;
							}

							var cellsMap = new GridMap(cells);
							var conclusions = new List<Conclusion>();
							foreach (int digit in tempMask)
							{
								foreach (int cell in (allCells - cellsMap) & CandMaps[digit])
								{
									conclusions.Add(new(Elimination, cell, digit));
								}
							}
							if (conclusions.Count == 0)
							{
								continue;
							}

							var candidateOffsets = new List<DrawingInfo>();
							foreach (int cell in pattern)
							{
								foreach (int digit in grid.GetCandidateMask(cell))
								{
									candidateOffsets.Add(new((tempMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
								}
							}
							foreach (int cell in cells)
							{
								foreach (int digit in grid.GetCandidateMask(cell))
								{
									candidateOffsets.Add(new(1, cell * 9 + digit));
								}
							}

							accumulator.Add(
								new UsType3TechniqueInfo(
									conclusions,
									new View[] { new(null, candidateOffsets, new DrawingInfo[] { new(0, region) }, null) },
									pattern,
									digitsMask,
									extraDigitsMask,
									cells));
						}
					}
				}
			}
		}

		/// <summary>
		/// Check type 4.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="pattern">(<see langword="in"/> parameter) The pattern.</param>
		/// <param name="mask">The mask.</param>
		private static partial void CheckType4(
			IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap pattern, short mask)
		{
			foreach (int[] digits in mask.GetMaskSubsets(4))
			{
				short digitsMask = 0;
				foreach (int digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				short extraDigitsMask = (short)(mask & ~digitsMask);
				var tempMap = GridMap.Empty;
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
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<DrawingInfo>();
					foreach (int cell in pattern - compareMap)
					{
						foreach (int digit in grid.GetCandidateMask(cell))
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
						new UsType4TechniqueInfo(
							conclusions,
							new View[]
							{
								new(
									null,
									candidateOffsets,
									new DrawingInfo[] { new(0, region) },
									null)
							},
							pattern,
							digitsMask,
							d1,
							d2,
							compareMap));
				}
			}
		}
	}
}
