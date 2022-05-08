namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 子频道类型
/// </summary>
public enum ChannelType
{
	/// <summary>
	/// 文字子频道
	/// </summary>
	文字 = 0,
	/// <summary>
	/// 保留，不可用
	/// </summary>
	Reserve1 = 1,
	/// <summary>
	/// 语音子频道
	/// </summary>
	语音 = 2,
	/// <summary>
	/// 保留，不可用
	/// </summary>
	Reserve2 = 3,
	/// <summary>
	/// 子频道分组
	/// </summary>
	分组 = 4,
	/// <summary>
	/// 直播子频道
	/// </summary>
	直播 = 10005,
	/// <summary>
	/// 应用子频道
	/// </summary>
	应用 = 10006,
	/// <summary>
	/// 论坛子频道
	/// </summary>
	论坛 = 10007
}
