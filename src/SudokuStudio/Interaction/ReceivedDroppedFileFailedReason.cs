namespace SudokuStudio.Interaction;

/// <summary>
/// Defines an enumeration type that describes reasons why failed to drag a file.
/// </summary>
public enum ReceivedDroppedFileFailedReason
{
	/// <summary>
	/// Indicates the failed reason is that the file is empty.
	/// </summary>
	FileIsEmpty,

	/// <summary>
	/// Indicates the failed reason is that the file is too large (more than 1MB).
	/// </summary>
	FileIsTooLarge,

	/// <summary>
	/// Indicates the failed reason is that the file cannot be parsed.
	/// </summary>
	FileCannotBeParsed
}
