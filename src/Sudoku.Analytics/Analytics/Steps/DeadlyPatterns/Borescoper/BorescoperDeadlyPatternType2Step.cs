namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Borescoper's Deadly Pattern Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="extraDigit">Indicates the extra digit used.</param>
public sealed partial class BorescoperDeadlyPatternType2Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap cells,
	Mask digitsMask,
	[PrimaryConstructorParameter] Digit extraDigit
) : BorescoperDeadlyPatternStep(conclusions, views, cells, digitsMask)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitsStr, CellsStr, ExtraDigitStr]), new(ChineseLanguage, [DigitsStr, CellsStr, ExtraDigitStr])];

	private string ExtraDigitStr => (ExtraDigit + 1).ToString();
}
