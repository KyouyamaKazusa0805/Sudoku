namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the reference message instance.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/gosdk/api/message/post_message.html#messagereference">this link</see>
/// (Original link) and
/// <see href="https://bot.q.qq.com/wiki/develop/gosdk/api/message/message_reference.html#%E5%8F%91%E9%80%81%E5%BC%95%E7%94%A8%E6%B6%88%E6%81%AF">this link</see>
/// (Example that use the message reference).
/// </remarks>
public sealed class MessageReference
{
	/// <summary>
	/// Indicates the message ID referenced.
	/// </summary>
	[JsonPropertyName("message_id")]
	public string? MessageId { get; set; }

	/// <summary>
	/// Indicates whether the operation ignores the wrong case that fetched the message reference.
	/// </summary>
	[JsonPropertyName("ignore_get_message_error")]
	public bool IgnoreGetMessageError { get; set; } = false;
}
