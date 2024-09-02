namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="loop"><inheritdoc/></param>
/// <param name="extraDigit">Indicates the extra digit used.</param>
/// <param name="loopPath"><inheritdoc/></param>
public sealed partial class UniqueLoopType2Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	ref readonly CellMap loop,
	[PrimaryConstructorParameter] Digit extraDigit,
	Cell[] loopPath
) : UniqueLoopStep(conclusions, views, options, digit1, digit2, in loop, loopPath)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 1;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(base.DigitsUsed | (Mask)(1 << ExtraDigit));

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [Digit1Str, Digit2Str, LoopStr, ExtraDigitStr]),
			new(SR.ChineseLanguage, [Digit1Str, Digit2Str, LoopStr, ExtraDigitStr])
		];

	private string ExtraDigitStr => Options.Converter.DigitConverter((Mask)(1 << ExtraDigit));
}
