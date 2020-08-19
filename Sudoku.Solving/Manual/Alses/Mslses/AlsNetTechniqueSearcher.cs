using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static System.Algorithms;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	/// <summary>
	/// Encapsulates a <b>multi-sector locked sets</b> (MSLS) technique. This searcher is
	/// the real technique, different with the abstract class <see cref="MslsTechniqueSearcher"/>.
	/// </summary>
	/// <seealso cref="MslsTechniqueSearcher"/>
	[TechniqueDisplay(nameof(TechniqueCode.Msls))]
	[SearcherProperty(96)]
	public sealed partial class AlsNetTechniqueSearcher : MslsTechniqueSearcher
	{
		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var linkForEachRegion = (Span<short>)stackalloc short[27];
			var linkForEachDigit = (Span<GridMap>)stackalloc GridMap[9];
			foreach (var pattern in Patterns)
			{
				var map = EmptyMap & pattern;
				if (pattern.Count < 12 && pattern.Count - map.Count > 1 || pattern.Count - map.Count > 2)
				{
					continue;
				}

				int n = 0, count = map.Count;
				for (int digit = 0; digit < 9; digit++)
				{
					ref var currentMap = ref linkForEachDigit[digit];
					currentMap = CandMaps[digit] & map;
					n +=
						Min(
							currentMap.RowMask.CountSet(),
							currentMap.ColumnMask.CountSet(),
							currentMap.BlockMask.CountSet());
				}

				if (n == count)
				{
					short canF = 0;
					var canL = new GridMap[9];
					var conclusions = new List<Conclusion>();
					var candidateOffsets = new List<(int, int)>();
					for (int digit = 0; digit < 9; digit++)
					{
						var currentMap = linkForEachDigit[digit];
						short rMask = currentMap.RowMask;
						short cMask = currentMap.ColumnMask;
						short bMask = currentMap.BlockMask;
						int temp = Min(rMask.CountSet(), cMask.CountSet(), bMask.CountSet());
						var elimMap = GridMap.Empty;
						int check = 0;
						if (rMask.CountSet() == temp)
						{
							check++;
							foreach (int i in rMask.GetAllSets())
							{
								int region = i + 9;
								linkForEachRegion[region] |= (short)(1 << digit);
								elimMap |= (CandMaps[digit] & RegionMaps[region] & map).PeerIntersection;
							}
						}
						if (cMask.CountSet() == temp)
						{
							check++;
							foreach (int i in cMask.GetAllSets())
							{
								int region = i + 18;
								linkForEachRegion[region] |= (short)(1 << digit);
								elimMap |= (CandMaps[digit] & RegionMaps[region] & map).PeerIntersection;
							}
						}
						if (bMask.CountSet() == temp)
						{
							check++;
							foreach (int i in bMask.GetAllSets())
							{
								linkForEachRegion[i] |= (short)(1 << digit);
								elimMap |= (CandMaps[digit] & RegionMaps[i] & map).PeerIntersection;
							}
						}

						if (check > 1)
						{
							canF |= (short)(1 << digit);
						}

						elimMap &= CandMaps[digit];
						if (elimMap.IsEmpty)
						{
							continue;
						}

						foreach (int cell in elimMap)
						{
							if (map[cell])
							{
								canL[digit].Add(cell);
							}

							conclusions.Add(new(Elimination, cell, digit));
						}
					}

					if (conclusions.Count == 0)
					{
						continue;
					}

					for (int region = 0; region < 27; region++)
					{
						short linkMask = linkForEachRegion[region];
						if (linkMask == 0)
						{
							continue;
						}

						foreach (int cell in map & RegionMaps[region])
						{
							short cands = (short)(grid.GetCandidateMask(cell) & linkMask);
							if (cands == 0)
							{
								continue;
							}

							foreach (int cand in cands.GetAllSets())
							{
								if (canL[cand][cell])
								{
									continue;
								}

								candidateOffsets.Add((region switch { < 9 => 2, < 18 => 0, _ => 1 }, cell * 9 + cand));
							}
						}
					}

					accumulator.Add(
						new AlsNetTechniqueInfo(
							conclusions,
							views: new[] { new View(candidateOffsets) },
							cells: map));
				}
			}
		}
	}
}
