namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Embed行内容
/// </summary>
public class MessageEmbedField
{
	/// <summary>
	/// 构造函数
	/// </summary>
	/// <param name="name"></param>
	public MessageEmbedField(string? name = null) { Name = name; }
	/// <summary>
	/// 字段名
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; set; }
}
