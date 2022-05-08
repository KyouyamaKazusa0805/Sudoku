namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 修改频道身份组的返回值
/// </summary>
public class ModifyRolesRes
{
	/// <summary>
	/// 身份组ID
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string? GuildId { get; set; }
	/// <summary>
	/// 身份组ID
	/// </summary>
	[JsonPropertyName("role_id")]
	public string? RoleId { get; set; }
	/// <summary>
	/// 新创建的频道身份组对象
	/// </summary>
	[JsonPropertyName("role")]
	public Role? Role { get; set; }
}
