namespace Sudoku.Analytics.Async;

/// <summary>
/// Represents a collector that can find for all possible steps from a grid, in asynchronous way.
/// </summary>
public static class AsyncCollector
{
	/// <summary>
	/// Asynchronously collects steps from a puzzle.
	/// </summary>
	/// <param name="collector">The collector.</param>
	/// <param name="grid">The grid to be analyzed.</param>
	/// <param name="progress">The progress reporter.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>An <see cref="ParallelAsyncCollectorAwaitable"/> object that can analyze the puzzle asynchronously.</returns>
	public static AsyncCollectorAwaitable CollectAsync(
		this Collector collector,
		in Grid grid,
		IProgress<StepGathererProgressPresenter>? progress = null,
		CancellationToken cancellationToken = default
	) => new(collector, grid, progress, false, cancellationToken);

	/// <summary>
	/// Asynchronously collects steps from a puzzle, with parallel checking on all <see cref="StepSearcher"/> instances.
	/// </summary>
	/// <param name="collector">The collector.</param>
	/// <param name="grid">The grid to be analyzed.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>An <see cref="ParallelAsyncCollectorAwaitable"/> object that can analyze the puzzle asynchronously.</returns>
	public static ParallelAsyncCollectorAwaitable ParallelCollectAsync(this Collector collector, in Grid grid, CancellationToken cancellationToken = default)
		=> new(collector, grid, cancellationToken);
}
