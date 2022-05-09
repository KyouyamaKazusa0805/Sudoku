namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the thumbnail of the embedded message.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/model.html#messageembedthumbnail">this link</see>.
/// </remarks>
public sealed class MessageEmbedThumbnail
{
	/// <summary>
	/// Indicates the URL of the thumbnail.
	/// </summary>
	[JsonPropertyName("url")]
	public string? Url { get; set; }
}
