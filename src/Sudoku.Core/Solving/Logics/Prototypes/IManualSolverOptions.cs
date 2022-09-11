namespace Sudoku.Solving.Prototypes;

/// <summary>
/// Defines an instance that stores the options that bound with a <see cref="ManualSolver"/> instance.
/// </summary>
/// <seealso cref="ManualSolver"/>
public interface IManualSolverOptions
{
	/// <summary>
	/// Indicates whether the solver will apply all found steps in a step searcher,
	/// in order to solve a puzzle faster. If the value is <see langword="true"/>,
	/// the third argument of <see cref="IStepSearcher.GetAll(ICollection{IStep}, in Grid, bool)"/>
	/// will be set <see langword="false"/> value, in order to find all possible steps in a step searcher,
	/// and all steps will be applied at the same time.
	/// </summary>
	/// <seealso cref="IStepSearcher.GetAll(ICollection{IStep}, in Grid, bool)"/>
	public abstract bool IsFullApplying { get; set; }

	/// <summary>
	/// Indicates whether the solver will ignore slow step searchers being marked <see cref="AlgorithmTooSlowAttribute"/>.
	/// </summary>
	/// <seealso cref="AlgorithmTooSlowAttribute"/>
	public abstract bool IgnoreSlowAlgorithms { get; set; }

	/// <summary>
	/// <para>
	/// Indicates the custom searcher collection you defined to solve a puzzle. By default,
	/// the solver will use <see cref="StepSearcherPool.Collection"/> to solve a puzzle.
	/// If you assign a new array of <see cref="IStepSearcher"/>s into this property
	/// the step searchers will use this property instead of <see cref="StepSearcherPool.Collection"/>
	/// to solve a puzzle.
	/// </para>
	/// <para>
	/// Please note that the property will keep the <see langword="null"/> value if you don't assign any values into it;
	/// however, if you want to use the customized collection to solve a puzzle, assign a non-<see langword="null"/>
	/// array into it.
	/// </para>
	/// </summary>
	/// <seealso cref="StepSearcherPool.Collection"/>
	[DisallowNull]
	public abstract IStepSearcher[]? CustomSearcherCollection { get; set; }
}
