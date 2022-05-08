namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the instance that describes an announce.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/announces/model.html">this link</see>.
/// </remarks>
public sealed class Announces
{
	/// <summary>
	/// Indicates the GUILD ID.
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string? GuildId { get; set; }

	/// <summary>
	/// Indicates the channel ID.
	/// </summary>
	[JsonPropertyName("channel_id")]
	public string? ChannelId { get; set; }

	/// <summary>
	/// Indicates the message ID.
	/// </summary>
	[JsonPropertyName("message_id")]
	public string? MessageId { get; set; }

	/// <summary>
	/// Indicates the announce type.
	/// </summary>
	[JsonPropertyName("announces_type")]
	public uint AnnounceType { get; set; }

	/// <summary>
	/// Indicates the recommend channels.
	/// </summary>
	[JsonPropertyName("recommend_channels")]
	public RecommendChannel[]? RecommendChannels { get; set; }
}
