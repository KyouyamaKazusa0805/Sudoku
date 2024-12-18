namespace Sudoku.Analytics.Steps.LastResorts;

/// <summary>
/// Provides with a step that is a <b>Template</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="isTemplateDeletion">Indicates the current template step is a template deletion.</param>
public sealed partial class TemplateStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	[Property] bool isTemplateDeletion
) : LastResortStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 90;

	/// <inheritdoc/>
	public override Technique Code => IsTemplateDeletion ? Technique.TemplateDelete : Technique.TemplateSet;

	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public Digit Digit => Conclusions.Span[0].Digit;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(1 << Digit);

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [DigitStr]), new(SR.ChineseLanguage, [DigitStr])];

	private string DigitStr => Options.Converter.DigitConverter((Mask)(1 << Digit));
}
