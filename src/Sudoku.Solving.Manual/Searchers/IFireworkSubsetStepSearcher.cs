namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Firework</b> step searcher. The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// <item>Firework Triple</item>
/// <item>Firework Quadruple</item>
/// </item>
/// </list>
/// </summary>
public interface IFireworkSubsetStepSearcher : IFireworkStepSearcher
{
	/// <summary>
	/// Indicates the patterns used.
	/// </summary>
	public static readonly FireworkPattern[] Patterns = new FireworkPattern[FireworkSubsetCount];


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static IFireworkSubsetStepSearcher()
	{
		int[][] houses =
		{
			new[] { 0, 1, 3, 4 }, new[] { 0, 2, 3, 5 }, new[] { 1, 2, 4, 5 },
			new[] { 0, 1, 6, 7 }, new[] { 0, 2, 6, 8 }, new[] { 1, 2, 7, 8 },
			new[] { 3, 4, 6, 7 }, new[] { 3, 5, 6, 8 }, new[] { 4, 5, 7, 8 }
		};

		int i = 0;
		foreach (int[] houseQuad in houses)
		{
			// Gather triples.
			foreach (int[] triple in houseQuad.GetSubsets(3))
			{
				foreach (int a in HouseMaps[triple[0]])
				{
					foreach (int b in HouseMaps[triple[1]])
					{
						foreach (int c in HouseMaps[triple[2]])
						{
							if (!(Cells.Empty + a + b).InOneHouse || !(Cells.Empty + b + c).InOneHouse)
							{
								continue;
							}

							Patterns[i++] = new(Cells.Empty + a + b + c, b);
						}
					}
				}
			}

			// Gather quadruples.
			foreach (int a in HouseMaps[houseQuad[0]])
			{
				foreach (int b in HouseMaps[houseQuad[1]])
				{
					foreach (int c in HouseMaps[houseQuad[2]])
					{
						foreach (int d in HouseMaps[houseQuad[3]])
						{
							if (!(Cells.Empty + a + b).InOneHouse || !(Cells.Empty + a + c).InOneHouse
								|| !(Cells.Empty + b + d).InOneHouse || !(Cells.Empty + c + d).InOneHouse)
							{
								continue;
							}

							Patterns[i++] = new(Cells.Empty + a + b + c + d, null);
						}
					}
				}
			}
		}
	}
}
