namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Empty Rectangle</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="block">Indicates the block that the real empty rectangle pattern lis in.</param>
/// <param name="conjugatePair">Indicates the conjugate pair used.</param>
public sealed partial class EmptyRectangleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit,
	[PrimaryConstructorParameter] House block,
	[PrimaryConstructorParameter] ref readonly Conjugate conjugatePair
) : SingleDigitPatternStep(conclusions, views, options, digit)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 46;

	/// <inheritdoc/>
	public override Technique Code => Technique.EmptyRectangle;

	/// <inheritdoc/>
	public override Interpolation[] Interpolations
		=> [new(EnglishLanguage, [DigitStr, HouseStr, ConjStr]), new(ChineseLanguage, [DigitStr, HouseStr, ConjStr])];

	private string DigitStr => Options.Converter.DigitConverter((Mask)(1 << Digit));

	private string HouseStr => Options.Converter.HouseConverter(1 << Block);

	private string ConjStr => Options.Converter.ConjugateConverter([ConjugatePair]);
}
