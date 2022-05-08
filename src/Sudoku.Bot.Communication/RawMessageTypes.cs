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
	/// Indicates the bot has received the message that has been mentioned itself.
	/// </summary>
	public const string MentionMessageCreated = "AT_MESSAGE_CREATE";

	/// <summary>
	/// Indicates the channel has received a normal message.
	/// </summary>
	public const string NormalMessageCreated = "MESSAGE_CREATE";

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
	/// Indicates an audio instance is started playing.
	/// </summary>
	public const string AudioStarted = "AUDIO_START";

	/// <summary>
	/// Indicates an audio instance is finished playing.
	/// </summary>
	public const string AudioFinished = "AUDIO_FINISH";

	/// <summary>
	/// Indicates an audio instance is on mic.
	/// </summary>
	public const string AudioOnMic = "AUDIO_ON_MIC";

	/// <summary>
	/// Indicates an audio instance is off mic.
	/// </summary>
	public const string AudioOffMic = "AUDIO_OFF_MIC";

	/// <summary>
	/// Indicates a connection is resumed.
	/// </summary>
	public const string Resumed = "RESUMED";

	/// <summary>
	/// Indicates a connection is ready for receiving messages.
	/// </summary>
	public const string Ready = "READY";
}
