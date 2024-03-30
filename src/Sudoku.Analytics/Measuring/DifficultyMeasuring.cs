namespace Sudoku.Measuring;

/// <summary>
/// Represents a factory type that stores a list of commonly-used difficulty-measuring formulas and its corresponding rule name.
/// </summary>
[Obsolete("This type is being deprecated. I will extract the type into a new position to store formulas, allow users modifying and customize calculation.", false)]
public static class DifficultyMeasuring
{
	/// <summary>
	/// Gets the difficulty rating value of the length. The length can be used in chaining-like techniques.
	/// </summary>
	/// <param name="length">The length of the pattern.</param>
	/// <returns>The result.</returns>
	public static decimal GetLengthDifficulty(int length)
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
