namespace Sudoku.Workflow.Bot.Oicq.ComponentModel;

/// <summary>
/// 一个内部类型，用于解析出一个每日一题的答案，和对应日期的对象。
/// </summary>
/// <param name="Puzzle">表示当天的题目。</param>
/// <param name="Digits">表示每日一题的答案。</param>
/// <param name="Date">表示每日一题发布的时间。</param>
[method: JsonConstructor]
public sealed record DailyPuzzleAnswerData(Grid Puzzle, Digit[] Digits, DateOnly Date);
