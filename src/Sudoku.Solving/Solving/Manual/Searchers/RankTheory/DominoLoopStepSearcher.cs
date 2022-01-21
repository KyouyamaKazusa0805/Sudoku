namespace Sudoku.Solving.Manual.Searchers.RankTheory;

/// <summary>
/// Provides with a <b>Domino Loop</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Domino Loop</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class DominoLoopStepSearcher : IDominoLoopStepSearcher
{
	/// <summary>
	/// The position table of all SK-loops.
	/// </summary>
	private static readonly int[][] SkLoopTable = new int[729][];

	/// <summary>
	/// The region maps.
	/// </summary>
	private static readonly Cells[] RegionMaps;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static DominoLoopStepSearcher()
	{
		// Initialize for region maps.
		RegionMaps = new Cells[27];
		for (int i = 0; i < 27; i++)
		{
			ref var map = ref RegionMaps[i];
			foreach (int cell in RegionCells[i])
			{
				map.AddAnyway(cell);
			}
		}

		// Initialize for SK-loop table.
		var s = (stackalloc int[4]);
		for (int a = 9, n = 0; a < 18; a++)
		{
			for (int b = 9; b < 18; b++)
			{
				if (a / 3 == b / 3 || b < a)
				{
					continue;
				}

				for (int c = 18; c < 27; c++)
				{
					for (int d = 18; d < 27; d++)
					{
						if (c / 3 == d / 3 || d < c)
						{
							continue;
						}

						var all = RegionMaps[a] | RegionMaps[b] | RegionMaps[c] | RegionMaps[d];
						var overlap = (RegionMaps[a] | RegionMaps[b]) & (RegionMaps[c] | RegionMaps[d]);
						short blockMask = overlap.BlockMask;
						for (int i = 0, count = 0; count < 4 && i < 16; i++)
						{
							if ((blockMask >> i & 1) != 0)
							{
								s[count++] = i;
							}
						}

						all &= RegionMaps[s[0]] | RegionMaps[s[1]] | RegionMaps[s[2]] | RegionMaps[s[3]];
						all -= overlap;

						SkLoopTable[n] = new int[16];
						int pos = 0;
						foreach (int cell in all & RegionMaps[a])
						{
							SkLoopTable[n][pos++] = cell;
						}
						foreach (int cell in all & RegionMaps[d])
						{
							SkLoopTable[n][pos++] = cell;
						}
						int[] cells = (all & RegionMaps[b]).ToArray();
						SkLoopTable[n][pos++] = cells[2];
						SkLoopTable[n][pos++] = cells[3];
						SkLoopTable[n][pos++] = cells[0];
						SkLoopTable[n][pos++] = cells[1];
						cells = (all & RegionMaps[c]).ToArray();
						SkLoopTable[n][pos++] = cells[2];
						SkLoopTable[n][pos++] = cells[3];
						SkLoopTable[n][pos++] = cells[0];
						SkLoopTable[n++][pos++] = cells[1];
					}
				}
			}
		}
	}


	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(34, DisplayingLevel.D);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
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
			short[] masks = MaskMarshal.GetMaskSubsets(candidateMask);
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
				linkRegion[0] = RegionLabel.ToRegion(cells[0], RegionLabels.Row);
				linkRegion[1] = RegionLabel.ToRegion(cells[2], RegionLabels.Block);
				linkRegion[2] = RegionLabel.ToRegion(cells[4], RegionLabels.Column);
				linkRegion[3] = RegionLabel.ToRegion(cells[6], RegionLabels.Block);
				linkRegion[4] = RegionLabel.ToRegion(cells[8], RegionLabels.Row);
				linkRegion[5] = RegionLabel.ToRegion(cells[10], RegionLabels.Block);
				linkRegion[6] = RegionLabel.ToRegion(cells[12], RegionLabels.Column);
				linkRegion[7] = RegionLabel.ToRegion(cells[14], RegionLabels.Block);
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
				var candidateOffsets = new List<(int, ColorIdentifier)>();
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
								(
									cell * 9 + digit,
									(ColorIdentifier)((k & 3) switch { 0 => 1, 1 => 2, _ => 0 })
								)
							);
						}
					}
				}

				// Gather the result.
				var step = new DominoLoopStep(
					conclusions.ToImmutableArray(),
					ImmutableArray.Create(new PresentationData { Candidates = candidateOffsets }),
					cells
				);
				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}

		return null;
	}
}
