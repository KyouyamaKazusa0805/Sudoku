namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates an emoticon.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/emoji/model.html#emoji">this link</see>.
/// </remarks>
public sealed class Emoji
{
	/// <summary>
	/// Indicates the ID of the emoji. The ID strictly satisfies the table given by the section in
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/emoji/model.html#emoji-%E5%88%97%E8%A1%A8">this link</see>.
	/// </summary>
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	/// <summary>
	/// Indicates the emoji type.
	/// </summary>
	[JsonPropertyName("type")]
	public EmojiType Type { get; set; }
}
