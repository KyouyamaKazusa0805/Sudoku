namespace Sudoku.Analytics.Steps.AlmostLockedSets;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Sets W-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="als1">Indicates the first ALS used.</param>
/// <param name="als2">Indicates the second ALS used.</param>
/// <param name="conjugatePair">Indicates the conjugate pair used as a bridge.</param>
/// <param name="wDigitsMask">Indicates the mask of W digits used.</param>
/// <param name="xDigit">Indicates the digit X.</param>
[DeprecatedSince(3, 4, DescriptionLink = "https://github.com/KyouyamaKazusa0805/Sudoku/issues/735")]
public sealed partial class AlmostLockedSetsWWingStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	[Property(NamingRule = "FirstAls")] AlmostLockedSetPattern als1,
	[Property(NamingRule = "SecondAls")] AlmostLockedSetPattern als2,
	[Property] Conjugate conjugatePair,
	[Property] Mask wDigitsMask,
	[Property] Digit xDigit
) : AlmostLockedSetsStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 62;

	/// <inheritdoc/>
	public override Technique Code => Technique.AlmostLockedSetsWWing;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(FirstAls.DigitsMask | SecondAls.DigitsMask);

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [Als1Str, Als2Str, ConjStr, WStr, XStr]), new(SR.ChineseLanguage, [Als1Str, Als2Str, ConjStr, WStr, XStr])];

	private string Als1Str => FirstAls.ToString(Options.Converter);

	private string Als2Str => SecondAls.ToString(Options.Converter);

	private string ConjStr => Options.Converter.ConjugateConverter([ConjugatePair]);

	private string WStr => Options.Converter.DigitConverter(WDigitsMask);

	private string XStr => Options.Converter.DigitConverter((Mask)(1 << XDigit));
}
