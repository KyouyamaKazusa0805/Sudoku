namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Defines a step is chain-like one.
/// </summary>
public interface IChainLikeStep : IStep
{
	/// <summary>
	/// Get extra difficulty rating for a chain node sequence.
	/// </summary>
	/// <param name="length">The length.</param>
	/// <returns>The difficulty.</returns>
	protected static decimal GetExtraDifficultyByLength(int length)
	{
		decimal added = 0;
		int ceil = 4;
		for (bool isOdd = false; length > ceil; isOdd = !isOdd)
		{
			added += .1M;
			ceil = isOdd ? ceil * 4 / 3 : ceil * 3 / 2;
		}

		return added;
	}
}
