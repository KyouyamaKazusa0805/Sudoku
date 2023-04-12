namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Sets W-Wing</b> technique.
/// </summary>
public sealed class AlmostLockedSetsWWingStep(
	Conclusion[] conclusions,
	View[]? views,
	AlmostLockedSet als1,
	AlmostLockedSet als2,
	Conjugate conjugatePair,
	Mask wDigitsMask,
	int xDigit
) : AlmostLockedSetsStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 6.2M;

	/// <summary>
	/// Indicates the digit X.
	/// </summary>
	public int XDigit { get; } = xDigit;

	/// <summary>
	/// Indicates the mask of W digits used.
	/// </summary>
	public Mask WDigitsMask { get; } = wDigitsMask;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Technique Code => Technique.AlmostLockedSetsWWing;

	/// <summary>
	/// Indicates the conjugate pair used as a bridge.
	/// </summary>
	public Conjugate ConjugatePair { get; } = conjugatePair;

	/// <summary>
	/// Indicates the first ALS used.
	/// </summary>
	public AlmostLockedSet FirstAls { get; } = als1;

	/// <summary>
	/// Indicates the second ALS used.
	/// </summary>
	public AlmostLockedSet SecondAls { get; } = als2;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { Als1Str, Als2Str, ConjStr, WStr, XStr } },
			{ "zh", new[] { Als1Str, Als2Str, ConjStr, WStr, XStr } }
		};

	private string Als1Str => FirstAls.ToString();

	private string Als2Str => SecondAls.ToString();

	private string ConjStr => ConjugatePair.ToString();

	private string WStr => DigitMaskFormatter.Format(WDigitsMask, FormattingMode.Normal);

	private string XStr => (XDigit + 1).ToString();
}
