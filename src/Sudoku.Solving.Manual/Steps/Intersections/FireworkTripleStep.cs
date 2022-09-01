namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Triple</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">The cells used.</param>
/// <param name="DigitsMask">The digits used.</param>
internal sealed partial record FireworkTripleStep(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells,
	short DigitsMask
) : FireworkStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .1M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.FireworkTriple;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[ResourceTextFormatter]
	private partial string CellsStr() => Cells.ToString();

	[ResourceTextFormatter]
	private partial string DigitsStr() => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);
}
