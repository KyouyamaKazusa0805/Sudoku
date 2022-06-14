namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the instance that describes the API permission.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/api_permissions/model.html#%E6%8E%A5%E5%8F%A3%E6%9D%83%E9%99%90%E5%AF%B9%E8%B1%A1-apipermission">this link</see>.
/// </remarks>
public sealed class ApiPermission
{
	/// <summary>
	/// Indicates the interface address. For example, <c>/guilds/{guild_id}/members/{user_id}</c>.
	/// </summary>
	[JsonPropertyName("path")]
	public string Path { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the method. For example, <c>GET</c>.
	/// </summary>
	[JsonPropertyName("method")]
	public string Method { get; set; } = "GET";

	/// <summary>
	/// Indicates the name of the API.
	/// </summary>
	[JsonPropertyName("desc")]
	public string Desc { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the permission status. 1 is for succeed.
	/// </summary>
	[JsonPropertyName("auth_status")]
	public int AuthStatus { get; set; } = 0;
}
