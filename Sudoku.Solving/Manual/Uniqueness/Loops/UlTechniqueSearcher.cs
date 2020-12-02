using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Encapsulates a <b>unique loop</b> (UL) technique searcher.
	/// In fact the unique loop can also search for URs.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.UlType1))]
	public sealed partial class UlTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(46) { DisplayLevel = 2 };


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
		{
			if (BivalueMap.Count < 6)
			{
				return;
			}

			var resultAccumulator = new List<UlTechniqueInfo>();
			var loops = new List<(GridMap Map, IReadOnlyList<Link> Links)>();
			var tempLoop = new List<int>();
			foreach (int cell in BivalueMap)
			{
				short mask = grid.GetCandidateMask(cell);
				int d1 = mask.FindFirstSet();
				int d2 = mask.GetNextSet(d1);
				var loopMap = GridMap.Empty;
				loops.Clear();
				tempLoop.Clear();

				f(grid, d1, d2, cell, (RegionLabel)(-1), default, 2);

				if (loops.Count == 0)
				{
					continue;
				}

				short comparer = (short)(1 << d1 | 1 << d2);
				foreach (var (currentLoop, links) in loops)
				{
					var extraCellsMap = currentLoop - BivalueMap;
					switch (extraCellsMap.Count)
					{
						//case <= 0:
						//{
						//	// Invalid grid status.
						//	throw NoSolutionException(grid);
						//}
						case 1:
						{
							// Type 1.
							CheckType1(resultAccumulator, grid, d1, d2, currentLoop, links, extraCellsMap);

							break;
						}
						default:
						{
							// Type 2, 3, 4.
							// Here use default label to ensure the order of
							// the handling will be 1->2->3->4.
							CheckType2(
								resultAccumulator, grid, d1, d2, currentLoop, links, extraCellsMap, comparer);

							if (extraCellsMap.Count == 2)
							{
								CheckType3(
									resultAccumulator, grid, d1, d2, currentLoop, links,
									extraCellsMap, comparer);
								CheckType4(
									resultAccumulator, grid, d1, d2, currentLoop, links,
									extraCellsMap, comparer);
							}

							break;
						}
					}
				}

				unsafe
				{
					var set = new Set<UlTechniqueInfo>(resultAccumulator);
					resultAccumulator.Clear();
					resultAccumulator.AddRange(set);
					resultAccumulator.Sort(&cmp);
					accumulator.AddRange(resultAccumulator);
				}

				static int cmp(in UlTechniqueInfo l, in UlTechniqueInfo r)
				{
					int ll = l.Loop.Count, rr = r.Loop.Count;
					return ll.CompareTo(rr);
				}

				void f(
					in SudokuGrid grid,
					int d1, int d2, int cell, RegionLabel lastLabel, short exDigitsMask,
					int allowedExtraCellsCount)
				{
					loopMap.AddAnyway(cell);
					tempLoop.Add(cell);

					for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
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
							if (tempLoop[0] == nextCell && tempLoop.Count >= 6 && LoopIsValid(tempLoop))
							{
								// The loop is closed. Now construct the result pair.
								var links = new List<Link>();
								for (int i = 0; i < tempLoop.Count - 1; i++)
								{
									links.Add(new(tempLoop[i] * 9 + d1, tempLoop[i + 1] * 9 + d1, LinkType.Line));
								}
								links.Add(new(tempLoop[^1] * 9 + d1, tempLoop[0] * 9 + d1, LinkType.Line));

								loops.Add((loopMap, links));
							}
							else if (!loopMap[nextCell] && grid[nextCell, d1] && grid[nextCell, d2])
							{
								// Here, unique loop can be found if and only if
								// two cells both contain 'd1' and 'd2'.
								// Incomplete ULs can't be found at present.
								short nextCellMask = grid.GetCandidateMask(nextCell);
								exDigitsMask |= nextCellMask;
								exDigitsMask &= (short)~((1 << d1) | (1 << d2));
								int digitsCount = nextCellMask.PopCount();

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
									grid, d1, d2, nextCell, label, exDigitsMask,
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

		partial void CheckType1(IList<UlTechniqueInfo> accumulator, in SudokuGrid grid, int d1, int d2, in GridMap loop, IReadOnlyList<Link> links, in GridMap extraCellsMap);
		partial void CheckType2(IList<UlTechniqueInfo> accumulator, in SudokuGrid grid, int d1, int d2, in GridMap loop, IReadOnlyList<Link> links, in GridMap extraCellsMap, short comparer);
		partial void CheckType3(IList<UlTechniqueInfo> accumulator, in SudokuGrid grid, int d1, int d2, in GridMap loop, IReadOnlyList<Link> links, in GridMap extraCellsMap, short comparer);
		partial void CheckType4(IList<UlTechniqueInfo> accumulator, in SudokuGrid grid, int d1, int d2, in GridMap loop, IReadOnlyList<Link> links, in GridMap extraCellsMap, short comparer);
	}
}
