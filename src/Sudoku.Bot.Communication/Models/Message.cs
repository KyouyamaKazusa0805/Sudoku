namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the message instance.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/model.html#message">this link</see>.
/// </remarks>
public sealed class Message
{
	/// <summary>
	/// Indicates the ID value of the message.
	/// </summary>
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the channel ID where this message is from.
	/// </summary>
	[JsonPropertyName("channel_id")]
	public string ChannelId { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the GUILD ID where this message is from.
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string GuildId { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the content of the message.
	/// </summary>
	[JsonPropertyName("content")]
	public string Content { get; set; } = string.Empty;

	/// <summary>
	/// Indicates whether the current message is the private message.
	/// </summary>
	[JsonPropertyName("direct_message")]
	public bool IsDirectMessage { get; set; }

	/// <summary>
	/// Indicates whether the message mentions all members.
	/// </summary>
	[JsonPropertyName("mention_everyone")]
	public bool IsAllMentioned { get; set; }

	/// <summary>
	/// Indicates the time when the message created.
	/// </summary>
	[JsonPropertyName("timestamp"), JsonConverter(typeof(DateTimeTimestampConverter))]
	public DateTime CreatedTime { get; set; }

	/// <summary>
	/// Indicates the time when the message edited.
	/// </summary>
	[JsonPropertyName("edited_timestamp"), JsonConverter(typeof(DateTimeTimestampConverter))]
	public DateTime EditedTime { get; set; }

	/// <summary>
	/// Indicates the member who created the message.
	/// </summary>
	[JsonPropertyName("author")]
	public User MessageCreator { get; set; } = new();

	/// <summary>
	/// Indicates the attachments.
	/// </summary>
	[JsonPropertyName("attachments")]
	public List<MessageAttachment>? Attachments { get; set; }

	/// <summary>
	/// Indicates the embedded items.
	/// </summary>
	[JsonPropertyName("embeds")]
	public List<MessageEmbed>? Embeds { get; set; }

	/// <summary>
	/// Indicates the members the message has mentioned.
	/// </summary>
	[JsonPropertyName("mentions")]
	public List<User>? Mentions { get; set; }

	/// <summary>
	/// Indicates the member info.
	/// </summary>
	[JsonPropertyName("member")]
	public Member Member { get; set; } = new();

	/// <summary>
	/// Indicates the ARK info.
	/// </summary>
	[JsonPropertyName("ark")]
	public MessageArk? Ark { get; set; }
}
