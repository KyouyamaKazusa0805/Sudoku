namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 消息对象
/// </summary>
public class Message
{
	/// <summary>
	/// 消息id
	/// </summary>
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;

	/// <summary>
	/// 子频道 id
	/// </summary>
	[JsonPropertyName("channel_id")]
	public string ChannelId { get; set; } = string.Empty;

	/// <summary>
	/// 频道 id
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string GuildId { get; set; } = string.Empty;

	/// <summary>
	/// 消息内容
	/// </summary>
	[JsonPropertyName("content")]
	public string Content { get; set; } = string.Empty;

	/// <summary>
	/// 是否 私聊消息
	/// </summary>
	[JsonPropertyName("direct_message")]
	public bool DirectMessage { get; set; }

	/// <summary>
	/// 是否 @全员消息
	/// </summary>
	[JsonPropertyName("mention_everyone")]
	public bool MentionEveryone { get; set; }

	/// <summary>
	/// 消息创建时间
	/// </summary>
	[JsonPropertyName("timestamp"), JsonConverter(typeof(DateTimeTimestampConverter))]
	public DateTime CreateTime { get; set; }

	/// <summary>
	/// 消息编辑时间
	/// </summary>
	[JsonPropertyName("edited_timestamp"), JsonConverter(typeof(DateTimeTimestampConverter))]
	public DateTime EditedTime { get; set; }

	/// <summary>
	/// 消息创建者
	/// </summary>
	[JsonPropertyName("author")]
	public User Author { get; set; } = new();

	/// <summary>
	/// 附件(可多个)
	/// </summary>
	[JsonPropertyName("attachments")]
	public List<MessageAttachment>? Attachments { get; set; }

	/// <summary>
	/// embed
	/// </summary>
	[JsonPropertyName("embeds")]
	public List<MessageEmbed>? Embeds { get; set; }

	/// <summary>
	/// 消息中@的人
	/// </summary>
	[JsonPropertyName("mentions")]
	public List<User>? Mentions { get; set; }

	/// <summary>
	/// 消息创建者的member信息
	/// </summary>
	[JsonPropertyName("member")]
	public Member Member { get; set; } = new();

	/// <summary>
	/// ark消息
	/// </summary>
	[JsonPropertyName("ark")]
	public MessageArk? Ark { get; set; }
}
