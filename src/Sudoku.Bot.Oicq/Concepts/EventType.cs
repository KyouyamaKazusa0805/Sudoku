namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Indicates the event type that is defined by <see href="https://onebot.dev/">Onebot</see> protocol.
/// </summary>
public enum EventType
{
	/// <summary>
	/// Indicates the event type is a message.
	/// </summary>
	Message,

	/// <summary>
	/// Indicates the event type is a notice.
	/// </summary>
	Notice,

	/// <summary>
	/// Indicates the event type is a request.
	/// </summary>
	Request,

	/// <summary>
	/// Indicates the event type is a meta event.
	/// </summary>
	MetaEvent
}
