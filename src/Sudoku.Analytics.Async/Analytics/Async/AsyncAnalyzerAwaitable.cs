namespace Sudoku.Analytics.Async;

/// <summary>
/// Represents an awaitable rule on analysis for a puzzle.
/// </summary>
public readonly ref struct AsyncAnalyzerAwaitable
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
	/// Indicates the backing analyzer.
	/// </summary>
	private readonly Analyzer _analyzer;

	/// <summary>
	/// Indicates the progress reporter.
	/// </summary>
	private readonly IProgress<StepGathererProgressPresenter>? _progress;


	/// <summary>
	/// Initializes an <see cref="AsyncAnalyzerAwaitable"/> object.
	/// </summary>
	/// <param name="analyzer">Indicates the analyzer.</param>
	/// <param name="grid">Indicates the grid.</param>
	/// <param name="progress">The progress reporter.</param>
	/// <param name="continueOnCapturedContext">
	/// Indicates whether to continue works on captured context instead of reverting back to previous context.
	/// </param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	public AsyncAnalyzerAwaitable(
		Analyzer analyzer,
		ref readonly Grid grid,
		IProgress<StepGathererProgressPresenter>? progress,
		bool continueOnCapturedContext,
		CancellationToken cancellationToken
	)
	{
		_analyzer = analyzer;
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
	internal AsyncAnalyzerAwaitable(scoped ref readonly AsyncAnalyzerAwaitable original, bool continueOnCapturedContext) :
		this(original._analyzer, in original._grid, original._progress, continueOnCapturedContext, original._cancellationToken)
	{
	}


	/// <summary>
	/// Returns an <see cref="AsyncAnalyzerAwaiter"/> object that supports the internal awaiting rule of analyzing a puzzle.
	/// </summary>
	/// <returns>An <see cref="AsyncAnalyzerAwaiter"/> object.</returns>
	public AsyncAnalyzerAwaiter GetAwaiter() => new(_analyzer, in _grid, _progress, _continueOnCapturedContext, _cancellationToken);
}
