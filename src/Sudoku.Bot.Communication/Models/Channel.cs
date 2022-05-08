namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Defines a channel instance. Please note that the channel is different with a GUILD instance.
/// A GUILD instance is a global environment for a chatting unit, and we can create multiple different channels
/// in a GUILD.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel/model.html#channel">this link</see>.
/// </remarks>
public sealed class Channel
{
	/// <summary>
	/// Indicates the ID of the channel.
	/// </summary>
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the containing GUILD ID value.
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string GuildId { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the name of the channel.
	/// </summary>
	[JsonPropertyName("name")]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the type of the channel.
	/// </summary>
	[JsonPropertyName("type")]
	public ChannelType Type { get; set; }

	/// <summary>
	/// Indicates the subtype of the channel.
	/// </summary>
	[JsonPropertyName("sub_type")]
	public ChannelSubtype? Subtype { get; set; }

	/// <summary>
	/// Indicates the ordering of the channel.
	/// </summary>
	/// <remarks>
	/// About more descriptions for this property please visit
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel/model.html#%E6%9C%89%E5%85%B3-position-%E7%9A%84%E8%AF%B4%E6%98%8E">this link</see>.
	/// </remarks>
	[JsonPropertyName("position")]
	public int Possition { get; set; }

	/// <summary>
	/// Indicates the parent ID. The value is unavailable when <see cref="Type"/> is <see cref="ChannelType.Grouping"/>.
	/// </summary>
	/// <seealso cref="Type"/>
	/// <seealso cref="ChannelType.Grouping"/>
	[JsonPropertyName("parent_id")]
	public string? ParentId { get; set; }

	/// <summary>
	/// Indicates the ID that corresponds to the owner.
	/// </summary>
	[JsonPropertyName("owner_id")]
	public string? OwerId { get; set; }

	/// <summary>
	/// Indicates the type that introduces the level that the channel can expose to what kinds of roles.
	/// </summary>
	[JsonPropertyName("private_type")]
	public ChannelPrivateType PrivateType { get; set; }

	/// <summary>
	/// Indicates the specified users that can visit the private channels. The field is useful when
	/// the property <see cref="PrivateType"/> is <see cref="ChannelPrivateType.SpecialIdentitiesAndSpecifiedMembers"/>.
	/// </summary>
	/// <seealso cref="ChannelPrivateType.SpecialIdentitiesAndSpecifiedMembers"/>
	[JsonPropertyName("private_user_ids")]
	public List<string>? PrivateUserIds { get; set; }

	/// <summary>
	/// Indicates the permission level on talking.
	/// </summary>
	[JsonPropertyName("speak_permission")]
	public ChannelTalkingPermission TalkingPermission { get; set; }

	/// <summary>
	/// Indicates the value that identities the application channel type.
	/// This property is only available when the property <see cref="Type"/> is <see cref="ChannelType.Application"/>.
	/// </summary>
	/// <remarks>
	/// About more information for this property please visit
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel/model.html#%E5%BA%94%E7%94%A8%E5%AD%90%E9%A2%91%E9%81%93%E7%9A%84%E5%BA%94%E7%94%A8%E7%B1%BB%E5%9E%8B">this link</see>.
	/// </remarks>
	/// <seealso cref="Type"/>
	/// <seealso cref="ChannelType.Application"/>
	[JsonPropertyName("application_id")]
	public string? ApplicationId { get; set; }

	/// <summary>
	/// Indicates the tag of the channel. The property will return the value <c>#channelName tag</c>.
	/// The property returns the string expression like <c><![CDATA[<#ChannelId>]]></c>.
	/// </summary>
	[JsonIgnore]
	public string Tag => $"<#{Id}>";


	/// <summary>
	/// Indicates the list of all possible application channel types.
	/// </summary>
	/// <remarks>
	/// The dictionary is referenced from
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel/model.html#%E5%BA%94%E7%94%A8%E5%AD%90%E9%A2%91%E9%81%93%E7%9A%84%E5%BA%94%E7%94%A8%E7%B1%BB%E5%9E%8B">this link</see>.
	/// </remarks>
	public static Dictionary<string, string> AppType => new()
	{
		{ "1000000", "王者开黑大厅" },
		{ "1000001", "互动小游戏" },
		{ "1000010", "腾讯投票" },
		{ "1000051", "飞车开黑大厅" },
		{ "1000050", "日程提醒" },
		{ "1000070", "CoDM开黑大厅" },
		{ "1010000", "和平精英开黑大厅" },
	};
}
