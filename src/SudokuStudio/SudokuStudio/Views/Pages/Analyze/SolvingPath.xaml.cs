namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines the solving path page.
/// </summary>
public sealed partial class SolvingPath : Page, IAnalyzeTabPage, INotifyPropertyChanged
{
	/// <summary>
	/// Indicates the analysis result.
	/// </summary>
	[NotifyBackingField(DoNotEmitPropertyChangedEventTrigger = true)]
	[NotifyCallback(nameof(AnalysisResultSetterAfter))]
	private LogicalSolverResult? _analysisResult;

	/// <summary>
	/// Indicates the tooltip display kind.
	/// </summary>
	[NotifyBackingField]
	private StepTooltipDisplayKind _stepTooltipDisplayKind = StepTooltipDisplayKind.TechniqueName
		| StepTooltipDisplayKind.DifficultyRating
		| StepTooltipDisplayKind.SimpleDescription;


	/// <summary>
	/// Initializes a <see cref="SolvingPath"/> instance.
	/// </summary>
	public SolvingPath() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	private void AnalysisResultSetterAfter(LogicalSolverResult? value)
		=> SolvingPathList.ItemsSource = value is null ? null : SolvingPathStepCollection.Create(value, StepTooltipDisplayKind);


	private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
	{
		if ((this, sender) is not ({ BasePage.SudokuPane: var pane }, ListViewItem { Tag: SolvingPathStep(_, var stepGrid, _, _) }))
		{
			return;
		}

		pane.SetPuzzle(stepGrid, clearStack: true, clearAnalyzeTabData: false);
	}
}
