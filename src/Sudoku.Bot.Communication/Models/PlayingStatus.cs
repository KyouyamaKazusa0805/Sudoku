namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 枚举播放状态
/// </summary>
public enum PlayingStatus
{
	/// <summary>
	/// 开始播放操作
	/// </summary>
	START,

	/// <summary>
	/// 暂停播放操作
	/// </summary>
	PAUSE,

	/// <summary>
	/// 继续播放操作
	/// </summary>
	RESUME,

	/// <summary>
	/// 停止播放操作
	/// </summary>
	STOP
}
