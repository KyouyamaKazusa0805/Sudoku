namespace Sudoku.Analytics.Async;

/// <summary>
/// Represents an awaiter object that analyzes the specified puzzle.
/// </summary>
public sealed class AsyncAnalyzerAwaiter : IStepGathererAwaiter<AnalysisResult>
{
	/// <summary>
	/// Indicates whether to continue works on captured context instead of reverting back to previous context.
	/// </summary>
	private readonly bool _continueOnCapturedContext;

	/// <summary>
	/// Indicates the backing grid to be analyzed.
	/// </summary>
	private readonly Grid _grid;

	/// <summary>
	/// Indicates the lock.
	/// </summary>
	private readonly Lock _lock = new();

	/// <summary>
	/// Indicates the backing analyzer.
	/// </summary>
	private readonly Analyzer _analyzer;

	/// <summary>
	/// Indicates the progress reporter.
	/// </summary>
	private readonly IProgress<StepGathererProgressPresenter>? _progress;

	/// <summary>
	/// Indicates whether the operation is completed.
	/// </summary>
	/// <remarks>
	/// <include file="../../../../global-doc-comments.xml" path="/g/developer-notes"/>
	/// <para>
	/// The field isn't marked as <see langword="volatile"/>,
	/// because the writting operation uses <see langword="lock"/> statement.
	/// </para>
	/// </remarks>
	[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "<Pending>")]
	private bool _isCompleted;

	/// <summary>
	/// Indicates the cancellation token that can cancel the current operation.
	/// </summary>
	[SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>")]
	private CancellationToken _cancellationToken;

	/// <summary>
	/// Indicates the result.
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="_isCompleted" path="/remarks"/>
	/// </remarks>
	private AnalysisResult? _result;

	/// <summary>
	/// Indicates the exception thrown.
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="_isCompleted" path="/remarks"/>
	/// </remarks>
	private Exception? _exception;

	/// <summary>
	/// Indicates the callback action on analysis operation having been finished.
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="_isCompleted" path="/remarks"/>
	/// </remarks>
	private Action? _continuation;


	/// <summary>
	/// Initializes an <see cref="AsyncAnalyzerAwaiter"/> instance via the specified analyzer.
	/// </summary>
	/// <param name="analyzer">Indicates the analyzer.</param>
	/// <param name="grid">Indicates the grid.</param>
	/// <param name="progress">Indicates the progress reporter.</param>
	/// <param name="continueOnCapturedContext">
	/// Indicates whether to continue works on captured context instead of reverting back to previous context.
	/// </param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	public AsyncAnalyzerAwaiter(
		Analyzer analyzer,
		in Grid grid,
		IProgress<StepGathererProgressPresenter>? progress,
		bool continueOnCapturedContext,
		CancellationToken cancellationToken
	)
	{
		(_grid, _analyzer, _progress, _cancellationToken) = (grid, analyzer, progress, cancellationToken);
		_continueOnCapturedContext = continueOnCapturedContext;

		// Use thread pool to execute the analysis operation.
		ThreadPool.QueueUserWorkItem(CoreOperation);
	}


	/// <inheritdoc/>
	[MemberNotNullWhen(true, nameof(_result))]
	[MemberNotNullWhen(true, nameof(Result))]
	public bool IsCompleted
	{
		get
		{
			lock (_lock)
			{
				return _isCompleted;
			}
		}
	}

	/// <inheritdoc/>
	public AnalysisResult? Result
	{
		get
		{
			lock (_lock)
			{
				return _result;
			}
		}
	}

	/// <inheritdoc/>
	public Exception? Exception
	{
		get
		{
			lock (_lock)
			{
				return _exception;
			}
		}
	}

	/// <inheritdoc/>
	AnalysisResult? IStepGathererAwaiter<AnalysisResult>.Result => Result;

	/// <inheritdoc/>
	Lock IStepGathererAwaiter<AnalysisResult>.Lock => _lock;


	/// <inheritdoc/>
	public AnalysisResult GetResult() => _exception is null ? _result! : throw _exception;

	/// <inheritdoc/>
	public void OnCompleted(Action continuation) => OnCompletedInternal(false, continuation);

	/// <inheritdoc/>
	public void UnsafeOnCompleted(Action continuation) => OnCompletedInternal(true, continuation);

	/// <inheritdoc/>
	void IStepGathererAwaiter<AnalysisResult>.OnCompletedInternal(bool continueOnCapturedContext, Action continuation)
		=> OnCompletedInternal(continueOnCapturedContext, continuation);

	/// <inheritdoc/>
	void IStepGathererAwaiter<AnalysisResult>.StartContinuation(bool continueOnCapturedContext, Action continuation)
		=> StartContinuation(continueOnCapturedContext, continuation);

	/// <inheritdoc/>
	void IStepGathererAwaiter<AnalysisResult>.CoreOperation(object? state) => CoreOperation(state);

	/// <summary>
	/// Executes a custom method on work having been completed.
	/// </summary>
	/// <param name="continueOnCapturedContext">Indicates whether to capture and marshal back to the current context.</param>
	/// <param name="continuation">The method to be executed.</param>
	private void OnCompletedInternal(bool continueOnCapturedContext, Action continuation)
	{
		lock (_lock)
		{
			if (IsCompleted)
			{
				StartContinuation(continueOnCapturedContext, continuation);
			}
			else
			{
				_continuation = continuation;
			}
		}
	}

	/// <summary>
	/// Determine whether we should switch execution context, and start continuation in thread pool from the context decided.
	/// </summary>
	/// <param name="continueOnCapturedContext">Indicates whether to capture and marshal back to the current context.</param>
	/// <param name="continuation">The method to be executed.</param>
	private void StartContinuation(bool continueOnCapturedContext, Action continuation)
	{
		if (continueOnCapturedContext)
		{
			ThreadPool.QueueUserWorkItem(callback);
		}
		else
		{
			var context = ExecutionContext.Capture();
			ThreadPool.QueueUserWorkItem(_ => ExecutionContext.Run(context!, callback, null));
		}


		void callback(object? _) => continuation();
	}

	/// <summary>
	/// Indicates the backing operation.
	/// </summary>
	/// <param name="state">An object containing information to be used by the callback method.</param>
	private void CoreOperation(object? state)
	{
		try
		{
			_result = _analyzer.Analyze(_grid, _progress, _cancellationToken);
		}
		catch (Exception ex)
		{
			_exception = ex;
		}
		finally
		{
			Action? continuation;
			lock (_lock)
			{
				_isCompleted = true;
				continuation = _continuation;
				_continuation = null;
			}

			if (continuation is not null)
			{
				StartContinuation(_continueOnCapturedContext, continuation);
			}
		}
	}
}
