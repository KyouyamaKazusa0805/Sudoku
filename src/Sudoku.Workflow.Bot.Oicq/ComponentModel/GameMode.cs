namespace Sudoku.Workflow.Bot.Oicq.ComponentModel;

/// <summary>
/// 表示一种 PK 答题模式。
/// </summary>
public enum GameMode
{
	/// <summary>
	/// 表示游戏规则为“九数找相同”。规则：预先给出一道数独题目，并给出 9 处位置，带上 1 到 9 的编号。
	/// 其中只有唯一一个格子，它自身的填数结果，和它当前给出的编号数值是一致的。玩家需要找到并回答该相同的编号；
	/// 极少数时候也会存在一个相同的都没有的情况，这个时候需要玩家回答 0 作为答案。
	/// </summary>
	[Name("九数找相同")]
	FindDifference
}
