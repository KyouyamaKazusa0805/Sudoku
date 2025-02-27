namespace Sudoku.SourceGeneration;

public partial class CachedMethodGenerator
{
	/// <summary>
	/// Represents a transform result that creates a list of <see cref="SuccessTransformResult"/> instances to be generated once.
	/// </summary>
	/// <param name="Results">Indicates the results created.</param>
	private sealed record AggregateSuccessTransformResult(SuccessTransformResult[] Results) : TransformResult(true), IEnumerable<SuccessTransformResult>
	{
		/// <inheritdoc/>
		public IEnumerator<SuccessTransformResult> GetEnumerator() => Results.AsEnumerable().GetEnumerator();

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => Results.GetEnumerator();
	}
}
