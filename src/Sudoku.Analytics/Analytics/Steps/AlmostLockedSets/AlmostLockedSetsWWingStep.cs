namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Sets W-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="als1">Indicates the first ALS used.</param>
/// <param name="als2">Indicates the second ALS used.</param>
/// <param name="conjugatePair">Indicates the conjugate pair used as a bridge.</param>
/// <param name="wDigitsMask">Indicates the mask of W digits used.</param>
/// <param name="xDigit">Indicates the digit X.</param>
public sealed partial class AlmostLockedSetsWWingStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter(GeneratedMemberName = "FirstAls")] AlmostLockedSet als1,
	[PrimaryConstructorParameter(GeneratedMemberName = "SecondAls")] AlmostLockedSet als2,
	[PrimaryConstructorParameter] Conjugate conjugatePair,
	[PrimaryConstructorParameter] Mask wDigitsMask,
	[PrimaryConstructorParameter] Digit xDigit
) : AlmostLockedSetsStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 6.2M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Technique Code => Technique.AlmostLockedSetsWWing;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ EnglishLanguage, new[] { Als1Str, Als2Str, ConjStr, WStr, XStr } },
			{ ChineseLanguage, new[] { Als1Str, Als2Str, ConjStr, WStr, XStr } }
		};

	private string Als1Str => FirstAls.ToString();

	private string Als2Str => SecondAls.ToString();

	private string ConjStr => ConjugatePair.ToString();

	private string WStr => DigitMaskFormatter.Format(WDigitsMask, FormattingMode.Normal);

	private string XStr => (XDigit + 1).ToString();
}
