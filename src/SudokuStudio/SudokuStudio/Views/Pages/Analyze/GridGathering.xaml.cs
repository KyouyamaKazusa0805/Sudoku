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
	internal static ObservableCollection<TechniqueGroup> GetTechniqueGroups(IEnumerable<IStep> collection)
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
			select new TechniqueGroup(stepGroupGroupedByName) { Key = stepGroupGroupedByName.Key }
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
}
