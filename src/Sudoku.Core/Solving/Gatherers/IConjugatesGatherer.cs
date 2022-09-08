namespace Sudoku.Solving.Manual.Searchers.Gatherers;

/// <summary>
/// Defines a conjugate pair gatherer.
/// </summary>
internal interface IConjugatesGatherer
{
	/// <summary>
	/// Gathers possible conjugate pairs grouped by digit.
	/// </summary>
	/// <param name="grid">The grid to be used.</param>
	/// <returns>The conjugate pairs found, grouped by digit.</returns>
	/// <remarks>
	/// This method uses buffers, so you cannot use it everywhere. You should initialize the buffer first,
	/// by calling the method <see cref="InitializeMaps(in Grid)"/>.
	/// </remarks>
	/// <seealso cref="InitializeMaps(in Grid)"/>
	protected internal static sealed ICollection<Conjugate>?[] Gather(scoped in Grid grid)
	{
		var conjugatePairs = new ICollection<Conjugate>?[9];
		for (int digit = 0; digit < 9; digit++)
		{
			for (int houseIndex = 0; houseIndex < 27; houseIndex++)
			{
				if ((HousesMap[houseIndex] & CandidatesMap[digit]) is { Count: 2 } temp)
				{
					(conjugatePairs[digit] ??= new List<Conjugate>()).Add(new(temp, digit));
				}
			}
		}

		return conjugatePairs;
	}
}
