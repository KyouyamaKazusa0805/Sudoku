namespace Sudoku.Solving.Mechanism;

/// <summary>
/// Represents with an analyzer, which can solve a puzzle, and return not only a <see cref="Grid"/> as its solution,
/// but a <typeparamref name="TResult"/> instance encapsulating all possible status of the analysis.
/// </summary>
/// <typeparam name="T">The solver's type.</typeparam>
/// <typeparam name="TResult">The type of the target result.</typeparam>
public interface IAnalyzer<in T, out TResult> where T : IAnalyzer<T, TResult> where TResult : IAnalyzerResult<T, TResult>
{
	/// <summary>
	/// Analyze the specified puzzle, and return a <typeparamref name="TResult"/> instance indicating the analyzed result.
	/// </summary>
	/// <param name="puzzle">The puzzle to be analyzed.</param>
	/// <param name="progress">A <see cref="IProgress{T}"/> instance that is used for reporting the status.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current analyzing operation.</param>
	/// <returns>The solver result that provides the information after analyzing.</returns>
	TResult Analyze(scoped in Grid puzzle, IProgress<double>? progress = null, CancellationToken cancellationToken = default);
}
