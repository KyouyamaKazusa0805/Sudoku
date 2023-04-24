namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Reverse Bi-value Universal Grave Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="pattern"><inheritdoc/></param>
/// <param name="emptyCells"><inheritdoc/></param>
public sealed class ReverseBivalueUniversalGraveType1Step(
	Conclusion[] conclusions,
	View[]? views,
	Digit digit1,
	Digit digit2,
	scoped in CellMap pattern,
	scoped in CellMap emptyCells
) : ReverseBivalueUniversalGraveStep(conclusions, views, digit1, digit2, pattern, emptyCells)
{
	/// <inheritdoc/>
	public override int Type => 1;
}
