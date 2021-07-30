using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.RankTheory
{
	/// <summary>
	/// Encapsulates a <b>multi-sector locked sets</b> (MSLS) technique. This searcher is
	/// the real technique, different with the abstract class <see cref="MslsStepSearcher"/>.
	/// </summary>
	/// <seealso cref="MslsStepSearcher"/>
	public sealed partial class MslsStepSearcher : RankTheoryStepSearcher
	{
		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		public static TechniqueProperties Properties { get; } = new(36, nameof(Technique.Msls))
		{
			DisplayLevel = 4
		};


		/// <inheritdoc/>
		public override unsafe void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			short* linkForEachRegion = stackalloc short[27];
			var linkForEachDigit = stackalloc Cells[9];
			for (int globalIndex = 0, iterationCount = Patterns.Count; globalIndex < iterationCount; globalIndex++)
			{
				Cells pattern = Patterns[globalIndex], map = EmptyMap & pattern;
				if (pattern.Count < 12 && pattern.Count - map.Count > 1 || pattern.Count - map.Count > 2)
				{
					continue;
				}

				int n = 0, count = map.Count;
				for (int digit = 0; digit < 9; digit++)
				{
					var pMap = linkForEachDigit + digit;
					*pMap = CandMaps[digit] & map;
					n += MathEx.Min(
						PopCount((uint)pMap->RowMask),
						PopCount((uint)pMap->ColumnMask),
						PopCount((uint)pMap->BlockMask)
					);
				}

				if (n == count)
				{
					short canF = 0;
					var canL = new Cells[9];
					var conclusions = new List<Conclusion>();
					var candidateOffsets = new List<DrawingInfo>();
					for (int digit = 0; digit < 9; digit++)
					{
						short q = (short)(1 << digit);
						var currentMap = linkForEachDigit[digit];
						uint
							rMask = (uint)currentMap.RowMask,
							cMask = (uint)currentMap.ColumnMask,
							bMask = (uint)currentMap.BlockMask;
						int temp = MathEx.Min(PopCount(rMask), PopCount(cMask), PopCount(bMask));
						var elimMap = Cells.Empty;
						int check = 0;
						if (PopCount(rMask) == temp)
						{
							check++;
							foreach (int i in rMask)
							{
								int region = i + 9;
								linkForEachRegion[region] |= q;
								elimMap |= (CandMaps[digit] & RegionMaps[region] & map).PeerIntersection;
							}
						}
						if (PopCount(cMask) == temp)
						{
							check++;
							foreach (int i in cMask)
							{
								int region = i + 18;
								linkForEachRegion[region] |= q;
								elimMap |= (CandMaps[digit] & RegionMaps[region] & map).PeerIntersection;
							}
						}
						if (PopCount(bMask) == temp)
						{
							check++;
							foreach (int i in bMask)
							{
								linkForEachRegion[i] |= q;
								elimMap |= (CandMaps[digit] & RegionMaps[i] & map).PeerIntersection;
							}
						}

						if (check > 1)
						{
							canF |= q;
						}

						elimMap &= CandMaps[digit];
						if (elimMap.IsEmpty)
						{
							continue;
						}

						foreach (int cell in elimMap)
						{
							if (map.Contains(cell))
							{
								canL[digit].AddAnyway(cell);
							}

							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
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
							short cands = (short)(grid.GetCandidates(cell) & linkMask);
							if (cands == 0)
							{
								continue;
							}

							foreach (int cand in cands)
							{
								if (!canL[cand].Contains(cell))
								{
									candidateOffsets.Add(
										new(region switch { < 9 => 2, < 18 => 0, _ => 1 }, cell * 9 + cand)
									);
								}
							}
						}
					}

					accumulator.Add(
						new MslsStepInfo(
							conclusions,
							new View[] { new() { Candidates = candidateOffsets } },
							map
						)
					);
				}
			}
		}
	}
}
