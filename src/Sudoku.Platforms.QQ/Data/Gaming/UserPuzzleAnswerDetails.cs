namespace Sudoku.Platforms.QQ.Data.Gaming;

/// <summary>
/// Defines a user answer data.
/// </summary>
/// <param name="User">The user who gives the conclusion.</param>
/// <param name="Conclusion">The answer conclusion index.</param>
internal sealed record UserPuzzleAnswerDetails(Member User, int Conclusion);
