namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 频道身份组列表
/// </summary>
public class GuildRoles
{
	/// <summary>
	/// 频道Id
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string? GuildId { get; set; }

	/// <summary>
	/// 身份组
	/// </summary>
	[JsonPropertyName("roles")]
	public List<Role>? Roles { get; set; }
	/// <summary>
	/// 默认分组上限
	/// </summary>
	[JsonPropertyName("role_num_limit")]
	public string? RoleNumLimit { get; set; }
}
