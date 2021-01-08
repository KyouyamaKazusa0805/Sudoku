using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Solving.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Uniqueness.Reversal
{
	/// <summary>
	/// Encapsulates a <b>reverse bi-value universal grave</b> technique searcher.
	/// </summary>
	public sealed partial class ReverseBugStepSearcher : UniquenessStepSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(56, nameof(TechniqueCode.ReverseUrType1))
		{
			DisplayLevel = 2,
			IsEnabled = false,
			DisabledReason = DisabledReason.TooSlow
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			var resultAccumulator = new List<ReverseBugStepInfo>();

			// Gather all value cells.
			var valueMap = Cells.Empty;
			foreach (var eachValueMap in ValueMaps)
			{
				valueMap |= eachValueMap;
			}

			// Now iterate on each combination of pair cells.
			int[] possibleCells = valueMap.ToArray();
			for (int i = 0, length = possibleCells.Length - 1; i < length; i++)
			{
				int cell1 = possibleCells[i];
				foreach (int cell2 in (PeerMaps[cell1] & valueMap) - cell1)
				{
					if (cell2 < cell1)
					{
						continue;
					}

					// Find all possible loops.
					// Iterate on each loop.
					int d1 = grid.GetCandidates(cell1).FindFirstSet();
					int d2 = grid.GetCandidates(cell2).FindFirstSet();
					short comparer = (short)(1 << d1 | 1 << d2);
					var loops = FindPossibleLoops(grid, cell1, cell2, d1, d2);
					if (loops.Count == 0)
					{
						continue;
					}

					foreach (var (loop, links) in loops)
					{
						var extraCells = EmptyMap & loop;
						switch (extraCells.Count)
						{
							//case 0:
							//{
							//	throw new SudokuHandlingException();
							//}
							case 1:
							{
								CheckType1(resultAccumulator, grid, d1, d2, loop, extraCells[0], links, comparer);
								break;
							}
							case 2: /*fallthrough*/
							{
								CheckType3(resultAccumulator, grid, d1, d2, loop, extraCells, links, comparer);
								CheckType4(resultAccumulator, grid, d1, d2, loop, extraCells, links, comparer);
								goto default;
							}
							default:
							{
								CheckType2(resultAccumulator, grid, d1, d2, loop, extraCells, links, comparer);
								break;
							}
						}
					}
				}
			}

			accumulator.AddRange(
				from info in resultAccumulator.RemoveDuplicateItems()
				orderby info.TechniqueCode
				select info);
		}

		/// <summary>
		/// Find all possible loops used for checking each type.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="cell1">The cell 1.</param>
		/// <param name="cell2">The cell 2.</param>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <returns>All possible loops.</returns>
		private static IReadOnlyList<(Cells Loop, IReadOnlyList<Link> Links)> FindPossibleLoops(
			in SudokuGrid grid, int cell1, int cell2, int d1, int d2)
		{
			var result = new List<(Cells, IReadOnlyList<Link>)>();
			var loop = new List<int> { cell1 };
			Cells pairMap = new() { cell1, cell2 }, loopMap = new() { cell1 };
			short digitsMask = (short)(1 << d1 | 1 << d2);

			try
			{
				f(grid, ValueMaps[d1] | ValueMaps[d2], cell2, d1, digitsMask, 2);
			}
			catch (SudokuHandlingException)
			{
				return result;
			}

			void f(
				in SudokuGrid grid, in Cells values,
				int cell, int anotherDigit, short digitsMask, int lastEmptyCellsCount)
			{
				if (lastEmptyCellsCount < 0)
				{
					return;
				}

				loop.Add(cell);
				loopMap.AddAnyway(cell);

				foreach (int nextCell in
					PeerMaps[cell] - new Cells { loop[^2], cell }.PeerIntersection
					& DigitMaps[anotherDigit])
				{
					if (loop[0] == nextCell && loop.Count >= 4 && loop.IsValidLoop())
					{
						// The loop is valid.
						if (values == loopMap - EmptyMap)
						{
							result.Add((loopMap, loop.GetLinks()));

							throw new SudokuHandlingException();
						}
						else
						{
							// TODO: Grouping the loop into several separate ones.
						}
					}
					else if (!loopMap.Contains(nextCell))
					{
						short mask = (short)(digitsMask & ~grid.GetCandidates(nextCell));
						if (mask == 0)
						{
							continue;
						}

						f(
							grid, values, nextCell, mask.FindFirstSet(), digitsMask,
							EmptyMap.Contains(nextCell) ? lastEmptyCellsCount - 1 : lastEmptyCellsCount);
					}
				}

				// Backtracking.
				loop.RemoveLastElement();
				loopMap.Remove(cell);
			}

			return result;
		}


		partial void CheckType1(IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop, int extraCell, IReadOnlyList<Link> links, short comparer);
		partial void CheckType2(IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop, in Cells extraCells, IReadOnlyList<Link> links, short comparer);
		partial void CheckType3(IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop, in Cells extraCells, IReadOnlyList<Link> links, short comparer);
		partial void CheckType4(IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop, in Cells extraCells, IReadOnlyList<Link> links, short comparer);
	}
}
