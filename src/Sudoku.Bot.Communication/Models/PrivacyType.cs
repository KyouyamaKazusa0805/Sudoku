namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 子频道私密权限
/// </summary>
[Flags]
public enum PrivacyType
{
	/// <summary>
	/// 没有任何权限
	/// </summary>
	隐藏 = 0,
	/// <summary>
	/// 可查看子频道	
	/// </summary>
	查看 = 1 << 0,
	/// <summary>
	/// 可管理子频道
	/// </summary>
	管理 = 1 << 1,
	/// <summary>
	/// 可发言子频道
	/// </summary>
	发言 = 1 << 2
}
