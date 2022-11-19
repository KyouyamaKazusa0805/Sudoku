namespace Sudoku.Communication.Qicq.Models;

/// <summary>
/// The answering context.
/// </summary>
/// <param name="AnsweredUsers">The answered users.</param>
/// <param name="CurrentRoundAnsweredValues">The answered raw values in a loop-scoped round.</param>
internal sealed record AnsweringContext(ConcurrentBag<string> AnsweredUsers, ConcurrentBag<UserPuzzleAnswerData> CurrentRoundAnsweredValues);
