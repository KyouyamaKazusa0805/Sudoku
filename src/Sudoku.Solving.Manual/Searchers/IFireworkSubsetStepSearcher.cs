namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Firework</b> step searcher. The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Subset types:
/// <list type="bullet">
/// <item>Firework Triple</item>
/// <item>Firework Quadruple</item>
/// </list>
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


	/// <summary>
	/// Determines whether the specified pattern forms a valid firework pattern.
	/// This method returns the digits that satisfied the condition. If none found,
	/// this method will return 0.
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
	protected static sealed short IsFirework(
		int c1, int c2, int pivot, scoped in Grid grid,
		scoped out Cells house1CellsExcluded, scoped out Cells house2CellsExcluded)
	{
		int pivotCellBlock = pivot.ToHouseIndex(HouseType.Block);
		var excluded1 = HouseMaps[(Cells.Empty + c1 + pivot).CoveredLine] - HouseMaps[pivotCellBlock] - c1;
		var excluded2 = HouseMaps[(Cells.Empty + c2 + pivot).CoveredLine] - HouseMaps[pivotCellBlock] - c2;

		short finalMask = 0;
		foreach (int digit in grid.GetDigitsUnion(Cells.Empty + c1 + c2 + pivot))
		{
			if (isFireworkFor(digit, excluded1, grid) && isFireworkFor(digit, excluded2, grid))
			{
				finalMask |= (short)(1 << digit);
			}
		}

		(house1CellsExcluded, house2CellsExcluded) = (excluded1, excluded2);
		return finalMask;


		static bool isFireworkFor(int digit, scoped in Cells houseCellsExcluded, scoped in Grid grid)
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
