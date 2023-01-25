namespace SudokuStudio.Models;

/// <summary>
/// Defines a path step in a whole solving path.
/// </summary>
public sealed partial class SolvingPathStep
{
	/// <summary>
	/// Indicates the index of the step.
	/// </summary>
	public int Index { get; set; }

	/// <summary>
	/// Indicates the step grid used.
	/// </summary>
	public Grid StepGrid { get; set; }

	/// <summary>
	/// Indicates the step details.
	/// </summary>
	public IStep Step { get; set; } = null!;


	[GeneratedDeconstruction]
	public partial void Deconstruct(out int index, out Grid stepGrid, out IStep step);
}
