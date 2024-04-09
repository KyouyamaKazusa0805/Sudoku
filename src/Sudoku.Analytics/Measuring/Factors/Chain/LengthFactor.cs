namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents the factor that describes the length of a pattern.
/// </summary>
/// <param name="options"><inheritdoc/></param>
public abstract class LengthFactor(StepSearcherOptions options) : Factor(options)
{
	/// <summary>
	/// Gets the difficulty rating value of the length. The length can be used in chaining-like techniques.
	/// </summary>
	/// <param name="length">The length of the pattern.</param>
	/// <returns>The result.</returns>
	protected static int GetLengthDifficulty(int length)
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
