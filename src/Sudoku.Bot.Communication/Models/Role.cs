namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the instance that describes for a real role.
/// </summary>
public sealed class Role
{
	/// <summary>
	/// Indicates the role ID. The default value can be referenced from <see cref="DefaultRoles"/>.
	/// </summary>
	/// <seealso cref="DefaultRoles"/>
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the role name.
	/// </summary>
	[JsonPropertyName("name")]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the color of the role.
	/// </summary>
	[JsonPropertyName("color"), JsonConverter(typeof(ColorToUint32Converter))]
	public Color Color { get; set; }

	/// <summary>
	/// Indicates whether the role is displayed alone. <see langword="true"/> means
	/// that the role will be displayed alone.
	/// </summary>
	[JsonPropertyName("hoist"), JsonConverter(typeof(BoolToInt32Converter))]
	public bool Hoist { get; set; }

	/// <summary>
	/// Indicates the total number of the members that is in the current role.
	/// </summary>
	[JsonPropertyName("number")]
	public uint Number { get; set; }

	/// <summary>
	/// Indicates the maximum number of members that can be in the current role.
	/// </summary>
	[JsonPropertyName("member_limit")]
	public uint MemberLimit { get; set; }

	/// <summary>
	/// Indicates the HTML-formatted color value. The value is displayed by a string
	/// using 8 hex digits (from 0 to F), leading with a tag sign <c>#</c>.
	/// </summary>
	[JsonIgnore]
	public string? ColorHtml => $"#{Color.ToArgb():X8}";
}
