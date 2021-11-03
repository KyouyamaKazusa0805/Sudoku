namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Defines a chain step.
/// </summary>
public interface IChainStep : IStep
{
	/// <summary>
	/// Indicates the flat complexity.
	/// </summary>
	int FlatComplexity { get; }

	/// <summary>
	/// Indicates the sort key. The result will be a <see cref="ChainTypeCode"/> instance to describe
	/// what and how the chain behaves. Please visit the type <see cref="ChainTypeCode"/>
	/// to learn about more details for a chain.
	/// </summary>
	/// <seealso cref="ChainTypeCode"/>
	ChainTypeCode SortKey { get; }


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

	/// <summary>
	/// Determines whether two different list of <see cref="Conclusion"/>s holds the same values.
	/// </summary>
	/// <param name="lConclusions">The first conclusion list to compare.</param>
	/// <param name="rConclusions">The second conclusion list to compare.</param>
	/// <param name="shouldSort">Indicates whether the method will sort the lists firstly.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	protected static bool ConclusionsEquals(
		ImmutableArray<Conclusion> lConclusions,
		ImmutableArray<Conclusion> rConclusions,
		bool shouldSort
	)
	{
		if (lConclusions.Length != rConclusions.Length)
		{
			goto ReturnFalse;
		}

		// Sort the array.
		if (shouldSort)
		{
			ImmutableArray<Conclusion> lc = lConclusions.Sort(), rc = rConclusions.Sort();
			for (int i = 0, length = lc.Length; i < length; i++)
			{
				if (lc[i] != rc[i])
				{
					goto ReturnFalse;
				}
			}

			goto ReturnTrue;
		}
		else
		{
			for (int i = 0, length = lConclusions.Length; i < length; i++)
			{
				if (lConclusions[i] != rConclusions[i])
				{
					goto ReturnFalse;
				}
			}

			goto ReturnTrue;
		}

	ReturnTrue:
		return true;

	ReturnFalse:
		return false;
	}
}
