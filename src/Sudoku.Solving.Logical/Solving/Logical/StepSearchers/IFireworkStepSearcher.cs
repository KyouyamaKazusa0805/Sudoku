namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Provides with a <b>Firework</b> step searcher. The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Firework Pair:
/// <list type="bullet">
/// <item>Firework Pair Type 1 (Single Firework + 2 Bi-value cells)</item>
/// <!--
/// <item>Firework Pair Type 2 (Double Fireworks)</item>
/// <item>Firework Pair Type 3 (Single Fireworks + Empty Rectangle)</item>
/// -->
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


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static IFireworkStepSearcher()
	{
		int[][] houses =
		{
			new[] { 0, 1, 3, 4 }, new[] { 0, 2, 3, 5 }, new[] { 1, 2, 4, 5 },
			new[] { 0, 1, 6, 7 }, new[] { 0, 2, 6, 8 }, new[] { 1, 2, 7, 8 },
			new[] { 3, 4, 6, 7 }, new[] { 3, 5, 6, 8 }, new[] { 4, 5, 7, 8 }
		};

		var i = 0;
		foreach (var houseQuad in houses)
		{
			// Gather triples.
			foreach (var triple in houseQuad.GetSubsets(3))
			{
				foreach (var a in HousesMap[triple[0]])
				{
					foreach (var b in HousesMap[triple[1]])
					{
						foreach (var c in HousesMap[triple[2]])
						{
							if ((CellsMap[a] + b).InOneHouse && (CellsMap[a] + c).InOneHouse)
							{
								Patterns[i++] = new(CellsMap[a] + b + c, a);
								continue;
							}

							if ((CellsMap[a] + b).InOneHouse && (CellsMap[b] + c).InOneHouse)
							{
								Patterns[i++] = new(CellsMap[a] + b + c, b);
								continue;
							}

							if ((CellsMap[a] + c).InOneHouse && (CellsMap[b] + c).InOneHouse)
							{
								Patterns[i++] = new(CellsMap[a] + b + c, c);
							}
						}
					}
				}
			}

			// Gather quadruples.
			foreach (var a in HousesMap[houseQuad[0]])
			{
				foreach (var b in HousesMap[houseQuad[1]])
				{
					foreach (var c in HousesMap[houseQuad[2]])
					{
						foreach (var d in HousesMap[houseQuad[3]])
						{
							if (!(CellsMap[a] + b).InOneHouse || !(CellsMap[a] + c).InOneHouse
								|| !(CellsMap[b] + d).InOneHouse || !(CellsMap[c] + d).InOneHouse)
							{
								continue;
							}

							Patterns[i++] = new(CellsMap[a] + b + c + d, null);
						}
					}
				}
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
		var pivotCellBlock = pivot.ToHouseIndex(HouseType.Block);
		var excluded1 = HousesMap[(CellsMap[c1] + pivot).CoveredLine] - HousesMap[pivotCellBlock] - c1;
		var excluded2 = HousesMap[(CellsMap[c2] + pivot).CoveredLine] - HousesMap[pivotCellBlock] - c2;

		short finalMask = 0;
		foreach (var digit in grid.GetDigitsUnion(CellsMap[c1] + c2 + pivot))
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
			foreach (var cell in houseCellsExcluded)
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
