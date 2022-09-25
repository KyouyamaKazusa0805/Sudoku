namespace Sudoku.Solving.Logical.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Reverse Unique Rectangle Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="TargetCell">The target cell used.</param>
/// <param name="TargetDigit">The target digit used.</param>
internal sealed record ReverseUniqueRectangleType1Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells,
	short DigitsMask,
	int TargetCell,
	int TargetDigit
) : ReverseUniqueRectangleStep(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override int Type => 1;

	[ResourceTextFormatter]
	internal string TargetCellStr() => RxCyNotation.ToCellString(TargetCell);

	[ResourceTextFormatter]
	internal string TargetDigitStr() => (TargetDigit + 1).ToString();
}
