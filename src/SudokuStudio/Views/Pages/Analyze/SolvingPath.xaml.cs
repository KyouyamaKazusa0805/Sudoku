namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines the solving path page.
/// </summary>
public sealed partial class SolvingPath : Page, IAnalyzerTab
{
	/// <summary>
	/// Initializes a <see cref="SolvingPath"/> instance.
	/// </summary>
	public SolvingPath() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;

	/// <summary>
	/// Indicates the analysis result.
	/// </summary>
	[DependencyProperty]
	public partial AnalysisResult? AnalysisResult { get; set; }


	[Callback]
	private static void AnalysisResultPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (SolvingPath { SolvingPathList: var pathListView }, { NewValue: var value and (null or Sudoku.Analytics.AnalysisResult) }))
		{
			if (value is not AnalysisResult analysisResult)
			{
				pathListView.ItemsSource = null;
				return;
			}

			var displayItems = ((App)Application.Current).Preference.UIPreferences.StepDisplayItems;
			var collection = new ObservableCollection<SolvingPathStepBindableSource>();
			pathListView.ItemsSource = collection;
			foreach (var item in SolvingPathStepCollection.Create(analysisResult, displayItems))
			{
				collection.Add(item);
			}
		}
	}


	private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
	{
		if (sender is not ListViewItem { Tag: SolvingPathStepBindableSource { InterimGrid: var stepGrid, InterimStep: var step } })
		{
			return;
		}

		BasePage.SudokuPane.Puzzle = stepGrid;
		BasePage.CurrentViewIndex = -1;
		BasePage.VisualUnit = step;
	}
}
