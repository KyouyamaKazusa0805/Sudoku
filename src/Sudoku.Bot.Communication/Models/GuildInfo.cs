namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 频道信息
/// </summary>
public class GuildInfo : Guild
{
	/// <summary>
	/// 子频道列表
	/// </summary>
	public HashSet<Channel> Channels { get; set; } = new();

	/// <summary>
	/// 角色列表
	/// </summary>
	public HashSet<Role> Roles { get; set; } = new();

	/// <summary>
	/// 成员列表
	/// </summary>
	public HashSet<Member> Members { get; set; } = new();
}
