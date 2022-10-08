namespace Sudoku.Compatibility.Hodoku;

/// <summary>
/// Defines an attribute that is applied to a field in type <see cref="Technique"/>,
/// indicating difficulty rating value defined by Hodoku.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
internal sealed class HodokuDifficultyRatingAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="HodokuDifficultyRatingAttribute"/> via the specified difficulty rating value
	/// and the difficulty level.
	/// </summary>
	/// <param name="difficultyRating">The difficulty rating value.</param>
	/// <param name="difficultyLevel">The difficulty level.</param>
	public HodokuDifficultyRatingAttribute(int difficultyRating, HodokuDifficultyLevel difficultyLevel)
		=> (DifficultyRating, DifficultyLevel) = (difficultyRating, difficultyLevel);


	/// <summary>
	/// Indicates the difficulty rating.
	/// </summary>
	public int DifficultyRating { get; }

	/// <summary>
	/// Indicates the difficulty level.
	/// </summary>
	public HodokuDifficultyLevel DifficultyLevel { get; }
}
