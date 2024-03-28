namespace Sudoku.Generating;

/// <summary>
/// Provides successful for generating full puzzle.
/// </summary>
public sealed class FullPuzzleSuccessful : FullPuzzle
{
	/// <summary>
	/// Initializes a <see cref="FullPuzzleSuccessful"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FullPuzzleSuccessful(scoped ref readonly Grid puzzle) => (Puzzle, Result) = (puzzle, GeneratingFailedReason.None);
}
