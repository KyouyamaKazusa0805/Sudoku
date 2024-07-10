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
		var disallowHighTimeComplexity = @this.Preference.AnalysisPreferences.AnalyzerIgnoresSlowAlgorithms;
		var disallowSpaceTimeComplexity = @this.Preference.AnalysisPreferences.AnalyzerIgnoresHighAllocationAlgorithms;
		return [
			..
			from data in @this.Preference.StepSearcherOrdering.StepSearchersOrder
			where data.IsEnabled
			select data.CreateStepSearcher() into stepSearcher
			let timeFlag = stepSearcher.Metadata.IsConfiguredSlow
			let spaceFlag = stepSearcher.Metadata.IsConfiguredHighAllocation
			where !timeFlag || timeFlag && !disallowHighTimeComplexity || !spaceFlag || spaceFlag && !disallowSpaceTimeComplexity
			select stepSearcher
		];
	}
}
