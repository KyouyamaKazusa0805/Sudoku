namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Template</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="isTemplateDeletion">Indicates the current template step is a template deletion.</param>
public sealed partial class TemplateStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] bool isTemplateDeletion
) : LastResortStep(conclusions, views, options)
{
	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public Digit Digit => Conclusions[0].Digit;

	/// <inheritdoc/>
	public override int BaseDifficulty => 90;

	/// <inheritdoc/>
	public override Technique Code => IsTemplateDeletion ? Technique.TemplateDelete : Technique.TemplateSet;

	/// <inheritdoc/>
	public override Interpolation[] Interpolations => [new(SR.EnglishLanguage, [DigitStr]), new(SR.ChineseLanguage, [DigitStr])];

	private string DigitStr => Options.Converter.DigitConverter((Mask)(1 << Digit));
}
