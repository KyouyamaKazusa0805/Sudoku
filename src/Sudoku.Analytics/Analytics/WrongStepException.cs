namespace Sudoku.Analytics;

/// <summary>
/// Indicates an error that throws when a solving step is wrong (may be due to wrong algorithm, bug, etc.).
/// </summary>
/// <param name="grid"><inheritdoc/></param>
/// <param name="wrongStep">Indicates the wrong step.</param>
public sealed partial class WrongStepException(scoped ref readonly Grid grid, [PrimaryCosntructorParameter] Step wrongStep) : RuntimeAnalyticsException(in grid)
{
	/// <inheritdoc/>
	public override string Message
		=> $"""
		The step may exist bug that causes the wrong handling.
		Current grid: '{InvalidGrid:#}'
		Current step: '{WrongStep}'
		""";
}
