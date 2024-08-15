namespace Sudoku.Analytics;

/// <summary>
/// Represents an analyzer, which can solve a puzzle and return not a solution <see cref="Grid"/>.
/// The result is a <typeparamref name="TResult"/> instance that encapsulates all possible information
/// produced in the whole analysis time-cycle.
/// </summary>
/// <typeparam name="TSelf">The type of the solver itself.</typeparam>
/// <typeparam name="TContext">The type of the context.</typeparam>
/// <typeparam name="TResult">The type of the target result.</typeparam>
public interface IAnalyzer<in TSelf, TContext, out TResult> : IAnalyzerOrCollector<TSelf, TContext, TResult>
	where TSelf : IAnalyzer<TSelf, TContext, TResult>, allows ref struct
	where TContext : allows ref struct
	where TResult : IAnalysisResult<TSelf, TContext, TResult>, allows ref struct
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
	/// Indicates the current culture that is used for displaying running information.
	/// </summary>
	public abstract IFormatProvider? CurrentCulture { get; set; }

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
