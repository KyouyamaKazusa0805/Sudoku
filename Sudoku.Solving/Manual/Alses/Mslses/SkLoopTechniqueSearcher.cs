using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	/// <summary>
	/// Encapsulates a <b>domino loop</b> technique.
	/// </summary>
	public sealed partial class SkLoopTechniqueSearcher : MslsTechniqueSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(96, nameof(TechniqueCode.SkLoop)) { DisplayLevel = 4 };


		/// <inheritdoc/>
		[SkipLocalsInit]
		public override void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
		{
			var pairs = (stackalloc short[8]);
			var tempLink = (stackalloc short[8]);
			var linkRegion = (stackalloc int[8]);
			foreach (int[] cells in SkLoopTable)
			{
				// Initialize the elements.
				int n = 0, candidateCount = 0, i = 0;
				for (i = 0; i < 8; i++)
				{
					pairs[i] = default;
					linkRegion[i] = default;
				}

				// Get the values count ('n') and pairs list ('pairs').
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

					if ((n, pairs[i].PopCount(), pairs[i]) is not ( <= 4, <= 5, not 0))
					{
						break;
					}

					candidateCount += pairs[i].PopCount();
				}

				// Check validity: If the number of candidate appearing is lower than 32 - (n * 2),
				// the status is invalid.
				if (i < 8 || candidateCount > 32 - (n << 1))
				{
					continue;
				}

				short candidateMask = (short)(pairs[0] & pairs[1]);
				if (candidateMask == 0)
				{
					continue;
				}

				// Check all combinations.
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

					// Check the associativity:
					// Each pair should find the digits that can combine with the next pair.
					int linkCount = (tempLink[0] = mask).PopCount();
					int k = 1;
					for (; k < 8; k++)
					{
						candidateMask = (short)(tempLink[k - 1] ^ pairs[k]);
						if ((candidateMask & pairs[(k + 1) % 8]) != candidateMask)
						{
							break;
						}

						linkCount += (tempLink[k] = candidateMask).PopCount();
					}

					if (k < 8 || linkCount != 16 - n)
					{
						continue;
					}

					// Last check: Check the first and the last pair.
					candidateMask = (short)(tempLink[^1] ^ pairs[0]);
					if ((candidateMask & pairs[(k + 1) % 8]) != candidateMask)
					{
						continue;
					}

					// Check elimination map.
					linkRegion[0] = RegionLabel.Row.GetRegion(cells[0]);
					linkRegion[1] = RegionLabel.Block.GetRegion(cells[2]);
					linkRegion[2] = RegionLabel.Column.GetRegion(cells[4]);
					linkRegion[3] = RegionLabel.Block.GetRegion(cells[6]);
					linkRegion[4] = RegionLabel.Row.GetRegion(cells[8]);
					linkRegion[5] = RegionLabel.Block.GetRegion(cells[10]);
					linkRegion[6] = RegionLabel.Column.GetRegion(cells[12]);
					linkRegion[7] = RegionLabel.Block.GetRegion(cells[14]);
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
								foreach (int digit in cands)
								{
									conclusions.Add(new(Elimination, cell, digit));
								}
							}
						}
					}

					// Check the number of the available eliminations.
					if (conclusions.Count == 0)
					{
						continue;
					}

					// Highlight candidates.
					var candidateOffsets = new List<DrawingInfo>();
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

							foreach (int digit in cands)
							{
								candidateOffsets.Add(
									new((k & 3) switch { 0 => 1, 1 => 2, _ => 0 }, cell * 9 + digit));
							}
						}
					}

					// Gather the result.
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
				foreach (short mask in new BitSubsetsGenerator(9, i))
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
