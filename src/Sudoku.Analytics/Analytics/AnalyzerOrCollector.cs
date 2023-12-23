namespace Sudoku.Analytics;

/// <summary>
/// Extracts a new type that represents an analyzer or a collector type.
/// </summary>
public abstract class AnalyzerOrCollector
{
	/// <summary>
	/// <para>
	/// Indicates the custom <see cref="StepSearcher"/>s you defined to solve a puzzle.
	/// By default, the solver will use <see cref="StepSearcherPool.Default(bool)"/> to solve a puzzle.
	/// If you assign a new array of <see cref="StepSearcher"/>s into this property
	/// the step searchers will use this property instead of <see cref="StepSearcherPool.Default(bool)"/> to solve a puzzle.
	/// </para>
	/// <para>
	/// Please note that the property will keep the <see langword="null"/> value if you don't assign any values into it;
	/// however, if you want to use the customized collection to solve a puzzle, assign a non-<see langword="null"/> array into it.
	/// </para>
	/// </summary>
	/// <seealso cref="StepSearcherPool.Default(bool)"/>
	[DisallowNull]
	public abstract StepSearcher[]? StepSearchers { get; protected internal set; }

	/// <summary>
	/// Indicates the result step searchers used in the current analyzer or collector.
	/// </summary>
	public abstract StepSearcher[] ResultStepSearchers { get; protected internal set; }

	/// <summary>
	/// Indicates the extra options to be set. The options will be passed into <see cref="Step"/> instances collected
	/// in internal method called <c>Collect</c>, and create <see cref="Step"/> instances and pass into constructor.
	/// </summary>
	/// <seealso cref="Step"/>
	public abstract StepSearcherOptions Options { get; protected internal set; }


	/// <summary>
	/// Try to filter step searchers via the specified running area; removing all step searchers if running area does not match.
	/// </summary>
	/// <param name="in">The step searchers passed in.</param>
	/// <param name="runningArea">The running area to be checked.</param>
	/// <returns>Filtered collection.</returns>
	protected static StepSearcher[] FilterStepSearchers(StepSearcher[] @in, StepSearcherRunningArea runningArea)
		=> from searcher in @in where searcher.RunningArea.Flags(runningArea) select searcher;
}
