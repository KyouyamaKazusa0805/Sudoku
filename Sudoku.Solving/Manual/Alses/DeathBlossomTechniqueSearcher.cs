using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates a <b>death blossom</b> technique.
	/// </summary>
	[TechniqueDisplay("Death Blossom")]
	public sealed class DeathBlossomTechniqueSearcher : AlsTechniqueSearcher
	{
		/// <summary>
		/// Indicates whether the ALSes can be overlapped with each other.
		/// </summary>
		private readonly bool _allowOverlapping;

		/// <summary>
		/// Indicates whether the ALSes shows their region rather than cells.
		/// </summary>
		private readonly bool _alsShowRegions;

		/// <summary>
		/// The region maps.
		/// </summary>
		private readonly GridMap[] _regionMaps;


		/// <summary>
		/// Initialize an instance with the specified information.
		/// </summary>
		/// <param name="regionMaps">The region maps.</param>
		/// <param name="allowOverlapping">
		/// Indicates whether the ALSes can be overlapped with each other.
		/// </param>
		/// <param name="alsShowRegions">
		/// Indicates whether all ALSes shows their regions rather than cells.
		/// </param>
		public DeathBlossomTechniqueSearcher(GridMap[] regionMaps, bool allowOverlapping, bool alsShowRegions) =>
			(_regionMaps, _allowOverlapping, _alsShowRegions) = (regionMaps, allowOverlapping, alsShowRegions);


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
			var (emptyCells, _, digitDistributions) = grid;

			short[] checkedCandidates = new short[81];
			int[,] death = new int[729, 1000];
			var alsList = PreprocessAndRecordAlses(grid, emptyCells);
			ProcessDeathAlsInfo(grid, digitDistributions, checkedCandidates, death, alsList);

			for (int pivot = 0; pivot < 81; pivot++)
			{
				if (grid.GetCellStatus(pivot) != CellStatus.Empty
					|| checkedCandidates[pivot] != grid.GetCandidatesReversal(pivot)
					|| checkedCandidates[pivot].CountSet() > 5)
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
							var elimMap =
								new GridMap(
									(temp & digitDistributions[d]).Offsets,
									GridMap.InitializeOption.ProcessPeersWithoutItself)
								& digitDistributions[d];
							if (elimMap.IsEmpty)
							{
								continue;
							}

							foreach (int cell in elimMap.Offsets)
							{
								if (!(grid.Exists(cell, d) is true))
								{
									continue;
								}

								conclusions.Add(new Conclusion(ConclusionType.Elimination, cell, d));
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
									}
								}
							}

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
							foreach (int c in
								from pos in a.RelativePos
								select RegionUtils.GetCellOffset(a.Region, pos))
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
							foreach (int c in
								from pos in a.RelativePos
								select RegionUtils.GetCellOffset(a.Region, pos))
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
		/// <param name="digitDistributions">The digit distributions.</param>
		/// <param name="checkedCandidates">All checked candidates.</param>
		/// <param name="death">The death table.</param>
		/// <param name="alses">The ALS list.</param>
		private static void ProcessDeathAlsInfo(
			IReadOnlyGrid grid, GridMap[] digitDistributions,
			short[] checkedCandidates, int[,] death, IReadOnlyList<Als> alses)
		{
			int max = 0;
			int i = 0;
			foreach (var als in alses)
			{
				var (region, relativePos, digits, map) = als;
				short digitsMask = als.DigitsMask;
				foreach (int digit in digits)
				{
					var temp =
						new GridMap(
							(digitDistributions[digit] & map).Offsets,
							GridMap.InitializeOption.ProcessPeersWithoutItself)
						& digitDistributions[digit];
					if (temp.IsEmpty)
					{
						continue;
					}

					int[] array = temp.ToArray();
					foreach (int cell in array)
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
		/// <param name="emptyCells">The empty cells.</param>
		/// <returns>All ALSes.</returns>
		private IReadOnlyList<Als> PreprocessAndRecordAlses(IReadOnlyGrid grid, GridMap emptyCells)
		{
			var list = new List<Als>();
			GridMap tempEmptyCells;
			for (int region = 0; region < 27; region++)
			{
				tempEmptyCells = emptyCells & _regionMaps[region];
				if (tempEmptyCells.Count < 3)
				{
					// Every death blossom should lies on more than 2 cells.
					continue;
				}

				int[] emptyCellsArray = tempEmptyCells.ToArray();
				for (int i = 1; i < emptyCellsArray.Length; i++)
				{
					foreach (short mask in new BitCombinationGenerator(9, i))
					{
						if (mask.GetAllSets().Any(
							pos => grid.GetCellStatus(RegionUtils.GetCellOffset(region, pos)) != CellStatus.Empty))
						{
							continue;
						}

						short cands = 0;
						for (int pos = 0; pos < 9; pos++)
						{
							if ((mask >> pos & 1) == 0)
							{
								continue;
							}

							cands |= grid.GetCandidatesReversal(RegionUtils.GetCellOffset(region, pos));
						}

						if (cands.CountSet() != i + 1)
						{
							// Not an ALS.
							continue;
						}

						if (new GridMap(
							from pos in mask.GetAllSets()
							select RegionUtils.GetCellOffset(region, pos)).BlockMask.CountSet() == 1 && region >= 9)
						{
							// If the current cells are in the same block and same line (i.e. in mini-line),
							// we will process them in blocks.
							continue;
						}

						list.Add(new Als(region << 18 | mask << 9 | (int)cands));
					}
				}
			}

			return list;
		}
	}
}
