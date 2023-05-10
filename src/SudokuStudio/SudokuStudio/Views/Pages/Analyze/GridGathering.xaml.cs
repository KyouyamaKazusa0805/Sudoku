namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines the gathering page.
/// </summary>
public sealed partial class GridGathering : Page, IAnalyzeTabPage
{
	/// <summary>
	/// Indicates the found steps currently.
	/// </summary>
	internal IEnumerable<Step>? _currentFountSteps;


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
	internal ObservableCollection<TechniqueGroupBindableSource> GetTechniqueGroups(IEnumerable<Step> collection, Grid grid)
	{
		var displayItems = ((App)Application.Current).Preference.UIPreferences.StepDisplayItems;
		return new(
			from step in collection
			group step by step.Name into stepGroupGroupedByName
			let techniqueName = stepGroupGroupedByName.Key
			orderby
				stepGroupGroupedByName.Average(static step => step.Difficulty),
				stepGroupGroupedByName.Average(static step => (byte)step.DifficultyLevel),
				techniqueName
			let groupedBindableSource =
				from step in stepGroupGroupedByName
				select new SolvingPathStepBindableSource { DisplayItems = displayItems, Step = step, StepGrid = grid }
			select new TechniqueGroupBindableSource(groupedBindableSource) { Key = techniqueName }
		);
	}


	private void TechniqueGroupView_StepChosen(object sender, TechniqueGroupViewStepChosenEventArgs e) => BasePage.VisualUnit = e.ChosenStep;

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

		var textFormat = GetString("AnalyzePage_AnalyzerProgress");

		var collector = ((App)Application.Current)
			.StepCollector
			.WithMaxSteps(((App)Application.Current).Preference.AnalysisPreferences.StepGathererMaxStepsGathered)
			.WithSameLevelConfigruation(((App)Application.Current).Preference.AnalysisPreferences.StepGathererOnlySearchSameLevelTechniquesInFindAllSteps)
			.WithStepSearchers(((App)Application.Current).GetStepSearchers())
			.WithRuntimeIdentifierSetters(BasePage.SudokuPane);

		var result = await Task.Run(
			() =>
			{
				lock (StepSearchingOrGatheringSyncRoot)
				{
					return collector.Search(
						grid,
						new Progress<double>(
							percent =>
								DispatcherQueue.TryEnqueue(
									() =>
									{
										BasePage.ProgressPercent = percent * 100;
										BasePage.AnalyzeProgressLabel.Text = string.Format(textFormat, percent);
									}
								)
						)
					);
				}
			}
		);

		_currentFountSteps = result;
		TechniqueGroupView.TechniqueGroups.Source = GetTechniqueGroups(result, grid);
		GatherButton.IsEnabled = true;
		BasePage.IsGathererLaunched = false;
	}
}
