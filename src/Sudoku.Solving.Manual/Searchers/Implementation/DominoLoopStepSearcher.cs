namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Domino Loop</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Domino Loop</item>
/// </list>
/// </summary>
[StepSearcher]
internal sealed unsafe partial class DominoLoopStepSearcher : IDominoLoopStepSearcher
{
	/// <summary>
	/// The position table of all SK-loops.
	/// </summary>
	private static readonly int[][] SkLoopTable = new int[729][];

	/// <summary>
	/// The house maps.
	/// </summary>
	private static readonly Cells[] HouseMaps;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static DominoLoopStepSearcher()
	{
		// Initialize for house maps.
		HouseMaps = new Cells[27];
		for (int i = 0; i < 27; i++)
		{
			scoped ref var map = ref HouseMaps[i];
			foreach (int cell in HouseCells[i])
			{
				map.Add(cell);
			}
		}

		// Initialize for SK-loop table.
		scoped var s = (stackalloc int[4]);
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

						var all = HouseMaps[a] | HouseMaps[b] | HouseMaps[c] | HouseMaps[d];
						var overlap = (HouseMaps[a] | HouseMaps[b]) & (HouseMaps[c] | HouseMaps[d]);
						short blockMask = overlap.BlockMask;
						for (int i = 0, count = 0; count < 4 && i < 16; i++)
						{
							if ((blockMask >> i & 1) != 0)
							{
								s[count++] = i;
							}
						}

						all &= HouseMaps[s[0]] | HouseMaps[s[1]] | HouseMaps[s[2]] | HouseMaps[s[3]];
						all -= overlap;

						SkLoopTable[n] = new int[16];
						int pos = 0;
						foreach (int cell in all & HouseMaps[a])
						{
							SkLoopTable[n][pos++] = cell;
						}
						foreach (int cell in all & HouseMaps[d])
						{
							SkLoopTable[n][pos++] = cell;
						}
						int[] cells = (all & HouseMaps[b]).ToArray();
						SkLoopTable[n][pos++] = cells[2];
						SkLoopTable[n][pos++] = cells[3];
						SkLoopTable[n][pos++] = cells[0];
						SkLoopTable[n][pos++] = cells[1];
						cells = (all & HouseMaps[c]).ToArray();
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
	public Step? GetAll(ICollection<Step> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		short* pairs = stackalloc short[8], tempLink = stackalloc short[8];
		int* linkHouse = stackalloc int[8];
		foreach (int[] cells in SkLoopTable)
		{
			// Initialize the elements.
			int n = 0, candidateCount = 0, i = 0;
			for (i = 0; i < 8; i++)
			{
				pairs[i] = default;
				linkHouse[i] = default;
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

				if (n > 4 || PopCount((uint)pairs[i]) > 5 || pairs[i] == 0)
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
				linkHouse[0] = cells[0].ToHouseIndex(HouseType.Row);
				linkHouse[1] = cells[2].ToHouseIndex(HouseType.Block);
				linkHouse[2] = cells[4].ToHouseIndex(HouseType.Column);
				linkHouse[3] = cells[6].ToHouseIndex(HouseType.Block);
				linkHouse[4] = cells[8].ToHouseIndex(HouseType.Row);
				linkHouse[5] = cells[10].ToHouseIndex(HouseType.Block);
				linkHouse[6] = cells[12].ToHouseIndex(HouseType.Column);
				linkHouse[7] = cells[14].ToHouseIndex(HouseType.Block);
				var conclusions = new List<Conclusion>();
				var map = (Cells)cells & EmptyCells;
				for (k = 0; k < 8; k++)
				{
					if ((HouseMaps[linkHouse[k]] & EmptyCells) - map is not { Count: not 0 } elimMap)
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
				var candidateOffsets = new List<CandidateViewNode>();
				short[] link = new short[27];
				for (k = 0; k < 8; k++)
				{
					link[linkHouse[k]] = tempLink[k];
					foreach (int cell in map & HouseMaps[linkHouse[k]])
					{
						short cands = (short)(grid.GetCandidates(cell) & tempLink[k]);
						if (cands == 0)
						{
							continue;
						}

						foreach (int digit in cands)
						{
							candidateOffsets.Add(
								new(
									(k & 3) switch
									{
										0 => DisplayColorKind.Auxiliary1,
										1 => DisplayColorKind.Auxiliary2,
										_ => DisplayColorKind.Normal
									},
									cell * 9 + digit
								)
							);
						}
					}
				}

				// Gather the result.
				var step = new DominoLoopStep(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(View.Empty | candidateOffsets),
					(Cells)cells
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
