namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// ark obj类型
/// </summary>
public class MessageArkObj
{
	/// <summary>
	/// ark objkv列表
	/// </summary>
	[JsonPropertyName("obj_kv")]
	public List<MessageArkObjKv>? ObjKv { get; set; }
}
