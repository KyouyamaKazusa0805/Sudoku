namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 表情表态
/// </summary>
public class MessageReaction
{
	/// <summary>
	/// 用户Id
	/// </summary>
	[JsonPropertyName("user_id")]
	public string UserId { get; set; } = string.Empty;
	/// <summary>
	/// 频道Id
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string GuildId { get; set; } = string.Empty;
	/// <summary>
	/// 子频道Id
	/// </summary>
	[JsonPropertyName("channel_id")]
	public string? ChannelId { get; set; }
	/// <summary>
	/// 表态对象
	/// </summary>
	[JsonPropertyName("target")]
	public ReactionTarget? Target { get; set; }
	/// <summary>
	/// 表态所用表情
	/// </summary>
	[JsonPropertyName("emoji")]
	public Emoji? Emoji { get; set; }
}
