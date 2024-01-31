namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>(Siamese) (Grouped) XYZ-Ring</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="intersectedDigit">Indicates the digit Z for XYZ-Wing pattern.</param>
/// <param name="pivot">Indicates the pivot cell.</param>
/// <param name="leafCell1">Indicates the leaf cell 1.</param>
/// <param name="leafCell2">Indicates the leaf cell 2.</param>
/// <param name="conjugateHousesMask">Indicates the conjugate houses used.</param>
/// <param name="isNice">Indicates whether the pattern is a nice loop.</param>
/// <param name="isGrouped">Indicates whether the conjugate pair is grouped one.</param>
/// <param name="isSiamese">Indicates whether the XYZ-loop is a Siamese one.</param>
public sealed partial class XyzRingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[RecordParameter] Digit intersectedDigit,
	[RecordParameter] Cell pivot,
	[RecordParameter] Cell leafCell1,
	[RecordParameter] Cell leafCell2,
	[RecordParameter] HouseMask conjugateHousesMask,
	[RecordParameter] bool isNice,
	[RecordParameter] bool isGrouped,
	[RecordParameter] bool isSiamese = false
) : WingStep(conclusions, views, options), ISiameseSupporter<XyzRingStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => IsNice ? 5.0M : 5.2M;

	/// <inheritdoc/>
	public override Technique Code
		=> (IsSiamese, IsGrouped, IsNice) switch
		{
			(true, true, true) => Technique.SiameseGroupedXyzNiceLoop,
			(_, true, true) => Technique.GroupedXyzNiceLoop,
			(true, true, _) => Technique.SiameseGroupedXyzLoop,
			(_, true, _) => Technique.GroupedXyzLoop,
			(true, _, true) => Technique.SiameseXyzNiceLoop,
			(_, _, true) => Technique.XyzNiceLoop,
			(true, _, _) => Technique.SiameseXyzLoop,
			_ => Technique.XyzLoop
		};


	/// <inheritdoc/>
	public static ReadOnlySpan<XyzRingStep> GetSiamese(List<XyzRingStep> accumulator, scoped ref readonly Grid grid)
	{
		var result = new List<XyzRingStep>();
		scoped var stepsSpan = accumulator.AsReadOnlySpan();
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


		static bool check(scoped ref readonly Grid grid, XyzRingStep xyz1, XyzRingStep xyz2, [NotNullWhen(true)] out XyzRingStep? siameseStep)
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

			var mergedConclusions = (xyz1.Conclusions.AsConclusionSet() | xyz2.Conclusions.AsConclusionSet()).ToArray();
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
