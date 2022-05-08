namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 私信会话对象（DMS）
/// </summary>
public record struct DirectMessageSource
{
	/// <summary>
	/// 私信会话关联的频道Id
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string GuildId { get; init; }
	/// <summary>
	/// 私信会话关联的子频道Id
	/// </summary>
	[JsonPropertyName("channel_id")]
	public string ChannelId { get; init; }
	/// <summary>
	/// 创建私信会话时间戳
	/// </summary>
	[JsonPropertyName("create_time"), JsonConverter(typeof(DateTimeToStringTimestampConverter))]
	public DateTime CreateTime { get; init; }
}
