namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the limitation data that controls and limits the session connection.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/wss/shard_url_get.html#sessionstartlimit">this link</see>.
/// </remarks>
public sealed class SessionStartLimit
{
	/// <summary>
	/// Indicates the number of sessions that can be created.
	/// </summary>
	[JsonPropertyName("total")]
	public int Total { get; set; }

	/// <summary>
	/// Indicates the last number of sessions that can be created.
	/// </summary>
	[JsonPropertyName("remaining")]
	public int Remaining { get; set; }

	/// <summary>
	/// Indicates the time lasting before resetting the next counting, in milliseconds.
	/// </summary>
	[JsonPropertyName("reset_after")]
	public int ResetAfter { get; set; }

	/// <summary>
	/// Indicates the maximum number of sessions can be created in every 5 seconds.
	/// </summary>
	[JsonPropertyName("max_concurrency")]
	public int MaxConcurrency { get; set; }
}
