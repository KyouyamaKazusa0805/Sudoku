namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the audited message instance.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/model.html#messageaudited">this link</see>.
/// </remarks>
public sealed class MessageAudited
{
	/// <summary>
	/// Indicates the ID of the audit.
	/// </summary>
	[JsonPropertyName("audit_id")]
	public string AuditId { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the message to be audited. The property has value if and only if the audit is passed.
	/// </summary>
	[JsonPropertyName("message_id")]
	public string? MessageId { get; set; }

	/// <summary>
	/// Indicates the GUILD ID.
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string GuildId { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the channel ID.
	/// </summary>
	[JsonPropertyName("channel_id")]
	public string ChannelId { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the time when the audit operated.
	/// </summary>
	[JsonPropertyName("audit_time"), JsonConverter(typeof(DateTimeTimestampConverter))]
	public DateTime AuditedTime { get; set; }

	/// <summary>
	/// Indicates the time when the message created.
	/// </summary>
	[JsonPropertyName("create_time"), JsonConverter(typeof(DateTimeTimestampConverter))]
	public DateTime CreatedTime { get; set; }

	/// <summary>
	/// Indicates whether the message has passed the audit.
	/// </summary>
	[JsonIgnore]
	public bool IsPassed { get; set; } = false;
}
