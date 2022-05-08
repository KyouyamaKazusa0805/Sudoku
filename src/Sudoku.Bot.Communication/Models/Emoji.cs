namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 表情对象
/// </summary>
public class Emoji
{
	/// <summary>
	/// 表情ID
	/// <para>
	/// 系统表情使用数字为ID，emoji使用emoji本身为id，参考 EmojiType 列表
	/// </para>
	/// </summary>
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	/// <summary>
	/// 表情类型
	/// </summary>
	[JsonPropertyName("type")]
	public EmojiType Type { get; set; }
}
