namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle Burred Subset</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="absoluteOffset"><inheritdoc/></param>
/// <param name="extraCells">Indicates the extra cells used.</param>
/// <param name="subsetIncludedCorner">Indicates the subset-included corner cell.</param>
/// <param name="extraDigitsMask">Indicates the extra digits used.</param>
public sealed partial class UniqueRectangleBurredSubsetStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap cells,
	int absoluteOffset,
	[PrimaryConstructorParameter] scoped ref readonly CellMap extraCells,
	[PrimaryConstructorParameter] Cell subsetIncludedCorner,
	[PrimaryConstructorParameter] Mask extraDigitsMask
) :
	UniqueRectangleBurredStep(
		conclusions,
		views,
		options,
		Technique.UniqueRectangleBurredSubset,
		digit1,
		digit2,
		in cells,
		false,
		absoluteOffset
	),
	IPatternType3StepTrait<UniqueRectangleBurredSubsetStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty - 1;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [CellsStr, DigitsStr, ExtraCellsStr, ExtraDigitsStr]),
			new(ChineseLanguage, [CellsStr, DigitsStr, ExtraCellsStr, ExtraDigitsStr])
		];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new UniqueRectangleBurredSubsetSizeFactor(Options)];

	/// <inheritdoc/>
	bool IPatternType3StepTrait<UniqueRectangleBurredSubsetStep>.IsHidden => false;

	/// <inheritdoc/>
	int IPatternType3StepTrait<UniqueRectangleBurredSubsetStep>.SubsetSize => PopCount((uint)ExtraDigitsMask);

	/// <inheritdoc/>
	Mask IPatternType3StepTrait<UniqueRectangleBurredSubsetStep>.SubsetDigitsMask => ExtraDigitsMask;

	/// <inheritdoc/>
	CellMap IPatternType3StepTrait<UniqueRectangleBurredSubsetStep>.SubsetCells => ExtraCells;

	private string ExtraCellsStr => Options.Converter.CellConverter(ExtraCells + SubsetIncludedCorner);

	private string ExtraDigitsStr => Options.Converter.DigitConverter(ExtraDigitsMask);
}
