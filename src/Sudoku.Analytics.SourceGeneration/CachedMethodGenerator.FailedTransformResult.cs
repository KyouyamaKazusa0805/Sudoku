namespace Sudoku.SourceGeneration;

public partial class CachedMethodGenerator
{
	/// <summary>
	/// Represents a transform result after failed to be handled.
	/// </summary>
	/// <param name="Diagnostic">The diagnostic result.</param>
	private sealed record FailedTransformResult(Diagnostic Diagnostic) : TransformResult(false);
}
