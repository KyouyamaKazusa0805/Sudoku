namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the message attachment instance.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/model.html#messageattachment">this link</see>.
/// </remarks>
public sealed class MessageAttachment
{
	/// <summary>
	/// Indicates the ID of the attachment.
	/// </summary>
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	/// <summary>
	/// Indicates the content type of the attachment.
	/// </summary>
	[JsonPropertyName("content_type")]
	public string? ContentType { get; set; }

	/// <summary>
	/// Indicates the download URL link of the attachment.
	/// </summary>
	[JsonPropertyName("url")]
	public string Url { set; get; } = string.Empty;

	/// <summary>
	/// Indicates the file name of the attachment.
	/// </summary>
	[JsonPropertyName("filename")]
	public string? FileName { get; set; }

	/// <summary>
	/// Indicates the size of the attachment.
	/// </summary>
	[JsonPropertyName("size")]
	public long? Size { get; set; }

	/// <summary>
	/// Indicates the width of the attachment if the file is a picture.
	/// </summary>
	[JsonPropertyName("width")]
	public int? Width { get; set; }

	/// <summary>
	/// Indicates the height of the attachment if the file is a picture.
	/// </summary>
	[JsonPropertyName("height")]
	public int? Height { get; set; }
}
