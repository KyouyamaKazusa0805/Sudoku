namespace Sudoku.Windows.Data;

/// <summary>
/// Indicates the difficulty information used for gather the technique information of a puzzle.
/// </summary>
/// <param name="Technique">
/// The technique name. If the value is <see langword="null"/>, the information will be a summary.
/// </param>
/// <param name="Count">The number of the technique used.</param>
/// <param name="Total">The total difficulty.</param>
/// <param name="MinToMax">
/// The difficulty that describes minimum and maximum difficulty of the current technique.
/// </param>
/// <param name="DifficultyLevel">Indicates the difficulty level.</param>
public sealed record class DifficultyInfo(
	string? Technique, int Count, decimal Total, string MinToMax, DifficultyLevel DifficultyLevel
);
