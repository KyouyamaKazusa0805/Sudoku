namespace Sudoku.Drawing.Ocr;

/// <summary>
/// Indicates the exception that throws when the value is failed to fill into a cell.
/// </summary>
/// <param name="_cell">Indicates the wrong cell.</param>
/// <param name="_digit">Indicates the wrong digit.</param>
public sealed class FailedToFillValueException(Cell _cell, Digit _digit) : Exception
{
	/// <inheritdoc/>
	public override string Message
		=> string.Format(SR.Get("Message_FailedToFillValueException"), [_cell.AsCellMap().ToString(), _digit + 1]);

	/// <inheritdoc/>
	public override string? HelpLink => null;
}
