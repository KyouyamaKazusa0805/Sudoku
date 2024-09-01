namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle Regular Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="code"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="branches">Indicates the branches used.</param>
/// <param name="petals">Indicates the petals used.</param>
/// <param name="extraDigitsMask">Indicates the mask that contains all extra digits.</param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleRegularWingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Technique code,
	Digit digit1,
	Digit digit2,
	ref readonly CellMap cells,
	bool isAvoidable,
	[PrimaryConstructorParameter] ref readonly CellMap branches,
	[PrimaryConstructorParameter] ref readonly CellMap petals,
	[PrimaryConstructorParameter] Mask extraDigitsMask,
	int absoluteOffset
) : UniqueRectangleStep(conclusions, views, options, code, digit1, digit2, in cells, isAvoidable, absoluteOffset)
{
	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [D1Str, D2Str, CellsStr, BranchesStr, SubsetDigitsStr]),
			new(SR.ChineseLanguage, [D1Str, D2Str, CellsStr, BranchesStr, SubsetDigitsStr])
		];

	/// <inheritdoc/>
	public override FactorArray Factors
		=> [new RectangleIsAvoidableFactor(), new UniqueRectangleWingSizeFactor()];

	private string BranchesStr => Options.Converter.CellConverter(Branches);

	private string SubsetDigitsStr => Options.Converter.DigitConverter(ExtraDigitsMask);
}
