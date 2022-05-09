namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Defines a key-value pair that stores the extra info being used by <see cref="MessageArk"/> instance.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/model.html#messagearkkv">this link</see>.
/// </remarks>
/// <seealso cref="MessageArk"/>
public sealed class MessageArkKeyValuePair
{
	/// <summary>
	/// Indicates the key of the instance.
	/// </summary>
	[JsonPropertyName("key")]
	public string Key { get; set; } = "";

	/// <summary>
	/// Indicates the value of the instance.
	/// </summary>
	[JsonPropertyName("value")]
	public string? Value { get; set; }

	/// <summary>
	/// Indicates the inner objects used.
	/// </summary>
	[JsonPropertyName("obj")]
	public List<MessageArkObj>? Obj { get; set; }
}
