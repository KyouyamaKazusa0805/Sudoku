namespace Sudoku.Bot;

/// <summary>
/// 表示一种游戏类型。
/// </summary>
public enum GameMode
{
	/// <summary>
	/// 表示占位字段。
	/// </summary>
	None,

	/// <summary>
	/// 找出 9 个标数的单元格里，唯一一个标的数字和单元格最终填数一致的那个项，并返回结果。
	/// </summary>
	[GameModeName("找编号")]
	NineMatch,
}
