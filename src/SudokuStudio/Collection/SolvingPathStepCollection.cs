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
	public static IEnumerable<SolvingPathStepBindableSource> Create(AnalyzerResult analyzerResult, StepTooltipDisplayItems displayItems)
	{
		if (analyzerResult is not { IsSolved: true, InterimSteps: var steps, InterimGrids: var grids })
		{
			yield break;
		}

		var showHodoku = ((App)Application.Current).Preference.AnalysisPreferences.DisplayDifficultyRatingForHodoku;
		var showSudokuExplainer = ((App)Application.Current).Preference.AnalysisPreferences.DisplayDifficultyRatingForSudokuExplainer;
		var path = StepMarshal.Combine(grids, steps).ToArray();
		for (var i = 0; i < path.Length; i++)
		{
			var (interimGrid, interimStep) = path[i];
			yield return new()
			{
				Index = i,
				InterimGrid = interimGrid,
				InterimStep = interimStep,
				DisplayItems = displayItems,
				ShowHodokuDifficulty = showHodoku,
				ShowSudokuExplainerDifficulty = showSudokuExplainer
			};
		}
	}
}
