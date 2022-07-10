namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Indicates the failed reason when received dropped file.
/// </summary>
public enum FailedReceivedDroppedFileReason
{
	/// <summary>
	/// Indicates the reason is that the file is empty.
	/// </summary>
	FileIsEmpty,

	/// <summary>
	/// Indicates the reason is that the file is too large (over 1KB).
	/// </summary>
	FileIsTooLarge
}
