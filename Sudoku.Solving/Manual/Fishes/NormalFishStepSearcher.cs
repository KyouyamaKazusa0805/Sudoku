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
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(32, nameof(TechniqueCode.XWing))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			unsafe
			{
				int** r = stackalloc int*[9], c = stackalloc int*[9];

				for (int digit = 0; digit < 9; digit++)
				{
					if (ValueMaps[digit].Count > 5)
					{
						continue;
					}

					// Gather.
					for (int region = 9; region < 27; region++)
					{
						if (RegionMaps[region].Overlaps(CandMaps[digit]))
						{
							if (region < 18)
							{
#pragma warning disable CA2014
								if (r[digit] == null)
								{
									int* ptr = stackalloc int[10];
									r[digit] = ptr;
								}
#pragma warning restore CA2014

								r[digit][++r[digit][0]] = region;
							}
							else
							{
#pragma warning disable CA2014
								if (c[digit] == null)
								{
									int* ptr = stackalloc int[10];
									c[digit] = ptr;
								}
#pragma warning restore CA2014

								c[digit][++c[digit][0]] = region;
							}
						}
					}
				}

				for (int size = 2; size <= 4; size++)
				{
					GetAll(accumulator, grid, size, r, c, withFin: false, searchRow: true);
					GetAll(accumulator, grid, size, r, c, withFin: false, searchRow: false);
					GetAll(accumulator, grid, size, r, c, withFin: true, searchRow: true);
					GetAll(accumulator, grid, size, r, c, withFin: true, searchRow: false);
				}
			}
		}

		/// <summary>
		/// Get all possible normal fishes.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="size">The size.</param>
		/// <param name="r">The possible row table to iterate.</param>
		/// <param name="c">The possible column table to iterate.</param>
		/// <param name="withFin">Indicates whether the searcher will check for the existence of fins.</param>
		/// <param name="searchRow">
		/// Indicates whether the searcher searches for fishes in the direction of rows.
		/// </param>
		private static unsafe void GetAll(
			IList<StepInfo> accumulator, in SudokuGrid grid, int size, int** r, int** c,
			bool withFin, bool searchRow)
		{
			// Iterate on each digit.
			int offsetBase = searchRow ? 9 : 18, offsetCover = searchRow ? 18 : 9;
			for (int digit = 0; digit < 9; digit++)
			{
				int* pBase = searchRow ? r[digit] : c[digit], pCover = searchRow ? c[digit] : r[digit];
				if (pBase == null || pBase[0] <= size)
				{
					continue;
				}

				foreach (int[] baseSets in Pointer.GetArrayFromStart(pBase, 10, 1, true).GetSubsets(size))
				{
					var baseLine = size switch
					{
						2 => CandMaps[digit] & (RegionMaps[baseSets[0]] | RegionMaps[baseSets[1]]),
						3 =>
							CandMaps[digit] & (
								RegionMaps[baseSets[0]] | RegionMaps[baseSets[1]]
								| RegionMaps[baseSets[2]]),
						4 =>
							CandMaps[digit] & (
								RegionMaps[baseSets[0]] | RegionMaps[baseSets[1]]
								| RegionMaps[baseSets[2]] | RegionMaps[baseSets[3]])
						
					};

					foreach (int[] coverSets in Pointer.GetArrayFromStart(pCover, 10, 1, true).GetSubsets(size))
					{
						var coverLine = size switch
						{
							2 => CandMaps[digit] & (RegionMaps[coverSets[0]] | RegionMaps[coverSets[1]]),
							3 =>
								CandMaps[digit] & (
									RegionMaps[coverSets[0]] | RegionMaps[coverSets[1]]
									| RegionMaps[coverSets[2]]),
							4 =>
								CandMaps[digit] & (
									RegionMaps[coverSets[0]] | RegionMaps[coverSets[1]]
									| RegionMaps[coverSets[2]] | RegionMaps[coverSets[3]])
						};

						Cells elimMap, fins = Cells.Empty;
						if (!withFin)
						{
							if (baseLine > coverLine || (elimMap = coverLine - baseLine).IsEmpty)
							{
								continue;
							}
						}
						else
						{
							if ((fins = baseLine - coverLine).IsEmpty || !fins.BlockMask.IsPowerOfTwo())
							{
								continue;
							}

							int finBlock = fins.BlockMask.FindFirstSet();
							if (!coverLine.Overlaps(RegionMaps[finBlock]))
							{
								continue;
							}

							if (!RegionMaps[finBlock].Overlaps(coverLine - baseLine))
							{
								continue;
							}

							elimMap = coverLine - baseLine & RegionMaps[finBlock];
						}

						if (elimMap.IsEmpty)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						List<DrawingInfo> candidateOffsets = new(), regionOffsets = new();
						foreach (int cell in elimMap)
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
						foreach (int cell in withFin ? baseLine - fins : baseLine)
						{
							candidateOffsets.Add(new(0, cell * 9 + digit));
						}
						if (withFin)
						{
							foreach (int cell in fins)
							{
								candidateOffsets.Add(new(1, cell * 9 + digit));
							}
						}
						foreach (int baseSet in baseSets)
						{
							regionOffsets.Add(new(0, baseSet));
						}
						foreach (int coverSet in coverSets)
						{
							regionOffsets.Add(new(2, coverSet));
						}

						accumulator.Add(
							new NormalFishStepInfo(
								conclusions,
								new View[]
								{
									new() { Candidates = candidateOffsets, Regions = regionOffsets },
									GetDirectView(grid, digit, baseSets, coverSets, fins, searchRow)
								},
								digit,
								baseSets,
								coverSets,
								fins,
								IsSashimi(baseSets, fins, digit)));
					}
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
