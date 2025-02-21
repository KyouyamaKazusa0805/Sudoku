namespace Sudoku.Analytics.Primitives;

/// <summary>
/// Represents an analyzer, which can solve a puzzle and return not a solution <see cref="Grid"/>.
/// The result is a <typeparamref name="TResult"/> instance that encapsulates all possible information
/// produced in the whole analysis time-cycle.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
/// <typeparam name="TResult">The type of the target result.</typeparam>
public interface IAnalyzer<in TSelf, out TResult> : IStepGatherer<TSelf, TResult>
	where TSelf : IAnalyzer<TSelf, TResult>, allows ref struct
	where TResult : IAnalysisResult<TResult, TSelf>, allows ref struct
{
	/// <summary>
	/// Indicates whether the solver will apply all found steps in a step searcher, in order to solve a puzzle faster.
	/// </summary>
	public abstract bool IsFullApplying { get; }

	/// <summary>
	/// Indicates whether the solver will choose a step to be applied after having searched all possible steps, in random.
	/// </summary>
	public abstract bool RandomizedChoosing { get; }

	/// <summary>
	/// The internal <see cref="Random"/> instance to be used.
	/// </summary>
	protected abstract Random RandomNumberGenerator { get; }


	/// <summary>
	/// Analyze the specified puzzle, and return a <typeparamref name="TResult"/> instance indicating the analyzed result.
	/// </summary>
	/// <param name="grid">Indicates the grid to be checked.</param>
	/// <param name="progress">Indicates the progress reporter object.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>The result value.</returns>
	public abstract TResult Analyze(ref readonly Grid grid, IProgress<StepGathererProgressPresenter>? progress, CancellationToken cancellationToken);
}
