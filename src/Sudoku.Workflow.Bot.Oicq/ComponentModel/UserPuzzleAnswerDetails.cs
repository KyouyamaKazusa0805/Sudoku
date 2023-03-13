namespace Sudoku.Workflow.Bot.Oicq.ComponentModel;

/// <summary>
/// 表示一个用户回答题目的数据。
/// </summary>
/// <param name="User">表示是谁回答的题目。</param>
/// <param name="Conclusion">表示用户回答的结果。</param>
internal sealed record UserPuzzleAnswerDetails(Member User, int Conclusion);
