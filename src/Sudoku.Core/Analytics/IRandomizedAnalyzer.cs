using Sudoku.Concepts;

namespace Sudoku.Analytics;

/// <summary>
/// Represents an analyzer, which can solve a puzzle and return not a solution <see cref="Grid"/>,
/// The result is a <typeparamref name="TSelf"/> instance that encapsulates all possible information
/// produced in the whole analysis time-cycle, with randomized chosing.
/// </summary>
/// <typeparam name="TSelf">The type of the solver itself.</typeparam>
/// <typeparam name="TResult">The type of the target result.</typeparam>
/// <seealso cref="Grid"/>
public interface IRandomizedAnalyzer<in TSelf, out TResult> : IAnalyzer<TSelf, TResult>
	where TSelf : IRandomizedAnalyzer<TSelf, TResult>
	where TResult : IAnalyzerResult<TSelf, TResult>
{
	/// <summary>
	/// Indicates whether the solver will choose a step to be applied after having searched all possible steps, in random.
	/// </summary>
	public abstract bool RandomizedChoosing { get; }

	/// <summary>
	/// The internal <see cref="Random"/> instance to be used.
	/// </summary>
	protected abstract Random RandomNumberGenerator { get; }
}
