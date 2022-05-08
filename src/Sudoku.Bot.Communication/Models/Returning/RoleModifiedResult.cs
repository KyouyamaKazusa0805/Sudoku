namespace Sudoku.Bot.Communication.Models.Returning;

/// <summary>
/// Indicates the result that describes the operation that modified a role.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/patch_guild_role.html#%E8%BF%94%E5%9B%9E">this link</see>.
/// </remarks>
public sealed class RoleModifiedResult
{
	/// <summary>
	/// Indicates the GUILD ID.
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string? GuildId { get; set; }

	/// <summary>
	/// Indicates the role ID.
	/// </summary>
	[JsonPropertyName("role_id")]
	public string? RoleId { get; set; }

	/// <summary>
	/// Indicates the modified role values.
	/// </summary>
	[JsonPropertyName("role")]
	public Role? Role { get; set; }
}
