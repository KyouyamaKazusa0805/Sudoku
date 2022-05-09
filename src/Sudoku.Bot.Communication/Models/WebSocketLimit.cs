namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the data in the web socket.
/// </summary>
public sealed class WebSocketLimit
{
	/// <summary>
	/// Indicates the URL link of the connection.
	/// </summary>
	[JsonPropertyName("url")]
	public string? Url { get; set; }

	/// <summary>
	/// Indicates the recommend number of possible shards.
	/// </summary>
	/// <remarks>
	/// The value is referenced from
	/// <see href="https://bot.q.qq.com/wiki/develop/api/gateway/shard.html#%E8%8E%B7%E5%BE%97%E5%90%88%E9%80%82%E7%9A%84%E5%88%86%E7%89%87%E6%95%B0">this link</see>.
	/// </remarks>
	[JsonPropertyName("shards")]
	public int Shards { get; set; } = 1;

	/// <summary>
	/// Indicates the session limit.
	/// </summary>
	[JsonPropertyName("session_start_limit")]
	public SessionStartLimit? SessionLimit { get; set; }
}
