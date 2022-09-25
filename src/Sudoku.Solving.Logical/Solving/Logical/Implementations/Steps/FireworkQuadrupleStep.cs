namespace Sudoku.Solving.Logical.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Quadruple</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">The cells used.</param>
/// <param name="DigitsMask">The digits used.</param>
internal sealed record FireworkQuadrupleStep(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells,
	short DigitsMask
) : FireworkStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .4M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.FireworkQuadruple;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[ResourceTextFormatter]
	internal string CellsStr() => Cells.ToString();

	[ResourceTextFormatter]
	internal string DigitsStr() => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);
}
