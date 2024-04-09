namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Wing</b> technique.
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
public sealed partial class UniqueRectangleWithWingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Technique code,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap cells,
	bool isAvoidable,
	[PrimaryConstructorParameter] scoped ref readonly CellMap branches,
	[PrimaryConstructorParameter] scoped ref readonly CellMap petals,
	[PrimaryConstructorParameter] Mask extraDigitsMask,
	int absoluteOffset
) : UniqueRectangleStep(conclusions, views, options, code, digit1, digit2, in cells, isAvoidable, absoluteOffset)
{
	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, BranchesStr, SubsetDigitsStr]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, BranchesStr, SubsetDigitsStr])
		];

	/// <inheritdoc/>
	public override FactorCollection Factors
		=> [new RectangleIsAvoidableFactor(), new UniqueRectangleWingSizeFactor()];

	private string BranchesStr => Options.Converter.CellConverter(Branches);

	private string SubsetDigitsStr => Options.Converter.DigitConverter(ExtraDigitsMask);
}
