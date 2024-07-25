namespace Sudoku.Runtime.GeneratingServices.JustOneCell;

/// <summary>
/// Provides with successful generating for just-one-cell puzzle.
/// </summary>
public sealed class JustOneCellPuzzleSuccessful : JustOneCellPuzzle
{
	/// <summary>
	/// Initializes a <see cref="JustOneCellPuzzleSuccessful"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public JustOneCellPuzzleSuccessful(ref readonly Grid puzzle, Cell cell, Digit digit, Step step, ref readonly CellMap interferingCells, double interferingRatio) :
		base(cell, digit, step, in interferingCells, interferingRatio)
		=> (Puzzle, FailedReason) = (puzzle, GeneratingFailedReason.None);
}
