namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the field of the embedded message.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/model.html#messageembedfield">this link</see>.
/// </remarks>
public sealed class MessageEmbedField
{
	/// <summary>
	/// 字段名
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; set; }
}
