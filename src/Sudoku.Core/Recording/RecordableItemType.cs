namespace Sudoku.Recording;

/// <summary>
/// Indicates what kind of recordable item it is.
/// </summary>
public enum RecordableItemType : byte
{
	/// <summary>
	/// Indicates the recordable item is to set a digit into a cell.
	/// </summary>
	SetDigit,

	/// <summary>
	/// Indicates the recordable item is to delete a digit from a cell.
	/// </summary>
	DeleteDigit
}
