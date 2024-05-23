namespace Sudoku.Drawing.Ocr;

/// <summary>
/// Indicates the exception that throws when the value is failed to fill into a cell.
/// </summary>
/// <param name="cell">Indicates the wrong cell.</param>
/// <param name="digit">Indicates the wrong digit.</param>
public sealed class FailedToFillValueException(Cell cell, Digit digit) : Exception
{
	/// <inheritdoc/>
	public override string Message
		=> string.Format(ResourceDictionary.Get("Message_FailedToFillValueException"), cell.AsCellMap().ToString(), digit + 1);

	/// <inheritdoc/>
	public override string? HelpLink => null;
}
