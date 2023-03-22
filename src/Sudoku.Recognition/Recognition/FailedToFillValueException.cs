namespace Sudoku.Recognition;

/// <summary>
/// Indicates the exception that throws when the value is failed to fill into a cell.
/// </summary>
/// <param name="cell">The cell.</param>
/// <param name="digit">The digit.</param>
public sealed class FailedToFillValueException(int cell, int digit) : Exception
{
	/// <summary>
	/// Indicates the wrong cell.
	/// </summary>
	public int Cell { get; } = cell;

	/// <summary>
	/// Indicates the wrong digit.
	/// </summary>
	public int Digit { get; } = digit;

	/// <inheritdoc/>
	public override string Message => $"Can't fill the cell {CellsMap[Cell]} with the digit {Digit + 1}.";

	/// <inheritdoc/>
	public override string? HelpLink => null;
}
