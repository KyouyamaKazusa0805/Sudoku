namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 接口权限需求对象
/// </summary>
public class ApiPermissionDemand
{
	/// <summary>
	/// 申请接口权限的频道 id
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string? GuildId { get; set; }

	/// <summary>
	/// 接口权限需求授权链接发送的子频道 id
	/// </summary>
	[JsonPropertyName("channel_id")]
	public string? ChannelId { get; set; }

	/// <summary>
	/// 接口权限链接中的接口权限描述信息
	/// </summary>
	[JsonPropertyName("title")]
	public string Title { get; set; } = string.Empty;

	/// <summary>
	/// 接口权限链接中的机器人可使用功能的描述信息
	/// </summary>
	[JsonPropertyName("desc")]
	public string Desc { get; set; } = string.Empty;

	/// <summary>
	/// 权限接口唯一标识
	/// </summary>
	[JsonPropertyName("api_identify")]
	public ApiPermissionDemandIdentify ApiIdentify { get; set; }
}
