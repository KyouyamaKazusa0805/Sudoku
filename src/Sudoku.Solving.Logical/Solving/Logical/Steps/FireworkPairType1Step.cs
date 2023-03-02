namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Pair Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">The cells used.</param>
/// <param name="DigitsMask">The digits used.</param>
/// <param name="ExtraCell1">The extra cell 1.</param>
/// <param name="ExtraCell2">The extra cell 2.</param>
internal sealed record FireworkPairType1Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells,
	short DigitsMask,
	int ExtraCell1,
	int ExtraCell2
) : FireworkStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.FireworkPairType1;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

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
