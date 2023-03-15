namespace Sudoku.Analytics;

/// <summary>
/// Indicates an error that throws when a solving step is wrong (may be due to wrong algorithm, bug, etc.).
/// </summary>
public sealed class WrongStepException : Exception
{
	/// <summary>
	/// Initializes a <see cref="WrongStepException"/> with the specified invalid grid and the wrong step.
	/// </summary>
	/// <param name="grid">The invalid sudoku grid.</param>
	/// <param name="wrongStep">The wrong step.</param>
	public WrongStepException(scoped in Grid grid, Step wrongStep) => (CurrentInvalidGrid, WrongStep) = (grid, wrongStep);


	/// <inheritdoc/>
	public override string Message
		=> $"""
		The step: may exist bug that causes the wrong handling.
		Current grid: '{CurrentInvalidGrid:#}'
		Current step: '{WrongStep}'
		""";

	/// <summary>
	/// Indicates the invalid sudoku grid. This property is also stored in the property
	/// <see cref="Exception.Data"/>.
	/// </summary>
	/// <seealso cref="Exception.Data"/>
	public Grid CurrentInvalidGrid { get; }

	/// <summary>
	/// Indicates the wrong step.
	/// </summary>
	public Step WrongStep { get; }
}
