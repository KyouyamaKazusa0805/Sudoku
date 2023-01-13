namespace SudokuStudio.Views.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="FailedReceivedDroppedFileEventHandler"/>.
/// </summary>
/// <seealso cref="FailedReceivedDroppedFileEventHandler"/>
public sealed class FailedReceivedDroppedFileEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="FailedReceivedDroppedFileEventArgs"/> instance via the specified reason.
	/// </summary>
	/// <param name="reason">The failed reason.</param>
	public FailedReceivedDroppedFileEventArgs(FailedReceivedDroppedFileReason reason) => Reason = reason;


	/// <summary>
	/// Indicates the failed reason.
	/// </summary>
	public FailedReceivedDroppedFileReason Reason { get; }
}
