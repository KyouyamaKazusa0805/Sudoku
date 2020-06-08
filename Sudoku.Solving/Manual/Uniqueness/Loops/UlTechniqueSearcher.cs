using System.Collections.Generic;
using System.Linq;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static System.Algorithms;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Encapsulates a <b>unique loop</b> (UL) technique searcher.
	/// In fact the unique loop can also search for URs.
	/// </summary>
	[TechniqueDisplay("Unique Loop")]
	public sealed class UlTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 46;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			if (BivalueMap.Count < 6)
			{
				return;
			}

			var loops = new List<GridMap>();
			var tempLoop = new List<int>();
			foreach (int cell in BivalueMap)
			{
				short mask = grid.GetCandidateMask(cell);
				int d1 = mask.FindFirstSet();
				int d2 = mask.GetNextSet(d1);
				var loopMap = GridMap.Empty;
				loops.Clear();
				tempLoop.Clear();

				f(d1, d2, cell, LowerLimit, default, 2);

				if (loops.Count == 0)
				{
					continue;
				}

				short comparer = (short)(1 << d1 | 1 << d2);
				foreach (var currentLoop in loops)
				{
					var extraCellsMap = currentLoop - BivalueMap;
					switch (extraCellsMap.Count)
					{
						//case int d when d <= 0:
						//{
						//	// Invalid grid status.
						//	throw Throwings.ImpossibleCase;
						//}
						case 1:
						{
							// Type 1.
							CheckType1(accumulator, grid, d1, d2, currentLoop, extraCellsMap);

							break;
						}
						default:
						{
							// Type 2, 3, 4.
							// Here use default label to ensure the order of
							// the handling will be 1->2->3->4.
							CheckType2(accumulator, grid, d1, d2, currentLoop, extraCellsMap, comparer);

							if (extraCellsMap.Count == 2)
							{
								CheckType3(accumulator, grid, d1, d2, currentLoop, extraCellsMap, comparer);
								CheckType4(accumulator, grid, d1, d2, currentLoop, extraCellsMap, comparer);
							}

							break;
						}
					}
				}

				void f(
					int d1, int d2, int cell, RegionLabel lastLabel,
					short exDigitsMask, int allowedExtraCellsCount)
				{
					loopMap.Add(cell);
					tempLoop.Add(cell);

					for (var label = Block; label <= Column; label++)
					{
						if (label == lastLabel)
						{
							continue;
						}

						int region = GetRegion(cell, label);
						var cellsMap = RegionMaps[region] & EmptyMap - cell;
						if (cellsMap.IsEmpty)
						{
							continue;
						}

						foreach (int nextCell in cellsMap)
						{
							if (tempLoop[0] == nextCell && tempLoop.Count >= 6 && LoopIsValid(tempLoop)
								&& !loops.Contains(loopMap))
							{
								// The loop is closed.
								loops.Add(loopMap);
							}
							else if (!loopMap[nextCell] && !grid[nextCell, d1] && !grid[nextCell, d2])
							{
								// Here, unique loop can be found if and only if
								// two cells both contain 'd1' and 'd2'.
								// Incompleted ULs cannot be found at present.
								short nextCellMask = grid.GetCandidateMask(nextCell);
								exDigitsMask |= nextCellMask;
								exDigitsMask &= (short)~((1 << d1) | (1 << d2));
								int digitsCount = nextCellMask.CountSet();

								// We can continue if:
								// - The cell has exactly 2 digits of the loop.
								// - The cell has 1 extra digit, the same as all previous cells
								// with an extra digit (for type 2 only).
								// - The cell has extra digits and the maximum number of cells
								// with extra digits, 2, is not reached.
								if (digitsCount != 2 && !exDigitsMask.IsPowerOfTwo()
									&& allowedExtraCellsCount <= 0)
								{
									continue;
								}

								f(
									d1, d2, nextCell, label, exDigitsMask,
									digitsCount > 2 ? allowedExtraCellsCount - 1 : allowedExtraCellsCount);
							}
						}
					}

					// Backtracking.
					loopMap.Remove(cell);
					tempLoop.RemoveLastElement();
				}
			}
		}

		/// <summary>
		/// Check type 1.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <param name="loop">The loop.</param>
		/// <param name="extraCellsMap">The extra cells map.</param>
		private void CheckType1(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, int d1, int d2,
			GridMap loop, GridMap extraCellsMap)
		{
			int extraCell = extraCellsMap.SetAt(0);
			var conclusions = new List<Conclusion>();
			if (grid.Exists(extraCell, d1) is true)
			{
				conclusions.Add(new Conclusion(Elimination, extraCell, d1));
			}
			if (grid.Exists(extraCell, d2) is true)
			{
				conclusions.Add(new Conclusion(Elimination, extraCell, d2));
			}
			if (conclusions.Count == 0)
			{
				return;
			}

			var candidateOffsets = new List<(int, int)>();
			foreach (int cell in loop - extraCell)
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
		private void CheckType2(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, int d1, int d2,
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
				conclusions.Add(new Conclusion(Elimination, cell, extraDigit));
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
		private void CheckType3(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, int d1, int d2,
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
					foreach (int[] cells in GetCombinationsOfArray(otherCells, size))
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

						var elimMap = (RegionMaps[region] & EmptyMap) - new GridMap(cells) - loop;
						if (elimMap.IsEmpty)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int digit in mask.GetAllSets())
						{
							foreach (int cell in elimMap & CandMaps[digit])
							{
								conclusions.Add(new Conclusion(Elimination, cell, digit));
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
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: new[] { (0, region) },
										links: null)
								},
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
		private void CheckType4(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, int d1, int d2,
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

					int first = extraCellsMap.SetAt(0);
					int second = extraCellsMap.SetAt(1);
					var conclusions = new List<Conclusion>();
					if (grid.Exists(first, otherDigit) is true)
					{
						conclusions.Add(new Conclusion(Elimination, first, otherDigit));
					}
					if (grid.Exists(second, otherDigit) is true)
					{
						conclusions.Add(new Conclusion(Elimination, second, otherDigit));
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
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: new[] { (0, region) },
									links: null)
							},
							d1,
							d2,
							loop,
							conjugatePair: new ConjugatePair(first, second, digit)));
				}
			}
		}


		/// <summary>
		/// To check whether the specified loop is valid.
		/// </summary>
		/// <param name="loop">The loop.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		private static bool LoopIsValid(IReadOnlyList<int> loop)
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
