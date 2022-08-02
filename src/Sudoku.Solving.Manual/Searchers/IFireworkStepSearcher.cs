namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for firework steps.
/// </summary>
public interface IFireworkStepSearcher : IIntersectionStepSearcher
{
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
