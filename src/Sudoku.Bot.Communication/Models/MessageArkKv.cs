namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// ark的键值对
/// </summary>
public class MessageArkKv
{
	/// <summary>
	/// 键
	/// </summary>
	[JsonPropertyName("key")]
	public string Key { get; set; } = "";
	/// <summary>
	/// 值
	/// </summary>
	[JsonPropertyName("value")]
	public string? Value { get; set; }
	/// <summary>
	/// ark obj类型的列表
	/// </summary>
	[JsonPropertyName("obj")]
	public List<MessageArkObj>? Obj { get; set; }
}
