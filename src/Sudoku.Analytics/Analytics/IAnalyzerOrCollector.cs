namespace Sudoku.Analytics;

/// <summary>
/// Represents an analyzer or a collector type.
/// </summary>
/// <typeparam name="TSelf">The type itself.</typeparam>
/// <typeparam name="TContext">The type of the context.</typeparam>
/// <typeparam name="TResult">The type of the result value.</typeparam>
public interface IAnalyzerOrCollector<in TSelf, TContext, out TResult>
	where TSelf : IAnalyzerOrCollector<TSelf, TContext, TResult>, allows ref struct
	where TContext : allows ref struct
	where TResult : allows ref struct
{
	/// <summary>
	/// <para>
	/// Indicates the custom <see cref="StepSearcher"/>s you defined to solve a puzzle.
	/// By default, the solver will use <see cref="StepSearcherPool.StepSearchers"/> to solve a puzzle.
	/// If you assign a new array of <see cref="StepSearcher"/>s into this property
	/// the step searchers will use this property instead of <see cref="StepSearcherPool.StepSearchers"/> to solve a puzzle.
	/// </para>
	/// <para>
	/// Please note that the property will keep the <see langword="null"/> value if you don't assign any values into it;
	/// however, if you want to use the customized collection to solve a puzzle, assign a non-<see langword="null"/> array into it.
	/// </para>
	/// </summary>
	/// <seealso cref="StepSearcherPool.StepSearchers"/>
	public abstract ReadOnlyMemory<StepSearcher> StepSearchers { get; set; }

	/// <summary>
	/// Indicates the result step searchers used in the current analyzer or collector.
	/// </summary>
	public abstract ReadOnlyMemory<StepSearcher> ResultStepSearchers { get; }

	/// <summary>
	/// Indicates the current culture that is used for displaying running information.
	/// </summary>
	public CultureInfo CurrentCulture => Options.CurrentCulture;

	/// <summary>
	/// Indicates the extra options to be set. The options will be passed into <see cref="Step"/> instances collected
	/// in internal method called <c>Collect</c>, and create <see cref="Step"/> instances and pass into constructor.
	/// </summary>
	/// <seealso cref="Step"/>
	public abstract StepSearcherOptions Options { get; set; }

	/// <summary>
	/// Represents a list of <see cref="Action{T}"/> of <see cref="StepSearcher"/> instances
	/// to assign extra configuration to step searcher instances.
	/// </summary>
	/// <seealso cref="Action{T}"/>
	/// <seealso cref="StepSearcher"/>
	public abstract ICollection<Action<StepSearcher>> Setters { get; }


	/// <summary>
	/// Try to apply setters.
	/// </summary>
	protected static sealed void ApplySetters(TSelf instance)
	{
		foreach (var setter in instance.Setters)
		{
			foreach (var stepSearcher in instance.ResultStepSearchers)
			{
				setter(stepSearcher);
			}
		}
	}

	/// <summary>
	/// Try to filter step searchers via the specified running area; removing all step searchers if running area does not match.
	/// </summary>
	/// <param name="in">The step searchers passed in.</param>
	/// <param name="runningArea">The running area to be checked.</param>
	/// <returns>Filtered collection.</returns>
	protected static sealed ReadOnlyMemory<StepSearcher> FilterStepSearchers(ReadOnlyMemory<StepSearcher> @in, StepSearcherRunningArea runningArea)
		=> from searcher in @in where searcher.RunningArea.HasFlag(runningArea) select searcher;
}
