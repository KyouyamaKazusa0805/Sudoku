namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents excluders information used by a Hidden Single step, describing which cells can raise conclusion,
/// and exclude which cells.
/// </summary>
/// <param name="BaseCells">The base cells to be used.</param>
/// <param name="EmptyCells">The empty cells in the final.</param>
/// <param name="ExcludedCells">The excluded cells to be used.</param>
public sealed record ExcluderInfo(in CellMap BaseCells, in CellMap EmptyCells, in CellMap ExcludedCells) :
	IComponent,
	IEqualityOperators<ExcluderInfo, ExcluderInfo, bool>
{
	/// <inheritdoc/>
	ComponentType IComponent.Type => ComponentType.Excluder;


	/// <summary>
	/// Try to get a pair of <see cref="CellMap"/> instances indicating the crosshatching information for the specified house,
	/// with the specified digit in the specified grid.
	/// </summary>
	/// <param name="grid">The puzzle relying on.</param>
	/// <param name="digit">The digit to be checked.</param>
	/// <param name="house">The house to be checked.</param>
	/// <param name="cells">The cell to be checked. The cell is the final hidden single cell or the locked candidate cells.</param>
	/// <returns>The result pair.</returns>
	public static ExcluderInfo? TryCreate(in Grid grid, Digit digit, House house, in CellMap cells)
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
		foreach (ref readonly var valueCombination in values | values.Count)
		{
			if ((valueCombination.ExpandedPeers & houseCells & emptyCells & ~cells) == (emptyCellsShouldBeCovered & ~emptyCellsNotNeedToBeCovered))
			{
				nullableCombination = valueCombination;
				break;
			}
		}

		return (nullableCombination, emptyCellsNotNeedToBeCovered) switch
		{
			(null, not []) => new([], emptyCellsShouldBeCovered, emptyCellsNotNeedToBeCovered),
			({ } combination, _) => new(combination, emptyCellsShouldBeCovered, emptyCellsNotNeedToBeCovered),
			_ => null
		};
	}
}
