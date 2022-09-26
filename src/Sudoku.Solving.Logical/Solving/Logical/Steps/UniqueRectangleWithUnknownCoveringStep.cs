namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Unknown Covering</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="TargetCell">Indicates the target cell.</param>
/// <param name="ExtraDigit">Indicates the extra digit used.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
internal sealed record UniqueRectangleWithUnknownCoveringStep(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit1,
	int Digit2,
	scoped in CellMap Cells,
	int TargetCell,
	int ExtraDigit,
	int AbsoluteOffset
) : UniqueRectangleStep(
	Conclusions,
	Views,
	Technique.UniqueRectangleUnknownCovering,
	Digit1,
	Digit2,
	Cells,
	false,
	AbsoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 4.9M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[ResourceTextFormatter]
	internal string TargetCellStr() => RxCyNotation.ToCellString(TargetCell);

	[ResourceTextFormatter]
	internal string DigitsStr() => DigitMaskFormatter.Format((short)(1 << Digit1 | 1 << Digit2), R["OrKeywordWithSpaces"]!);

	[ResourceTextFormatter]
	internal string ExtraDigitStr() => (ExtraDigit + 1).ToString();
}
