namespace Sudoku.UI.Data.Steps;

/// <summary>
/// Defines a step that is the undoing operation.
/// </summary>
/// <param name="Grid"><inheritdoc/></param>
public sealed record UndoStep(in Grid Grid) : Step(Grid)
{
	/// <inheritdoc/>
	protected override string StepTypeName => "Undo step";

	/// <inheritdoc/>
	protected override string StepDescription => $"Undo the grid {Grid:#}";
}
