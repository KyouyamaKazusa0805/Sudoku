#define OLD_ALGORITHM

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates a <b>death blossom</b> technique.
	/// </summary>
	public sealed class DbStepSearcher : AlsStepSearcher
	{
		/// <inheritdoc/>
		public override SearchingOptions Options { get; set; } = new(
			31,
			DisplayingLevel.C,
			Stableness: Stableness.Unstable,
#if OLD_ALGORITHM
			EnabledAreas: EnabledAreas.None,
			DisabledReason: DisabledReason.TooSlow
#endif
		);

		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		[Obsolete("Please use the property '" + nameof(Options) + "' instead.", false)]
		public static TechniqueProperties Properties { get; } = new(31, nameof(Technique.DeathBlossom))
		{
			DisplayLevel = 3,
#if OLD_ALGORITHM
			IsEnabled = false,
			DisabledReason = DisabledReason.TooSlow | DisabledReason.Unstable
#endif
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			const int maxPetals = 5;

			var tempAccumulator = new List<DbStepInfo>();
			short[] checkedCandidates = new short[81];
			int[,] death = new int[729, 1000];
			var alsList = PreprocessAndGatherAlses(grid, EmptyMap);
			ProcessDeathAlsInfo(grid, CandMaps, checkedCandidates, death, alsList);

			for (int pivot = 0; pivot < 81; pivot++)
			{
				if (grid.GetStatus(pivot) != CellStatus.Empty
					|| checkedCandidates[pivot] != grid.GetCandidates(pivot)
					|| PopCount((uint)checkedCandidates[pivot]) > maxPetals)
				{
					continue;
				}

				short cands = grid.GetCandidates(pivot);
				int digitsCount = PopCount((uint)cands);
				short[] allZ = new short[digitsCount];
				int[] stack = new int[digitsCount];

				var digits = cands.GetAllSets();
				int n = 0;
				while (n >= 0)
				{
					bool flag = true;
					int digit = digits[n];
					for (int i = stack[n] + 1; i <= death[pivot * 9 + digit, 0]; i++)
					{
						short value = (short)(alsList[death[pivot * 9 + digit, i]].DigitsMask & ~cands);
						allZ[n] = (short)(n == 0 ? value : (allZ[n - 1] & value));

						if (allZ[n] > 0)
						{
							stack[n] = i;
							flag = false;
							break;
						}
					}

					if (flag)
					{
						stack[n--] = 0;
					}
					else if (n == digitsCount - 1)
					{
						int k = 0;
						var temp = Cells.Empty;
						foreach (int d in digits)
						{
							var map = alsList[death[pivot * 9 + d, stack[k]]].Map;
							temp = k++ == 0 ? map : temp | map;
						}

						if (temp.InOneRegion || allZ[n] == 0)
						{
							// All in same region means that they with target cell together
							// forms a naked subset.
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int d in allZ[n])
						{
							var elimMap = (temp & CandMaps[d]).PeerIntersection & CandMaps[d];
							if (elimMap.IsEmpty)
							{
								continue;
							}

							foreach (int cell in elimMap)
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, d));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						// Sort all used ALSes into the dictionary.
						var dic = new Dictionary<int, Als>();
						k = 0;
						foreach (int d in digits)
						{
							dic.Add(d, alsList[death[pivot * 9 + d, stack[k++]]]);
						}

						// Check overlap ALSes.
						if (!AllowOverlapping)
						{
							var alsesUsed = dic.Values.ToArray();
							bool overlap = false;
							for (
								int i = 0, length = alsesUsed.Length, outerLoopIterationLength = length - 1;
								i < outerLoopIterationLength;
								i++
							)
							{
								for (int j = i + 1; j < length; j++)
								{
									if (!(alsesUsed[i].Map & alsesUsed[j].Map).IsEmpty)
									{
										overlap = true;
										goto LastCheck;
									}
								}
							}

						LastCheck:
							if (overlap)
							{
								continue;
							}
						}

						// Record all highlight cells.
						var cellOffsets = new List<DrawingInfo>();
						int z = 0;
						foreach (var (d, a) in dic)
						{
							foreach (int c in a.Map)
							{
								cellOffsets.Add(new(-z - 1, c));
							}

							z = (z + 1) % 4;
						}

						// Record all highlight candidates.
						var candidateOffsets = new List<DrawingInfo>();
						foreach (int pivotDigit in grid.GetCandidates(pivot))
						{
							candidateOffsets.Add(new(0, pivot * 9 + pivotDigit));
						}

						z = 0;
						foreach (var (d, a) in dic)
						{
							foreach (int c in a.Map)
							{
								foreach (int dd in grid.GetCandidates(c))
								{
									candidateOffsets.Add(new(d == dd ? 1 : -z - 1, c * 9 + dd));
								}
							}

							z = (z + 1) % 4;
						}

						// Record all highlight regions.
						var regionOffsets = new List<DrawingInfo>();
						z = 0;
						foreach (var als in dic.Values)
						{
							regionOffsets.Add(new(-z - 1, als.Region));

							z = (z + 1) % 4;
						}

						// Add item.
						tempAccumulator.Add(
							new DbStepInfo(
								conclusions,
								new View[]
								{
									new()
									{
										Cells = AlsShowRegions ? cellOffsets : null,
										Candidates = AlsShowRegions ? null : candidateOffsets,
										Regions = AlsShowRegions ? regionOffsets : null
									}
								},
								pivot,
								dic
							)
						);
					}
					else
					{
						n++;
					}
				}
			}

			accumulator.AddRange(
				from info in tempAccumulator.RemoveDuplicateItems()
				orderby info.Petals.Count, info.Pivot
				select info
			);
		}

		/// <summary>
		/// Process death ALSes information.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="candMaps">The digit distributions.</param>
		/// <param name="checkedCandidates">All checked candidates.</param>
		/// <param name="death">The death table.</param>
		/// <param name="alses">The ALS list.</param>
		private static void ProcessDeathAlsInfo(
			in SudokuGrid grid, Cells[] candMaps, short[] checkedCandidates,
			int[,] death, IReadOnlyList<Als> alses)
		{
			int max = 0, i = 0;
			foreach (var (_, region, digitsMask, map, _, _) in alses)
			{
				foreach (int digit in digitsMask)
				{
					var temp = (candMaps[digit] & map).PeerIntersection & candMaps[digit];
					if (temp.IsEmpty)
					{
						continue;
					}

					foreach (int cell in temp)
					{
						if ((digitsMask & ~grid.GetCandidates(cell)) == 0)
						{
							continue;
						}

						checkedCandidates[cell] |= (short)(1 << digit);
						int candidate = cell * 9 + digit;
						death[candidate, 0]++;

						int value = death[candidate, 0];
						if (value > max)
						{
							max = value;
						}

						death[candidate, value] = i;
					}
				}

				i++;
			}
		}

		/// <summary>
		/// To preprocess and gather all ALSes.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="emptyMap">(<see langword="in"/> parameter) The map of all empty cells.</param>
		/// <returns>All ALSes.</returns>
		private IReadOnlyList<Als> PreprocessAndGatherAlses(in SudokuGrid grid, in Cells emptyMap)
		{
			var list = new List<Als>();
			Cells tempEmptyCells;
			for (int region = 0; region < 27; region++)
			{
				tempEmptyCells = emptyMap & RegionMaps[region];
				if (tempEmptyCells.Count < 3)
				{
					// Every death blossom should lies in more than 2 cells.
					continue;
				}

				int[] emptyCellsArray = tempEmptyCells.ToArray();
				for (int i = 1, length = emptyCellsArray.Length; i < length; i++)
				{
					foreach (int[] cells in emptyCellsArray.GetSubsets(i))
					{
						if ((cells | emptyMap) != emptyMap)
						{
							continue;
						}

						short cands = 0;
						foreach (int cell in cells)
						{
							cands |= grid.GetCandidates(cell);
						}
						if (PopCount((uint)cands) != i + 1)
						{
							// Not an ALS.
							continue;
						}

						var map = new Cells(cells);
						if (
							(CurrentBlockCellsCount: PopCount((uint)map.BlockMask), Region: region) is (
								CurrentBlockCellsCount: 1,
								Region: >= 9
							)
						)
						{
							// If the current cells are in the same block and same line (i.e. in mini-line),
							// we will process them in blocks.
							continue;
						}

						list.Add(new(cands, map));
					}
				}
			}

			return list;
		}
	}
}
