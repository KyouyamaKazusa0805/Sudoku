namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 子频道私密类型
/// </summary>
public enum ChannelPrivateType
{
	/// <summary>
	/// 公开频道
	/// </summary>
	Public,
	/// <summary>
	/// 群主和管理员可见
	/// </summary>
	Admin,
	/// <summary>
	/// 群主和管理员+指定成员可见
	/// </summary>
	Members
}
