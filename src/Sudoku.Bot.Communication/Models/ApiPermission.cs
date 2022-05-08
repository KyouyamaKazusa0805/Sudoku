namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 接口权限对象
/// </summary>
public class ApiPermission
{
	/// <summary>
	/// 接口地址
	/// <para>
	/// 例：/guilds/{guild_id}/members/{user_id}
	/// </para>
	/// </summary>
	[JsonPropertyName("path")]
	public string Path { get; set; } = string.Empty;

	/// <summary>
	/// 请求方法，例：GET
	/// </summary>
	[JsonPropertyName("method")]
	public string Method { get; set; } = "GET";

	/// <summary>
	/// API 接口名称，例：获取频道信息
	/// </summary>
	[JsonPropertyName("desc")]
	public string Desc { get; set; } = string.Empty;

	/// <summary>
	/// 授权状态
	/// <para>
	/// 0 - 未授权<br/>
	/// 1 - 已授权
	/// </para>
	/// </summary>
	[JsonPropertyName("auth_status")]
	public int AuthStatus { get; set; } = 0;
}
