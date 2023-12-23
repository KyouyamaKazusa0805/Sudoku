namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines the solving path page.
/// </summary>
[DependencyProperty<AnalyzerResult>("AnalysisResult?", DocSummary = "Indicates the analysis result.")]
public sealed partial class SolvingPath : Page, IAnalyzeTabPage
{
	/// <summary>
	/// Initializes a <see cref="SolvingPath"/> instance.
	/// </summary>
	public SolvingPath() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;


	[Callback]
	private static void AnalysisResultPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (SolvingPath { SolvingPathList: var pathListView }, { NewValue: var value and (null or AnalyzerResult) }))
		{
			pathListView.ItemsSource = value is AnalyzerResult analyzerResult
				? SolvingPathStepCollection.Create(analyzerResult, ((App)Application.Current).Preference.UIPreferences.StepDisplayItems)
				: null;
		}
	}


	private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
	{
		if (sender is not ListViewItem { Tag: SolvingPathStepBindableSource { StepGrid: var stepGrid, Step: var step } })
		{
			return;
		}

		BasePage.SudokuPane.Puzzle = stepGrid;

		BasePage.CurrentViewIndex = -1;
		BasePage.VisualUnit = step;
	}
}
