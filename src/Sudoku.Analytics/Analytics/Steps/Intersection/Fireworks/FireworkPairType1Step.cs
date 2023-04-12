namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Pair Type 1</b> technique.
/// </summary>
public sealed class FireworkPairType1Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap cells,
	Mask digitsMask,
	int extraCell1,
	int extraCell2
) : FireworkStep(conclusions, views)
{
	/// <summary>
	/// Indicates the first extra digit used.
	/// </summary>
	public int ExtraCell1 { get; } = extraCell1;

	/// <summary>
	/// Indicates the second extra digit used.
	/// </summary>
	public int ExtraCell2 { get; } = extraCell2;

	/// <summary>
	/// Indicates the digits used.
	/// </summary>
	public Mask DigitsMask { get; } = digitsMask;

	/// <inheritdoc/>
	public override Technique Code => Technique.FireworkPairType1;

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public CellMap Cells { get; } = cells;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { CellsStr, DigitsStr, ExtraCell1Str, ExtraCell2Str } },
			{ "zh", new[] { CellsStr, DigitsStr, ExtraCell1Str, ExtraCell2Str } }
		};

	private string CellsStr => Cells.ToString();

	private string DigitsStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private string ExtraCell1Str => RxCyNotation.ToCellString(ExtraCell1);

	private string ExtraCell2Str => RxCyNotation.ToCellString(ExtraCell2);
}
