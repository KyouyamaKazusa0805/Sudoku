namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents XYZ-Ring module.
/// </summary>
internal static class XyzRingModule
{
	/// <summary>
	/// Get all possible Siamese patterns of type <see cref="XyzRingStep"/> for the specified accumulator.
	/// </summary>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <returns>All Siamese steps of type <see cref="XyzRingStep"/>.</returns>
	public static ReadOnlySpan<XyzRingStep> GetSiamese(List<XyzRingStep> accumulator, ref readonly Grid grid)
	{
		var result = new List<XyzRingStep>();
		var stepsSpan = accumulator.AsReadOnlySpan();
		for (var index1 = 0; index1 < accumulator.Count - 1; index1++)
		{
			var xyz1 = stepsSpan[index1];
			for (var index2 = index1 + 1; index2 < accumulator.Count; index2++)
			{
				var xyz2 = stepsSpan[index2];
				if (check(in grid, xyz1, xyz2, out var siameseStep))
				{
					result.Add(siameseStep);
				}
			}
		}
		return result.AsReadOnlySpan();


		static bool check(ref readonly Grid grid, XyzRingStep xyz1, XyzRingStep xyz2, [NotNullWhen(true)] out XyzRingStep? siameseStep)
		{
			if ((xyz1.Pivot, xyz1.LeafCell1, xyz1.LeafCell2) != (xyz2.Pivot, xyz2.LeafCell1, xyz2.LeafCell2))
			{
				// Should be same XYZ-Wing.
				goto ReturnFalse;
			}

			if (xyz1.IntersectedDigit != xyz2.IntersectedDigit)
			{
				// They should contain a same digit Z in XYZ-Wing pattern.
				goto ReturnFalse;
			}

			var house1 = Log2((uint)xyz1.ConjugateHousesMask);
			var house2 = Log2((uint)xyz2.ConjugateHousesMask);
			if ((HousesMap[house1] & CandidatesMap[xyz1.IntersectedDigit]) == (HousesMap[house2] & CandidatesMap[xyz1.IntersectedDigit]))
			{
				// They cannot hold a same cells of the conjugate.
				goto ReturnFalse;
			}

			var mergedConclusions = (xyz1.Conclusions.AsSet() | xyz2.Conclusions.AsSet()).ToDigitsArray();
			var xyz1ViewNodes = xyz1.Views![0];
			var xyz2ViewNodes = xyz2.Views![0];
			siameseStep = new(
				mergedConclusions,
				[
					[
						.. from node in xyz1ViewNodes where node.Identifier == ColorIdentifier.Auxiliary2 select node,
						.. from node in xyz2ViewNodes where node.Identifier == ColorIdentifier.Auxiliary2 select node,
						.. xyz1ViewNodes
					],
					xyz1ViewNodes,
					xyz2ViewNodes
				],
				xyz1.Options,
				xyz1.IntersectedDigit,
				xyz1.Pivot,
				xyz1.LeafCell1,
				xyz1.LeafCell2,
				xyz1.ConjugateHousesMask | xyz2.ConjugateHousesMask,
				xyz1.IsNice && xyz2.IsNice,
				xyz1.IsGrouped || xyz2.IsGrouped,
				true
			);
			return true;

		ReturnFalse:
			siameseStep = null;
			return false;
		}
	}
}
