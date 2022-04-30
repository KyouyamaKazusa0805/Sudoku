namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Indicates the event type that is defined by <see href="https://onebot.dev/">Onebot</see> protocol.
/// </summary>
public enum MessageEventType
{
	/// <summary>
	/// Indicates the message type is from C2C.
	/// </summary>
	C2C = 1,

	/// <summary>
	/// Indicates the message type is from group.
	/// </summary>
	Group = 2
}
