namespace Sudoku.Generating;

/// <summary>
/// Represents a just-one-cell puzzle, but contains the base puzzle that has a unique solution.
/// </summary>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="step"><inheritdoc/></param>
/// <param name="baseGrid">Indicates the base grid.</param>
[GetHashCode]
public abstract partial class PhasedJustOneCellPuzzle(
	Cell cell,
	Digit digit,
	Step? step,
	[PrimaryConstructorParameter, HashCodeMember] scoped ref readonly Grid baseGrid
) : JustOneCellPuzzle(cell, digit, step)
{
	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] PuzzleBase? other)
		=> other is PhasedJustOneCellPuzzle comparer
		&& (FailedReason, Puzzle, Cell, Digit, Step, BaseGrid) == (comparer.FailedReason, comparer.Puzzle, comparer.Cell, comparer.Digit, comparer.Step, comparer.BaseGrid);
}
