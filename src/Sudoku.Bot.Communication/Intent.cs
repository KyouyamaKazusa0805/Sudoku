namespace Sudoku.Bot.Communication;

/// <summary>
/// Indicates the intents on events triggering.
/// </summary>
/// <remarks>
/// For more information please visit
/// <see href="https://bot.q.qq.com/wiki/develop/api/gateway/intents.html">this link</see>.
/// You can visit the link given above to get the idea and the usage of the concept "intent".
/// </remarks>
/// <completionlist cref="Intents"/>
[Flags]
public enum Intent
{
	/// <summary>
	/// Indicates the events to be triggered are on GUILD-related ones.
	/// The field contains the following events:
	/// <list type="bullet">
	/// <item><see cref="RawMessageTypes.GuildCreated"/></item>
	/// <item><see cref="RawMessageTypes.GuildUpdated"/></item>
	/// <item><see cref="RawMessageTypes.GuildDeleted"/></item>
	/// <item><see cref="RawMessageTypes.ChannelCreated"/></item>
	/// <item><see cref="RawMessageTypes.ChannelUpdated"/></item>
	/// <item><see cref="RawMessageTypes.ChannelDeleted"/></item>
	/// </list>
	/// </summary>
	GUILDS = 1,

	/// <summary>
	/// Indicates the events to be triggered are on GUILD-member-related ones.
	/// The field contains the following events:
	/// <list type="bullet">
	/// <item><see cref="RawMessageTypes.GuildMemberAdded"/></item>
	/// <item><see cref="RawMessageTypes.GuildMemberUpdated"/></item>
	/// <item><see cref="RawMessageTypes.GuildMemberRemoved"/></item>
	/// </list>
	/// </summary>
	GUILD_MEMBERS = 1 << 1,

	/// <summary>
	/// Indicates the events to be triggered are on message-creation-related ones.
	/// The field is only used for private-domained bots.
	/// The field contains the following events:
	/// <list type="bullet">
	/// <item><see cref="RawMessageTypes.NormalMessageCreated"/></item>
	/// <item><see cref="RawMessageTypes.NormalMessageDeletedOrRecalled"/></item>
	/// </list>
	/// </summary>
	MESSAGE_CREATE = 1 << 9,

	/// <summary>
	/// Indicates the events to be triggered are on message-reaction-related ones.
	/// The field contains the following events:
	/// <list type="bullet">
	/// <item><see cref="RawMessageTypes.MessageReactionAdded"/></item>
	/// <item><see cref="RawMessageTypes.MessageReactionRemoved"/></item>
	/// </list>
	/// </summary>
	GUILD_MESSAGE_REACTIONS = 1 << 10,

	/// <summary>
	/// Indicates the events to be triggered are on direct-message-related ones.
	/// The field contains the following events:
	/// <list type="bullet">
	/// <item><see cref="RawMessageTypes.DirectMessageCreated"/></item>
	/// <item><see cref="RawMessageTypes.DirectMessageDeletedOrRecalled"/></item>
	/// </list>
	/// </summary>
	DIRECT_MESSAGE_CREATE = 1 << 12,

	/// <summary>
	/// Indicates the events to be triggered are on interaction-related ones.
	/// The field contains the following events:
	/// <list type="bullet">
	/// <item><see cref="RawMessageTypes.InteractionCreated"/></item>
	/// </list>
	/// </summary>
	Interaction = 1 << 26,

	/// <summary>
	/// Indicates the events to be triggered are on message-audition-related ones.
	/// The field contains the following events:
	/// <list type="bullet">
	/// <item><see cref="RawMessageTypes.MessageAuditPassed"/></item>
	/// <item><see cref="RawMessageTypes.MessageAuditRejected"/></item>
	/// </list>
	/// </summary>
	MESSAGE_AUDIT = 1 << 27,

	/// <summary>
	/// Indicates the events to be triggered are on forum-related ones.
	/// The field is only used for private-domained bots.
	/// The field contains the following events:
	/// <list type="bullet">
	/// <item><see cref="RawMessageTypes.ThreadCreated"/></item>
	/// <item><see cref="RawMessageTypes.ThreadUpdated"/></item>
	/// <item><see cref="RawMessageTypes.ThreadDeleted"/></item>
	/// <item><see cref="RawMessageTypes.PostCreated"/></item>
	/// <item><see cref="RawMessageTypes.PostDeleted"/></item>
	/// <item><see cref="RawMessageTypes.ReplyCreated"/></item>
	/// <item><see cref="RawMessageTypes.ReplyDeleted"/></item>
	/// <item><see cref="RawMessageTypes.ForumPublishedAuditResult"/></item>
	/// </list>
	/// </summary>
	FORUM_EVENT = 1 << 28,

	/// <summary>
	/// Indicates the events to be triggered are on audio-related ones.
	/// The field contains the following events:
	/// <list type="bullet">
	/// <item><see cref="RawMessageTypes.AudioStarted"/></item>
	/// <item><see cref="RawMessageTypes.AudioFinished"/></item>
	/// <item><see cref="RawMessageTypes.AudioOnMic"/></item>
	/// <item><see cref="RawMessageTypes.AudioOffMic"/></item>
	/// </list>
	/// </summary>
	AUDIO_ACTION = 1 << 29,

	/// <summary>
	/// Indicates the events to be triggered are on mentioning-message-related ones.
	/// <list type="bullet">
	/// <item><see cref="RawMessageTypes.MentionMessageCreated"/></item>
	/// <item><see cref="RawMessageTypes.PublicMessageDeleted"/></item>
	/// </list>
	/// </summary>
	PUBLIC_GUILD_MESSAGES = 1 << 30
}
