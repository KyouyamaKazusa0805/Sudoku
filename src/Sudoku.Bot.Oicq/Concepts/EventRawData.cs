namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Defines the data of the event, as the raw one.
/// </summary>
/// <param name="BotQq">The QQ number of the bot.</param>
/// <param name="EventType">The event type.</param>
/// <param name="Subtype">The subtype of the event.</param>
/// <param name="From">The message from.</param>
/// <param name="FromQq">The QQ number of the message from.</param>
/// <param name="TargetQq">The target QQ number.</param>
/// <param name="Content">The message content.</param>
/// <param name="Index">The index.</param>
/// <param name="MessageId">The message ID.</param>
/// <param name="UdpMessage">The UDP protocol message.</param>
/// <param name="Unix">The Unix message if worth.</param>
/// <param name="P">The P value.</param>
[Obsolete("The type is deprecated. Please use other types instead.")]
public sealed record class EventRawData(
	string BotQq, int EventType, int Subtype, string From, string FromQq, string TargetQq,
	string Content, string Index, string MessageId, string UdpMessage, string Unix, int P);
