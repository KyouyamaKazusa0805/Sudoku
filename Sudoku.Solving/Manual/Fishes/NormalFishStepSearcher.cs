using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Encapsulates a <b>normal fish</b> technique searcher. Fins can also be found.
	/// </summary>
	public sealed class NormalFishStepSearcher : FishStepSearcher
	{
		/// <summary>
		/// The weights.
		/// </summary>
		private const int RowWeight = 9, ColumnWeight = 18;


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(32, nameof(TechniqueCode.XWing))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			for (int size = 2; size <= 4; size++)
			{
				GetAll(accumulator, grid, size, searchRow: true);
				GetAll(accumulator, grid, size, searchRow: false);
			}
		}


		/// <summary>
		/// Searches all basic fish of the specified size.
		/// </summary>
		/// <param name="accumulator">The result accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="size">The size.</param>
		/// <param name="searchRow">
		/// Indicates the solver will searching rows or columns.
		/// </param>
		/// <returns>The result.</returns>
		private static void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid, int size, bool searchRow)
		{
			// Iterate on each digit.
			for (int digit = 0; digit < 9; digit++)
			{
				// A possible normal fish exists if the number of the value of the current digit
				// is lower than or equal to 5.
				if (ValueMaps[digit].Count > 5)
				{
					continue;
				}

				// Confirm the base sets.
				int[] baseTable = (
					searchRow
					? CandMaps[digit].RowMask << RowWeight
					: CandMaps[digit].ColumnMask << ColumnWeight).GetAllSets().ToArray();

				// Iterate on each combination.
				foreach (int[] baseSets in baseTable.GetSubsets(size))
				{
					// Get the cover sets table to iterate.
					var baseSetsMap = Cells.Empty;
					foreach (int baseSet in baseSets)
					{
						var targetMap = RegionMaps[baseSet] & CandMaps[digit];
						if (targetMap.Count < 2)
						{
							goto NextBaseSetsCombination;
						}

						if (targetMap.GetSubviewMask(baseSet).PopCount() > size + 3)
						{
							goto NextBaseSetsCombination;
						}

						baseSetsMap |= targetMap;
					}
					int[] coverTable = (
						searchRow
						? baseSetsMap.ColumnMask << ColumnWeight
						: baseSetsMap.RowMask << RowWeight).GetAllSets().ToArray();

					// Now iterate on each combination in cover sets table.
					foreach (int[] coverSets in coverTable.GetSubsets(size))
					{
						// Now check fin cells and body cells.
						var coverSetsMap = Cells.Empty;
						foreach (int coverSet in coverSets)
						{
							coverSetsMap |= RegionMaps[coverSet];
						}

						var finCells = baseSetsMap - coverSetsMap;

						// Any normal fish doesn't contain more than 4 fins.
						if (finCells.Count > 4)
						{
							continue;
						}

						// Determine whether the cells of elimination sets (cover sets) is not empty.
						var elimMap = coverSetsMap - baseSetsMap;
						if (elimMap.IsEmpty)
						{
							continue;
						}

						// If fins is not empty, we should check the peer intersection
						// to get the real elimination set.
						if (!finCells.IsEmpty)
						{
							elimMap &= finCells.PeerIntersection;
							elimMap &= CandMaps[digit];
						}

						// Determine again.
						if (elimMap.IsEmpty)
						{
							continue;
						}

						// Okay, now the normal fish forms.
						var conclusions = new List<Conclusion>();
						List<DrawingInfo> candidateOffsets = new(), regionOffsets = new();

						foreach (int cell in elimMap)
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
						foreach (int cell in baseSetsMap & coverSetsMap) // Body cells.
						{
							candidateOffsets.Add(new(0, cell * 9 + digit));
						}
						foreach (int cell in finCells)
						{
							candidateOffsets.Add(new(1, cell * 9 + digit));
						}
						foreach (int baseSet in baseSets)
						{
							regionOffsets.Add(new(0, baseSet));
						}
						foreach (int coverSet in coverSets)
						{
							regionOffsets.Add(new(2, coverSet));
						}

						// Gather the result.
						accumulator.Add(
							new NormalFishStepInfo(
								conclusions,
								new View[]
								{
									new() { Candidates = candidateOffsets, Regions = regionOffsets },
									GetDirectView(grid, digit, baseSets, coverSets, finCells, searchRow)
								},
								digit,
								baseSets,
								coverSets,
								finCells,
								finCells.IsEmpty ? null : IsSashimi(baseSets, finCells, digit)));
					}

				NextBaseSetsCombination:;
				}
			}
		}

		/// <summary>
		/// Get the direct fish view with the specified grid and the base sets.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="baseSets">The base sets.</param>
		/// <param name="coverSets">The cover sets.</param>
		/// <param name="fins">
		/// (<see langword="in"/> parameter) The cells of the fin in the current fish.
		/// </param>
		/// <param name="searchRow">Indicates whether the current searcher searches row.</param>
		/// <returns>The view.</returns>
		private static View GetDirectView(
			in SudokuGrid grid, int digit, int[] baseSets, int[] coverSets, in Cells fins, bool searchRow)
		{
			// Get the highlight cells (necessary).
			var cellOffsets = new List<DrawingInfo>();
			var candidateOffsets = fins.IsEmpty ? null : new List<DrawingInfo>();
			foreach (int baseSet in baseSets)
			{
				foreach (int cell in RegionMaps[baseSet])
				{
					switch (grid.Exists(cell, digit))
					{
						case true when fins[cell]:
						{
							cellOffsets.Add(new(1, cell));
							break;
						}
						case false or null:
						{
							bool flag;
							static bool i(int c, in bool searchRow, in int cell) =>
								RegionMaps[(searchRow ? RegionLabel.Column : RegionLabel.Row).ToRegion(c)][cell];
							unsafe
							{
								flag = ValueMaps[digit].Any(&i, searchRow, cell);
							}
							if (flag)
							{
								continue;
							}

							var baseMap = Cells.Empty;
							foreach (int b in baseSets)
							{
								baseMap |= RegionMaps[b];
							}
							var coverMap = Cells.Empty;
							foreach (int c in coverSets)
							{
								coverMap |= RegionMaps[c];
							}
							baseMap &= coverMap;
							if (baseMap[cell])
							{
								continue;
							}

							cellOffsets.Add(new(0, cell));
							break;
						}
						//default:
						//{
						//	// Don't forget this case.
						//	continue;
						//}
					}
				}
			}
			foreach (int cell in ValueMaps[digit])
			{
				cellOffsets.Add(new(2, cell));
			}

			foreach (int cell in fins)
			{
				candidateOffsets!.Add(new(1, cell * 9 + digit));
			}

			return new() { Cells = cellOffsets, Candidates = candidateOffsets };
		}
	}
}
