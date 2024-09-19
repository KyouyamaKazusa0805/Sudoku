namespace Sudoku.Analytics.Primitives;

/// <summary>
/// Represents a list of difficulty levels.
/// </summary>
public static class DifficultyLevels
{
	/// <summary>
	/// Indicates all valid difficulty levels.
	/// </summary>
	public const DifficultyLevel AllValid = DifficultyLevel.Easy | DifficultyLevel.Moderate | DifficultyLevel.Hard
		| DifficultyLevel.Fiendish | DifficultyLevel.Nightmare;

	/// <summary>
	/// Indicates all difficulty levels.
	/// </summary>
	public const DifficultyLevel All = AllValid | DifficultyLevel.LastResort;
}
