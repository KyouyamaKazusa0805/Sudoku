namespace Sudoku.Analytics;

/// <summary>
/// Represents an exception type that will be thrown by an <see cref="IAnalyzer{TSelf, TContext, TResult}"/> instance.
/// </summary>
/// <param name="grid">Indicates the grid to be analyzed.</param>
/// <seealso cref="IAnalyzer{TSelf, TContext, TResult}"/>
public abstract partial class RuntimeAnalysisException([PrimaryConstructorParameter(NamingRule = "Invalid>@")] ref readonly Grid grid) : Exception
{
	/// <inheritdoc/>
	public abstract override string Message { get; }
}
