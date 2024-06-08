namespace Sudoku.Generating.JustOneCell;

/// <summary>
/// Represents a just-one-cell puzzle, but contains the base puzzle that has a unique solution.
/// </summary>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="step"><inheritdoc/></param>
/// <param name="baseGrid">Indicates the base grid.</param>
/// <param name="interferingCells"><inheritdoc/></param>
/// <param name="interferingRatio"><inheritdoc/></param>
[TypeImpl(TypeImplFlag.Object_GetHashCode)]
public abstract partial class PhasedJustOneCellPuzzle(
	Cell cell,
	Digit digit,
	Step? step,
	[PrimaryConstructorParameter, HashCodeMember] ref readonly Grid baseGrid,
	ref readonly CellMap interferingCells,
	double interferingRatio
) : JustOneCellPuzzle(cell, digit, step, in interferingCells, interferingRatio)
{
	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] PuzzleBase? other)
		=> other is PhasedJustOneCellPuzzle comparer && base.Equals(other) && BaseGrid == comparer.BaseGrid;
}
