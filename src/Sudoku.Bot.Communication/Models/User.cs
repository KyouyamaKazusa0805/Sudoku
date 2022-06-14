namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates a user info.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/user/model.html#user">this link</see>.
/// </remarks>
public sealed class User
{
	/// <summary>
	/// Indicates the ID of the user.
	/// </summary>
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the name of the user.
	/// </summary>
	[JsonPropertyName("username")]
	public string UserName { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the URL address that corresponds to the user's avatar.
	/// </summary>
	[JsonPropertyName("avatar")]
	public string? Avatar { get; set; }

	/// <summary>
	/// Indicates whether the current user is a bot.
	/// </summary>
	[JsonPropertyName("bot")]
	public bool IsBot { get; set; } = true;

	/// <summary>
	/// Indicates the open ID value that connects applications.
	/// </summary>
	/// <remarks>
	/// The value is special: you should connect platform operators to get the value;
	/// otherwise, keep the value be <see langword="null"/>.
	/// </remarks>
	[JsonPropertyName("union_openid")]
	public string? UnionOpenId { get; set; }

	/// <summary>
	/// Indicates the account info for the application connected,
	/// which connects the same application as the property <see cref="UnionOpenId"/>.
	/// </summary>
	/// <remarks>
	/// The value is special: you should connect platform operators to get the value;
	/// otherwise, keep the value be <see langword="null"/>.
	/// </remarks>
	[JsonPropertyName("union_user_account")]
	public string? UnionUserAccount { get; set; }

	/// <summary>
	/// Indicates the user tag. The format is <c><![CDATA[<@!Id>]]></c>
	/// </summary>
	/// <seealso cref="Id"/>
	[JsonIgnore]
	public string Tag => $"<@!{Id}>";
}
