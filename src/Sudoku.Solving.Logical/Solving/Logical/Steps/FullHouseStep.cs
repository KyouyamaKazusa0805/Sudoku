namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Full House</b> technique.
/// </summary>
/// <param name="Cell"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
internal sealed record FullHouseStep(ConclusionList Conclusions, ViewList Views, int Cell, int Digit) : SingleStep(Conclusions, Views, Cell, Digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 1.0M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.FullHouse;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Always;


	[ResourceTextFormatter]
	internal string CellStr() => RxCyNotation.ToCellString(Cell);

	[ResourceTextFormatter]
	internal string DigitStr() => (Digit + 1).ToString();
}
