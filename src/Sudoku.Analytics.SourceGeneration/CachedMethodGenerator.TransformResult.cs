namespace Sudoku.SourceGeneration;

public partial class CachedMethodGenerator
{
	/// <summary>
	/// Represents a transform result.
	/// </summary>
	/// <param name="Success">Indicates whether the result describes for "successful".</param>
	private abstract record TransformResult(bool Success)
	{
		/// <summary>
		/// Implicit cast from <see cref="Diagnostic"/> to <see cref="TransformResult"/>.
		/// </summary>
		/// <param name="diagnostic">The diagnostic result.</param>
		public static implicit operator TransformResult(Diagnostic diagnostic) => new FailedTransformResult(diagnostic);
	}

	/// <summary>
	/// Represents a transform result after successfully to be handled.
	/// </summary>
	/// <param name="Text">Indicates the source text generated.</param>
	/// <param name="Location">Indicates the location information.</param>
	private sealed record SuccessTransformResult(string Text, InterceptedLocation Location) : TransformResult(true);

	/// <summary>
	/// Represents a transform result after failed to be handled.
	/// </summary>
	/// <param name="Diagnostic">The diagnostic result.</param>
	private sealed record FailedTransformResult(Diagnostic Diagnostic) : TransformResult(false);
}
