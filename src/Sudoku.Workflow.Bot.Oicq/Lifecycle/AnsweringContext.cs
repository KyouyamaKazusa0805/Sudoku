namespace Sudoku.Workflow.Bot.Oicq.Lifecycle;

/// <summary>
/// 一个上下文类型，记录了用户回答题目的结果信息。
/// </summary>
internal sealed class AnsweringContext
{
	/// <summary>
	/// 表示本局游戏已经有哪些人回答了题目。
	/// </summary>
	public ConcurrentDictionary<string, int> AnsweredUsers { get; set; } = new();

	/// <summary>
	/// 表示在判别结果的循环过程的单位量下，用户回答的信息列表。
	/// </summary>
	public ConcurrentBag<UserPuzzleAnswerDetails> CurrentRoundAnsweredValues { get; set; } = new();

	/// <summary>
	/// 表示是否游戏被玩家取消。
	/// </summary>
	public bool IsCancelled { get; set; }
}
