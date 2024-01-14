namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents a fish module.
/// </summary>
internal static class FishModule
{
	/// <summary>
	/// Check whether the fish is sashimi.
	/// </summary>
	/// <param name="baseSets">The base sets.</param>
	/// <param name="fins">All fins.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>
	/// <para>A <see cref="bool"/> value indicating that.</para>
	/// <para>
	/// <inheritdoc cref="FishStep.IsSashimi" path="/remarks"/>
	/// </para>
	/// </returns>
	public static bool? IsSashimi(House[] baseSets, scoped ref readonly CellMap fins, Digit digit)
	{
		if (!fins)
		{
			return null;
		}

		var isSashimi = false;
		foreach (var baseSet in baseSets)
		{
			if ((HousesMap[baseSet] - fins & CandidatesMap[digit]).Count == 1)
			{
				isSashimi = true;
				break;
			}
		}

		return isSashimi;
	}

	/// <summary>
	/// Determine the fish kind of the shape.
	/// </summary>
	/// <param name="pattern">The fish pattern.</param>
	/// <returns>The shape kind.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FishShapeKind GetShapeKind(FishStep pattern)
	{
		return pattern switch
		{
			ComplexFishStep { BaseSetsMask: var baseSets, CoverSetsMask: var coverSets } => (k(baseSets), k(coverSets)) switch
			{
				(FishShapeKind.Mutant, _) or (_, FishShapeKind.Mutant) => FishShapeKind.Mutant,
				(FishShapeKind.Franken, _) or (_, FishShapeKind.Franken) => FishShapeKind.Franken,
				_ => FishShapeKind.Basic
			},
			_ => FishShapeKind.Basic
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static FishShapeKind k(HouseMask mask)
		{
			var blockMask = mask & Grid.MaxCandidatesMask;
			var rowMask = mask >> 9 & Grid.MaxCandidatesMask;
			var columnMask = mask >> 18 & Grid.MaxCandidatesMask;
			return rowMask * columnMask != 0
				? FishShapeKind.Mutant
				: (rowMask | columnMask) != 0 && blockMask != 0
					? FishShapeKind.Franken
					: FishShapeKind.Basic;
		}
	}
}
