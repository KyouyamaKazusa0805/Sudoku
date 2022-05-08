namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 消息类型枚举
/// </summary>
public enum MessageType
{
	/// <summary>
	/// 公共
	/// </summary>
	Public,
	/// <summary>
	/// @机器人
	/// </summary>
	AtMe,
	/// <summary>
	/// @全员
	/// </summary>
	AtAll,
	/// <summary>
	/// 私聊
	/// </summary>
	Private
}
