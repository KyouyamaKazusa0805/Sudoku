namespace Sudoku.Analytics.Async;

/// <summary>
/// Represents an awaitable rule on collecting steps from a puzzle.
/// </summary>
[TypeImpl(TypeImplFlags.AllObjectMethods)]
public readonly ref partial struct ParallelAsyncCollectorAwaitable : IStepGathererAwaitable<ParallelAsyncCollectorAwaiter>
{
	/// <summary>
	/// Indicates the reference to the grid.
	/// </summary>
	private readonly ref readonly Grid _grid;

	/// <summary>
	/// Indicates the cancellation token that can cancel the current operation.
	/// </summary>
	private readonly CancellationToken _cancellationToken;

	/// <summary>
	/// Indicates the backing analyzer.
	/// </summary>
	private readonly Collector _collector;


	/// <summary>
	/// Initializes an <see cref="ParallelAsyncCollectorAwaitable"/> object.
	/// </summary>
	/// <param name="collector">Indicates the collector.</param>
	/// <param name="grid">Indicates the grid.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	public ParallelAsyncCollectorAwaitable(Collector collector, ref readonly Grid grid, CancellationToken cancellationToken)
	{
		_collector = collector;
		_grid = ref grid;
		_cancellationToken = cancellationToken;
	}


	/// <inheritdoc/>
	public ParallelAsyncCollectorAwaiter GetAwaiter() => new(_collector, in _grid, _cancellationToken);
}
