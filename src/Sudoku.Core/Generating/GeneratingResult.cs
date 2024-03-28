namespace Sudoku.Generating;

/// <summary>
/// Represents a result that describes why the generation failed/successful.
/// </summary>
public enum GeneratingResult
{
	/// <summary>
	/// Indicates the generation is success without any error.
	/// </summary>
	Success = 0,

	/// <summary>
	/// Indicates the generation not supported.
	/// </summary>
	NotSupported,

	/// <summary>
	/// Indicates a user canceled the task.
	/// </summary>
	Canceled,

	/// <summary>
	/// Indicates the generation operation is unncessary; nearly all puzzles contain such feature.
	/// </summary>
	Unnecessary
}
