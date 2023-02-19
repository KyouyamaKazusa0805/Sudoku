namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines the solving path page.
/// </summary>
[DependencyProperty<StepTooltipDisplayKind>("StepTooltipDisplayKind", DefaultValue = StepTooltipDisplayKind.TechniqueName | StepTooltipDisplayKind.DifficultyRating | StepTooltipDisplayKind.SimpleDescription, DocSummary = "Indicates the tooltip display kind.")]
[DependencyProperty<LogicalSolverResult>("AnalysisResult", IsNullable = true, DocSummary = "Indicates the analysis result.")]
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
		if (
#pragma warning disable format
			(d, e) is not (
				SolvingPath { StepTooltipDisplayKind: var kind, SolvingPathList: var pathListView } path,
				{ NewValue: var value and (null or LogicalSolverResult) }
			)
#pragma warning restore format
		)
		{
			return;
		}

		pathListView.ItemsSource = value is LogicalSolverResult analysisResult
			? SolvingPathStepCollection.Create(analysisResult, kind)
			: null;
	}


	private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
	{
		if (sender is not ListViewItem { Tag: SolvingPathStep { StepGrid: var stepGrid, Step: var step } })
		{
			return;
		}

		BasePage.SudokuPane.SetPuzzle(stepGrid, clearStack: true, clearAnalyzeTabData: false);
		BasePage.CurrentViewIndex = -1;
		BasePage.VisualUnit = step;
	}
}
