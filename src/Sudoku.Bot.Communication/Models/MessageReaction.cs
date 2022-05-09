namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the reaction to a message.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/reaction/model.html#messagereaction">this link</see>.
/// </remarks>
public sealed class MessageReaction
{
	/// <summary>
	/// Indicates the ID of the user who created the reaction.
	/// </summary>
	[JsonPropertyName("user_id")]
	public string UserId { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the GUILD ID.
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string GuildId { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the channel ID.
	/// </summary>
	[JsonPropertyName("channel_id")]
	public string? ChannelId { get; set; }

	/// <summary>
	/// Indicates the reaction instance.
	/// </summary>
	[JsonPropertyName("target")]
	public ReactionTarget? Target { get; set; }

	/// <summary>
	/// Indicates the emoji that is used by a reaction instance.
	/// </summary>
	[JsonPropertyName("emoji")]
	public Emoji? Emoji { get; set; }
}
