namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 公告对象
/// </summary>
public class Announces
{
	/// <summary>
	/// 频道 id
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string? GuildId { get; set; }

	/// <summary>
	/// 子频道 id
	/// </summary>
	[JsonPropertyName("channel_id")]
	public string? ChannelId { get; set; }

	/// <summary>
	/// 消息 id
	/// </summary>
	[JsonPropertyName("message_id")]
	public string? MessageId { get; set; }
}
