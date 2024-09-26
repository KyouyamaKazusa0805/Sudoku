namespace Sudoku.Inferring;

/// <summary>
/// Represents an exception that will be thrown if more than 10000 solutions found in a possible deadly pattern to be verified.
/// </summary>
public sealed class DeadlyPatternInferrerLimitReachedException : Exception
{
	/// <inheritdoc/>
	public override string Message => SR.Get("Message_DeadlyPatternInferrerLimitReachedException");
}
