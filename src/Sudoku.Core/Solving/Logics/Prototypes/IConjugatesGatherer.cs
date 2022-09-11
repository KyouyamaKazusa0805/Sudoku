namespace Sudoku.Solving.Logics.Prototypes;

/// <summary>
/// Defines a conjugate pair gatherer.
/// </summary>
internal interface IConjugatesGatherer : IStructureGatherer<Conjugate>
{
	/// <summary>
	/// Gathers possible conjugate pairs grouped by digit.
	/// </summary>
	/// <returns>The conjugate pairs found, grouped by digit.</returns>
	/// <remarks><i>
	/// This method uses buffers, so you cannot use it everywhere. You should initialize the buffer first,
	/// by calling the method <see cref="InitializeMaps(in Grid)"/>.
	/// </i></remarks>
	/// <seealso cref="InitializeMaps(in Grid)"/>
	protected internal static sealed ICollection<Conjugate>?[] Gather()
	{
		var conjugatePairs = new ICollection<Conjugate>?[9];
		for (var digit = 0; digit < 9; digit++)
		{
			for (var houseIndex = 0; houseIndex < 27; houseIndex++)
			{
				if ((HousesMap[houseIndex] & CandidatesMap[digit]) is { Count: 2 } temp)
				{
					(conjugatePairs[digit] ??= new List<Conjugate>()).Add(new(temp, digit));
				}
			}
		}

		return conjugatePairs;
	}

	/// <inheritdoc/>
	static ICollection<Conjugate>?[] IStructureGatherer<Conjugate>.Gather(scoped in Grid grid) => Gather();
}
