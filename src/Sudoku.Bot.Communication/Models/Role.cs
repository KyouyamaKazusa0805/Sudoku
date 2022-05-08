namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 身分组对象
/// </summary>
public class Role
{
	/// <summary>
	/// 身份组ID, 默认值可参考 <see cref="DefaultRoles"/>
	/// </summary>
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;
	/// <summary>
	/// 身分组名称
	/// </summary>
	[JsonPropertyName("name")]
	public string Name { get; set; } = string.Empty;
	/// <summary>
	/// 身份组颜色
	/// </summary>
	[JsonPropertyName("color"), JsonConverter(typeof(ColorToUint32Converter))]
	public Color Color { get; set; }
	/// <summary>
	/// ARGB颜色值的HTML表现形式（如：#FFFFFFFF）
	/// </summary>
	[JsonIgnore]
	public string? ColorHtml { get => $"#{Color.ToArgb():X8}"; }
	/// <summary>
	/// 该身分组是否在成员列表中单独展示
	/// </summary>
	[JsonPropertyName("hoist"), JsonConverter(typeof(BoolToInt32Converter))]
	public bool Hoist { get; set; }
	/// <summary>
	/// 该身分组的人数
	/// </summary>
	[JsonPropertyName("number")]
	public uint Number { get; set; }
	/// <summary>
	/// 成员上限
	/// </summary>
	[JsonPropertyName("member_limit")]
	public uint MemberLimit { get; set; }
}
