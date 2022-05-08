namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the instance that describes the recommend channel.
/// </summary>
/// <param name="ChannelId">Indicates the channel ID.</param>
/// <param name="Introduce">Indicates the introducing text.</param>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/announces/model.html#RecommendChannel">this link</see>.
/// </remarks>
public record struct RecommendChannel(
	[property: JsonPropertyName("channel_id")] string? ChannelId,
	[property: JsonPropertyName("introduce")] string? Introduce);