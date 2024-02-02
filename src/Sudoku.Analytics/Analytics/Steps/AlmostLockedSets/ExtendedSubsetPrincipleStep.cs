namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Subset Principle</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="digitsMask">Indicates the digits used.</param>
/// <param name="extraDigit">Indicates the extra digit used.</param>
public sealed partial class ExtendedSubsetPrincipleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] scoped ref readonly CellMap cells,
	[PrimaryConstructorParameter] Mask digitsMask,
	[PrimaryConstructorParameter] Digit extraDigit
) : AlmostLockedSetsStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.5M;

	/// <inheritdoc/>
	public override Technique Code => Technique.ExtendedSubsetPrinciple;

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [new(ExtraDifficultyFactorNames.WingSize, Cells.Count switch { 3 or 4 => 0, 5 or 6 or 7 => .2M, 8 or 9 => .4M })];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [EspDigitStr, CellsStr]), new(ChineseLanguage, [EspDigitStr, CellsStr])];

	private string CellsStr => Options.Converter.CellConverter(Cells);

	private string EspDigitStr => Options.Converter.DigitConverter((Mask)(1 << ExtraDigit));
}
