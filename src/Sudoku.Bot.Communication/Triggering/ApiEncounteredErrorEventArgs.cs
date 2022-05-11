namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Provides with the event data for <see cref="ApiEncounteredErrorEventHandler"/>.
/// </summary>
/// <seealso cref="ApiEncounteredErrorEventHandler"/>
public sealed class ApiEncounteredErrorEventArgs : EventArgs
{
	/// <summary>
	/// Initializes an <see cref="ApiEncounteredErrorEventArgs"/> instance via the specified sender,
	/// and the specified error information.
	/// </summary>
	/// <param name="sender">The sender who sends the message.</param>
	/// <param name="errorInfo">The error information.</param>
	public ApiEncounteredErrorEventArgs(Sender? sender, ApiErrorInfo errorInfo)
		=> (Sender, ErrorInfo) = (sender, errorInfo);


	/// <summary>
	/// Indicates the sender who sends the message.
	/// </summary>
	public Sender? Sender { get; }

	/// <summary>
	/// Indicates the error information.
	/// </summary>
	public ApiErrorInfo ErrorInfo { get; }
}
