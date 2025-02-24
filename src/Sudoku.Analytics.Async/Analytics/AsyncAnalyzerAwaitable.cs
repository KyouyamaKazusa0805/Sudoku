namespace Sudoku.Analytics;

/// <summary>
/// Represents an awaitable rule on analysis for a puzzle.
/// </summary>
public readonly ref struct AsyncAnalyzerAwaitable
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
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	public AsyncAnalyzerAwaitable(
		Analyzer analyzer,
		ref readonly Grid grid,
		IProgress<StepGathererProgressPresenter>? progress,
		CancellationToken cancellationToken
	)
	{
		_analyzer = analyzer;
		_grid = ref grid;
		_progress = progress;
		_cancellationToken = cancellationToken;
	}


	/// <summary>
	/// Returns an <see cref="AsyncAnalyzerAwaiter"/> object that supports the internal awaiting rule of analyzing a puzzle.
	/// </summary>
	/// <returns>An <see cref="AsyncAnalyzerAwaiter"/> object.</returns>
	public AsyncAnalyzerAwaiter GetAwaiter() => new(_analyzer, in _grid, _progress, _cancellationToken);
}
