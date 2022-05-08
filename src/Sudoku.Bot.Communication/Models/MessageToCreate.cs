namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 消息体结构
/// </summary>
public class MessageToCreate
{
	/// <summary>
	/// 消息内容，文本内容，支持内嵌格式
	/// </summary>
	[JsonPropertyName("content")]
	public string? Content { get; set; }
	/// <summary>
	/// embed 消息，一种特殊的 ark
	/// </summary>
	[JsonPropertyName("embed")]
	public MessageEmbed? Embed { get; set; }
	/// <summary>
	/// ark 消息
	/// </summary>
	[JsonPropertyName("ark")]
	public MessageArk? Ark { get; set; }
	/// <summary>
	/// 引用消息（需要传递被引用的消息Id）
	/// </summary>
	[JsonPropertyName("message_reference")]
	public MessageReference? Reference { get; set; }
	/// <summary>
	/// 图片 url 地址
	/// </summary>
	[JsonPropertyName("image")]
	public string? Image { get; set; }
	/// <summary>
	/// 要回复的目标消息Id
	/// <para>带了 id 视为被动回复消息，否则视为主动推送消息</para>
	/// </summary>
	[JsonPropertyName("msg_id")]
	public string? Id { get; set; }
}
