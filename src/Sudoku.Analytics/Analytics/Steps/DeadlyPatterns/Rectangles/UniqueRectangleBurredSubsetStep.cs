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
) : UniqueRectangleBurredStep(
	conclusions,
	views,
	options,
	Technique.UniqueRectangleBurredSubset,
	digit1,
	digit2,
	in cells,
	false,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty - .1M;

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [new(ExtraDifficultyFactorNames.Size, PopCount((uint)ExtraDigitsMask) * .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [CellsStr, DigitsStr, ExtraCellsStr, ExtraDigitsStr]),
			new(ChineseLanguage, [CellsStr, DigitsStr, ExtraCellsStr, ExtraDigitsStr])
		];

	private string ExtraCellsStr => Options.Converter.CellConverter(ExtraCells + SubsetIncludedCorner);

	private string ExtraDigitsStr => Options.Converter.DigitConverter(ExtraDigitsMask);
}
