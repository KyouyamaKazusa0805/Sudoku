namespace Sudoku.Snyder;

/// <summary>
/// Represents a type that supports excluders marking inside a direct technique.
/// </summary>
public static class Excluder
{
	/// <summary>
	/// Get all <see cref="Cell"/> offsets that represents as excluders.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="excluderHouses">The excluder houses.</param>
	/// <returns>A <see cref="CellMap"/> instance.</returns>
	public static CellMap GetNakedSingleExcluderCells(ref readonly Grid grid, Cell cell, Digit digit, out House[] excluderHouses)
	{
		(var (result, i), excluderHouses) = ((CellMap.Empty, 0), new House[8]);
		foreach (var otherDigit in (Mask)(Grid.MaxCandidatesMask & (Mask)~(1 << digit)))
		{
			foreach (var otherCell in PeersMap[cell])
			{
				if (grid.GetDigit(otherCell) == otherDigit)
				{
					result.Add(otherCell);
					(cell.AsCellMap() + otherCell).InOneHouse(out excluderHouses[i]);
					i++;
					break;
				}
			}
		}
		return result;
	}

	/// <summary>
	/// Try to create a list of <see cref="IconViewNode"/>s indicating the crosshatching base cells.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="house">The house.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="chosenCells">The chosen cells.</param>
	/// <param name="excluderInfo">The excluder information.</param>
	/// <returns>A list of <see cref="IconViewNode"/> instances.</returns>
	public static ReadOnlySpan<IconViewNode> GetHiddenSingleExcluders(
		ref readonly Grid grid,
		Digit digit,
		House house,
		Cell cell,
		out CellMap chosenCells,
		out ExcluderInfo excluderInfo
	)
	{
		excluderInfo = ExcluderInfo.TryCreate(in grid, digit, house, in cell.AsCellMap())!;
		if (excluderInfo is var (cc, covered, excluded))
		{
			chosenCells = cc;
			return (IconViewNode[])[
				.. from c in chosenCells select new CircleViewNode(ColorIdentifier.Normal, c),
				..
				from c in covered
				let p = excluded.Contains(c) ? ColorIdentifier.Auxiliary2 : ColorIdentifier.Auxiliary1
				select (IconViewNode)(p == ColorIdentifier.Auxiliary2 ? new TriangleViewNode(p, c) : new CrossViewNode(p, c))
			];
		}

		chosenCells = [];
		return [];
	}

	/// <summary>
	/// Get all <see cref="IconViewNode"/>s that represents as excluders.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="excluderHouses">The excluder houses.</param>
	/// <returns>A list of <see cref="IconViewNode"/> instances.</returns>
	public static ReadOnlySpan<IconViewNode> GetNakedSingleExcluders(ref readonly Grid grid, Cell cell, Digit digit, out House[] excluderHouses)
	{
		(var (result, i), excluderHouses) = ((new IconViewNode[8], 0), new House[8]);
		foreach (var otherDigit in (Mask)(Grid.MaxCandidatesMask & (Mask)~(1 << digit)))
		{
			foreach (var otherCell in PeersMap[cell])
			{
				if (grid.GetDigit(otherCell) == otherDigit)
				{
					result[i] = new CircleViewNode(ColorIdentifier.Normal, otherCell);
					(cell.AsCellMap() + otherCell).InOneHouse(out excluderHouses[i]);
					i++;
					break;
				}
			}
		}
		return result;
	}

	/// <summary>
	/// Try to create a list of <see cref="IconViewNode"/>s indicating the crosshatching base cells.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="house">The house.</param>
	/// <param name="cells">The cells.</param>
	/// <returns>A list of <see cref="IconViewNode"/> instances.</returns>
	public static ReadOnlySpan<IconViewNode> GetLockedCandidatesExcluders(ref readonly Grid grid, Digit digit, House house, ref readonly CellMap cells)
	{
		var info = ExcluderInfo.TryCreate(in grid, digit, house, in cells);
		if (info is not var (combination, emptyCellsShouldBeCovered, emptyCellsNotNeedToBeCovered))
		{
			return [];
		}

		var result = new List<IconViewNode>();
		foreach (var c in combination)
		{
			result.Add(new CircleViewNode(ColorIdentifier.Normal, c));
		}
		foreach (var c in emptyCellsShouldBeCovered)
		{
			var p = emptyCellsNotNeedToBeCovered.Contains(c) ? ColorIdentifier.Auxiliary2 : ColorIdentifier.Auxiliary1;
			result.Add(p == ColorIdentifier.Auxiliary2 ? new TriangleViewNode(p, c) : new CrossViewNode(p, c));
		}
		return result.AsReadOnlySpan();
	}

	/// <summary>
	/// Try to create a list of <see cref="IconViewNode"/>s indicating the crosshatching base cells.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="house">The house.</param>
	/// <param name="cells">The cells.</param>
	/// <returns>A list of <see cref="IconViewNode"/> instances.</returns>
	public static ReadOnlySpan<IconViewNode> GetSubsetExcluders(ref readonly Grid grid, Digit digit, House house, ref readonly CellMap cells)
	{
		var info = ExcluderInfo.TryCreate(in grid, digit, house, in cells);
		if (info is not var (combination, emptyCellsShouldBeCovered, emptyCellsNotNeedToBeCovered))
		{
			return [];
		}

		var result = new List<IconViewNode>();
		foreach (var c in combination)
		{
			result.Add(new CircleViewNode(ColorIdentifier.Normal, c));
		}
		foreach (var c in emptyCellsShouldBeCovered)
		{
			var p = emptyCellsNotNeedToBeCovered.Contains(c) ? ColorIdentifier.Auxiliary2 : ColorIdentifier.Auxiliary1;
			result.Add(p == ColorIdentifier.Auxiliary2 ? new TriangleViewNode(p, c) : new CrossViewNode(p, c));
		}
		return result.AsReadOnlySpan();
	}
}
