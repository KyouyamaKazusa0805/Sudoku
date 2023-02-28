namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Pair Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">The cells used.</param>
/// <param name="DigitsMask">The digits used.</param>
/// <param name="EmptyRectangleBlock">The empty rectangle block used.</param>
internal sealed record FireworkPairType3Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells,
	short DigitsMask,
	int EmptyRectangleBlock
) : FireworkStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .2M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.FireworkPairType3;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;


	[ResourceTextFormatter]
	internal string CellsStr() => Cells.ToString();

	[ResourceTextFormatter]
	internal string DigitsStr() => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	internal string EmptyRectangleStr() => HouseFormatter.Format(1 << EmptyRectangleBlock);
}
