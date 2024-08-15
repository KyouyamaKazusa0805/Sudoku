namespace Sudoku.Analytics;

/// <summary>
/// Represents a collector type.
/// </summary>
/// <typeparam name="TSelf">The type itself.</typeparam>
/// <typeparam name="TContext">The type of the context.</typeparam>
/// <typeparam name="TResult">The type of the result value.</typeparam>
public interface ICollector<in TSelf, TContext, out TResult> : IAnalyzerOrCollector<TSelf, TContext, TResult>
	where TSelf : ICollector<TSelf, TContext, TResult>, allows ref struct
	where TContext : allows ref struct
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
	/// Indicates the current culture that is used for displaying running information.
	/// </summary>
	public abstract IFormatProvider? CurrentCulture { get; set; }


	/// <summary>
	/// Search for all possible <see cref="Step"/> instances appeared at the specified grid state.
	/// </summary>
	/// <param name="context">A context instance that can be used for analyzing a puzzle.</param>
	/// <returns>The result value.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when property <see cref="DifficultyLevelMode"/> is not defined.
	/// </exception>
	public abstract ReadOnlySpan<Step> Collect(ref readonly TContext context);
}
