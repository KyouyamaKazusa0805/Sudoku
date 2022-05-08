namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 接口权限需求标识对象
/// </summary>
/// <param name="Path">The path of the interface. For example, <c>/guilds/{guild_id}/members/{user_id}</c>.</param>
/// <param name="Method">The method. For example, <c>GET</c>.</param>
public readonly record struct ApiPermissionDemandIdentify(
	[property: JsonPropertyName("path")] string Path,
	[property: JsonPropertyName("method")] string Method);
