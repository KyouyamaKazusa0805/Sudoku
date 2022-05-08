namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the instance that describes the API permission demand.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/api_permissions/model.html#%E6%8E%A5%E5%8F%A3%E6%9D%83%E9%99%90%E9%9C%80%E6%B1%82%E5%AF%B9%E8%B1%A1-apipermissiondemand">this link</see>.
/// </remarks>
public sealed class ApiPermissionDemand
{
	/// <summary>
	/// The GUILD ID that tries to request permssions.
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string? GuildId { get; set; }

	/// <summary>
	/// The channel ID whose containing GUILD tries to request permissions.
	/// </summary>
	[JsonPropertyName("channel_id")]
	public string? ChannelId { get; set; }

	/// <summary>
	/// The title of the demand.
	/// </summary>
	[JsonPropertyName("title")]
	public string Title { get; set; } = string.Empty;

	/// <summary>
	/// The description of the demand.
	/// </summary>
	[JsonPropertyName("desc")]
	public string Desc { get; set; } = string.Empty;

	/// <summary>
	/// The permission identity.
	/// </summary>
	[JsonPropertyName("api_identify")]
	public ApiPermissionDemandIdentify ApiIdentify { get; set; }
}
