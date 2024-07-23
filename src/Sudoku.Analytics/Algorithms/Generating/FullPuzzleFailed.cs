namespace Sudoku.Algorithms.Generating;

/// <summary>
/// Provides failed for generating full puzzle.
/// </summary>
public sealed class FullPuzzleFailed : FullPuzzle
{
	/// <summary>
	/// Initializes a <see cref="FullPuzzleFailed"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FullPuzzleFailed(GeneratingFailedReason failedResult) => (Puzzle, FailedReason) = (Grid.Undefined, failedResult);
}
