namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Pair Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digitsMask">The digits used.</param>
/// <param name="pattern1">The first pattern used.</param>
/// <param name="pattern2">The second pattern used.</param>
/// <param name="extraCell">The extra cell used.</param>
public sealed class FireworkPairType2Step(
	Conclusion[] conclusions,
	View[]? views,
	short digitsMask,
	scoped in Firework pattern1,
	scoped in Firework pattern2,
	int extraCell
) : FireworkStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .3M;

	/// <summary>
	/// Indicates the extra cell used.
	/// </summary>
	public int ExtraCell { get; } = extraCell;

	/// <summary>
	/// Indicates the mask of digits used.
	/// </summary>
	public short DigitsMask { get; } = digitsMask;

	/// <inheritdoc/>
	public override Technique Code => Technique.FireworkPairType2;

	/// <summary>
	/// Indicates the first firework pattern used.
	/// </summary>
	public Firework Pattern1 { get; } = pattern1;

	/// <summary>
	/// Indicates the second firework pattern used.
	/// </summary>
	public Firework Pattern2 { get; } = pattern2;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { Firework1Str, Firework2Str, DigitsStr, ExtraCellStr } },
			{ "zh", new[] { Firework1Str, Firework2Str, DigitsStr, ExtraCellStr } }
		};

	private string ExtraCellStr => RxCyNotation.ToCellString(ExtraCell);

	private string DigitsStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private string Firework1Str => $"{Pattern1.Map}({DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal)})";

	private string Firework2Str => $"{Pattern2.Map}({DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal)})";
}
