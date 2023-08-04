namespace SudokuStudio.Collection;

/// <summary>
/// Defines a solving path.
/// </summary>
public sealed class SolvingPathStepCollection : List<SolvingPathStepBindableSource>
{
	/// <summary>
	/// Creates a <see cref="SolvingPathStepCollection"/> instance via the specified <see cref="AnalyzerResult"/> instance.
	/// </summary>
	/// <param name="analyzerResult">A <see cref="AnalyzerResult"/> instance.</param>
	/// <param name="displayItems">Indicates all displaying values.</param>
	/// <returns>An instance of the current type.</returns>
	public static SolvingPathStepCollection Create(AnalyzerResult analyzerResult, StepTooltipDisplayItems displayItems)
	{
		if (analyzerResult is not { IsSolved: true, SolvingPath: { Length: var pathStepsCount } steps })
		{
			return [];
		}

		var showHodoku = ((App)Application.Current).Preference.AnalysisPreferences.DisplayDifficultyRatingForHodoku;
		var showSudokuExplainer = ((App)Application.Current).Preference.AnalysisPreferences.DisplayDifficultyRatingForSudokuExplainer;

		var result = new List<SolvingPathStepBindableSource>();
		for (var i = 0; i < pathStepsCount; i++)
		{
			var (sGrid, s) = steps[i];
			result.Add(new() { Index = i, StepGrid = sGrid, Step = s, DisplayItems = displayItems, ShowHodokuDifficulty = showHodoku, ShowSudokuExplainerDifficulty = showSudokuExplainer });
		}

		return [.. result];
	}
}
