namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Provides with the event data for <see cref="MessageReactionRelatedEventHandler"/>.
/// </summary>
/// <seealso cref="MessageReactionRelatedEventHandler"/>
public sealed class MessageReactionRelatedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="MessageReactionRelatedEventArgs"/> instance via the specified
	/// message reaction and the specified event type.
	/// </summary>
	/// <param name="messageReaction">The message reaction.</param>
	/// <param name="eventType">The event type.</param>
	public MessageReactionRelatedEventArgs(MessageReaction messageReaction, string eventType)
		=> (MessageReaction, EventType) = (messageReaction, eventType);


	/// <summary>
	/// Indicates the event triggered.
	/// </summary>
	public string EventType { get; }

	/// <summary>
	/// Indicates the message reaction.
	/// </summary>
	public MessageReaction MessageReaction { get; }
}
