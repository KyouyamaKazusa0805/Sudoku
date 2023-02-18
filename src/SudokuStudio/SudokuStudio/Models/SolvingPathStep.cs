namespace SudokuStudio.Models;

/// <summary>
/// Defines a path step in a whole solving path.
/// </summary>
public sealed class SolvingPathStep
{
	/// <summary>
	/// Indicates the index of the step.
	/// </summary>
	public required int Index { get; set; }

	/// <summary>
	/// Indicates the step grid used.
	/// </summary>
	public required Grid StepGrid { get; set; }

	/// <summary>
	/// Indicates the kinds of items that the step tooltip will be displayed.
	/// </summary>
	public required StepTooltipDisplayKind DisplayKinds { get; set; }

	/// <summary>
	/// Indicates the step details.
	/// </summary>
	public required IStep Step { get; set; }
}
