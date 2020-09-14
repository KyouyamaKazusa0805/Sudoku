using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	/// <summary>
	/// Encapsulates a <b>domino loop</b> technique.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.SkLoop))]
	[SearcherProperty(96)]
	public sealed partial class SkLoopTechniqueSearcher : MslsTechniqueSearcher
	{
		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, Grid grid)
		{
			var pairs = (stackalloc short[8]);
			var tempLink = (stackalloc short[8]);
			var linkRegion = (stackalloc int[8]);
			foreach (int[] cells in SkLoopTable)
			{
				int n = 0, candidateCount = 0, i = 0;
				for (i = 0; i < 8; i++)
				{
					pairs[i] = default;
					linkRegion[i] = default;
				}
				for (i = 0; i < 8; i++)
				{
					if (grid.GetStatus(cells[i << 1]) != Empty)
					{
						n++;
					}
					else
					{
						pairs[i] |= grid.GetCandidateMask(cells[i << 1]);
					}

					if (grid.GetStatus(cells[(i << 1) + 1]) != Empty)
					{
						n++;
					}
					else
					{
						pairs[i] |= grid.GetCandidateMask(cells[(i << 1) + 1]);
					}

					if ((n, pairs[i].CountSet(), pairs[i]) is not ( <= 4, <= 5, not 0))
					{
						break;
					}

					candidateCount += pairs[i].CountSet();
				}

				if (i < 8 || candidateCount > 32 - (n << 1))
				{
					continue;
				}

				short candidateMask = (short)(pairs[0] & pairs[1]);
				if (candidateMask == 0)
				{
					continue;
				}

				short[] masks = GetCombinations(candidateMask).ToArray();
				for (int j = masks.Length - 1; j >= 0; j--)
				{
					short mask = masks[j];
					if (mask == 0)
					{
						continue;
					}

					for (int p = 0; p < 8; p++)
					{
						tempLink[p] = default;
					}

					int linkCount = (tempLink[0] = mask).CountSet();
					int k = 1;
					for (; k < 8; k++)
					{
						candidateMask = (short)(tempLink[k - 1] ^ pairs[k]);
						if ((candidateMask & pairs[(k + 1) % 8]) != candidateMask)
						{
							break;
						}

						linkCount += (tempLink[k] = candidateMask).CountSet();
					}

					if (k < 8 || linkCount != 16 - n)
					{
						continue;
					}

					linkRegion[0] = GetRegion(cells[0], Row);
					linkRegion[1] = GetRegion(cells[2], Block);
					linkRegion[2] = GetRegion(cells[4], Column);
					linkRegion[3] = GetRegion(cells[6], Block);
					linkRegion[4] = GetRegion(cells[8], Row);
					linkRegion[5] = GetRegion(cells[10], Block);
					linkRegion[6] = GetRegion(cells[12], Column);
					linkRegion[7] = GetRegion(cells[14], Block);

					var conclusions = new List<Conclusion>();
					var map = cells & EmptyMap;
					for (k = 0; k < 8; k++)
					{
						var elimMap = (RegionMaps[linkRegion[k]] & EmptyMap) - map;
						if (elimMap.IsEmpty)
						{
							continue;
						}

						foreach (int cell in elimMap)
						{
							short cands = (short)(grid.GetCandidateMask(cell) & tempLink[k]);
							if (cands != 0)
							{
								foreach (int digit in cands.GetAllSets())
								{
									conclusions.Add(new(Elimination, cell, digit));
								}
							}
						}
					}

					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<(int, int)>();
					short[] link = new short[27];
					for (k = 0; k < 8; k++)
					{
						link[linkRegion[k]] = tempLink[k];
						foreach (int cell in map & RegionMaps[linkRegion[k]])
						{
							short cands = (short)(grid.GetCandidateMask(cell) & tempLink[k]);
							if (cands == 0)
							{
								continue;
							}

							foreach (int digit in cands.GetAllSets())
							{
								candidateOffsets.Add(((k & 3) switch { 0 => 1, 1 => 2, _ => 0 }, cell * 9 + digit));
							}
						}
					}

					accumulator.Add(
						new SkLoopTechniqueInfo(
							conclusions,
							new View[] { new(candidateOffsets) },
							cells));
				}
			}
		}


		/// <summary>
		/// Get all combinations that contains all set bits from the specified number.
		/// </summary>
		/// <param name="seed">The specified number.</param>
		/// <returns>All combinations.</returns>
		private static IEnumerable<short> GetCombinations(short seed)
		{
			for (int i = 0; i < 9; i++)
			{
				foreach (short mask in new BitCombinationGenerator(9, i))
				{
					if ((mask & seed) == mask)
					{
						yield return mask;
					}
				}
			}
		}
	}
}
