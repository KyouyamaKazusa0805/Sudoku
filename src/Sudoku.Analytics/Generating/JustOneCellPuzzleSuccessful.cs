namespace Sudoku.Generating;

/// <summary>
/// Provides with successful generating for just-one-cell puzzle.
/// </summary>
public sealed class JustOneCellPuzzleSuccessful : JustOneCellPuzzle
{
	/// <summary>
	/// Initializes a <see cref="JustOneCellPuzzleSuccessful"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public JustOneCellPuzzleSuccessful(scoped ref readonly Grid puzzle, Cell cell, Digit digit, Step step) : base(cell, digit, step)
		=> (Puzzle, FailedReason) = (puzzle, GeneratingFailedReason.None);
}
