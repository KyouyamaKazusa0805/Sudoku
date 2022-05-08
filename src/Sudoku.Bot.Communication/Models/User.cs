namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 用户
/// </summary>
public class User
{
	/// <summary>
	/// 用户 id
	/// </summary>
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;

	/// <summary>
	/// 用户名
	/// </summary>
	[JsonPropertyName("username")]
	public string UserName { get; set; } = string.Empty;

	/// <summary>
	/// 用户头像地址
	/// </summary>
	[JsonPropertyName("avatar")]
	public string? Avatar { get; set; }

	/// <summary>
	/// 是否机器人
	/// </summary>
	[JsonPropertyName("bot")]
	public bool Bot { get; set; } = true;

	/// <summary>
	/// 特殊关联应用的 openid，需要特殊申请并配置后才会返回。如需申请，请联系平台运营人员。
	/// </summary>
	[JsonPropertyName("union_openid")]
	public string? UnionOpenid { get; set; }

	/// <summary>
	/// 机器人关联的互联应用的用户信息，与 UnionOpenid 关联的应用是同一个。如需申请，请联系平台运营人员。
	/// </summary>
	[JsonPropertyName("union_user_account")]
	public string? UnionUserAccount { get; set; }

	/// <summary>
	/// @用户 标签
	/// <para>数据内容为：&lt;@!UserId&gt;</para>
	/// </summary>
	[JsonIgnore]
	public string Tag => $"<@!{Id}>";
}
