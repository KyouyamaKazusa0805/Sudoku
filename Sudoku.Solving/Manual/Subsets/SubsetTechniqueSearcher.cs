using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Subsets
{
	/// <summary>
	/// Encapsulates a <b>subset</b> technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.NakedPair))]
	[SearcherProperty(30)]
	public sealed class SubsetTechniqueSearcher : TechniqueSearcher
	{
		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, Grid grid)
		{
			for (int size = 2; size <= 4; size++)
			{
				// Get naked subsets.
				for (int region = 0; region < 27; region++)
				{
					if ((RegionMaps[region] & EmptyMap) is var currentEmptyMap && currentEmptyMap is { Count: < 2 })
					{
						continue;
					}

					// Iterate on each combination.
					foreach (int[] cells in currentEmptyMap.ToArray().GetSubsets(size))
					{
						short mask = 0;
						foreach (int cell in cells)
						{
							mask |= grid.GetCandidateMask(cell);
						}
						if (mask.CountSet() != size)
						{
							continue;
						}

						// Naked subset found. Now check eliminations.
						short flagMask = 0;
						var conclusions = new List<Conclusion>();
						foreach (int digit in mask.GetAllSets())
						{
							var map = (cells & CandMaps[digit]).PeerIntersection & CandMaps[digit];
							flagMask |= map.AllSetsAreInOneRegion(out _) ? (short)0 : (short)(1 << digit);

							foreach (int cell in map)
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<(int, int)>();
						foreach (int cell in cells)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add((0, cell * 9 + digit));
							}
						}

						accumulator.Add(
							new NakedSubsetTechniqueInfo(
								conclusions,
								views: new[] { new View(null, candidateOffsets, new[] { (0, region) }, null) },
								regionOffset: region,
								cellOffsets: cells,
								digits: mask.GetAllSets().ToArray(),
								isLocked:
									flagMask switch
									{
										_ when flagMask == mask => true,
										not 0 => false,
										_ => null
									}));
					}
				}

				// Get hidden subsets.
				for (int region = 0; region < 27; region++)
				{
					var traversingMap = RegionMaps[region] - EmptyMap;
					if (traversingMap.Count >= 8)
					{
						// No available digit (Or hidden single).
						continue;
					}

					short mask = Grid.MaxCandidatesMask;
					foreach (int cell in traversingMap)
					{
						mask &= (short)~(1 << grid[cell]);
					}
					foreach (int[] digits in mask.GetAllSets().ToArray().GetSubsets(size))
					{
						short tempMask = mask;
						var map = GridMap.Empty;
						foreach (int digit in digits)
						{
							tempMask &= (short)~(1 << digit);
							map |= RegionMaps[region] & CandMaps[digit];
						}
						if (map.Count != size)
						{
							continue;
						}

						// Gather eliminations.
						var conclusions = new List<Conclusion>();
						foreach (int digit in tempMask.GetAllSets())
						{
							foreach (int cell in map & CandMaps[digit])
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						// Gather highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						foreach (int digit in digits)
						{
							foreach (int cell in map & CandMaps[digit])
							{
								candidateOffsets.Add((0, cell * 9 + digit));
							}
						}

						accumulator.Add(
							new HiddenSubsetTechniqueInfo(
								conclusions,
								views: new[] { new View(null, candidateOffsets, new[] { (0, region) }, null) },
								regionOffset: region,
								cellOffsets: map.ToArray(),
								digits));
					}
				}
			}
		}
	}
}
