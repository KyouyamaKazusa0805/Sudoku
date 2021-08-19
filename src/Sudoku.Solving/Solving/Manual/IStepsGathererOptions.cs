namespace Sudoku.Solving.Manual;

/// <summary>
/// Defines an instance that stores the options that bound with a <see cref="StepsGatherer"/> instance.
/// </summary>
/// <seealso cref="StepsGatherer"/>
public interface IStepsGathererOptions
{
	/// <summary>
	/// Indicates whether the solver only displays the techniques with the same displaying level.
	/// </summary>
	bool OnlyShowSameLevelTechniquesInFindAllSteps { get; set; }
}
