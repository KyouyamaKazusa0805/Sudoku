namespace Sudoku.Concepts;

/// <summary>
/// Defines the target crosshatching information.
/// </summary>
/// <param name="BaseCells">The base cells to be used.</param>
/// <param name="EmptyCells">The empty cells in the final.</param>
/// <param name="ExcludedCells">The excluded cells to be used.</param>
public sealed record Crosshatching(ref readonly CellMap BaseCells, ref readonly CellMap EmptyCells, ref readonly CellMap ExcludedCells)
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
	public static Crosshatching? TryCreate(ref readonly Grid grid, Digit digit, House house, ref readonly CellMap cells)
	{
		var (houseCells, valueCells, emptyCells) = (HousesMap[house], grid.ValuesMap[digit], grid.EmptyCells);
		var (emptyCellsShouldBeCovered, emptyCellsNotNeedToBeCovered, values) = (houseCells & ~cells & emptyCells, CellMap.Empty, CellMap.Empty);
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
		foreach (ref readonly var valueCombination in values.GetSubsetsAll())
		{
			if ((valueCombination.ExpandedPeers & houseCells & emptyCells & ~cells)
				== (emptyCellsShouldBeCovered & ~emptyCellsNotNeedToBeCovered))
			{
				nullableCombination = valueCombination;
				break;
			}
		}

		return (nullableCombination, emptyCellsNotNeedToBeCovered) switch
		{
			(null, not []) => new([], in emptyCellsShouldBeCovered, in emptyCellsNotNeedToBeCovered),
			({ } combination, _) => new(in combination, in emptyCellsShouldBeCovered, in emptyCellsNotNeedToBeCovered),
			_ => null
		};
	}
}
