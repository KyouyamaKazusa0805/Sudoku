namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the roles of the GUILD.
/// </summary>
public sealed class GuildRoles
{
	/// <summary>
	/// Indicates the GUILD value.
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string? GuildId { get; set; }

	/// <summary>
	/// Indicates the role list.
	/// </summary>
	[JsonPropertyName("roles")]
	public List<Role>? Roles { get; set; }

	/// <summary>
	/// Indicates the maximum value of the possible roles in this GUILD.
	/// </summary>
	[JsonPropertyName("role_num_limit")]
	public string? RoleNumLimit { get; set; }
}
