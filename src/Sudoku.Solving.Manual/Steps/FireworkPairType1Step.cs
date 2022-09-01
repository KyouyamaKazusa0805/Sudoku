namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Pair Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">The cells used.</param>
/// <param name="DigitsMask">The digits used.</param>
/// <param name="ExtraCell1">The extra cell 1.</param>
/// <param name="ExtraCell2">The extra cell 2.</param>
internal sealed partial record FireworkPairType1Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells,
	short DigitsMask,
	int ExtraCell1,
	int ExtraCell2
) : FireworkStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.FireworkPairType1;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[ResourceTextFormatter]
	private partial string CellsStr() => Cells.ToString();

	[ResourceTextFormatter]
	private partial string DigitsStr() => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	private partial string ExtraCell1Str() => RxCyNotation.ToCellString(ExtraCell1);

	[ResourceTextFormatter]
	private partial string ExtraCell2Str() => RxCyNotation.ToCellString(ExtraCell2);
}
