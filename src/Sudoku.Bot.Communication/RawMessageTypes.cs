namespace Sudoku.Bot.Communication;

/// <summary>
/// Provides with the constants for the usages of the message types.
/// </summary>
internal static class RawMessageTypes
{
	/// <summary>
	/// Indicates the bot has received the private message.
	/// </summary>
	public const string DirectMessageCreated = "DIRECT_MESSAGE_CREATE";

	/// <summary>
	/// Indicates the a direct message has been removed or recalled.
	/// </summary>
	public const string DirectMessageDeletedOrRecalled = "DIRECT_MESSAGE_DELETE";

	/// <summary>
	/// Indicates the bot has received the message that has been mentioned itself.
	/// </summary>
	public const string MentionMessageCreated = "AT_MESSAGE_CREATE";

	/// <summary>
	/// Indicates a message from GUILD has been deleted.
	/// </summary>
	public const string PublicMessageDeleted = "PUBLIC_MESSAGE_DELETE";

	/// <summary>
	/// Indicates the channel has received a normal message.
	/// </summary>
	public const string NormalMessageCreated = "MESSAGE_CREATE";

	/// <summary>
	/// Indicates a message is recalled or deleted.
	/// </summary>
	public const string NormalMessageDeletedOrRecalled = "MESSAGE_DELETE";

	/// <summary>
	/// Indicates a GUILD is created.
	/// </summary>
	public const string GuildCreated = "GUILD_CREATE";

	/// <summary>
	/// Indicates a GUILD is updated.
	/// </summary>
	public const string GuildUpdated = "GUILD_UPDATE";

	/// <summary>
	/// Indicates a GUILD is removed.
	/// </summary>
	public const string GuildDeleted = "GUILD_DELETE";

	/// <summary>
	/// Indicates a channel is created.
	/// </summary>
	public const string ChannelCreated = "CHANNEL_CREATE";

	/// <summary>
	/// Indicates a channel is updated.
	/// </summary>
	public const string ChannelUpdated = "CHANNEL_UPDATE";

	/// <summary>
	/// Indicates a channel is removed.
	/// </summary>
	public const string ChannelDeleted = "CHANNEL_DELETE";

	/// <summary>
	/// Indicates a member is added into a GUILD.
	/// </summary>
	public const string GuildMemberAdded = "GUILD_MEMBER_ADD";

	/// <summary>
	/// Indicates the status of a member in a GUILD is updated.
	/// </summary>
	public const string GuildMemberUpdated = "GUILD_MEMBER_UPDATE";

	/// <summary>
	/// Indicates a member is reomved from a GUILD.
	/// </summary>
	public const string GuildMemberRemoved = "GUILD_MEMBER_REMOVE";

	/// <summary>
	/// Indicates a message reaction is added.
	/// </summary>
	public const string MessageReactionAdded = "MESSAGE_REACTION_ADD";

	/// <summary>
	/// Indicates a message reaction is removed.
	/// </summary>
	public const string MessageReactionRemoved = "MESSAGE_REACTION_REMOVE";

	/// <summary>
	/// Indiactes a message is passed after an audition.
	/// </summary>
	public const string MessageAuditPassed = "MESSAGE_AUDIT_PASS";

	/// <summary>
	/// Indicates a message is rejected from an audition.
	/// </summary>
	public const string MessageAuditRejected = "MESSAGE_AUDIT_REJECT";

	/// <summary>
	/// Indicates an interaction is created.
	/// </summary>
	public const string InteractionCreated = "INTERACTION_CREATE";

	/// <summary>
	/// Indicates an audio instance is started playing.
	/// </summary>
	public const string AudioStarted = "AUDIO_START";

	/// <summary>
	/// Indicates an audio instance is finished playing.
	/// </summary>
	public const string AudioFinished = "AUDIO_FINISH";

	/// <summary>
	/// Indicates an audio instance has detected the microphone has been plugged-in.
	/// </summary>
	public const string AudioOnMic = "AUDIO_ON_MIC";

	/// <summary>
	/// Indicates an audio instance has detected the microphone has been plugged-out.
	/// </summary>
	public const string AudioOffMic = "AUDIO_OFF_MIC";

	public const string ThreadCreated = "FORUM_THREAD_CREATE";

	public const string ThreadUpdated = "FORUM_THREAD_UPDATE";

	public const string ThreadDeleted = "FORUM_THREAD_DELETE";

	public const string PostCreated = "FORUM_POST_CREATE";

	public const string PostDeleted = "FORUM_POST_DELETE";

	public const string ReplyCreated = "FORUM_REPLY_CREATE";

	public const string ReplyDeleted = "FORUM_REPLY_DELETE";

	public const string ForumPublishedAuditResult = "FORUM_PUBLISH_AUDIT_RESULT";

	/// <summary>
	/// Indicates a connection is resumed.
	/// </summary>
	public const string Resumed = "RESUMED";

	/// <summary>
	/// Indicates a connection is ready for receiving messages.
	/// </summary>
	public const string Ready = "READY";
}
