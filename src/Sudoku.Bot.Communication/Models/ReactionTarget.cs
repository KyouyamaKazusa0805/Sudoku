namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 表态对象
/// </summary>
public class ReactionTarget
{
	/// <summary>
	/// 表态对象ID
	/// </summary>
	[JsonPropertyName("id")]
	public string? Id { get; set; }
	/// <summary>
	/// 表态对象类型
	/// </summary>
	[JsonPropertyName("type")]
	public ReactionTargetType Type { get; set; }
}
