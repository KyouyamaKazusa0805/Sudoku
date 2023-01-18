namespace SudokuStudio.Views.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="SaveFileFailedEventHandler"/>.
/// </summary>
/// <seealso cref="SaveFileFailedEventHandler"/>
public sealed class SaveFileFailedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes an <see cref="SaveFileFailedEventArgs"/> instance via the specified reason.
	/// </summary>
	/// <param name="reason">The failed reason.</param>
	public SaveFileFailedEventArgs(SaveFileFailedReason reason) => Reason = reason;


	/// <summary>
	/// Indicates the reason.
	/// </summary>
	public SaveFileFailedReason Reason { get; }
}
