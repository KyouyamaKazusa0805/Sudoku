namespace SudokuStudio.Interaction;

/// <summary>
/// Represents a kind of visibility for candidates in a library puzzle to be displayed.
/// </summary>
public enum LibraryCandidatesVisibility
{
	/// <summary>
	/// Indicates all candidates will be hidden no matter what kind of the state the puzzle is at.
	/// </summary>
	AlwaysHidden,

	/// <summary>
	/// Indicates the candidates will be shown when the state of the puzzle is found a solving step
	/// with <see cref="DifficultyLevel.Hard"/> or greater difficulty.
	/// </summary>
	/// <seealso cref="DifficultyLevel.Hard"/>
	ShownWhenPuzzleIsGreaterThanModerate,

	/// <summary>
	/// Indicates the candidates will be shown when extra eliminated candidates exists.
	/// </summary>
	/// <remarks>
	/// An <b>extra eliminated candidate</b> is a candidate that is removed from a technique,
	/// but it does exist at the current grid state if we don't remove it.
	/// </remarks>
	ShownWhenExtraEliminatedCandidatesFound,

	/// <summary>
	/// Indicates all candidates will be shown no matter what kind of the state the puzzle is at.
	/// </summary>
	AlwaysShown
}
