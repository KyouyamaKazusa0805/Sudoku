namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="is2LinesWith2Cells"><inheritdoc/></param>
/// <param name="houses"><inheritdoc/></param>
/// <param name="corner1"><inheritdoc/></param>
/// <param name="corner2"><inheritdoc/></param>
/// <param name="targetDigit">Indicates the target digit.</param>
public sealed partial class QiuDeadlyPatternType2Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	bool is2LinesWith2Cells,
	HouseMask houses,
	Cell? corner1,
	Cell? corner2,
	[PrimaryConstructorParameter] Digit targetDigit
) : QiuDeadlyPatternStep(conclusions, views, options, is2LinesWith2Cells, houses, corner1, corner2)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 1;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(1 << TargetDigit);

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [PatternStr, ExtraDigitStr]), new(SR.ChineseLanguage, [PatternStr, ExtraDigitStr])];

	private string ExtraDigitStr => Options.Converter.DigitConverter((Mask)(1 << TargetDigit));
}
