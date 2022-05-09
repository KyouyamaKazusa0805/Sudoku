namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates a real reaction target instance, which means what target the reaction is applied to.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/reaction/model.html#reactiontarget">this link</see>.
/// </remarks>
public sealed class ReactionTarget
{
	/// <summary>
	/// Indicates the ID of the reaction instance.
	/// </summary>
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	/// <summary>
	/// Indicates the type of the reaction.
	/// </summary>
	[JsonPropertyName("type")]
	public ReactionTargetType Type { get; set; }
}
