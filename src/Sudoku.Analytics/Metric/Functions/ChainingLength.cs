namespace Sudoku.Metric.Functions;

/// <summary>
/// Represents a function that calculates for length on chaining rules.
/// </summary>
public sealed class ChainingLength : IFunctionProvider
{
	/// <summary>
	/// Gets the difficulty rating value of the length. The length can be used in chaining-like techniques.
	/// </summary>
	/// <param name="length">The length of the pattern.</param>
	/// <returns>The result.</returns>
	public static int GetLengthDifficulty(int length)
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
