namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Provides failed for generating just-one-cell puzzles.
/// </summary>
public sealed class JustOneCellPuzzleFailed : JustOneCellPuzzle
{
	/// <summary>
	/// Initializes a <see cref="JustOneCellPuzzleFailed"/> instance.
	/// </summary>
	/// <param name="reason">The failed reason.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public JustOneCellPuzzleFailed(GeneratingFailedReason reason) : base(-1, -1, null) => (Puzzle, Result) = (Grid.Undefined, reason);
}
