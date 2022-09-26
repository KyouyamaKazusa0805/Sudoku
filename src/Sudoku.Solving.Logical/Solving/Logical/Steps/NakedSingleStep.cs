namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Naked Single</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cell"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
internal sealed record NakedSingleStep(ConclusionList Conclusions, ViewList Views, int Cell, int Digit) :
	SingleStep(Conclusions, Views, Cell, Digit)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 2.3M;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.NakedSingle;

	[ResourceTextFormatter]
	internal string CellStr() => RxCyNotation.ToCellString(Cell);

	[ResourceTextFormatter]
	internal string DigitStr() => (Digit + 1).ToString();
}
