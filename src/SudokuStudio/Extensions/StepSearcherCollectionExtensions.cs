namespace SudokuStudio;

/// <summary>
/// Represents extension methods on step searcher collection.
/// </summary>
public static class StepSearcherCollectionExtensions
{
	/// <summary>
	/// Try to get <see cref="StepSearcher"/> instances via configuration for the specified application.
	/// </summary>
	/// <param name="this">The application.</param>
	/// <returns>A list of <see cref="StepSearcher"/> instances.</returns>
	public static StepSearcher[] GetStepSearchers(this App @this)
	{
		var disallowHighTimeComplexity = @this.Preference.AnalysisPreferences.LogicalSolverIgnoresSlowAlgorithms;
		var disallowSpaceTimeComplexity = @this.Preference.AnalysisPreferences.LogicalSolverIgnoresHighAllocationAlgorithms;
		return [
			..
			from data in @this.Preference.StepSearcherOrdering.StepSearchersOrder
			where data.IsEnabled
			select data.CreateStepSearchers() into stepSearchers
			from s in stepSearchers
			let timeFlag = s.Metadata.IsConfiguredSlow
			let spaceFlag = s.Metadata.IsConfiguredHighAllocation
			where !timeFlag || timeFlag && !disallowHighTimeComplexity || !spaceFlag || spaceFlag && !disallowSpaceTimeComplexity
			select s
		];
	}
}
