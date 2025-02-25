namespace Sudoku.Analytics.Async;

/// <summary>
/// Represents an awaitable rule on collecting steps from a puzzle.
/// </summary>
[TypeImpl(TypeImplFlags.AllObjectMethods)]
public readonly ref partial struct AsyncCollectorAwaitable : IStepGathererAwaitable<AsyncCollectorAwaiter>
{
	/// <summary>
	/// Indicates whether to continue works on captured context instead of reverting back to previous context.
	/// </summary>
	private readonly bool _continueOnCapturedContext;

	/// <summary>
	/// Indicates the reference to the grid.
	/// </summary>
	private readonly ref readonly Grid _grid;

	/// <summary>
	/// Indicates the cancellation token that can cancel the current operation.
	/// </summary>
	private readonly CancellationToken _cancellationToken;

	/// <summary>
	/// Indicates the backing collector.
	/// </summary>
	private readonly Collector _collector;

	/// <summary>
	/// Indicates the progress reporter.
	/// </summary>
	private readonly IProgress<StepGathererProgressPresenter>? _progress;


	/// <summary>
	/// Initializes an <see cref="AsyncCollectorAwaitable"/> object.
	/// </summary>
	/// <param name="collector">Indicates the collector.</param>
	/// <param name="grid">Indicates the grid.</param>
	/// <param name="progress">The progress reporter.</param>
	/// <param name="continueOnCapturedContext">
	/// Indicates whether to continue works on captured context instead of reverting back to previous context.
	/// </param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	public AsyncCollectorAwaitable(
		Collector collector,
		ref readonly Grid grid,
		IProgress<StepGathererProgressPresenter>? progress,
		bool continueOnCapturedContext,
		CancellationToken cancellationToken
	)
	{
		_collector = collector;
		_grid = ref grid;
		_progress = progress;
		_cancellationToken = cancellationToken;
		_continueOnCapturedContext = continueOnCapturedContext;
	}

	/// <summary>
	/// Copies the specified source, and update for field <see cref="_continueOnCapturedContext"/>.
	/// </summary>
	/// <param name="original">The original value.</param>
	/// <param name="continueOnCapturedContext">The new value to be assigned to <see cref="_continueOnCapturedContext"/>.</param>
	internal AsyncCollectorAwaitable(scoped ref readonly AsyncCollectorAwaitable original, bool continueOnCapturedContext) :
		this(original._collector, in original._grid, original._progress, continueOnCapturedContext, original._cancellationToken)
	{
	}


	/// <inheritdoc/>
	public AsyncCollectorAwaiter GetAwaiter() => new(_collector, in _grid, _progress, _continueOnCapturedContext, _cancellationToken);
}
