namespace Sudoku.Solving.Prototypes;

/// <summary>
/// Provides with a <b>Domino Loop</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Domino Loop</item>
/// </list>
/// </summary>
public interface IDominoLoopStepSearcher : INonnegativeRankStepSearcher
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
			for (var b = 9; b < 18; b++)
			{
				if (a / 3 == b / 3 || b < a)
				{
					continue;
				}

				for (var c = 18; c < 27; c++)
				{
					for (var d = 18; d < 27; d++)
					{
						if (c / 3 == d / 3 || d < c)
						{
							continue;
						}

						var all = HousesMap[a] | HousesMap[b] | HousesMap[c] | HousesMap[d];
						var overlap = (HousesMap[a] | HousesMap[b]) & (HousesMap[c] | HousesMap[d]);
						var blockMask = overlap.BlockMask;
						for (int i = 0, count = 0; count < 4 && i < 16; i++)
						{
							if ((blockMask >> i & 1) != 0)
							{
								s[count++] = i;
							}
						}

						all &= HousesMap[s[0]] | HousesMap[s[1]] | HousesMap[s[2]] | HousesMap[s[3]];
						all -= overlap;

						SkLoopTable[n] = new int[16];
						var pos = 0;
						foreach (var cell in all & HousesMap[a])
						{
							SkLoopTable[n][pos++] = cell;
						}
						foreach (var cell in all & HousesMap[d])
						{
							SkLoopTable[n][pos++] = cell;
						}
						var cells = (all & HousesMap[b]).ToArray();
						SkLoopTable[n][pos++] = cells[2];
						SkLoopTable[n][pos++] = cells[3];
						SkLoopTable[n][pos++] = cells[0];
						SkLoopTable[n][pos++] = cells[1];
						cells = (all & HousesMap[c]).ToArray();
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
