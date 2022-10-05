namespace Sudoku.Recognition;

/// <summary>
/// Indicates the exception that throws when the tesseract has encountered an error.
/// </summary>
public sealed class TesseractException : Exception
{
	/// <summary>
	/// Initializes a <see cref="TesseractException"/> with the specified detail.
	/// </summary>
	/// <param name="detail">The detail.</param>
	public TesseractException(string detail) => Detail = detail;


	/// <summary>
	/// Indicates the detail.
	/// </summary>
	public string? Detail { get; }

	/// <inheritdoc/>
	public override string Message => $"Tesseract has encountered an error: {Detail}.";

	/// <inheritdoc/>
	public override string? HelpLink => null;
}
