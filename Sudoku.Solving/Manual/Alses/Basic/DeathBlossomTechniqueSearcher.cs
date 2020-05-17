using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Data.GridMap.InitializeOption;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Encapsulates a <b>death blossom</b> technique.
	/// </summary>
	[TechniqueDisplay("Death Blossom")]
	[HighAllocation]
	public sealed class DeathBlossomTechniqueSearcher : AlsTechniqueSearcher
	{
		/// <summary>
		/// Indicates the max petals to search.
		/// </summary>
		private readonly int _maxPetals;


		/// <summary>
		/// Initialize an instance with the specified information.
		/// </summary>
		/// <param name="allowOverlapping">
		/// Indicates whether the ALSes can be overlapped with each other.
		/// </param>
		/// <param name="alsShowRegions">
		/// Indicates whether all ALSes shows their regions rather than cells.
		/// </param>
		/// <param name="maxPetals">
		/// Indicates the max petals of instance to search.
		/// </param>
		public DeathBlossomTechniqueSearcher(bool allowOverlapping, bool alsShowRegions, int maxPetals)
			: base(allowOverlapping, alsShowRegions, true) => _maxPetals = maxPetals;


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 80;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			short[] checkedCandidates = new short[81];
			int[,] death = new int[729, 1000];
			var alsList = PreprocessAndRecordAlses(grid, EmptyMap);
			ProcessDeathAlsInfo(grid, CandMaps, checkedCandidates, death, alsList);

			for (int pivot = 0; pivot < 81; pivot++)
			{
				if (grid.GetStatus(pivot) != Empty
					|| checkedCandidates[pivot] != grid.GetCandidatesReversal(pivot)
					|| checkedCandidates[pivot].CountSet() > _maxPetals)
				{
					continue;
				}

				short cands = grid.GetCandidatesReversal(pivot);
				int digitsCount = cands.CountSet();
				short[] allZ = new short[digitsCount];
				int[] stack = new int[digitsCount];

				var digits = cands.GetAllSets();
				int n = 0;
				while (n >= 0)
				{
					bool flag = true;
					int digit = digits.ElementAt(n);
					for (int i = stack[n] + 1; i <= death[pivot * 9 + digit, 0]; i++)
					{
						short value = (short)(alsList[death[pivot * 9 + digit, i]].DigitsMask & ~cands);
						allZ[n] = n == 0 ? value : (short)(allZ[n - 1] & value);

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
						var temp = GridMap.Empty;
						foreach (int d in digits)
						{
							var map = alsList[death[pivot * 9 + d, stack[k]]].Map;
							if (k++ == 0)
							{
								temp = map;
							}
							else
							{
								temp |= map;
							}
						}

						if (temp.AllSetsAreInOneRegion(out _) || allZ[n] == 0)
						{
							// All in same region means that they with target cell together
							// forms a naked subset.
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int d in allZ[n].GetAllSets())
						{
							var elimMap = new GridMap(temp & CandMaps[d], ProcessPeersWithoutItself) & CandMaps[d];
							if (elimMap.IsEmpty)
							{
								continue;
							}

							foreach (int cell in elimMap.Offsets)
							{
								if (grid.Exists(cell, d) is true)
								{
									conclusions.Add(new Conclusion(Elimination, cell, d));
								}
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
						if (!_allowOverlapping)
						{
							var alsesUsed = dic.Values.ToArray();
							bool overlap = false;
							for (int i = 0, length = alsesUsed.Length; i < length - 1; i++)
							{
								for (int j = i + 1; j < length; j++)
								{
									if ((alsesUsed[i].Map & alsesUsed[j].Map).IsNotEmpty)
									{
										overlap = true;
										goto Label_Determine;
									}
								}
							}

						Label_Determine:
							if (overlap)
							{
								continue;
							}
						}

						// Record all highlight cells.
						var cellOffsets = new List<(int, int)> { (0, pivot) };
						int z = 0;
						foreach (var (d, a) in dic)
						{
							foreach (int c in a.Map.Offsets)
							{
								cellOffsets.Add((-z - 1, c));
							}

							z = (z + 1) % 4;
						}

						// Record all highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						z = 0;
						foreach (var (d, a) in dic)
						{
							foreach (int c in a.Map.Offsets)
							{
								foreach (int dd in grid.GetCandidatesReversal(c).GetAllSets())
								{
									candidateOffsets.Add((d == dd ? 1 : -z - 1, c * 9 + dd));
								}
							}

							z = (z + 1) % 4;
						}

						// Record all highlight regions.
						var regionOffsets = new List<(int, int)>();
						z = 0;
						foreach (var als in dic.Values)
						{
							regionOffsets.Add((-z - 1, als.Region));

							z = (z + 1) % 4;
						}

						// Add item.
						accumulator.Add(
							new DeathBlossomTechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets:
											_alsShowRegions
												? (IReadOnlyList<(int, int)>)new[] { (0, pivot) }
												: cellOffsets,
										candidateOffsets: _alsShowRegions ? candidateOffsets : null,
										regionOffsets: _alsShowRegions ? regionOffsets : null,
										links: null)
								},
								pivot,
								alses: dic));
					}
					else
					{
						n++;
					}
				}
			}
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
			IReadOnlyGrid grid, GridMap[] candMaps, short[] checkedCandidates,
			int[,] death, IReadOnlyList<Als> alses)
		{
			int max = 0;
			int i = 0;
			foreach (var als in alses)
			{
				var (_, region, digitsMask, map, _, _) = als;
				foreach (int digit in digitsMask.GetAllSets())
				{
					var temp =
						new GridMap((candMaps[digit] & map).Offsets, ProcessPeersWithoutItself)
						& candMaps[digit];
					if (temp.IsEmpty)
					{
						continue;
					}

					foreach (int cell in temp.Offsets)
					{
						if ((digitsMask & ~grid.GetCandidatesReversal(cell)) == 0)
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
		/// To preprocess and record all ALSes.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="emptyMap">The map of all empty cells.</param>
		/// <returns>All ALSes.</returns>
		private IReadOnlyList<Als> PreprocessAndRecordAlses(IReadOnlyGrid grid, GridMap emptyMap)
		{
			var list = new List<Als>();
			GridMap tempEmptyCells;
			for (int region = 0; region < 27; region++)
			{
				tempEmptyCells = emptyMap & RegionMaps[region];
				if (tempEmptyCells.Count < 3)
				{
					// Every death blossom should lies on more than 2 cells.
					continue;
				}

				int[] emptyCellsArray = tempEmptyCells.ToArray();
				for (int i = 1; i < emptyCellsArray.Length; i++)
				{
					foreach (int[] cells in Algorithms.GetCombinationsOfArray(emptyCellsArray, i))
					{
						if ((new GridMap(cells) | emptyMap) != emptyMap)
						{
							continue;
						}

						short cands = 0;
						foreach (int cell in cells)
						{
							cands |= grid.GetCandidatesReversal(cell);
						}
						if (cands.CountSet() != i + 1)
						{
							// Not an ALS.
							continue;
						}

						var map = new GridMap(cells);
						if (map.BlockMask.CountSet() == 1 && region >= 9)
						{
							// If the current cells are in the same block and same line (i.e. in mini-line),
							// we will process them in blocks.
							continue;
						}

						list.Add(new Als(cands, map));
					}
				}
			}

			return list;
		}
	}
}
