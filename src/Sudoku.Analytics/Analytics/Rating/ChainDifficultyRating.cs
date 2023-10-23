namespace Sudoku.Analytics.Rating;

/// <summary>
/// Represents a type that calculates for chain difficulty.
/// </summary>
public static class ChainDifficultyRating
{
	/// <summary>
	/// Get extra difficulty rating for a chain node sequence.
	/// </summary>
	/// <param name="length">The length.</param>
	/// <returns>The difficulty.</returns>
	public static decimal GetExtraDifficultyByLength(int length)
	{
		var result = 0M;
		for (var (isOdd, ceil) = (false, 4); length > ceil; isOdd = !isOdd)
		{
			result += .1M;
			ceil = isOdd ? ceil * 4 / 3 : ceil * 3 / 2;
		}

		return result;
	}
}
