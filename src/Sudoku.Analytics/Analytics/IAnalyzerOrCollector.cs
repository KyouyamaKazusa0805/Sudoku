namespace Sudoku.Analytics;

/// <summary>
/// Extracts a new type that represents an analyzer or a collector type.
/// </summary>
public interface IAnalyzerOrCollector
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
	StepSearcher[]? StepSearchers { get; }

	/// <summary>
	/// Indicates the result step searchers used in the current analyzer or collector.
	/// </summary>
	StepSearcher[] ResultStepSearchers { get; }
}
