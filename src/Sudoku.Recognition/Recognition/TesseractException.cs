namespace Sudoku.Recognition;

/// <summary>
/// Indicates the exception that throws when the tesseract has encountered an error.
/// </summary>
/// <param name="detail">The detail.</param>
public sealed class TesseractException(string detail) : Exception
{
	/// <summary>
	/// Indicates the detail.
	/// </summary>
	public string Detail { get; } = detail;

	/// <inheritdoc/>
	public override string Message => $"Tesseract has encountered an error: {Detail}.";

	/// <inheritdoc/>
	public override string? HelpLink => null;
}
