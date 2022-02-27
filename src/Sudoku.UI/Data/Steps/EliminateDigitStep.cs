using Sudoku.Presentation;

namespace Sudoku.UI.Data.Steps;

/// <summary>
/// Defines a step that removes the specified <paramref name="Digit"/> from the specified <paramref name="Cell"/>
/// in a specified <paramref name="Grid"/>.
/// </summary>
/// <param name="Grid"><inheritdoc/></param>
/// <param name="Cell">Indicates the cell used.</param>
/// <param name="Digit">Indicates the digit used.</param>
public sealed record EliminateDigitStep(in Grid Grid, int Cell, int Digit) : Step(Grid)
{
	/// <inheritdoc/>
	protected override string StepTypeName => "Digit-removing step";

	/// <inheritdoc/>
	protected override string StepDescription =>
		$"Removes the digit {Digit + 1} from the cell {new Coordinate((byte)Cell)}";
}
