namespace Sudoku.Runtime.GeneratingServices.JustOneCell;

/// <summary>
/// Provides failed message for <see cref="PhasedJustOneCellPuzzle"/>.
/// </summary>
/// <seealso cref="PhasedJustOneCellPuzzle"/>
public sealed class PhasedJustOneCellPuzzleFailed : PhasedJustOneCellPuzzle
{
	/// <summary>
	/// Initializes a <see cref="JustOneCellPuzzleFailed"/> instance.
	/// </summary>
	/// <param name="reason">The failed reason.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PhasedJustOneCellPuzzleFailed(GeneratingFailedReason reason) : base(-1, -1, null, in Grid.Undefined, [], default)
		=> (Puzzle, FailedReason) = (Grid.Undefined, reason);
}
