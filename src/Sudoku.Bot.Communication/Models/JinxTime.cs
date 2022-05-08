namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 禁言时间
/// </summary>
public class JinxTime
{
	/// <summary>
	/// 禁言时间（默认1分钟）
	/// </summary>
	public JinxTime() => JinxSeconds = "60";

	/// <summary>
	/// 禁言指定的时长
	/// </summary>
	/// <param name="seconds">禁言多少秒</param>
	public JinxTime(int seconds) => JinxSeconds = seconds.ToString();

	/// <summary>
	/// 禁言到指定时间
	/// </summary>
	/// <param name="timestamp">解禁时间戳
	/// <para>
	/// 格式："yyyy-MM-dd HH:mm:ss"<br/>
	/// 示例："2077-01-01 08:00:00"
	/// </para></param>
	public JinxTime(string timestamp)
		=> JinxEndTimestamp = new DateTimeOffset(Convert.ToDateTime(timestamp)).ToUnixTimeSeconds().ToString();


	/// <summary>
	/// 禁言到期时间戳，绝对时间戳，单位：秒
	/// </summary>
	[JsonPropertyName("mute_end_timestamp")]
	public string? JinxEndTimestamp { get; set; }

	/// <summary>
	/// 禁言多少秒
	/// </summary>
	[JsonPropertyName("mute_seconds")]
	public string? JinxSeconds { get; set; }
}
