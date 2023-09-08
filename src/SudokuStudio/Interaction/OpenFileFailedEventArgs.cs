namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="OpenFileFailedEventHandler"/>.
/// </summary>
/// <seealso cref="OpenFileFailedEventHandler"/>
public sealed class OpenFileFailedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes an <see cref="OpenFileFailedEventArgs"/> instance via the specified reason.
	/// </summary>
	/// <param name="reason">The failed reason.</param>
	public OpenFileFailedEventArgs(OpenFileFailedReason reason) => Reason = reason;


	/// <summary>
	/// Indicates the reason.
	/// </summary>
	public OpenFileFailedReason Reason { get; }
}
