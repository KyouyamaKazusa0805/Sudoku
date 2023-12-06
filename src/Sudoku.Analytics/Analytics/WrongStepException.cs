using System.SourceGeneration;
using Sudoku.Concepts;

namespace Sudoku.Analytics;

/// <summary>
/// Indicates an error that throws when a solving step is wrong (may be due to wrong algorithm, bug, etc.).
/// </summary>
/// <param name="grid">Indicates the invalid sudoku grid. This property is also stored in the property <see cref="Exception.Data"/>.</param>
/// <param name="wrongStep">Indicates the wrong step.</param>
public sealed partial class WrongStepException(
	[Data(GeneratedMemberName = "CurrentInvalidGrid")] scoped ref readonly Grid grid,
	[Data] Step wrongStep
) : Exception
{
	/// <inheritdoc/>
	public override string Message
		=> $"""
		The step may exist bug that causes the wrong handling.
		Current grid: '{CurrentInvalidGrid:#}'
		Current step: '{WrongStep}'
		""";
}
