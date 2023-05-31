namespace SudokuStudio.Interaction;

/// <summary>
/// Defines a kind of failed reason that describes why failed to load a file.
/// </summary>
public enum OpenFileFailedReason
{
	/// <summary>
	/// Indicates the failed reason is the program cannot unsnap.
	/// </summary>
	UnsnappingFailed,

	/// <summary>
	/// Indicates the failed reason is the target file is empty.
	/// </summary>
	FileIsEmpty,

	/// <summary>
	/// Indicates the failed reason is the target file is oversized.
	/// </summary>
	FileIsTooLarge,

	/// <summary>
	/// Indicates the failed reason is the file cannot be parsed and converted into a valid sudoku grid.
	/// </summary>
	FileCannotBeParsed
}
