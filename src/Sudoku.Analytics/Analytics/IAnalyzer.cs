namespace Sudoku.Analytics;

/// <summary>
/// Represents an analyzer, which can solve a puzzle and return not a solution <see cref="Grid"/>.
/// The result is a <typeparamref name="TResult"/> instance that encapsulates all possible information
/// produced in the whole analysis time-cycle.
/// </summary>
/// <typeparam name="TSelf">The type of the solver itself.</typeparam>
/// <typeparam name="TResult">The type of the target result.</typeparam>
public interface IAnalyzer<in TSelf, out TResult> where TSelf : IAnalyzer<TSelf, TResult> where TResult : IAnalysisResult<TSelf, TResult>
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
	public abstract CultureInfo? CurrentCulture { get; set; }

	/// <summary>
	/// The internal <see cref="Random"/> instance to be used.
	/// </summary>
	protected abstract Random RandomNumberGenerator { get; }


	/// <summary>
	/// Analyze the specified puzzle, and return a <typeparamref name="TResult"/> instance indicating the analyzed result.
	/// </summary>
	/// <param name="puzzle">The puzzle to be analyzed.</param>
	/// <param name="progress">An <see cref="IProgress{T}"/> instance that is used for reporting the state.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current analyzing operation.</param>
	/// <returns>The solver result that provides the information after analyzing.</returns>
	public abstract TResult Analyze(ref readonly Grid puzzle, IProgress<AnalysisProgress>? progress = null, CancellationToken cancellationToken = default);
}
