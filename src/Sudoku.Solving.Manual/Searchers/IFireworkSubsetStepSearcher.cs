namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Firework</b> step searcher. The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Firework Pair:
/// <list type="bullet">
/// <item>Firework Pair Type 1 (Single Firework + 2 Bi-value cells)</item>
/// <item>Firework Pair Type 2 (Double Fireworks)</item>
/// <item>Firework Pair Type 3 (Single Fireworks + Empty Rectangle)</item>
/// </list>
/// </item>
/// <item>Firework Triple</item>
/// <item>Firework Quadruple</item>
/// </list>
/// </summary>
public interface IFireworkSubsetStepSearcher : IFireworkStepSearcher
{
	/// <summary>
	/// Indicates the patterns used.
	/// </summary>
	protected static readonly FireworkPattern[] Patterns = new FireworkPattern[FireworkSubsetCount];

	/// <summary>
	/// Indicates the pattern pairs. This field is used for checking the technique firework pair type 2.
	/// </summary>
	protected static readonly (FireworkPattern First, FireworkPattern Second, int MeetCell)[] PatternPairs = new (FireworkPattern, FireworkPattern, int)[PairFireworksCount];


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

		i = 0;
		for (int firstIndex = 0; firstIndex < Patterns.Length - 1; firstIndex++)
		{
			for (int secondIndex = firstIndex + 1; secondIndex < Patterns.Length; secondIndex++)
			{
				scoped ref readonly var a = ref Patterns[firstIndex];
				scoped ref readonly var b = ref Patterns[secondIndex];
				if ((a, b) is not ((var aMap, { } aPivot), (var bMap, { } bPivot)))
				{
					continue;
				}

				if ((aMap & bMap) is not [])
				{
					continue;
				}

				if (aPivot.ToHouseIndex(HouseType.Block) == bPivot.ToHouseIndex(HouseType.Block))
				{
					continue;
				}

				if ((!(aMap - aPivot) & !(bMap - bPivot)) is not [var meetCell])
				{
					continue;
				}

				PatternPairs[i++] = (a, b, meetCell);
			}
		}
	}
}
