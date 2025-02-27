namespace Sudoku.SourceGeneration;

public partial class CachedMethodGenerator
{
	/// <summary>
	/// Represents a transform result after successfully to be handled.
	/// </summary>
	/// <param name="Text">Indicates the source text generated.</param>
	/// <param name="Location">Indicates the location information.</param>
	private sealed record SuccessTransformResult(string Text, InterceptedLocation Location) : TransformResult(true);
}
