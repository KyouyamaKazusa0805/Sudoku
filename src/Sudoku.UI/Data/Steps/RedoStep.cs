namespace Sudoku.UI.Data.Steps;

/// <summary>
/// Defines a step that is the redoing operation.
/// </summary>
/// <param name="Grid"><inheritdoc/></param>
public sealed record RedoStep(in Grid Grid) : Step(Grid)
{
	/// <inheritdoc/>
	protected override string StepTypeName => "Redo step";

	/// <inheritdoc/>
	protected override string StepDescription => $"Redo the grid {Grid:#}";
}
