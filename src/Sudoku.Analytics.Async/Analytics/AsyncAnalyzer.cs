namespace Sudoku.Analytics;

/// <summary>
/// Represents an analyzer that can asynchronously analyze a puzzle.
/// </summary>
public static class AsyncAnalyzer
{
	/// <summary>
	/// Asynchronously analyzes the specified puzzle.
	/// </summary>
	/// <param name="analyzer">The analyzer.</param>
	/// <param name="grid">The grid to be analyzed.</param>
	/// <param name="progress">The progress reporter.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>An <see cref="AsyncAnalyzerAwaitable"/> object that can analyze the puzzle asynchronously.</returns>
	public static AsyncAnalyzerAwaitable AnalyzeAsync(
		this Analyzer analyzer,
		ref readonly Grid grid,
		IProgress<StepGathererProgressPresenter>? progress = null,
		CancellationToken cancellationToken = default
	) => new(analyzer, in grid, progress, cancellationToken);
}
