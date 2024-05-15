namespace Sudoku.Analytics;

/// <summary>
/// Represents an analyzer, which can solve a puzzle and return not a solution <see cref="Grid"/>.
/// The result is a <typeparamref name="TResult"/> instance that encapsulates all possible information
/// produced in the whole analysis time-cycle, using the specified culture to display running information.
/// </summary>
/// <typeparam name="TSelf">The type of the solver itself.</typeparam>
/// <typeparam name="TResult">The type of the target result.</typeparam>
public interface IGlobalizedAnalyzer<in TSelf, out TResult> : IAnalyzer<TSelf, TResult>
	where TSelf : IGlobalizedAnalyzer<TSelf, TResult>
	where TResult : IAnalysisResult<TSelf, TResult>
{
	/// <summary>
	/// Indicates the current culture that is used for displaying running information.
	/// </summary>
	public abstract CultureInfo? CurrentCulture { get; set; }
}
