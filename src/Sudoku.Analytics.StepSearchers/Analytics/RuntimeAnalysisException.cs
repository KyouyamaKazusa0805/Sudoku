namespace Sudoku.Analytics;

/// <summary>
/// Represents an exception type that will be thrown in analyzing a puzzle.
/// </summary>
/// <param name="grid">Indicates the grid to be analyzed.</param>
public abstract partial class RuntimeAnalysisException(
	[PrimaryConstructorParameter(GeneratedMemberName = "InvalidGrid")]
	ref readonly Grid grid
) : Exception
{
	/// <inheritdoc/>
	public abstract override string Message { get; }
}
