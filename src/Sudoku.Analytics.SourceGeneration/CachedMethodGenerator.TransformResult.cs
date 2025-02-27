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

		/// <summary>
		/// Implicit cast from <see cref="SuccessTransformResult"/> array to <see cref="TransformResult"/>.
		/// </summary>
		/// <param name="results">The results.</param>
		public static implicit operator TransformResult(SuccessTransformResult[] results)
			=> new AggregateSuccessTransformResult(results);
	}
}
