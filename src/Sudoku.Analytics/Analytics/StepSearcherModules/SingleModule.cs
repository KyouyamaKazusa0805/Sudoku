namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents single module.
/// </summary>
internal static class SingleModule
{
	/// <summary>
	/// Gets the lasting value of the full house in the target house.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="house">The house.</param>
	/// <returns>The lasting value.</returns>
	public static int GetLasting(ref readonly Grid grid, Cell cell, House house)
	{
		var result = 0;
		foreach (var c in HousesMap[house])
		{
			if (grid.GetState(c) == CellState.Empty)
			{
				result++;
			}
		}
		return result;
	}

	/// <summary>
	/// Gets the lasting value of the naked single in the target direction.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="house">The house.</param>
	/// <returns>The lasting value.</returns>
	public static int GetLastingAllHouses(ref readonly Grid grid, Cell cell, out House house)
	{
		var (resultCount, resultHouse) = (9, 0);
		foreach (var houseType in HouseTypes)
		{
			var (h, tempCount) = (cell.ToHouseIndex(houseType), 0);
			foreach (var c in HousesMap[h])
			{
				if (grid.GetState(c) == CellState.Empty)
				{
					tempCount++;
				}
			}

			if (tempCount <= resultCount)
			{
				(resultCount, resultHouse) = (tempCount, h);
			}
		}

		house = resultHouse;
		return resultCount;
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
		ref readonly Grid grid,
		Cell cell,
		House house,
		ref readonly CellMap chosenCells
	)
	{
		ref readonly var houseCells = ref HousesMap[house];
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

	/// <summary>
	/// Get subtype of the naked single.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="cell">The cell.</param>
	/// <returns>The subtype of the naked single.</returns>
	public static SingleSubtype GetNakedSingleSubtype(ref readonly Grid grid, Cell cell)
	{
		var (valuesCountInBlock, valuesCountInRow, valuesCountInColumn) = (0, 0, 0);
		foreach (var houseType in HouseTypes)
		{
			foreach (var c in HousesMap[cell.ToHouseIndex(houseType)])
			{
				if (grid.GetState(c) != CellState.Empty)
				{
					(
						houseType == HouseType.Block
							? ref valuesCountInBlock
							: ref houseType == HouseType.Row ? ref valuesCountInRow : ref valuesCountInColumn
					)++;
				}
			}
		}
		var maxValue = MathExtensions.Max(valuesCountInBlock, valuesCountInRow, valuesCountInColumn);
		return Enum.Parse<SingleSubtype>(
			maxValue == valuesCountInBlock
				? $"NakedSingleBlock{maxValue}"
				: maxValue == valuesCountInRow ? $"NakedSingleRow{maxValue}" : $"NakedSingleColumn{maxValue}"
		);
	}
}
