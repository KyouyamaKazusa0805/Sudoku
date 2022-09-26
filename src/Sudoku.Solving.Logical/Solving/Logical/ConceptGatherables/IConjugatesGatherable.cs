namespace Sudoku.Solving.Logical.ConceptGatherables;

/// <summary>
/// Represents a type that can gather <see cref="Conjugate"/> instances that exists in a grid.
/// </summary>
internal interface IConjugatesGatherable : IConceptGatherable<Conjugate>
{
	/// <summary>
	/// Gathers possible conjugate pairs of the specified digit.
	/// </summary>
	/// <returns>The conjugate pairs found.</returns>
	/// <remarks><i>
	/// This method uses buffers, so you cannot use it everywhere. You should initialize the buffer first,
	/// by calling the method <see cref="InitializeMaps(in Grid)"/>.
	/// </i></remarks>
	/// <seealso cref="InitializeMaps(in Grid)"/>
	protected internal static sealed IEnumerable<Conjugate> Gather(int digit)
	{
		var result = new List<Conjugate>();
		for (var houseIndex = 0; houseIndex < 27; houseIndex++)
		{
			if ((HousesMap[houseIndex] & CandidatesMap[digit]) is { Count: 2 } temp)
			{
				result.Add(new(temp, digit));
			}
		}

		return result;
	}

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
	static ICollection<Conjugate>?[] IConceptGatherable<Conjugate>.Gather(scoped in Grid grid) => Gather();
}
