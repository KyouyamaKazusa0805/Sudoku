namespace SudokuStudio.Views.Interaction;

/// <summary>
/// Defines a display kind that displays for a step on tooltip.
/// </summary>
[Flags]
public enum StepTooltipDisplayKind : int
{
	/// <summary>
	/// Indicates none of all elements mentioned below will be displayed.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the technique name will be displayed.
	/// </summary>
	TechniqueName = 1,

	/// <summary>
	/// Indicates the difficulty rating.
	/// </summary>
	DifficultyRating = 1 << 1,

	/// <summary>
	/// Indicates the simple description.
	/// </summary>
	SimpleDescription = 1 << 2
}
