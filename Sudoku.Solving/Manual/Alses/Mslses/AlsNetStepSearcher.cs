using System.Collections.Generic;
using System.Extensions;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;
using static System.Algorithms;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	/// <summary>
	/// Encapsulates a <b>multi-sector locked sets</b> (MSLS) technique. This searcher is
	/// the real technique, different with the abstract class <see cref="MslsStepSearcher"/>.
	/// </summary>
	/// <seealso cref="MslsStepSearcher"/>
	public sealed partial class AlsNetStepSearcher : MslsStepSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(96, nameof(TechniqueCode.Msls))
		{
			DisplayLevel = 4
		};


		/// <inheritdoc/>
		[SkipLocalsInit]
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			var linkForEachRegion = (stackalloc short[27]);
			var linkForEachDigit = (stackalloc Cells[9]);
			foreach (var pattern in Patterns)
			{
				var map = EmptyMap & pattern;
				if (pattern.Count < 12
					&& (pattern.Count - map.Count, pattern.Count - map.Count) is not ( <= 1, <= 2))
				{
					continue;
				}

				int n = 0, count = map.Count;
				for (int digit = 0; digit < 9; digit++)
				{
					ref var curMap = ref linkForEachDigit[digit];
					curMap = CandMaps[digit] & map;
					n += Min(curMap.RowMask.PopCount(), curMap.ColumnMask.PopCount(), curMap.BlockMask.PopCount());
				}

				if (n == count)
				{
					short canF = 0;
					var canL = new Cells[9];
					var conclusions = new List<Conclusion>();
					var candidateOffsets = new List<DrawingInfo>();
					for (int digit = 0; digit < 9; digit++)
					{
						var currentMap = linkForEachDigit[digit];
						short rMask = currentMap.RowMask;
						short cMask = currentMap.ColumnMask;
						short bMask = currentMap.BlockMask;
						int temp = Min(rMask.PopCount(), cMask.PopCount(), bMask.PopCount());
						var elimMap = Cells.Empty;
						int check = 0;
						if (rMask.PopCount() == temp)
						{
							check++;
							foreach (int i in rMask)
							{
								int region = i + 9;
								linkForEachRegion[region] |= (short)(1 << digit);
								elimMap |= (CandMaps[digit] & RegionMaps[region] & map).PeerIntersection;
							}
						}
						if (cMask.PopCount() == temp)
						{
							check++;
							foreach (int i in cMask)
							{
								int region = i + 18;
								linkForEachRegion[region] |= (short)(1 << digit);
								elimMap |= (CandMaps[digit] & RegionMaps[region] & map).PeerIntersection;
							}
						}
						if (bMask.PopCount() == temp)
						{
							check++;
							foreach (int i in bMask)
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
								canL[digit].AddAnyway(cell);
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
						if (linkForEachRegion[region] is var linkMask && linkMask == 0)
						{
							continue;
						}

						foreach (int cell in map & RegionMaps[region])
						{
							if ((short)(grid.GetCandidates(cell) & linkMask) is var cands && cands == 0)
							{
								continue;
							}

							foreach (int cand in cands)
							{
								if (canL[cand][cell])
								{
									continue;
								}

								candidateOffsets.Add(
									new(region switch { < 9 => 2, < 18 => 0, _ => 1 }, cell * 9 + cand));
							}
						}
					}

					accumulator.Add(
						new AlsNetStepInfo(
							conclusions,
							new View[] { new() { Candidates = candidateOffsets } },
							map));
				}
			}
		}
	}
}
