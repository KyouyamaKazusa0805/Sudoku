namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Pattern Overlay</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public sealed class PatternOverlayStep(Conclusion[] conclusions, StepSearcherOptions options) :
	LastResortStep(conclusions, null, options)
{
	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public Digit Digit => Conclusions[0].Digit;

	/// <inheritdoc/>
	public override int BaseDifficulty => 85;

	/// <inheritdoc/>
	public override Technique Code => Technique.PatternOverlay;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [DigitStr]), new(SR.ChineseLanguage, [DigitStr])];

	private string DigitStr => Options.Converter.DigitConverter((Mask)(1 << Digit));
}
