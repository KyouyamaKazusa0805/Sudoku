namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// WebSocket连接分片信息
/// </summary>
public class WebSocketLimit
{
	/// <summary>
	/// WebSocket 的连接地址
	/// </summary>
	[JsonPropertyName("url")]
	public string? Url { get; set; }

	/// <summary>
	/// 建议的 shard 数
	/// </summary>
	[JsonPropertyName("shards")]
	public int Shards { get; set; } = 1;

	/// <summary>
	/// 创建Session限制信息
	/// </summary>
	[JsonPropertyName("session_start_limit")]
	public SessionStartLimit? SessionLimit { get; set; }
}
