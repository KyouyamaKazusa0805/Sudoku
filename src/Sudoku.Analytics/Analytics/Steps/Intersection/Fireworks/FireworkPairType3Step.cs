namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Pair Type 3</b> technique.
/// </summary>
public sealed class FireworkPairType3Step(Conclusion[] conclusions, View[]? views, scoped in CellMap cells, short digitsMask, int emptyRectangleBlock) :
	FireworkStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .2M;

	/// <summary>
	/// Indicates the block index that empty rectangle forms.
	/// </summary>
	public int EmptyRectangleBlock { get; } = emptyRectangleBlock;

	/// <summary>
	/// Indicates the mask of digits used.
	/// </summary>
	public short DigitsMask { get; } = digitsMask;

	/// <inheritdoc/>
	public override Technique Code => Technique.FireworkPairType3;

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public CellMap Cells { get; } = cells;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { CellsStr, DigitsStr, EmptyRectangleStr } },
			{ "zh", new[] { CellsStr, DigitsStr, EmptyRectangleStr } }
		};

	private string CellsStr => Cells.ToString();

	private string DigitsStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private string EmptyRectangleStr => HouseFormatter.Format(1 << EmptyRectangleBlock);
}
