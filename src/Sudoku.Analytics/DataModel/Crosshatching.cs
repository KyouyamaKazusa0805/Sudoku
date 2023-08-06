namespace Sudoku.DataModel;

/// <summary>
/// Represents a list of methods that operates with single techniques, in order to get crosshatching information.
/// </summary>
internal static class Crosshatching
{
	/// <summary>
	/// Try to get a pair of <see cref="CellMap"/> instances indicating the crosshatching information for the specified house,
	/// with the specified digit in the specified grid.
	/// </summary>
	/// <param name="grid">The puzzle relying on.</param>
	/// <param name="digit">The digit to be checked.</param>
	/// <param name="house">The house to be checked.</param>
	/// <param name="cells">The cell to be checked. The cell is the final hidden single cell or the locked candidate cells.</param>
	/// <returns>The result pair.</returns>
	public static CrosshatchingInfo? GetCrosshatchingInfo(scoped in Grid grid, Digit digit, House house, scoped in CellMap cells)
	{
		var (houseCells, valueCells, emptyCells) = (HousesMap[house], grid.ValuesMap[digit], grid.EmptyCells);
		var (emptyCellsShouldBeCovered, emptyCellsNotNeedToBeCovered, values) = (houseCells - cells & emptyCells, CellMap.Empty, CellMap.Empty);
		foreach (var c in emptyCellsShouldBeCovered)
		{
			var tempValues = PeersMap[c] & valueCells;
			if (tempValues)
			{
				values |= tempValues;
			}
			else
			{
				emptyCellsNotNeedToBeCovered.Add(c);
			}
		}

		var nullableCombination = default(CellMap?);
		foreach (var valueCombination in values.GetAllSubsets())
		{
			if ((valueCombination.ExpandedPeers & houseCells & emptyCells) - cells == emptyCellsShouldBeCovered - emptyCellsNotNeedToBeCovered)
			{
				nullableCombination = valueCombination;
				break;
			}
		}

		return (nullableCombination, emptyCellsNotNeedToBeCovered) switch
		{
			(null, not []) => new(CellMap.Empty, emptyCellsShouldBeCovered, emptyCellsNotNeedToBeCovered),
			({ } combination, _) => new(combination, emptyCellsShouldBeCovered, emptyCellsNotNeedToBeCovered),
			_ => null
		};
	}
}
