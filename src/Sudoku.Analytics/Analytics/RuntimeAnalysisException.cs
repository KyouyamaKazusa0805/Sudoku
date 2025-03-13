namespace Sudoku.Analytics;

/// <summary>
/// Represents an exception type that will be thrown by an <see cref="IAnalyzer{TSelf, TResult}"/> instance.
/// </summary>
/// <param name="grid">Indicates the grid to be analyzed.</param>
/// <seealso cref="IAnalyzer{TSelf, TResult}"/>
public abstract partial class RuntimeAnalysisException([Property(NamingRule = "Invalid>@")] in Grid grid) : Exception
{
	/// <inheritdoc/>
	public abstract override string Message { get; }
}
