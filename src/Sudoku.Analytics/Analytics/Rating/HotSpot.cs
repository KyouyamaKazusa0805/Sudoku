namespace Sudoku.Analytics.Rating;

/// <summary>
/// The hotspot data.
/// </summary>
public static class HotSpot
{
	/// <summary>
	/// The internal table.
	/// </summary>
	private static readonly int[] HotSpotTable = [3, 2, 3, 2, 1, 2, 3, 2, 3, 5, 4, 3, 2, 1, 2, 3, 4, 5, 5, 4, 3, 2, 1, 2, 3, 4, 5];


	/// <summary>
	/// Try to get the hotspot of the house.
	/// </summary>
	/// <param name="house">The house.</param>
	/// <returns>The hotspot value.</returns>
	public static int GetHotSpot(House house) => HotSpotTable[house];
}
