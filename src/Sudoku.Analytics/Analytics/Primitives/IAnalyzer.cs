namespace Sudoku.Analytics.Primitives;

/// <summary>
/// Represents an analyzer, which can solve a puzzle and return not a solution <see cref="Grid"/>.
/// The result is a <typeparamref name="TResult"/> instance that encapsulates all possible information
/// produced in the whole analysis time-cycle.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
/// <typeparam name="TContext">The type of the context.</typeparam>
/// <typeparam name="TResult">The type of the target result.</typeparam>
public interface IAnalyzer<in TSelf, TContext, out TResult> : IStepGatherer<TSelf, TContext, TResult>
	where TSelf : IAnalyzer<TSelf, TContext, TResult>, allows ref struct
	where TContext : allows ref struct
	where TResult : IAnalysisResult<TResult, TSelf, TContext>, allows ref struct
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
	/// <param name="context">A context instance that can be used for analyzing a puzzle.</param>
	/// <returns>The result value.</returns>
	/// <exception cref="InvalidOperationException">Throws when the puzzle has already been solved.</exception>
	public abstract TResult Analyze(ref readonly TContext context);
}
