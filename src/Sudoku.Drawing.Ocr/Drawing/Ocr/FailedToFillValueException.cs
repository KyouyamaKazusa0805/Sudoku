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
		=> string.Format(
			SR.Get("Message_FailedToFillValueException"),
#if NET9_0_OR_GREATER
			[
#endif
			cell.AsCellMap().ToString(),
			digit + 1
#if NET9_0_OR_GREATER
			]
#endif
		);

	/// <inheritdoc/>
	public override string? HelpLink => null;
}
