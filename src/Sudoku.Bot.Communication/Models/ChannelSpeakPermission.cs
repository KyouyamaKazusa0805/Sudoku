namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 子频道发言权限
/// </summary>
public enum ChannelSpeakPermission
{
	/// <summary>
	/// 无效类型
	/// </summary>
	Null,
	/// <summary>
	/// 所有人
	/// </summary>
	Everyone,
	/// <summary>
	/// 群主和管理员+指定成员
	/// </summary>
	Members
}
