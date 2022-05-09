namespace Sudoku.Bot.Communication;

/// <summary>
/// Indicates the member with GUILD ID value.
/// </summary>
public sealed class MemberWithGuildId : Member
{
	/// <summary>
	/// Indicates the GUILD ID value.
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string GuildId { get; set; } = string.Empty;
}
