namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Indicates the event type that is defined by <see href="https://onebot.dev/">Onebot</see> protocol.
/// </summary>
public enum EventType
{
	/// <summary>
	/// Indicates the event type is a message.
	/// </summary>
	MESSAGE,
	/// <summary>
	/// Indicates the event type is a notice.
	/// </summary>
	NOTICE,

	/// <summary>
	/// Indicates the event type is a request.
	/// </summary>
	REQUEST,

	/// <summary>
	/// Indicates the event type is a meta event.
	/// </summary>
	META_EVENT
}
