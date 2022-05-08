namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 语音Action
/// </summary>
public class AudioAction
{
	/// <summary>
	/// 频道id
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string? GuildId { get; set; }

	/// <summary>
	/// 子频道id
	/// </summary>
	[JsonPropertyName("channel_id")]
	public string? ChannelId { get; set; }

	/// <summary>
	/// 音频数据的url status为0时传
	/// </summary>
	[JsonPropertyName("audio_url")]
	public string? AudioUrl { get; set; }

	/// <summary>
	/// 状态文本（比如：简单爱-周杰伦），可选，status为0时传，其他操作不传
	/// </summary>
	[JsonPropertyName("text")]
	public string? Text { set; get; }
}
