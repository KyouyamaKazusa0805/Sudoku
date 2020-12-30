using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Uniqueness.Reversal
{
	partial class ReverseBugStepSearcher
	{
		/// <summary>
		/// Check the type 1.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="d1">The digit 1 to check.</param>
		/// <param name="d2">The digit 2 to check.</param>
		/// <param name="loop">(<see langword="in"/> parameter) The cells of that loop.</param>
		/// <param name="extraCell">Indicates the extra cell.</param>
		/// <param name="links">The links.</param>
		/// <param name="comparer">The comparer.</param>
		partial void CheckType1(
			IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2,
			in Cells loop, int extraCell, IReadOnlyList<Link> links, short comparer)
		{
			var conclusions = new List<Conclusion>();
			if (grid.Exists(extraCell, d1) is true)
			{
				conclusions.Add(new(ConclusionType.Elimination, extraCell, d1));
			}
			if (grid.Exists(extraCell, d2) is true)
			{
				conclusions.Add(new(ConclusionType.Elimination, extraCell, d2));
			}
			if (conclusions.Count == 0)
			{
				return;
			}

			var cellOffsets = new List<DrawingInfo>();
			foreach (int cell in loop)
			{
				cellOffsets.Add(new(0, cell));
			}

			accumulator.AddIfDoesNotContain(
				new ReverseBugType1StepInfo(
					conclusions,
					new View[] { new() { Cells = cellOffsets, Links = links } },
					loop,
					d1,
					d2,
					extraCell,
					(grid.GetCandidates(extraCell) & comparer).FindFirstSet()));
		}

		/// <summary>
		/// Check the type 2.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="d1">The digit 1 to check.</param>
		/// <param name="d2">The digit 2 to check.</param>
		/// <param name="loop">(<see langword="in"/> parameter) The cells of that loop.</param>
		/// <param name="extraCells">(<see langword="in"/> parameter) The cells of all extra cells.</param>
		/// <param name="links">The links.</param>
		/// <param name="comparer">The comparer.</param>
		partial void CheckType2(
			IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop,
			in Cells extraCells, IReadOnlyList<Link> links, short comparer)
		{
			short mask = 0;
			foreach (int cell in extraCells)
			{
				mask |= grid.GetCandidates(cell);
			}
			mask &= (short)~comparer;

			if (!mask.IsPowerOfTwo())
			{
				return;
			}

			int extraDigit = mask.FindFirstSet();
			var elimMap = (extraCells & CandMaps[extraDigit]).PeerIntersection & CandMaps[extraDigit];
			if (elimMap.IsEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int cell in elimMap)
			{
				conclusions.Add(new(ConclusionType.Elimination, cell, extraDigit));
			}

			var candidateOffsets = new List<DrawingInfo>();
			foreach (int cell in loop)
			{
				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(new(digit == extraDigit ? 1 : 0, cell * 9 + digit));
				}
			}

			accumulator.AddIfDoesNotContain(
				new ReverseBugType2StepInfo(
					conclusions,
					new View[] { new() { Candidates = candidateOffsets, Links = links } },
					loop,
					extraCells,
					d1,
					d2,
					extraDigit));
		}

		partial void CheckType3(
			IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop,
			in Cells extraCells, IReadOnlyList<Link> links, short comparer)
		{
			bool satisfyType3 = false;
			foreach (int cell in extraCells)
			{
				short mask = grid.GetCandidates(cell);
				if (!mask.Overlaps(comparer) || mask == comparer)
				{
					satisfyType3 = true;
					break;
				}
			}
			if (!extraCells.InOneRegion || satisfyType3)
			{
				return;
			}

			short m = 0;
			foreach (int cell in extraCells)
			{
				m |= grid.GetCandidates(cell);
			}
			if ((m & comparer) != comparer)
			{
				return;
			}

			short otherDigitsMask = (short)(m & ~comparer);
			foreach (int region in extraCells.CoveredRegions)
			{
				if ((ValueMaps[d1] | ValueMaps[d2]).Overlaps(RegionMaps[region]))
				{
					return;
				}

				int[] otherCells = ((RegionMaps[region] & EmptyMap) - loop).ToArray();
				for (int size = otherDigitsMask.PopCount() - 1, count = otherCells.Length; size < count; size++)
				{
					foreach (int[] cells in otherCells.GetSubsets(size))
					{
						short mask = 0;
						foreach (int cell in cells)
						{
							mask |= grid.GetCandidates(cell);
						}

						if (mask.PopCount() != size + 1 || (mask & otherDigitsMask) != otherDigitsMask)
						{
							continue;
						}

						var elimMap = (RegionMaps[region] & EmptyMap) - cells - loop;
						if (elimMap.IsEmpty)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int digit in mask)
						{
							foreach (int cell in elimMap & CandMaps[digit])
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<DrawingInfo>();
						foreach (int cell in loop)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(
									new(otherDigitsMask.ContainsBit(digit) ? 1 : 0, cell * 9 + digit));
							}
						}
						foreach (int cell in cells)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(1, cell * 9 + digit));
							}
						}

						accumulator.AddIfDoesNotContain(
							new ReverseBugType3StepInfo(
								conclusions,
								new View[]
								{
									new()
									{
										Candidates = candidateOffsets,
										Regions = new DrawingInfo[] { new(0, region) },
										Links = links
									}
								},
								loop,
								cells,
								d1,
								d2,
								mask,
								true));
					}
				}
			}
		}

		partial void CheckType4(
			IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop,
			in Cells extraCells, IReadOnlyList<Link> links, short comparer)
		{
			if (!extraCells.InOneRegion)
			{
				return;
			}

			foreach (int region in extraCells.CoveredRegions)
			{
				foreach (var (digit, otherDigit) in stackalloc[] { (d1, d2), (d2, d1) })
				{
					var map = RegionMaps[region] & CandMaps[digit];
					if (map != (RegionMaps[region] & loop))
					{
						continue;
					}

					int[] offsets = extraCells.ToArray();
					int first = offsets[0], second = offsets[1];
					var conclusions = new List<Conclusion>();
					if (grid.Exists(first, otherDigit) is true)
					{
						conclusions.Add(new(ConclusionType.Elimination, first, otherDigit));
					}
					if (grid.Exists(second, otherDigit) is true)
					{
						conclusions.Add(new(ConclusionType.Elimination, second, otherDigit));
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<DrawingInfo>();
					foreach (int cell in loop - extraCells)
					{
						foreach (int d in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(0, cell * 9 + d));
						}
					}
					foreach (int cell in extraCells)
					{
						candidateOffsets.Add(new(1, cell * 9 + digit));
					}

					accumulator.AddIfDoesNotContain(
						new ReverseBugType4StepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Candidates = candidateOffsets,
									Regions = new DrawingInfo[] { new(0, region) },
									Links = links
								}
							},
							loop,
							extraCells,
							d1,
							d2,
							new(first, second, digit)));
				}
			}
		}
	}
}
