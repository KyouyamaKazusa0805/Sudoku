namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Provides with event arguments for event <see cref="SudokuPane.FailedReceivedDroppedFile"/>.
/// </summary>
/// <seealso cref="SudokuPane.FailedReceivedDroppedFile"/>
public sealed class FailedReceivedDroppedFileEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="FailedReceivedDroppedFileEventArgs"/> instance via the specified failed reason.
	/// </summary>
	/// <param name="reason">The failed reason.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FailedReceivedDroppedFileEventArgs(FailedReceivedDroppedFileReason reason) => Reason = reason;


	/// <summary>
	/// Indicates the failed reason.
	/// </summary>
	public FailedReceivedDroppedFileReason Reason { get; }
}
