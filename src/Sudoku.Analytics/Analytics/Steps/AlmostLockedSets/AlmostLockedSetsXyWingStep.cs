namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Sets XY-Wing</b> technique.
/// </summary>
public sealed class AlmostLockedSetsXyWingStep(
	Conclusion[] conclusions,
	View[]? views,
	AlmostLockedSet als1,
	AlmostLockedSet als2,
	AlmostLockedSet bridge,
	short xDigitsMask,
	short yDigitsMask,
	short zDigitsMask
) : AlmostLockedSetsStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 6.0M;

	/// <summary>
	/// Indicates the mask that holds the digits for the X value.
	/// </summary>
	public short XDigitsMask { get; } = xDigitsMask;

	/// <summary>
	/// Indicates the mask that holds the digits for the Y value.
	/// </summary>
	public short YDigitsMask { get; } = yDigitsMask;

	/// <summary>
	/// Indicates the mask that holds the digits for the Z value.
	/// </summary>
	public short ZDigitsMask { get; } = zDigitsMask;

	/// <inheritdoc/>
	public override TechniqueGroup Group => TechniqueGroup.AlmostLockedSetsChainingLike;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Technique Code => Technique.AlmostLockedSetsXyWing;

	/// <summary>
	/// Indicates the first ALS used in this pattern.
	/// </summary>
	public AlmostLockedSet FirstAls { get; } = als1;

	/// <summary>
	/// Indicates the second ALS used in this pattern.
	/// </summary>
	public AlmostLockedSet SecondAls { get; } = als2;

	/// <summary>
	/// Indicates the ALS that is as a bridge.
	/// </summary>
	public AlmostLockedSet BridgeAls { get; } = bridge;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { Als1Str, BridgeStr, Als2Str, XStr, YStr, ZStr } },
			{ "zh", new[] { Als1Str, BridgeStr, Als2Str, XStr, YStr, ZStr } }
		};

	private string Als1Str => FirstAls.ToString();

	private string BridgeStr => BridgeAls.ToString();

	private string Als2Str => SecondAls.ToString();

	private string XStr => DigitMaskFormatter.Format(XDigitsMask, FormattingMode.Normal);

	private string YStr => DigitMaskFormatter.Format(YDigitsMask, FormattingMode.Normal);

	private string ZStr => DigitMaskFormatter.Format(ZDigitsMask, FormattingMode.Normal);
}
