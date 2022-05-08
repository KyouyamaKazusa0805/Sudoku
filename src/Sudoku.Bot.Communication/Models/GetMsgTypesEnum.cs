namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 拉取消息的操作类型
/// </summary>
public enum GetMsgTypesEnum
{
	/// <summary>
	/// 获取目标id前后的消息
	/// </summary>
	around,
	/// <summary>
	/// 获取目标id之前的消息
	/// </summary>
	before,
	/// <summary>
	/// 获取目标id之后的消息
	/// </summary>
	after,
	/// <summary>
	/// 最新limit的消息
	/// </summary>
	latest
}
