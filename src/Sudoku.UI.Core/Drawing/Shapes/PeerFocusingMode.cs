namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a set of modes that constraints the displaying for focused cells.
/// </summary>
public enum PeerFocusingMode : byte
{
	/// <summary>
	/// Indicates the focusing mode is to display none.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the focusing mode is to display the focused cell itself.
	/// </summary>
	FocusedCell = 1,

	/// <summary>
	/// Indicates the focusing mode is to display the peer cells.
	/// </summary>
	FocusedCellAndPeerCells = 2
}
