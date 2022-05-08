namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// embed消息
/// </summary>
public class MessageEmbed
{
	/// <summary>
	/// 标题
	/// </summary>
	[JsonPropertyName("title")]
	public string? Title { get; set; }
	/// <summary>
	/// 描述 (见NodeSDK文档)
	/// </summary>
	[JsonPropertyName("description")]
	public string? Description { get; set; }
	/// <summary>
	/// 消息弹窗内容
	/// </summary>
	[JsonPropertyName("prompt")]
	public string? Prompt { get; set; }
	/// <summary>
	/// 缩略图
	/// </summary>
	[JsonPropertyName("thumbnail")]
	public MessageEmbedThumbnail? Thumbnail { get; set; }
	/// <summary>
	/// 消息列表
	/// </summary>
	[JsonPropertyName("fields")]
	public List<MessageEmbedField>? Fields { get; set; }
}
