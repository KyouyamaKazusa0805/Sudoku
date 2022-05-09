namespace Sudoku.Bot.Communication.Models.Interaction;

/// <summary>
/// Indicates a message type.
/// </summary>
public enum MessageType
{
	/// <summary>
	/// Indicates the message is public.
	/// </summary>
	Public,

	/// <summary>
	/// Indicates the message is mentioned.
	/// </summary>
	BotMentioned,

	/// <summary>
	/// Indicates the message mentions all members.
	/// </summary>
	MentionAll,

	/// <summary>
	/// Indicates the message is private.
	/// </summary>
	Private
}
