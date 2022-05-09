namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the channel permissions.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel_permissions/model.html">this link</see>.
/// </remarks>
public sealed class ChannelPermissions
{
	/// <summary>
	/// Indicates the channel ID.
	/// </summary>
	[JsonPropertyName("channel_id")]
	public string ChannelId { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the user ID.
	/// When the current property is available, the property <see cref="RoleId"/> will be unavailable.
	/// </summary>
	/// <seealso cref="RoleId"/>
	[JsonPropertyName("user_id")]
	public string? UserId { get; set; }

	/// <summary>
	/// Indicates the role ID.
	/// When the current property is available, the property <see cref="UserId"/> will be unavailable.
	/// </summary>
	/// <seealso cref="UserId"/>
	[JsonPropertyName("role_id")]
	public string? RoleId { get; set; }

	/// <summary>
	/// Indicates the privacy type that the user is allowed doing in the current channel.
	/// </summary>
	[JsonPropertyName("permissions"), JsonConverter(typeof(PrivacyTypeNumberConverter))]
	public PrivacyType Permissions { get; set; }
}
