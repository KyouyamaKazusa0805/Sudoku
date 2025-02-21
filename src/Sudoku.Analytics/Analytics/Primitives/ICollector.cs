namespace Sudoku.Analytics.Primitives;

/// <summary>
/// Represents a collector type.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
/// <typeparam name="TResult">The type of the result value.</typeparam>
public interface ICollector<in TSelf, out TResult> : IStepGatherer<TSelf, TResult>
	where TSelf : ICollector<TSelf, TResult>, allows ref struct
	where TResult : allows ref struct
{
	/// <summary>
	/// Indicates the maximum steps can be collected.
	/// </summary>
	public abstract int MaxStepsCollected { get; set; }

	/// <summary>
	/// Indicates whether the solver only displays the techniques with the same displaying level.
	/// </summary>
	public abstract CollectorDifficultyLevelMode DifficultyLevelMode { get; set; }


	/// <summary>
	/// Search for all possible <see cref="Step"/> instances appeared at the specified grid state.
	/// </summary>
	/// <param name="grid">Indicates the grid to be checked.</param>
	/// <param name="progress">Indicates the progress reporter object.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>The result value.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when property <see cref="DifficultyLevelMode"/> is not defined.
	/// </exception>
	public abstract ReadOnlySpan<Step> Collect(scoped ref readonly Grid grid, IProgress<StepGathererProgressPresenter>? progress, CancellationToken cancellationToken);
}
