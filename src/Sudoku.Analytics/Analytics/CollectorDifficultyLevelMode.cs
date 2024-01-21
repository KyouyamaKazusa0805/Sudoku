namespace Sudoku.Analytics;

/// <summary>
/// Indicates the mode that describe the difficulty level controlling for a <see cref="Collector"/> instance.
/// </summary>
/// <seealso cref="Collector"/>
public enum CollectorDifficultyLevelMode
{
	/// <summary>
	/// Indicates the collecting mode is to collect steps whose difficulty level are same as the state of the current grid.
	/// </summary>
	OnlySame,

	/// <summary>
	/// Indicates the collecting mode is to collect steps whose difficulty level are same as the state of the current grid or one level harder.
	/// </summary>
	OneLevelHarder,

	/// <summary>
	/// Indicates the collecting mode is to collect all possible steps, no matter which level the step belongs to.
	/// </summary>
	All
}
