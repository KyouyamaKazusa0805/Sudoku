namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Defines an emoticon type.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/emoji/model.html#emojitype">this link</see>.
/// </remarks>
public enum EmojiType
{
	/// <summary>
	/// Indicates the emoji type is system-defined.
	/// </summary>
	System = 1,

	/// <summary>
	/// Indicates the type is real emoji.
	/// </summary>
	Emoji = 2
}
