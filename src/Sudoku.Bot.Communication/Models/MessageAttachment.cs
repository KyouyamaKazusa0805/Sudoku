namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 附件对象
/// </summary>
public class MessageAttachment
{
	/// <summary>
	/// 附件Id
	/// </summary>
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	/// <summary>
	/// 附件类型
	/// </summary>
	[JsonPropertyName("content_type")]
	public string? ContentType { get; set; }

	/// <summary>
	/// 下载地址
	/// </summary>
	[JsonPropertyName("url")]
	public string Url { set; get; } = string.Empty;

	/// <summary>
	/// 文件名
	/// </summary>
	[JsonPropertyName("filename")]
	public string? FileName { get; set; }

	/// <summary>
	/// 附件大小
	/// </summary>
	[JsonPropertyName("size")]
	public long? Size { get; set; }

	/// <summary>
	/// 图片宽度
	/// <para>仅附件为图片时才有</para>
	/// </summary>
	[JsonPropertyName("width")]
	public int? Width { get; set; }

	/// <summary>
	/// 图片高度
	/// <para>仅附件为图片时才有</para>
	/// </summary>
	[JsonPropertyName("height")]
	public int? Height { get; set; }
}
