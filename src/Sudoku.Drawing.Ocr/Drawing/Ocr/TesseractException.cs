namespace Sudoku.Drawing.Ocr;

/// <summary>
/// Indicates the exception that throws when the tesseract has encountered an error.
/// </summary>
/// <param name="_detail">Indicates the detail.</param>
public sealed class TesseractException(string _detail) : Exception
{
	/// <inheritdoc/>
	public override string Message => string.Format(SR.Get("Message_TesseractException"), _detail);

	/// <inheritdoc/>
	public override string? HelpLink => null;
}
