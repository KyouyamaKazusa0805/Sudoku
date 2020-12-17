using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Extensions;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Runtime;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Extensions;
using static Sudoku.Constants.Processings;

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
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			var resultAccumulator = new List<ReverseBugStepInfo>();
			for (int d1 = 0; d1 < 8; d1++)
			{
				for (int d2 = d1 + 1; d2 < 9; d2++)
				{
					var possibleLoop = ValueMaps[d1] | ValueMaps[d2];

					int[] emptyMapCells = EmptyMap.Offsets;
					for (int i = 0, length = emptyMapCells.Length; i < length; i++)
					{
						int cell = emptyMapCells[i];
						var currMap = possibleLoop + cell;
						if (!ContainsValidPath(currMap, out _, out var links))
						{
							continue;
						}

						CheckType1(resultAccumulator, grid, d1, d2, currMap, links, cell);
					}
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
		/// <returns></returns>
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
				f(cells.Offsets[0], (RegionLabel)byte.MaxValue, cells, ref flag);
			}
			catch (SudokuRuntimeException)
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
						if (tempLoop[0] == nextCell && tempLoop.Count >= 4 && LoopIsValid(tempLoop))
						{
							if (loopMap == cells)
							{
								// The loop is closed. Now construct the result pair.
								flag = true;
								tempLinks = tempLoop.GetLinks();

								// Break the recursion.
								throw new SudokuRuntimeException();
							}
							else
							{
								// TODO: Grouping cells.
							}
						}
						else if (!loopMap[nextCell])
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

		/// <summary>
		/// To check whether the specified loop is valid.
		/// </summary>
		/// <param name="loop">The loop.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		[SkipLocalsInit]
		private static unsafe bool LoopIsValid(IReadOnlyList<int> loop)
		{
			int visitedOddRegions = 0, visitedEvenRegions = 0;
			bool isOdd;
			foreach (int cell in loop)
			{
				for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
				{
					int region = label.ToRegion(cell);
					if (*&isOdd)
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

				(&isOdd)->Flip();
			}

			return visitedEvenRegions == visitedOddRegions;
		}

		partial void CheckType1(IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop, IReadOnlyList<Link> links, int extraCell);
		partial void CheckType2(IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop, in Cells extraCellsMap, IReadOnlyList<Link> links, short comparer);
		partial void CheckType3(IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop, in Cells extraCellsMap, IReadOnlyList<Link> links, short comparer);
		partial void CheckType4(IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop, in Cells extraCellsMap, IReadOnlyList<Link> links, short comparer);
	}
}
