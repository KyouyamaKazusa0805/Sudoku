using Sudoku.Presentation;

namespace Sudoku.UI.Data.Steps;

/// <summary>
/// Defines a step that fill the specified <paramref name="Cell"/> with the specified <paramref name="Digit"/>
/// for a specified <paramref name="Grid"/>.
/// </summary>
/// <param name="Grid"><inheritdoc/></param>
/// <param name="Cell">Indicates the cell used.</param>
/// <param name="Digit">Indicates the digit used.</param>
public sealed record MakeDigitStep(in Grid Grid, int Cell, int Digit) : Step(Grid)
{
	/// <inheritdoc/>
	protected override string StepTypeName => "Digit-filling step";

	/// <inheritdoc/>
	protected override string StepDescription =>
		$"Fills the cell {new Coordinate((byte)Cell)} with digit {Digit + 1}";
}
