namespace Sudoku.Bot.Communication;

/// <summary>
/// Indicates a member info.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/member/model.html#member">this link</see>.
/// </remarks>
public class Member
{
	/// <summary>
	/// Indicates the user info. The information is from QQ profile.
	/// </summary>
	/// <remarks>
	/// The property will be filled if the member-related interfaces are used.
	/// </remarks>
	[JsonPropertyName("user")]
	public User? User { get; set; }

	/// <summary>
	/// Indicates the nickname of the user, in the GUILD.
	/// </summary>
	[JsonPropertyName("nick")]
	public string? Nickname { get; set; }

	/// <summary>
	/// Indicates the roles that the member related.
	/// </summary>
	/// <remarks>
	/// You can visit the type <see cref="DefaultRoles"/> to get all possible roles.
	/// </remarks>
	[JsonPropertyName("roles")]
	public List<string> Roles { get; set; } = new() { "1" };

	/// <summary>
	/// Indicates the time that the user has been joined in this GUILD. The value satisfies the standard
	/// ISO8601 timestamp.
	/// </summary>
	[JsonPropertyName("joined_at"), JsonConverter(typeof(DateTimeTimestampConverter))]
	public DateTime? JoinedAt { get; set; }

	/// <summary>
	/// <i><b>The property is not used. Author doesn't know the usage of the property.</b></i>
	/// </summary>
	[JsonPropertyName("deaf")]
	public bool? IsDeaf { get; set; }

	/// <summary>
	/// Indicates whether the user has been jinxed.
	/// </summary>
	[JsonPropertyName("mute")]
	public bool? IsJinxed { get; set; }

	/// <summary>
	/// <i><b>The property is not used. Author doesn't know the usage of the property.</b></i>
	/// </summary>
	[JsonPropertyName("pending")]
	public bool? Pending { get; set; }
}
