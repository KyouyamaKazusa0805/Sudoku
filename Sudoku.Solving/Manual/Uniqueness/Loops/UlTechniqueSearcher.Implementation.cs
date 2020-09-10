using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	partial class UlTechniqueSearcher
	{
		/// <summary>
		/// Check type 1.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <param name="loop">The loop.</param>
		/// <param name="extraCellsMap">The extra cells map.</param>
		partial void CheckType1(
			IList<TechniqueInfo> accumulator, Grid grid, int d1, int d2,
			GridMap loop, GridMap extraCellsMap)
		{
			int extraCell = extraCellsMap.First;
			var conclusions = new List<Conclusion>();
			if (grid.Exists(extraCell, d1) is true)
			{
				conclusions.Add(new(Elimination, extraCell, d1));
			}
			if (grid.Exists(extraCell, d2) is true)
			{
				conclusions.Add(new(Elimination, extraCell, d2));
			}
			if (conclusions.Count == 0)
			{
				return;
			}

			var candidateOffsets = new List<(int, int)>();
			foreach (int cell in new GridMap(loop) { ~extraCell })
			{
				candidateOffsets.Add((0, cell * 9 + d1));
				candidateOffsets.Add((0, cell * 9 + d2));
			}

			accumulator.AddIfDoesNotContain(
				new UlType1TechniqueInfo(
					conclusions,
					views: new[] { new View(candidateOffsets) },
					d1,
					d2,
					loop));
		}

		/// <summary>
		/// Check type 2.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <param name="loop">The loop.</param>
		/// <param name="extraCellsMap">The extra cells map.</param>
		/// <param name="comparer">The comparer mask (equals to <c>1 &lt;&lt; d1 | 1 &lt;&lt; d2</c>).</param>
		partial void CheckType2(
			IList<TechniqueInfo> accumulator, Grid grid, int d1, int d2,
			GridMap loop, GridMap extraCellsMap, short comparer)
		{
			short mask = 0;
			foreach (int cell in extraCellsMap)
			{
				mask |= grid.GetCandidateMask(cell);
			}
			mask &= (short)~comparer;

			if (!mask.IsPowerOfTwo())
			{
				return;
			}

			int extraDigit = mask.FindFirstSet();
			var elimMap = (extraCellsMap & CandMaps[extraDigit]).PeerIntersection & CandMaps[extraDigit];
			if (elimMap.IsEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int cell in elimMap)
			{
				conclusions.Add(new(Elimination, cell, extraDigit));
			}

			var candidateOffsets = new List<(int, int)>();
			foreach (int cell in loop)
			{
				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add((digit == extraDigit ? 1 : 0, cell * 9 + digit));
				}
			}

			accumulator.AddIfDoesNotContain(
				new UlType2TechniqueInfo(
					conclusions,
					views: new[] { new View(candidateOffsets) },
					d1,
					d2,
					loop,
					extraDigit));
		}

		/// <summary>
		/// Check type 3.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <param name="loop">The loop.</param>
		/// <param name="extraCellsMap">The extra cells map.</param>
		/// <param name="comparer">The comparer mask (equals to <c>1 &lt;&lt; d1 | 1 &lt;&lt; d2</c>).</param>
		partial void CheckType3(
			IList<TechniqueInfo> accumulator, Grid grid, int d1, int d2,
			GridMap loop, GridMap extraCellsMap, short comparer)
		{
			if (!extraCellsMap.AllSetsAreInOneRegion(out _)
				|| extraCellsMap.Any(c =>
				{
					short mask = grid.GetCandidateMask(c);
					return (mask & comparer) == 0 || mask == comparer;
				}))
			{
				return;
			}

			short m = 0;
			foreach (int cell in extraCellsMap)
			{
				m |= grid.GetCandidateMask(cell);
			}
			if ((m & comparer) != comparer)
			{
				return;
			}

			short otherDigitsMask = (short)(m & ~comparer);
			foreach (int region in extraCellsMap.CoveredRegions)
			{
				if ((ValueMaps[d1] | ValueMaps[d2]).Overlaps(RegionMaps[region]))
				{
					return;
				}

				int[] otherCells = ((RegionMaps[region] & EmptyMap) - loop).ToArray();
				for (int size = otherDigitsMask.CountSet() - 1, count = otherCells.Length; size < count; size++)
				{
					foreach (int[] cells in otherCells.GetSubsets(size))
					{
						short mask = 0;
						foreach (int cell in cells)
						{
							mask |= grid.GetCandidateMask(cell);
						}

						if (mask.CountSet() != size + 1 || (mask & otherDigitsMask) != otherDigitsMask)
						{
							continue;
						}

						var elimMap = (RegionMaps[region] & EmptyMap) - cells - loop;
						if (elimMap.IsEmpty)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int digit in mask.GetAllSets())
						{
							foreach (int cell in elimMap & CandMaps[digit])
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<(int, int)>();
						foreach (int cell in loop)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(
									((otherDigitsMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
							}
						}
						foreach (int cell in cells)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add((1, cell * 9 + digit));
							}
						}

						accumulator.AddIfDoesNotContain(
							new UlType3TechniqueInfo(
								conclusions,
								views: new[] { new View(null, candidateOffsets, new[] { (0, region) }, null) },
								d1,
								d2,
								loop,
								subsetDigitsMask: mask,
								subsetCells: cells));
					}
				}
			}
		}

		/// <summary>
		/// Check type 4.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <param name="loop">The loop.</param>
		/// <param name="extraCellsMap">The extra cells map.</param>
		/// <param name="comparer">The comparer mask (equals to <c>1 &lt;&lt; d1 | 1 &lt;&lt; d2</c>).</param>
		partial void CheckType4(
			IList<TechniqueInfo> accumulator, Grid grid, int d1, int d2,
			GridMap loop, GridMap extraCellsMap, short comparer)
		{
			if (!extraCellsMap.AllSetsAreInOneRegion(out _))
			{
				return;
			}

			foreach (int region in extraCellsMap.CoveredRegions)
			{
				foreach (var (digit, otherDigit) in stackalloc[] { (d1, d2), (d2, d1) })
				{
					var map = RegionMaps[region] & CandMaps[digit];
					if (map != (RegionMaps[region] & loop))
					{
						continue;
					}

					int first = extraCellsMap.First;
					int second = extraCellsMap.SetAt(1);
					var conclusions = new List<Conclusion>();
					if (grid.Exists(first, otherDigit) is true)
					{
						conclusions.Add(new(Elimination, first, otherDigit));
					}
					if (grid.Exists(second, otherDigit) is true)
					{
						conclusions.Add(new(Elimination, second, otherDigit));
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<(int, int)>();
					foreach (int cell in loop - extraCellsMap)
					{
						foreach (int d in grid.GetCandidates(cell))
						{
							candidateOffsets.Add((0, cell * 9 + d));
						}
					}
					foreach (int cell in extraCellsMap)
					{
						candidateOffsets.Add((1, cell * 9 + digit));
					}

					accumulator.AddIfDoesNotContain(
						new UlType4TechniqueInfo(
							conclusions,
							views: new[] { new View(null, candidateOffsets, new[] { (0, region) }, null) },
							d1,
							d2,
							loop,
							conjugatePair: new(first, second, digit)));
				}
			}
		}


		/// <summary>
		/// To check whether the specified loop is valid.
		/// </summary>
		/// <param name="loop">The loop.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		static bool LoopIsValid(IReadOnlyList<int> loop)
		{
			int visitedOddRegions = 0, visitedEvenRegions = 0;
			bool isOdd = default;
			foreach (int cell in loop)
			{
				for (var label = Block; label <= Column; label++)
				{
					int region = GetRegion(cell, label);
					if (isOdd)
					{
						if ((visitedOddRegions >> region & 1) != 0)
						{
							return false;
						}
						else
						{
							visitedOddRegions |= 1 << region;
						}
					}
					else
					{
						if ((visitedEvenRegions >> region & 1) != 0)
						{
							return false;
						}
						else
						{
							visitedEvenRegions |= 1 << region;
						}
					}
				}

				isOdd.Flip();
			}

			return visitedEvenRegions == visitedOddRegions;
		}
	}
}
