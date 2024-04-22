namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents the factor that describes the length of a pattern.
/// </summary>
public abstract class LengthFactor : Factor
{
	/// <summary>
	/// Gets the difficulty rating value of the length. The length can be used in chaining-like techniques.
	/// </summary>
	/// <param name="length">The length of the pattern.</param>
	/// <returns>The result.</returns>
	protected internal static int GetLengthDifficulty(int length)
	{
		var result = 0;
		for (var (isOdd, ceil) = (false, 4); length > ceil; isOdd = !isOdd)
		{
			result++;
			ceil = isOdd ? ceil * 4 / 3 : ceil * 3 / 2;
		}
		return result;
	}
}
