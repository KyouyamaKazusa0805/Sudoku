namespace Sudoku.Platforms.QQ.Models;

/// <summary>
/// Defines a user answer data.
/// </summary>
/// <param name="User">The user who gives the conclusion.</param>
/// <param name="Conclusion">The answer conclusion index.</param>
internal sealed record UserPuzzleAnswerData(Member User, int Conclusion);
