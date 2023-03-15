namespace Sudoku.Compatibility.Hodoku;

/// <summary>
/// Defines an attribute that is applied to a field in technique, indicating difficulty rating value defined by Hodoku.
/// </summary>
/// <param name="difficultyRating">The difficulty rating value.</param>
/// <param name="difficultyLevel">The difficulty level.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class HodokuDifficultyRatingAttribute(int difficultyRating, HodokuDifficultyLevel difficultyLevel) : Attribute
{
	/// <summary>
	/// Indicates the difficulty rating.
	/// </summary>
	public int DifficultyRating { get; } = difficultyRating;

	/// <summary>
	/// Indicates the difficulty level.
	/// </summary>
	public HodokuDifficultyLevel DifficultyLevel { get; } = difficultyLevel;
}
