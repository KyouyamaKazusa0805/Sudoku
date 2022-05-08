namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 接口权限需求标识对象
/// </summary>
public record struct ApiPermissionDemandIdentify
{
	/// <summary>
	/// 接口地址
	/// <para>
	/// 例：/guilds/{guild_id}/members/{user_id}
	/// </para>
	/// </summary>
	[JsonPropertyName("path")]
	public string Path { get; init; }

	/// <summary>
	/// 请求方法，例：GET
	/// </summary>
	[JsonPropertyName("method")]
	public string Method { get; init; }
}
