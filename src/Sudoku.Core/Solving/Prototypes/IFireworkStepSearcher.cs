namespace Sudoku.Solving.Prototypes;

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
public interface IFireworkStepSearcher : IIntersectionStepSearcher
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
	static IFireworkStepSearcher()
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
				foreach (int a in HousesMap[triple[0]])
				{
					foreach (int b in HousesMap[triple[1]])
					{
						foreach (int c in HousesMap[triple[2]])
						{
							if ((CellMap.Empty + a + b).InOneHouse && (CellMap.Empty + a + c).InOneHouse)
							{
								Patterns[i++] = new(CellMap.Empty + a + b + c, a);
								continue;
							}

							if ((CellMap.Empty + a + b).InOneHouse && (CellMap.Empty + b + c).InOneHouse)
							{
								Patterns[i++] = new(CellMap.Empty + a + b + c, b);
								continue;
							}

							if ((CellMap.Empty + a + c).InOneHouse && (CellMap.Empty + b + c).InOneHouse)
							{
								Patterns[i++] = new(CellMap.Empty + a + b + c, c);
							}
						}
					}
				}
			}

			// Gather quadruples.
			foreach (int a in HousesMap[houseQuad[0]])
			{
				foreach (int b in HousesMap[houseQuad[1]])
				{
					foreach (int c in HousesMap[houseQuad[2]])
					{
						foreach (int d in HousesMap[houseQuad[3]])
						{
							if (!(CellMap.Empty + a + b).InOneHouse || !(CellMap.Empty + a + c).InOneHouse
								|| !(CellMap.Empty + b + d).InOneHouse || !(CellMap.Empty + c + d).InOneHouse)
							{
								continue;
							}

							Patterns[i++] = new(CellMap.Empty + a + b + c + d, null);
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

				if (aMap & bMap)
				{
					continue;
				}

				if (aPivot.ToHouseIndex(HouseType.Block) == bPivot.ToHouseIndex(HouseType.Block))
				{
					continue;
				}

				if ((+(aMap - aPivot) & +(bMap - bPivot)) is not [var meetCell])
				{
					continue;
				}

				PatternPairs[i++] = (a, b, meetCell);
			}
		}
	}


	/// <summary>
	/// <para>Checks for all digits which the cells containing form a firework pattern.</para>
	/// <para>
	/// This method returns the digits that satisfied the condition. If none found,
	/// this method will return 0.
	/// </para>
	/// </summary>
	/// <param name="c1">The cell 1 used in this pattern.</param>
	/// <param name="c2">The cell 2 used in this pattern.</param>
	/// <param name="pivot">The pivot cell.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="house1CellsExcluded">
	/// The excluded cells that is out of the firework structure in the <paramref name="c1"/>'s house.
	/// </param>
	/// <param name="house2CellsExcluded">
	/// The excluded cells that is out of the firework structure in the <paramref name="c2"/>'s house.
	/// </param>
	/// <returns>All digits that satisfied the firework rule. If none found, 0.</returns>
	protected static sealed short GetFireworkDigits(
		int c1, int c2, int pivot, scoped in Grid grid,
		scoped out CellMap house1CellsExcluded, scoped out CellMap house2CellsExcluded)
	{
		int pivotCellBlock = pivot.ToHouseIndex(HouseType.Block);
		var excluded1 = HousesMap[(CellMap.Empty + c1 + pivot).CoveredLine] - HousesMap[pivotCellBlock] - c1;
		var excluded2 = HousesMap[(CellMap.Empty + c2 + pivot).CoveredLine] - HousesMap[pivotCellBlock] - c2;

		short finalMask = 0;
		foreach (int digit in grid.GetDigitsUnion(CellMap.Empty + c1 + c2 + pivot))
		{
			if (isFireworkFor(digit, excluded1, grid) && isFireworkFor(digit, excluded2, grid))
			{
				finalMask |= (short)(1 << digit);
			}
		}

		(house1CellsExcluded, house2CellsExcluded) = (excluded1, excluded2);
		return finalMask;


		static bool isFireworkFor(int digit, scoped in CellMap houseCellsExcluded, scoped in Grid grid)
		{
			foreach (int cell in houseCellsExcluded)
			{
				switch (grid[cell])
				{
					case -1 when CandidatesMap[digit].Contains(cell):
					case var cellValue when cellValue == digit:
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
