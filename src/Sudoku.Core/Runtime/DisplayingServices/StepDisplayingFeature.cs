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
}
