namespace Sudoku.Runtime;

/// <summary>
/// 一个上下文类型，记录了用户回答题目的结果信息。
/// </summary>
internal sealed class UserAnswerContext
{
	/// <summary>
	/// 表示本局游戏已经有哪些人回答了题目。
	/// </summary>
	public ConcurrentDictionary<string, Digit> AnsweredUsers { get; set; } = new();

	/// <summary>
	/// 表示在判别结果的循环过程的单位量（250 毫秒为单位的时间片）下，用户回答的信息列表。
	/// </summary>
	public ConcurrentBag<UserAnswer> CurrentTimesliceAnswers { get; set; } = [];

	/// <summary>
	/// 表示是否游戏被玩家取消。
	/// </summary>
	public bool IsCancelled { get; set; }
}
