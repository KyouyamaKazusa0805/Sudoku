namespace Sudoku.Recognition;

/// <summary>
/// Indicates the exception that throws when the value is failed to fill into a cell.
/// </summary>
public sealed class FailedToFillValueException : Exception
{
	/// <summary>
	/// Initializes a <see cref="FailedToFillValueException"/> instance
	/// with the specified cell and the digit.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	public FailedToFillValueException(int cell, int digit) => (Cell, Digit) = (cell, digit);


	/// <summary>
	/// Indicates the wrong cell.
	/// </summary>
	public int Cell { get; }

	/// <summary>
	/// Indicates the wrong digit.
	/// </summary>
	public int Digit { get; }

	/// <inheritdoc/>
	public override string Message => $"Can't fill the cell {Cells.Empty + Cell} with the digit {Digit + 1}.";

	/// <inheritdoc/>
	public override string? HelpLink => null;
}
