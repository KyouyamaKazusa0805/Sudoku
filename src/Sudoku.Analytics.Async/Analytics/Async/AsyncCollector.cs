namespace Sudoku.Analytics.Async;

/// <summary>
/// Represents a collector that can find for all possible steps from a grid, in asynchronous way.
/// </summary>
public static class AsyncCollector
{
	/// <summary>
	/// Updates the awaiting rule to specify whether the execution context will be back to the previous one,
	/// instead of just using the current context, to reduce memory allocation.
	/// </summary>
	/// <param name="this">Indicates the current instance.</param>
	/// <param name="continueOnCapturedContext">
	/// Indicates whether to continue works on captured context instead of reverting back to previous context.
	/// </param>
	/// <returns>A new <see cref="AsyncCollectorAwaitable"/> instance, with context switching option updated.</returns>
	public static AsyncCollectorAwaitable ConfigureAwait(
		this scoped ref readonly AsyncCollectorAwaitable @this,
		bool continueOnCapturedContext
	) => new(in @this, continueOnCapturedContext);

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
		ref readonly Grid grid,
		IProgress<StepGathererProgressPresenter>? progress = null,
		CancellationToken cancellationToken = default
	) => new(collector, in grid, progress, false, cancellationToken);

	/// <summary>
	/// Asynchronously collects steps from a puzzle, with parallel checking on all <see cref="StepSearcher"/> instances.
	/// </summary>
	/// <param name="collector">The collector.</param>
	/// <param name="grid">The grid to be analyzed.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>An <see cref="ParallelAsyncCollectorAwaitable"/> object that can analyze the puzzle asynchronously.</returns>
	public static ParallelAsyncCollectorAwaitable ParallelCollectAsync(
		this Collector collector,
		ref readonly Grid grid,
		CancellationToken cancellationToken = default
	) => new(collector, in grid, cancellationToken);
}
