namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave XZ</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask">Indicates the mask of digits used.</param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="xzCell">Indicates the extra cell used. This cell is a bivalue cell that only contains digit X and Z.</param>
public sealed partial class BivalueUniversalGraveXzStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryCosntructorParameter] Mask digitsMask,
	[PrimaryCosntructorParameter] scoped ref readonly CellMap cells,
	[PrimaryCosntructorParameter] Cell xzCell
) : BivalueUniversalGraveStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveXzRule;

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors => [new(ExtraDifficultyFactorNames.ExtraDigit, .2M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitStr, CellsStr, ExtraCellStr]), new(ChineseLanguage, [DigitStr, CellsStr, ExtraCellStr])];

	private string DigitStr => Options.Converter.DigitConverter(DigitsMask);

	private string CellsStr => Options.Converter.CellConverter(Cells);

	private string ExtraCellStr => Options.Converter.CellConverter([XzCell]);
}
