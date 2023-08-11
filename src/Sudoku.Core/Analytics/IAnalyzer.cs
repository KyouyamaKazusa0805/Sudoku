namespace Sudoku.Analytics;

/// <summary>
/// Represents with an analyzer, which can solve a puzzle, and return not only a <see cref="Grid"/> as its solution,
/// but a <typeparamref name="TResult"/> instance encapsulating all possible state of the analysis.
/// </summary>
/// <typeparam name="TSelf">The type of the solver itself.</typeparam>
/// <typeparam name="TResult">The type of the target result.</typeparam>
public interface IAnalyzer<in TSelf, out TResult> where TSelf : IAnalyzer<TSelf, TResult> where TResult : IAnalyzerResult<TSelf, TResult>
{
	/// <summary>
	/// Analyze the specified puzzle, and return a <typeparamref name="TResult"/> instance indicating the analyzed result.
	/// </summary>
	/// <param name="puzzle">The puzzle to be analyzed.</param>
	/// <param name="progress">A <see cref="IProgress{T}"/> instance that is used for reporting the state.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current analyzing operation.</param>
	/// <returns>The solver result that provides the information after analyzing.</returns>
	public abstract TResult Analyze(scoped in Grid puzzle, IProgress<AnalyzerProgress>? progress = null, CancellationToken cancellationToken = default);
}
