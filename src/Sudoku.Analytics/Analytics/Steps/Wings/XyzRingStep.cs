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
	[PrimaryConstructorParameter] Digit intersectedDigit,
	[PrimaryConstructorParameter] Cell pivot,
	[PrimaryConstructorParameter] Cell leafCell1,
	[PrimaryConstructorParameter] Cell leafCell2,
	[PrimaryConstructorParameter] HouseMask conjugateHousesMask,
	[PrimaryConstructorParameter] bool isNice,
	[PrimaryConstructorParameter] bool isGrouped,
	[PrimaryConstructorParameter] bool isSiamese = false
) : WingStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => IsNice ? 50 : 52;

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
}
