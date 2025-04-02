namespace Sudoku.Analytics.Async;

/// <summary>
/// Represents an awaitable object that runs for step gathering rule (analysis or collecting operation).
/// </summary>
/// <typeparam name="TAwaiter">The type of awaiter.</typeparam>
internal interface IStepGathererAwaitable<out TAwaiter>
{
	/// <summary>
	/// Returns an object of type <typeparamref name="TAwaiter"/> that supports the internal awaiting rule of analyzing a puzzle.
	/// </summary>
	/// <returns>An object of type <typeparamref name="TAwaiter"/>.</returns>
	public abstract TAwaiter GetAwaiter();
}
