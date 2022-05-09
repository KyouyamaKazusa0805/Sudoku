namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the embedded message.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/model.html#messageembed">this link</see>
/// and <see href="https://bot.q.qq.com/wiki/develop/gosdk/api/message/message_format.html#embed">this link</see>
/// (<c>Embed</c> type in <see cref="MessageToCreate"/>).
/// </remarks>
public sealed class MessageEmbed
{
	/// <summary>
	/// Indicates the title of the embedded message.
	/// </summary>
	[JsonPropertyName("title")]
	public string? Title { get; set; }

	/// <summary>
	/// Indicates the description of the embedded message.
	/// 描述 (见NodeSDK文档)
	/// </summary>
	[JsonPropertyName("description")]
	public string? Description { get; set; }

	/// <summary>
	/// Indicates the message popped-up.
	/// </summary>
	[JsonPropertyName("prompt")]
	public string? Prompt { get; set; }

	/// <summary>
	/// Indicates the thumbnail of the message.
	/// </summary>
	[JsonPropertyName("thumbnail")]
	public MessageEmbedThumbnail? Thumbnail { get; set; }

	/// <summary>
	/// Indicates the fields of the message.
	/// </summary>
	[JsonPropertyName("fields")]
	public List<MessageEmbedField>? Fields { get; set; }
}
