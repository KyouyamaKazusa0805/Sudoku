namespace Sudoku.Solving.Logics.Prototypes;

/// <summary>
/// Defines an instance that stores the options that bound with an
/// <see cref="IStepGatherableSearcher{TElement, TGroupingKey, TGroupingValue}"/> instance.
/// </summary>
/// <seealso cref="IStepGatherableSearcher{TElement, TGroupingKey, TGroupingValue}"/>
public interface IStepsGathererOptions
{
	/// <summary>
	/// Indicates whether the solver only displays the techniques with the same displaying level.
	/// </summary>
	public abstract bool OnlyShowSameLevelTechniquesInFindAllSteps { get; set; }
}
