namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="PuzzleCompletedEventHandler"/>.
/// </summary>
/// <param name="finishedPuzzle">Indicates the finished puzzle.</param>
/// <seealso cref="PuzzleCompletedEventHandler"/>
public sealed partial class PuzzleCompletedEventArgs([Property] ref readonly Grid finishedPuzzle) : EventArgs
{
	/// <summary>
	/// Indicates whether the puzzle is fully fixed.
	/// </summary>
	public bool IsFullyFixed => FinishedPuzzle.ModifiablesCount == 0;
}
