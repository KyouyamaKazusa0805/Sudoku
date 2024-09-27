namespace Sudoku.Measuring;

/// <summary>
/// Represents an exception type that will be thrown if factor resource is mismatched.
/// </summary>
public sealed class FactorResourceMismatchedException : Exception
{
	/// <inheritdoc/>
	public override string Message => SR.Get("Message_FactorResourceMismatchedException");
}
