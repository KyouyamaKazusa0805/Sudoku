namespace Sudoku.Generating;

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
		scoped ref readonly Grid puzzle,
		scoped ref readonly Grid baseGrid,
		Cell cell,
		Digit digit,
		Step step
	) : base(cell, digit, step, in baseGrid) => (Puzzle, Result) = (puzzle, GeneratingFailedReason.None);
}
