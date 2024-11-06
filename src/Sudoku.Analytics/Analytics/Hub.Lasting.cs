namespace Sudoku.Analytics;

public partial class Hub
{
	/// <summary>
	/// Represents lasting rule of a single technique.
	/// </summary>
	public static class Lasting
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
				var (h, tempCount) = (cell.ToHouse(houseType), 0);
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
	}
}
