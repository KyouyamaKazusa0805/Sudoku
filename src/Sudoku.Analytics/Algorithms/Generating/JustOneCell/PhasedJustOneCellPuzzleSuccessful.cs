namespace Sudoku.Runtime.GeneratingServices.JustOneCell;

/// <summary>
/// Provides successful message for <see cref="PhasedJustOneCellPuzzle"/>.
/// </summary>
/// <seealso cref="PhasedJustOneCellPuzzle"/>
public sealed class PhasedJustOneCellPuzzleSuccessful : PhasedJustOneCellPuzzle
{
	/// <summary>
	/// Initializes a <see cref="JustOneCellPuzzleSuccessful"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PhasedJustOneCellPuzzleSuccessful(
		ref readonly Grid puzzle,
		ref readonly Grid baseGrid,
		Cell cell,
		Digit digit,
		Step step,
		ref readonly CellMap interferingCells,
		double interferingRatio
	) : base(cell, digit, step, in baseGrid, in interferingCells, interferingRatio) => (Puzzle, FailedReason) = (puzzle, GeneratingFailedReason.None);
}
