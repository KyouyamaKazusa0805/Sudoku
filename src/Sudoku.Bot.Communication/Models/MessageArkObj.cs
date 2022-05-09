namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the ARK message instance.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/model.html#messagearkobj">this link</see>.
/// </remarks>
public sealed class MessageArkObj
{
	/// <summary>
	/// Indicates the list of ARK message key-value pair extra info.
	/// </summary>
	[JsonPropertyName("obj_kv")]
	public List<MessageArkObjKeyValuePair>? ObjKv { get; set; }
}
