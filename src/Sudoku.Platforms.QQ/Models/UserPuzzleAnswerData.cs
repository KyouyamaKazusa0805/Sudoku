namespace Sudoku.Platforms.QQ.Models;

/// <summary>
/// Defines a user answer data.
/// </summary>
/// <param name="User">The user who gives the conclusion.</param>
/// <param name="Conclusions">The answer conclusion digit value (-1 is for unknown value, 1 to 9 is for the target digit filled).</param>
internal sealed record UserPuzzleAnswerData(Member User, int[]? Conclusions);
