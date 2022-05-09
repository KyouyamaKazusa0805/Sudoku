namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates a message to be created.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/gosdk/api/message/message_format.html#messagetocreate">this link</see>.
/// </remarks>
public class MessageToCreate
{
	/// <summary>
	/// Indicates the content of the message.
	/// </summary>
	[JsonPropertyName("content")]
	public string? Content { get; set; }

	/// <summary>
	/// Indicates the embedded message instance.
	/// </summary>
	[JsonPropertyName("embed")]
	public MessageEmbed? Embed { get; set; }

	/// <summary>
	/// Indicates the ARK message.
	/// </summary>
	[JsonPropertyName("ark")]
	public MessageArk? Ark { get; set; }

	/// <summary>
	/// Indicates the message referenced.
	/// </summary>
	[JsonPropertyName("message_reference")]
	public MessageReference? Reference { get; set; }

	/// <summary>
	/// Indicates the URL of the image.
	/// </summary>
	[JsonPropertyName("image")]
	public string? Image { get; set; }

	/// <summary>
	/// Indicates the message ID that the message is replied.
	/// </summary>
	[JsonPropertyName("msg_id")]
	public string? Id { get; set; }
}
