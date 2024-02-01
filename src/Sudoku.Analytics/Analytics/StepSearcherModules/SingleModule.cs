namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents single module.
/// </summary>
internal static class SingleModule
{
	/// <summary>
	/// Try to create a list of <see cref="CellViewNode"/>s indicating the crosshatching base cells.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="house">The house.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="chosenCells">The chosen cells.</param>
	/// <returns>A list of <see cref="CellViewNode"/> instances.</returns>
	public static ReadOnlySpan<CellViewNode> GetHiddenSingleExcluders(
		scoped ref readonly Grid grid,
		Digit digit,
		House house,
		Cell cell,
		out CellMap chosenCells
	)
	{
		if (Crosshatching.GetCrosshatchingInfo(in grid, digit, house, in CellsMap[cell]) is { } info)
		{
			(chosenCells, var covered, var excluded) = info;
			return (CellViewNode[])[
				.. from c in chosenCells select new CellViewNode(ColorIdentifier.Normal, c) { RenderingMode = DirectModeOnly },
				..
				from c in covered
				let p = excluded.Contains(c) ? ColorIdentifier.Auxiliary2 : ColorIdentifier.Auxiliary1
				select new CellViewNode(p, c) { RenderingMode = DirectModeOnly }
			];
		}

		chosenCells = [];
		return [];
	}


	/// <summary>
	/// Get subtype of the hidden single.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="house">Indicates the house.</param>
	/// <param name="chosenCells">The chosen cells.</param>
	/// <returns>The subtype of the hidden single.</returns>
	public static SingleSubtype GetHiddenSingleSubtype(
		scoped ref readonly Grid grid,
		Cell cell,
		House house,
		scoped ref readonly CellMap chosenCells
	)
	{
		scoped ref readonly var houseCells = ref HousesMap[house];
		var (b, r, c) = (0, 0, 0);
		foreach (var chosenCell in chosenCells)
		{
			foreach (var houseType in HouseTypes)
			{
				if (HousesMap[chosenCell.ToHouseIndex(houseType)] & houseCells)
				{
					(houseType == HouseType.Block ? ref b : ref houseType == HouseType.Row ? ref r : ref c)++;
					break;
				}
			}
		}

		return Enum.Parse<SingleSubtype>(
			house switch
			{
				>= 0 and < 9 => $"{HouseType.Block}HiddenSingle0{r}{c}",
				>= 9 and < 18 => $"{HouseType.Row}HiddenSingle{b}0{c}",
				>= 18 and < 27 => $"{HouseType.Column}HiddenSingle{b}{r}0"
			}
		);
	}
}
