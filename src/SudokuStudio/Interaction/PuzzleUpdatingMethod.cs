namespace SudokuStudio.Interaction;

/// <summary>
/// Represents a kind that describes what kind of the method that makes a puzzle to be completed.
/// </summary>
public enum PuzzleUpdatingMethod
{
	/// <summary>
	/// Indicates the method is by user updating.
	/// </summary>
	UserUpdating,

	/// <summary>
	/// Indicates the method is by program.
	/// </summary>
	Programmatic
}
