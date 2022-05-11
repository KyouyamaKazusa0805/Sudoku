namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Provides with the event data for <see cref="GuildMemberRelatedEventHandler"/>.
/// </summary>
/// <seealso cref="GuildMemberRelatedEventHandler"/>
public sealed class GuildMemberRelatedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="GuildMemberRelatedEventArgs"/> instance via the specified GUILD value
	/// and the event triggered.
	/// </summary>
	/// <param name="memberWithGuildId">The member with a GUILD.</param>
	/// <param name="eventType">The event type.</param>
	public GuildMemberRelatedEventArgs(MemberWithGuildId memberWithGuildId, string eventType)
		=> (MemberWithGuildId, EventType) = (memberWithGuildId, eventType);


	/// <summary>
	/// Indicates a <see cref="string"/> value indicating which event is triggered.
	/// The possible values are <see cref="RawMessageTypes.GuildMemberAdded"/>,
	/// <see cref="RawMessageTypes.GuildMemberUpdated"/> and <see cref="RawMessageTypes.GuildMemberRemoved"/>.
	/// </summary>
	public string EventType { get; }

	/// <summary>
	/// Indicates the member with a GUILD.
	/// </summary>
	public MemberWithGuildId MemberWithGuildId { get; }
}