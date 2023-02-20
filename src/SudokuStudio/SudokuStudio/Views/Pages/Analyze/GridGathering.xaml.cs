namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines the gathering page.
/// </summary>
public sealed partial class GridGathering : Page, IAnalyzeTabPage
{
	/// <summary>
	/// Indicates the found steps currently.
	/// </summary>
	internal IEnumerable<IStep>? _currentFountSteps;


	/// <summary>
	/// Initializes a <see cref="GridGathering"/> instance.
	/// </summary>
	public GridGathering() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;

	/// <inheritdoc/>
	LogicalSolverResult? IAnalyzeTabPage.AnalysisResult
	{
		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => throw new NotSupportedException();

		set
		{
		}
	}


	/// <summary>
	/// Converts the specified collection into the target view source collection.
	/// </summary>
	/// <param name="collection">The raw collection.</param>
	/// <returns>The collection that can be used as view source.</returns>
	internal static ObservableCollection<TechniqueGroupModel> GetTechniqueGroups(IEnumerable<IStep> collection)
	{
		return new(
			from step in collection
			group step by step.Name into stepGroupGroupedByName
			let showDifficultySteps = from step in stepGroupGroupedByName where step.ShowDifficulty select step
			let stepsDifficultyLevelIntegerGroup = from step in stepGroupGroupedByName select (decimal)step.DifficultyLevel
			orderby
				showDifficultySteps.Average(difficultySelector),
				stepsDifficultyLevelIntegerGroup.Average(),
				stepGroupGroupedByName.Key
			select new TechniqueGroupModel(stepGroupGroupedByName) { Key = stepGroupGroupedByName.Key }
		);


		static decimal difficultySelector(IStep step) => step.Difficulty;
	}


	private void TechniqueGroupView_StepChosen(object sender, TechniqueGroupViewStepChosenEventArgs e)
		=> BasePage.VisualUnit = e.ChosenStep;

	private void FilterGatheredStepsButton_Click(object sender, RoutedEventArgs e)
	{
		var puzzle = BasePage.SudokuPane.Puzzle;
		if (_currentFountSteps is null || !puzzle.IsValid())
		{
			return;
		}

		try
		{
			var filtered = TechniqueFiltering.Filter(_currentFountSteps, StepGatheringTextBox.Text);
			TechniqueGroupView.TechniqueGroups.Source = GetTechniqueGroups(filtered);
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
		if (!grid.IsValid())
		{
			return;
		}

		BasePage.GatherTabPage.GatherButton.IsEnabled = false;
		BasePage.IsGathererLaunched = true;
		BasePage.GatherTabPage.TechniqueGroupView.ClearViewSource();

		var textFormat = GetString("AnalyzePage_AnalyzerProgress");

		var gatherer = ((App)Application.Current).ProgramGatherer;
		var result = await Task.Run(gather);

		BasePage.GatherTabPage._currentFountSteps = result;
		BasePage.GatherTabPage.TechniqueGroupView.TechniqueGroups.Source = GetTechniqueGroups(result);
		BasePage.GatherTabPage.GatherButton.IsEnabled = true;
		BasePage.IsGathererLaunched = false;


		IEnumerable<IStep> gather()
		{
			lock (App.SyncRoot)
			{
				return gatherer.Search(grid, new Progress<double>(progressReportHandler));
			}


			void progressReportHandler(double percent)
			{
				DispatcherQueue.TryEnqueue(updatePercentValueCallback);


				void updatePercentValueCallback()
				{
					BasePage.ProgressPercent = percent * 100;
					BasePage.AnalyzeProgressLabel.Text = string.Format(textFormat!, percent);
				}
			}
		}
	}
}
