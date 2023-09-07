using System.SourceGeneration;

namespace Sudoku.Analytics;

/// <summary>
/// Indicates an error that throws when a solving step is wrong (may be due to wrong algorithm, bug, etc.).
/// </summary>
/// <param name="grid">Indicates the invalid sudoku grid. This property is also stored in the property <see cref="Exception.Data"/>.</param>
/// <param name="wrongStep">Indicates the wrong step.</param>
public sealed partial class WrongStepException(
	[DataMember(GeneratedMemberName = "CurrentInvalidGrid")] scoped in Grid grid,
	[DataMember] Step wrongStep
) : Exception
{
	/// <inheritdoc/>
	public override string Message
		=> $"""
		The step: may exist bug that causes the wrong handling.
		Current grid: '{CurrentInvalidGrid:#}'
		Current step: '{WrongStep}'
		""";
}
