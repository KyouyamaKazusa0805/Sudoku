namespace Sudoku.Runtime.DisplayingServices;

/// <summary>
/// Defines one or more step displaying feature, which controls the extra information about target displaying control.
/// </summary>
[Flags]
public enum StepDisplayingFeature : byte
{
	/// <summary>
	/// Indicates the feature is none.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the feature is to hide difficulty rating value.
	/// </summary>
	HideDifficultyRating = 1,

	/// <summary>
	/// Indicates the feature is to display "very rare" information.
	/// </summary>
	VeryRare = 1 << 1,

	/// <summary>
	/// Indicates the difficulty rating is not stable, which means the rating value may be changed
	/// if the pattern is changed.
	/// </summary>
	DifficultyRatingNotStable = 1 << 2,

	/// <summary>
	/// Indicates the current technique is constructed technique usage, which means it uses two or more techniques
	/// to form a single technique usage. For example, UR + XY-Wing.
	/// </summary>
	ConstructedTechnique = 1 << 3
}
