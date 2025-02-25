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
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>An <see cref="AsyncCollectorAwaitable"/> object that can analyze the puzzle asynchronously.</returns>
	public static AsyncCollectorAwaitable CollectAsync(this Collector collector, ref readonly Grid grid, CancellationToken cancellationToken = default)
		=> new(collector, in grid, false, cancellationToken);
}
