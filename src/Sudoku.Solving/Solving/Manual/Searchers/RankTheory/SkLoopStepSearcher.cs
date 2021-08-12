using System.Numerics;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.RankTheory
{
	/// <summary>
	/// Encapsulates a <b>domino loop</b> technique.
	/// </summary>
	public sealed partial class SkLoopStepSearcher : RankTheoryStepSearcher
	{
		/// <inheritdoc/>
		public override SearchingOptions Options { get; set; } = new(37, DisplayingLevel.D);

		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		[Obsolete("Please use the property '" + nameof(Options) + "' instead.", false)]
		public static TechniqueProperties Properties { get; } = new(37, nameof(Technique.SkLoop))
		{
			DisplayLevel = 4
		};


		/// <inheritdoc/>
		public override unsafe void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			short* pairs = stackalloc short[8], tempLink = stackalloc short[8];
			int* linkRegion = stackalloc int[8];
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
					if (grid.GetStatus(cells[i << 1]) != CellStatus.Empty)
					{
						n++;
					}
					else
					{
						pairs[i] |= grid.GetCandidates(cells[i << 1]);
					}

					if (grid.GetStatus(cells[(i << 1) + 1]) != CellStatus.Empty)
					{
						n++;
					}
					else
					{
						pairs[i] |= grid.GetCandidates(cells[(i << 1) + 1]);
					}

					if (
						(
							NumbersCount: n,
							PairNumbersCount: PopCount((uint)pairs[i]),
							PairMask: pairs[i]
						) is not (NumbersCount: <= 4, PairNumbersCount: <= 5, PairMask: not 0)
					)
					{
						break;
					}

					candidateCount += PopCount((uint)pairs[i]);
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
				short[] masks = MaskSubsetExtractor.GetMaskSubsets(candidateMask);
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
					int linkCount = PopCount((uint)(tempLink[0] = mask));
					int k = 1;
					for (; k < 8; k++)
					{
						candidateMask = (short)(tempLink[k - 1] ^ pairs[k]);
						if ((candidateMask & pairs[(k + 1) % 8]) != candidateMask)
						{
							break;
						}

						linkCount += PopCount((uint)(tempLink[k] = candidateMask));
					}

					if (k < 8 || linkCount != 16 - n)
					{
						continue;
					}

					// Last check: Check the first and the last pair.
					candidateMask = (short)(tempLink[7] ^ pairs[0]);
					if ((candidateMask & pairs[(k + 1) % 8]) != candidateMask)
					{
						continue;
					}

					// Check elimination map.
					linkRegion[0] = cells[0].ToRegion(RegionLabel.Row);
					linkRegion[1] = cells[2].ToRegion(RegionLabel.Block);
					linkRegion[2] = cells[4].ToRegion(RegionLabel.Column);
					linkRegion[3] = cells[6].ToRegion(RegionLabel.Block);
					linkRegion[4] = cells[8].ToRegion(RegionLabel.Row);
					linkRegion[5] = cells[10].ToRegion(RegionLabel.Block);
					linkRegion[6] = cells[12].ToRegion(RegionLabel.Column);
					linkRegion[7] = cells[14].ToRegion(RegionLabel.Block);
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
							short cands = (short)(grid.GetCandidates(cell) & tempLink[k]);
							if (cands != 0)
							{
								foreach (int digit in cands)
								{
									conclusions.Add(new(ConclusionType.Elimination, cell, digit));
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
							short cands = (short)(grid.GetCandidates(cell) & tempLink[k]);
							if (cands == 0)
							{
								continue;
							}

							foreach (int digit in cands)
							{
								candidateOffsets.Add(
									new((k & 3) switch { 0 => 1, 1 => 2, _ => 0 }, cell * 9 + digit)
								);
							}
						}
					}

					// Gather the result.
					accumulator.Add(
						new SkLoopStepInfo(
							conclusions,
							new View[] { new() { Candidates = candidateOffsets } },
							cells
						)
					);
				}
			}
		}
	}
}
