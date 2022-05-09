namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the key-value pair that displays for a ARK message extra info.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/model.html#messagearkobjkv">this link</see>.
/// </remarks>
public sealed class MessageArkObjKeyValuePair
{
	/// <summary>
	/// Indicates the key.
	/// </summary>
	[JsonPropertyName("key")]
	public string Key { get; set; } = "";

	/// <summary>
	/// Indicates the value.
	/// </summary>
	[JsonPropertyName("value")]
	public string? Value { get; set; }
}
