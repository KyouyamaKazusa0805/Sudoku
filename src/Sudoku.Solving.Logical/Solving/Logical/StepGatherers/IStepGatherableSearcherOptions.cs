namespace Sudoku.Solving.Logical.StepGatherers;

/// <summary>
/// Defines an instance that stores the options that bound with an <see cref="IStepGatherableSearcher"/> instance.
/// </summary>
/// <seealso cref="IStepGatherableSearcher"/>
public interface IStepGatherableSearcherOptions
{
	/// <summary>
	/// Indicates whether the solver only displays the techniques with the same displaying level.
	/// </summary>
	bool OnlyShowSameLevelTechniquesInFindAllSteps { get; set; }
}
