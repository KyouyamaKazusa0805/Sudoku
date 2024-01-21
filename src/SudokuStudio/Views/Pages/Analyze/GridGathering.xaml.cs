namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines the gathering page.
/// </summary>
public sealed partial class GridGathering : Page, IAnalyzeTabPage
{
	/// <summary>
	/// Indicates the found steps currently.
	/// </summary>
	internal Step[]? _currentFountSteps;


	/// <summary>
	/// Initializes a <see cref="GridGathering"/> instance.
	/// </summary>
	public GridGathering() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;

	/// <inheritdoc/>
	AnalyzerResult? IAnalyzeTabPage.AnalysisResult { get; set; }


	/// <summary>
	/// Converts the specified collection into the target view source collection.
	/// </summary>
	/// <param name="collection">The raw collection.</param>
	/// <param name="grid">The puzzle.</param>
	/// <returns>The collection that can be used as view source.</returns>
	private ObservableCollection<TechniqueGroupBindableSource> GetTechniqueGroups(Step[] collection, Grid grid)
	{
		var displayItems = ((App)Application.Current).Preference.UIPreferences.StepDisplayItems;
		return new(
			from step in collection
			let technique = step.Code
			orderby step.DifficultyLevel, technique.GetGroup(), technique
			group step by step.GetName(App.CurrentCulture) into stepsGroupedByName
			let name = stepsGroupedByName.Key
			let elements =
				from step in stepsGroupedByName
				orderby step.DifficultyLevel, step.Difficulty
				select new SolvingPathStepBindableSource { DisplayItems = displayItems, Step = step, StepGrid = grid }
			select new TechniqueGroupBindableSource(elements) { Key = name }
		);
	}


	private void TechniqueGroupView_StepChosen(TechniqueGroupView sender, TechniqueGroupViewStepChosenEventArgs e)
		=> BasePage.VisualUnit = e.ChosenStep;

	private void TechniqueGroupView_StepApplied(TechniqueGroupView sender, TechniqueGroupViewStepAppliedEventArgs e)
	{
		var appliedPuzzle = BasePage.SudokuPane.Puzzle;
		appliedPuzzle.Apply(e.ChosenStep);

		BasePage.SudokuPane.Puzzle = appliedPuzzle;
	}

	private void FilterGatheredStepsButton_Click(object sender, RoutedEventArgs e)
	{
		var grid = BasePage.SudokuPane.Puzzle;
		if (_currentFountSteps is null || !grid.IsValid)
		{
			return;
		}

		try
		{
			var filtered = TechniqueFiltering.Filter(_currentFountSteps, StepGatheringTextBox.Text);
			TechniqueGroupView.TechniqueGroups.Source = GetTechniqueGroups(filtered, grid);
		}
		catch (ExpressiveException)
		{
			FilteringExpressionInvalidHint.Visibility = Visibility.Visible;
		}
	}

	private void StepGatheringTextBox_TextChanged(object sender, TextChangedEventArgs e)
		=> FilteringExpressionInvalidHint.Visibility = Visibility.Collapsed;

	private async void GatherButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var grid = BasePage.SudokuPane.Puzzle;
		if (!grid.IsValid)
		{
			return;
		}

		GatherButton.IsEnabled = false;
		BasePage.IsGathererLaunched = true;
		TechniqueGroupView.ClearViewSource();

		var textFormat = ResourceDictionary.Get("AnalyzePage_AnalyzerProgress", App.CurrentCulture);
		using var cts = new CancellationTokenSource();
		var uiPref = ((App)Application.Current).Preference.UIPreferences;
		var analysisPref = ((App)Application.Current).Preference.AnalysisPreferences;
		var collector = ((App)Application.Current)
			.Collector
			.WithMaxSteps(analysisPref.StepGathererMaxStepsGathered)
			.WithCulture(App.CurrentCulture)
			.WithSameLevelConfigruation((CollectorDifficultyLevelMode)analysisPref.DifficultyLevelMode)
			.WithStepSearchers(((App)Application.Current).GetStepSearchers())
			.WithRuntimeIdentifierSetters(BasePage.SudokuPane)
			.WithUserDefinedOptions(App.CreateStepSearcherOptions());
		BasePage._ctsForAnalyzingRelatedOperations = cts;

		try
		{
			switch (await Task.Run(() =>
			{
				lock (AnalyzingRelatedSyncRoot)
				{
					return collector.Collect(in grid, new Progress<AnalyzerProgress>(progress => DispatcherQueue.TryEnqueue(() =>
					{
						var (stepSearcherName, percent) = progress;
						BasePage.ProgressPercent = percent * 100;
						BasePage.AnalyzeProgressLabel.Text = string.Format(textFormat, percent);
						BasePage.AnalyzeStepSearcherNameLabel.Text = stepSearcherName;
					})), cts.Token).ToArray();
				}
			}))
			{
				case { } result:
				{
					_currentFountSteps = result;
					TechniqueGroupView.TechniqueGroups.Source = GetTechniqueGroups(result, grid);
					break;
				}
			}
		}
		catch (TaskCanceledException)
		{
		}
		finally
		{
			BasePage._ctsForAnalyzingRelatedOperations = null;
			GatherButton.IsEnabled = true;
			BasePage.IsGathererLaunched = false;
		}
	}
}
