namespace Sudoku.Recognition.Imaging;

/// <summary>
/// Indicates the exception that throws when the recognition tools hasn't been initialized
/// before using a function.
/// </summary>
public sealed class RecognizerNotInitializedException : Exception
{
	/// <inheritdoc/>
	public override string Message => "The recognition tools should have been initialized before using the current function.";

	/// <inheritdoc/>
	public override string? HelpLink => null;
}
