namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 引用消息
/// </summary>
public class MessageReference
{
	/// <summary>
	/// 需要引用回复的消息 id
	/// </summary>
	[JsonPropertyName("message_id")]
	public string? MessageId { get; set; }
	/// <summary>
	/// 是否忽略获取引用消息详情错误，默认否
	/// </summary>
	[JsonPropertyName("ignore_get_message_error")]
	public bool IgnoreGetMessageError { get; set; } = false;
}
