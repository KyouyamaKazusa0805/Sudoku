namespace Sudoku.Analytics.Async;

/// <summary>
/// Represents an awaitable rule on collecting steps from a puzzle.
/// </summary>
[TypeImpl(TypeImplFlags.AllObjectMethods)]
public readonly ref partial struct AsyncCollectorAwaitable : IStepGathererAwaitable<AsyncCollectorAwaiter>
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
	/// Initializes an <see cref="AsyncCollectorAwaitable"/> object.
	/// </summary>
	/// <param name="collector">Indicates the collector.</param>
	/// <param name="grid">Indicates the grid.</param>
	/// <param name="continueOnCapturedContext">
	/// Indicates whether to continue works on captured context instead of reverting back to previous context.
	/// </param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	public AsyncCollectorAwaitable(Collector collector, ref readonly Grid grid, bool continueOnCapturedContext, CancellationToken cancellationToken)
	{
		_collector = collector;
		_grid = ref grid;
		_cancellationToken = cancellationToken;
	}


	/// <inheritdoc/>
	public AsyncCollectorAwaiter GetAwaiter() => new(_collector, in _grid, _cancellationToken);
}
