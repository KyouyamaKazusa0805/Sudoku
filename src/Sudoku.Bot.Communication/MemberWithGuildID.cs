namespace Sudoku.Bot.Communication;

/// <summary>
/// 有频道ID的成员
/// </summary>
public class MemberWithGuildID : Member
{
	/// <summary>
	/// 频道id
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string GuildId { get; set; } = string.Empty;
}
