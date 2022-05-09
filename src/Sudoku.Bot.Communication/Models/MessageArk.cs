namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates an ARK message unit encapsulation.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/model.html#messageark">this link</see>.
/// </remarks>
public sealed class MessageArk
{
	/// <summary>
	/// Indicates the templated ID of the ARK message. The templated ID should be requested to be registered firstly.
	/// </summary>
	[JsonPropertyName("template_id")]
	public int TemplateId { get; set; }

	/// <summary>
	/// Indicates the extra info that is displayed as a list of key-value pairs.
	/// </summary>
	[JsonPropertyName("kv")]
	public List<MessageArkKeyValuePair> Kv { get; set; } = new();
}
