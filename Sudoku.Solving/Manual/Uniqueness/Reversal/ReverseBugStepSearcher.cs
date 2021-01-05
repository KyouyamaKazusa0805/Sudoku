using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Extensions;
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
			for (int d1 = 0; d1 < 8; d1++)
			{
				for (int d2 = d1 + 1; d2 < 9; d2++)
				{
					Cells possibleLoop = ValueMaps[d1] | ValueMaps[d2], possibleEmptyMap = Cells.Empty;
					foreach (int region in possibleLoop.Regions)
					{
						possibleEmptyMap |= RegionMaps[region] & EmptyMap;
					}

#if SO_SLOW_THAT_I_DISABLED_THIS_BLOCK
					int[] emptyMapCells = possibleEmptyMap.Offsets;
					for (int size = 1; size <= 3/*10*/; size++)
					{
						foreach (Cells cells in emptyMapCells.GetSubsets(size))
						{
							var currMap = possibleLoop | cells;
							if (!ContainsValidPath(currMap, out _, out var links))
							{
								continue;
							}

							short comparer = (short)(1 << d1 | 1 << d2);
							switch (size)
							{
								case 1:
								{
									CheckType1(resultAccumulator, grid, d1, d2, currMap, cells.Offsets[0], links, comparer);
									break;
								}
								case 2: /*fallthrough*/
								{
									CheckType3(resultAccumulator, grid, d1, d2, currMap, cells, links, comparer);
									CheckType4(resultAccumulator, grid, d1, d2, currMap, cells, links, comparer);
									goto default;
								}
								default:
								{
									CheckType2(resultAccumulator, grid, d1, d2, currMap, cells, links, comparer);
									break;
								}
							}
						}
					}
#else
					foreach (int cell in possibleEmptyMap)
					{
						var currMap = possibleLoop + cell;
						if (!ContainsValidPath(currMap, out _, out var links))
						{
							continue;
						}

						short comparer = (short)(1 << d1 | 1 << d2);
						CheckType1(resultAccumulator, grid, d1, d2, currMap, cell, links, comparer);
					}
#endif
				}
			}

			accumulator.AddRange(resultAccumulator);
		}


		/// <summary>
		/// Check whether the specified cells form a valid unique loop (or rectangle).
		/// </summary>
		/// <param name="cells">(<see langword="in"/> parameter) The cells.</param>
		/// <param name="list">
		/// (<see langword="out"/> parameter) If found any valid loop, the value will be the cells in order;
		/// otherwise, <see langword="null"/>.
		/// </param>
		/// <param name="links">
		/// (<see langword="out"/> parameter) If found any valid loop, the value will be 
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		private static bool ContainsValidPath(
			in Cells cells,
			[NotNullWhen(true)] out IReadOnlyList<int>? list,
			[NotNullWhen(true)] out IReadOnlyList<Link>? links)
		{
			bool flag = false;
			var loopMap = Cells.Empty;
			var tempLoop = new List<int>();
			IReadOnlyList<Link>? tempLinks = null;
			try
			{
				f(cells[0], (RegionLabel)byte.MaxValue, cells, ref flag);
			}
			catch (SudokuHandlingException)
			{
			}

			if (flag)
			{
				list = tempLoop;
				links = tempLinks!;
				return true;
			}
			else
			{
				list = null;
				links = null;
				return false;
			}

			void f(int cell, RegionLabel lastLabel, in Cells cells, ref bool flag)
			{
				tempLoop.Add(cell);
				loopMap.AddAnyway(cell);

				for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
				{
					if (label == lastLabel)
					{
						continue;
					}

					int region = label.ToRegion(cell);
					var cellsMap = RegionMaps[region] & cells - cell;
					if (cellsMap.IsEmpty)
					{
						continue;
					}

					foreach (int nextCell in cellsMap)
					{
						if (tempLoop[0] == nextCell && tempLoop.Count >= 4 && tempLoop.IsValidLoop())
						{
							if (loopMap == cells)
							{
								// The loop is closed. Now construct the result pair.
								flag = true;
								tempLinks = tempLoop.GetLinks();

								// Break the recursion.
								throw new SudokuHandlingException();
							}
							else
							{
								// TODO: Grouping cells.
							}
						}
						else if (!loopMap.Contains(nextCell))
						{
							f(nextCell, label, cells, ref flag);
						}
					}
				}

				// Backtracking.
				tempLoop.RemoveLastElement();
				loopMap.Remove(cell);
			}
		}


		partial void CheckType1(IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop, int extraCell, IReadOnlyList<Link> links, short comparer);
		partial void CheckType2(IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop, in Cells extraCells, IReadOnlyList<Link> links, short comparer);
		partial void CheckType3(IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop, in Cells extraCells, IReadOnlyList<Link> links, short comparer);
		partial void CheckType4(IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop, in Cells extraCells, IReadOnlyList<Link> links, short comparer);
	}
}
