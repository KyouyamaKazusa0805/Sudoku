namespace Sudoku.Drawing.Ocr;

/// <summary>
/// Indicates the exception that throws when the tesseract has encountered an error.
/// </summary>
/// <param name="detail">Indicates the detail.</param>
public sealed class TesseractException(string detail) : Exception
{
	/// <inheritdoc/>
	public override string Message => string.Format(ResourceDictionary.Get("Message_TesseractException"), detail);

	/// <inheritdoc/>
	public override string? HelpLink => null;
}
