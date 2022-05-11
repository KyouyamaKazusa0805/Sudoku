namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Provides with the event data for <see cref="MessageAuditedEventHandler"/>.
/// </summary>
/// <seealso cref="MessageAuditedEventHandler"/>
public sealed class MessageAuditedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="MessageAuditedEventArgs"/> instance via the specified result.
	/// </summary>
	/// <param name="messageAuditedResult">The result instance.</param>
	public MessageAuditedEventArgs(MessageAudited messageAuditedResult) => MessageAuditedResult = messageAuditedResult;


	/// <summary>
	/// Indicates the result instance that describes the message audition.
	/// </summary>
	public MessageAudited MessageAuditedResult { get; }
}
