using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Encapsulates a <b>death blossom</b> technique.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.DeathBlossom))]
	[HighAllocation]
	[SearcherProperty(80, IsEnabled = false, DisabledReason = DisabledReason.TooSlow)]
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


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, Grid grid)
		{
			var tempAccumulator = new List<DeathBlossomTechniqueInfo>();
			short[] checkedCandidates = new short[81];
			int[,] death = new int[729, 1000];
			var alsList = PreprocessAndRecordAlses(grid, EmptyMap);
			ProcessDeathAlsInfo(grid, CandMaps, checkedCandidates, death, alsList);

			for (int pivot = 0; pivot < 81; pivot++)
			{
				if (grid.GetStatus(pivot) != Empty
					|| checkedCandidates[pivot] != grid.GetCandidateMask(pivot)
					|| checkedCandidates[pivot].CountSet() > _maxPetals)
				{
					continue;
				}

				short cands = grid.GetCandidateMask(pivot);
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

						if (temp.InOneRegion || allZ[n] == 0)
						{
							// All in same region means that they with target cell together
							// forms a naked subset.
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int d in allZ[n].GetAllSets())
						{
							var elimMap = (temp & CandMaps[d]).PeerIntersection & CandMaps[d];
							if (elimMap.IsEmpty)
							{
								continue;
							}

							foreach (int cell in elimMap)
							{
								conclusions.Add(new(Elimination, cell, d));
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
									if (alsesUsed[i].Map.Overlaps(alsesUsed[j].Map))
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
						var cellOffsets = new List<(int, int)> { (0, pivot) };
						int z = 0;
						foreach (var (d, a) in dic)
						{
							foreach (int c in a.Map)
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
							foreach (int c in a.Map)
							{
								foreach (int dd in grid.GetCandidates(c))
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
						tempAccumulator.Add(
							new DeathBlossomTechniqueInfo(
								conclusions,
								new View[]
								{
									new(
										_alsShowRegions ? new[] { (0, pivot) } : (IReadOnlyList<(int, int)>)cellOffsets,
										_alsShowRegions ? candidateOffsets : null,
										_alsShowRegions ? regionOffsets : null,
										null)
								},
								pivot,
								dic));
					}
					else
					{
						n++;
					}
				}
			}

			accumulator.AddRange(from Info in tempAccumulator orderby Info.PetalsCount, Info.Pivot select Info);
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
			Grid grid, GridMap[] candMaps, short[] checkedCandidates, int[,] death, IReadOnlyList<Als> alses)
		{
			int max = 0;
			int i = 0;
			foreach (var als in alses)
			{
				var (_, region, digitsMask, map, _, _) = als;
				foreach (int digit in digitsMask.GetAllSets())
				{
					var temp = (candMaps[digit] & map).PeerIntersection & candMaps[digit];
					if (temp.IsEmpty)
					{
						continue;
					}

					foreach (int cell in temp)
					{
						if ((digitsMask & ~grid.GetCandidateMask(cell)) == 0)
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
		private IReadOnlyList<Als> PreprocessAndRecordAlses(Grid grid, GridMap emptyMap)
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
					foreach (int[] cells in emptyCellsArray.GetSubsets(i))
					{
						if ((cells | emptyMap) != emptyMap)
						{
							continue;
						}

						short cands = 0;
						foreach (int cell in cells)
						{
							cands |= grid.GetCandidateMask(cell);
						}
						if (cands.CountSet() != i + 1)
						{
							// Not an ALS.
							continue;
						}

						var map = new GridMap(cells);
						if ((map.BlockMask.CountSet(), region) is (1, >= 9))
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
