namespace Sudoku.Bot.Communication;

/// <summary>
/// 成员
/// </summary>
public class Member
{
	/// <summary>
	/// 用户基础信息，来自QQ资料，只有成员相关接口中会填充此信息
	/// </summary>
	[JsonPropertyName("user")]
	public User? User { get; set; }

	/// <summary>
	/// 用户在频道内的昵称(默认为空)
	/// </summary>
	[JsonPropertyName("nick")]
	public string? Nick { get; set; }

	/// <summary>
	/// 用户在频道内的身份组ID, 默认值可参考DefaultRoles
	/// </summary>
	[JsonPropertyName("roles")]
	public List<string> Roles { get; set; } = new() { "1" };

	/// <summary>
	/// 用户加入频道的时间 ISO8601 timestamp
	/// </summary>
	[JsonPropertyName("joined_at"), JsonConverter(typeof(DateTimeToStringTimestampConverter))]
	public DateTime? JoinedAt { get; set; }

	/// <summary>
	/// 该字段作用未知，等待官方文档更新
	/// </summary>
	[JsonPropertyName("deaf")]
	public bool? Deaf { get; set; }

	/// <summary>
	/// 该成员是否被禁言
	/// </summary>
	[JsonPropertyName("mute")]
	public bool? Mute { get; set; }

	/// <summary>
	/// 该字段作用未知，等待官方文档更新
	/// </summary>
	[JsonPropertyName("pending")]
	public bool? Pending { get; set; }
}
