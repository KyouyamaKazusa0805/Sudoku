namespace Sudoku.Analytics;

/// <summary>
/// Indicates an error that throws when a solving step is wrong (may be due to wrong algorithm, bug, etc.).
/// </summary>
public sealed class WrongStepException(scoped in Grid grid, Step wrongStep) : Exception
{
	/// <inheritdoc/>
	public override string Message
		=>
		$"""
		The step: may exist bug that causes the wrong handling.
		Current grid: '{CurrentInvalidGrid:#}'
		Current step: '{WrongStep}'
		""";

	/// <summary>
	/// Indicates the invalid sudoku grid. This property is also stored in the property
	/// <see cref="Exception.Data"/>.
	/// </summary>
	/// <seealso cref="Exception.Data"/>
	public Grid CurrentInvalidGrid { get; } = grid;

	/// <summary>
	/// Indicates the wrong step.
	/// </summary>
	public Step WrongStep { get; } = wrongStep;
}
