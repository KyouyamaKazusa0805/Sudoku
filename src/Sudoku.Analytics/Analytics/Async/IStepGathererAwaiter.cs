namespace Sudoku.Analytics.Async;

/// <summary>
/// Represents an awaiter object that handles the backing logic on asynchronous programming.
/// </summary>
/// <typeparam name="TResult">The type of result.</typeparam>
public interface IStepGathererAwaiter<out TResult> : ICriticalNotifyCompletion, INotifyCompletion
	where TResult : allows ref struct
{
	/// <summary>
	/// Indicates whether the operation is completed.
	/// </summary>
	[MemberNotNullWhen(true, nameof(Result))]
	public abstract bool IsCompleted { get; }

	/// <summary>
	/// Indicates the result.
	/// </summary>
	public abstract TResult? Result { get; }

	/// <summary>
	/// Indicates the exception thrown.
	/// </summary>
	public abstract Exception? Exception { get; }

	/// <summary>
	/// Indicates the backing lock.
	/// </summary>
	protected abstract Lock Lock { get; }


	/// <summary>
	/// Returns the result value, or throw the internal exception if unhandled exception is encountered.
	/// </summary>
	/// <returns>The result value.</returns>
	public abstract TResult GetResult();

	/// <summary>
	/// Executes a custom method on work having been completed.
	/// </summary>
	/// <param name="continueOnCapturedContext">Indicates whether to capture and marshal back to the current context.</param>
	/// <param name="continuation">The method to be executed.</param>
	protected abstract void OnCompletedInternal(bool continueOnCapturedContext, Action continuation);

	/// <summary>
	/// Determine whether we should switch execution context, and start continuation in thread pool from the context decided.
	/// </summary>
	/// <param name="continueOnCapturedContext">Indicates whether to capture and marshal back to the current context.</param>
	/// <param name="continuation">The method to be executed.</param>
	protected abstract void StartContinuation(bool continueOnCapturedContext, Action continuation);

	/// <summary>
	/// Indicates the backing operation.
	/// </summary>
	/// <param name="state">An object containing information to be used by the callback method.</param>
	protected abstract void CoreOperation(object? state);
}
