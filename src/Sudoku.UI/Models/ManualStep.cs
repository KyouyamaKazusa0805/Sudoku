namespace Sudoku.UI.Models;

/// <summary>
/// Defines a manual step.
/// </summary>
public sealed class ManualStep
{
	/// <summary>
	/// Indicates the current grid used.
	/// </summary>
	public required Grid Grid { get; set; }

	/// <summary>
	/// Indicates the step.
	/// </summary>
	public required Step Step { get; set; }
}
