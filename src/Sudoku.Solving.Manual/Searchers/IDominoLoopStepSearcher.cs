namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Domino Loop</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Domino Loop</item>
/// </list>
/// </summary>
public interface IDominoLoopStepSearcher : IRankTheoryStepSearcher
{
	/// <summary>
	/// The position table of all SK-loops.
	/// </summary>
	protected static readonly int[][] SkLoopTable = new int[729][];


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static IDominoLoopStepSearcher()
	{
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
}
