namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 缩略图对象
/// </summary>
public class MessageEmbedThumbnail
{
	/// <summary>
	/// 图片地址
	/// </summary>
	[JsonPropertyName("url")]
	public string? Url { get; set; }
}
