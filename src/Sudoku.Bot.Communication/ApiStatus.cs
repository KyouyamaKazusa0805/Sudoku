namespace Sudoku.Bot.Communication;

/// <summary>
/// API请求状态
/// </summary>
public class ApiStatus
{
	/// <summary>
	/// 代码
	/// </summary>
	[JsonPropertyName("code")]
	public int Code { get; set; }

	/// <summary>
	/// 原因
	/// </summary>
	[JsonPropertyName("message")]
	public string? Message { get; set; }
}
