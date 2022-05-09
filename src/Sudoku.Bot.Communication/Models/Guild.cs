namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the GUILD instance.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/guild/model.html">this link</see>.
/// </remarks>
public sealed class Guild
{
	/// <summary>
	/// Indicates the GUILD ID value.
	/// </summary>
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the name of the GUILD.
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; set; }

	/// <summary>
	/// Indicates the URL link that corresponds to the avatar of the current GUILD.
	/// </summary>
	[JsonPropertyName("icon")]
	public string? Icon { get; set; }

	/// <summary>
	/// Indicates the ID value that corresponds to the owner of the GUILD.
	/// </summary>
	[JsonPropertyName("owner_id")]
	public string? OwnerId { get; set; }

	/// <summary>
	/// Indicates whether the current role is the owner.
	/// </summary>
	[JsonPropertyName("owner")]
	public bool Owner { get; set; }

	/// <summary>
	/// Indicates the number of members joined in this GUILD.
	/// </summary>
	[JsonPropertyName("member_count")]
	public int MemberCount { get; set; }

	/// <summary>
	/// Indicates the maximum number of members can be joined in this GUILD.
	/// </summary>
	[JsonPropertyName("max_members")]
	public int MaxMembers { get; set; }

	/// <summary>
	/// Indicates the description of the GUILD.
	/// </summary>
	[JsonPropertyName("description")]
	public string? Description { get; set; }

	/// <summary>
	/// Indicates the date time describes when the GUILD is joined.
	/// </summary>
	[JsonPropertyName("joined_at"), JsonConverter(typeof(DateTimeTimestampConverter))]
	public DateTime JoinedAt { get; set; }

	/// <summary>
	/// Indicates the permissions that the bot can use in this GUILD.
	/// </summary>
	[JsonIgnore]
	public List<ApiPermission>? APIPermissions { get; set; }
}
