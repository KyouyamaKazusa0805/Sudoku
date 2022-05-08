namespace Sudoku.Bot.Communication.Models.Returning;

/// <summary>
/// Indicates the returning value that describes the operation for being created a GUILD.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/post_guild_role.html#%E8%BF%94%E5%9B%9E">this link</see>.
/// </remarks>
public sealed class RoleCreatedResult
{
	/// <summary>
	/// Indicates the role ID.
	/// </summary>
	[JsonPropertyName("role_id")]
	public string? RoleId { get; set; }

	/// <summary>
	/// Indicates the role instance that creates the GUILD.
	/// </summary>
	[JsonPropertyName("role")]
	public Role? Role { get; set; }
}
