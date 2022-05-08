namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 音频对象
/// </summary>
public class AudioControl
{
	/// <summary>
	/// 构建音频消息
	/// </summary>
	/// <param name="status">播放状态</param>
	/// <param name="audioUrl">音频数据URL</param>
	/// <param name="text">状态文本</param>
	public AudioControl(PlayingStatus status, string? audioUrl = null, string? text = null)
	{
		AudioUrl = audioUrl;
		Status = status;
		Text = text;
	}

	/// <summary>
	/// 音频数据的url status为0时传
	/// </summary>
	[JsonPropertyName("audio_url")]
	public string? AudioUrl { get; set; }

	/// <summary>
	/// 状态文本（比如：简单爱-周杰伦），可选，status为0时传，其他操作不传
	/// </summary>
	[JsonPropertyName("text")]
	public string? Text { get; set; }

	/// <summary>
	/// 播放状态，参考 <see cref="PlayingStatus"/>
	/// </summary>
	[JsonPropertyName("status")]
	public PlayingStatus Status { get; set; }
}
