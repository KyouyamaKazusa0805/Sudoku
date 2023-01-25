namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines a summary page.
/// </summary>
public sealed partial class Summary : Page, IAnalyzeTabPage
{
	/// <summary>
	/// Indicates the analysis result.
	/// </summary>
	[NotifyBackingField(DoNotEmitPropertyChangedEventTrigger = true)]
	[NotifyCallback(nameof(AnalysisResultSetterAfter))]
	private LogicalSolverResult? _analysisResult;


	/// <summary>
	/// Initializes a <see cref="Summary"/> instance.
	/// </summary>
	public Summary() => InitializeComponent();


	/// <summary>
	/// Indicates the base page.
	/// </summary>
	public AnalyzePage BasePage { get; set; } = null!;


	private void AnalysisResultSetterAfter(LogicalSolverResult? value)
		=> SummaryTable.ItemsSource = value is null ? null : AnalysisResultTableRow.CreateListFrom(value);
}
