namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 消息审核对象
/// </summary>
public class MessageAudited
{
	/// <summary>
	/// 消息审核Id
	/// </summary>
	[JsonPropertyName("audit_id")]
	public string AuditId { get; set; } = string.Empty;
	/// <summary>
	/// 被审核的消息Id
	/// <para>只有审核通过事件才会有值</para>
	/// </summary>
	[JsonPropertyName("message_id")]
	public string? MessageId { get; set; }
	/// <summary>
	/// 频道Id
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string GuildId { get; set; } = string.Empty;
	/// <summary>
	/// 子频道Id
	/// </summary>
	[JsonPropertyName("channel_id")]
	public string ChannelId { get; set; } = string.Empty;
	/// <summary>
	/// 消息审核时间
	/// </summary>
	[JsonPropertyName("audit_time"), JsonConverter(typeof(DateTimeToStringTimestamp))]
	public DateTime AuditTime { get; set; }
	/// <summary>
	/// 消息创建时间
	/// </summary>
	[JsonPropertyName("create_time"), JsonConverter(typeof(DateTimeToStringTimestamp))]
	public DateTime CreateTime { get; set; }
	/// <summary>
	/// 扩展属性，用于标注审核是否通过
	/// </summary>
	[JsonIgnore]
	public bool IsPassed { get; set; } = false;
}
