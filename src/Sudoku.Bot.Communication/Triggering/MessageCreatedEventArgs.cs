namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Provides with the event data for <see cref="MessageCreatedEventHandler"/>.
/// </summary>
/// <seealso cref="MessageCreatedEventHandler"/>
public sealed class MessageCreatedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="MessageCreatedEventArgs"/> instance via the specified sender.
	/// </summary>
	/// <param name="sender">The real sender instance.</param>
	public MessageCreatedEventArgs(Sender sender) => Sender = sender;


	/// <summary>
	/// Indicates the real sender.
	/// </summary>
	public Sender Sender { get; }
}
