namespace Sudoku.Solving;

/// <summary>
/// Represents an exception type that will be thrown if a puzzle has multiple solutions,
/// which is an unexpected case in operation handling.
/// </summary>
public sealed class MultipleSolutionException : Exception
{
	/// <inheritdoc/>
	public override string Message => SR.Get("Message_MultipleSolutionException");
}
