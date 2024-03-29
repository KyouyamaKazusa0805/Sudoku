namespace Sudoku.Generating;

/// <summary>
/// Represents a just-one-cell puzzle, but contains the base puzzle that has a unique solution.
/// </summary>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="step"><inheritdoc/></param>
/// <param name="baseGrid">Indicates the base grid.</param>
[GetHashCode]
public partial class PhasedJustOneCellPuzzle(
	Cell cell,
	Digit digit,
	Step? step,
	[PrimaryConstructorParameter, HashCodeMember] scoped ref readonly Grid baseGrid
) : JustOneCellPuzzle(cell, digit, step)
{
	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] PuzzleBase? other)
		=> other is PhasedJustOneCellPuzzle comparer
		&& (Result, Puzzle, Cell, Digit, Step, BaseGrid) == (comparer.Result, comparer.Puzzle, comparer.Cell, comparer.Digit, comparer.Step, comparer.BaseGrid);
}
