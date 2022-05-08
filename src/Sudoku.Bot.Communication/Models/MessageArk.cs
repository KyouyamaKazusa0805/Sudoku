namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// ark消息
/// </summary>
public class MessageArk
{
	/// <summary>
	/// ark模板id（需要先申请）
	/// </summary>
	[JsonPropertyName("template_id")]
	public int TemplateId { get; set; }
	/// <summary>
	/// kv值列表
	/// </summary>
	[JsonPropertyName("kv")]
	public List<MessageArkKv> Kv { get; set; } = new();
}
