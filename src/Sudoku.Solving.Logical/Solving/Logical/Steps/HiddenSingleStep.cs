namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Hidden Single</b> or <b>Last Digit</b> (for special cases) technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cell"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
/// <param name="House">Indicates the house used.</param>
/// <param name="EnableAndIsLastDigit">Indicates whether the current step is a <b>Last Digit</b> technique usage.</param>
internal sealed record HiddenSingleStep(ConclusionList Conclusions, ViewList Views, int Cell, int Digit, int House, bool EnableAndIsLastDigit) :
	SingleStep(Conclusions, Views, Cell, Digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => this switch { { EnableAndIsLastDigit: true } => 1.1M, { House: < 9 } => 1.2M, _ => 1.5M };

	/// <inheritdoc/>
	public override Rarity Rarity => EnableAndIsLastDigit || House < 9 ? Rarity.Always : Rarity.Often;

	/// <inheritdoc/>
	public override Technique TechniqueCode
		=> EnableAndIsLastDigit ? Technique.LastDigit : (Technique)((int)Technique.HiddenSingleBlock + (int)House.ToHouseType());

	/// <inheritdoc/>
	public override string? Format => R[EnableAndIsLastDigit ? "TechniqueFormat_LastDigit" : "TechniqueFormat_HiddenSingle"];


	[ResourceTextFormatter]
	internal string CellStr() => RxCyNotation.ToCellString(Cell);

	[ResourceTextFormatter]
	internal string DigitStr() => (Digit + 1).ToString();

	[ResourceTextFormatter]
	internal string HouseStr() => HouseFormatter.Format(1 << House);
}
