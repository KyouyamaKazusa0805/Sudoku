namespace Sudoku.Analytics;

/// <summary>
/// Represents a collector type.
/// </summary>
/// <typeparam name="TSelf">The type itself.</typeparam>
public interface ICollector<in TSelf> where TSelf : ICollector<TSelf>, allows ref struct
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
	/// Search for all possible steps in a grid.
	/// </summary>
	/// <param name="context">A <see cref="CollectorContext"/> instance that can be used for analyzing a puzzle.</param>
	/// <returns>
	/// The result. If cancelled, the return value will be an empty instance; otherwise, a real list even though it may be empty.
	/// </returns>
	public abstract ReadOnlySpan<Step> Collect(ref readonly CollectorContext context);
}
