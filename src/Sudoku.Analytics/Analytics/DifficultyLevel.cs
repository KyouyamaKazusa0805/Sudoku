namespace Sudoku.Analytics;

/// <summary>
/// Provides a difficulty kind for a puzzle.
/// </summary>
[Flags]
public enum DifficultyLevel
{
	/// <summary>
	/// Indicates the difficulty level is unknown.
	/// </summary>
	Unknown = 0,

	/// <summary>
	/// Indicates the difficulty is easy.
	/// </summary>
	Easy = 1 << 0,

	/// <summary>
	/// Indicates the difficulty is moderate.
	/// </summary>
	Moderate = 1 << 1,

	/// <summary>
	/// Indicates the difficulty is hard.
	/// </summary>
	Hard = 1 << 2,

	/// <summary>
	/// Indicates the difficulty is fiendish.
	/// </summary>
	Fiendish = 1 << 3,

	/// <summary>
	/// Indicates the difficulty is nightmare.
	/// </summary>
	Nightmare = 1 << 4,

	/// <summary>
	/// Indicates the puzzle can't be solved unless using last resort methods.
	/// </summary>
	LastResort = 1 << 5
}
