namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines a summary page.
/// </summary>
public sealed partial class Summary : Page, IAnalyzeTabPage
{
	/// <summary>
	/// Initializes a <see cref="Summary"/> instance.
	/// </summary>
	public Summary() => InitializeComponent();


	/// <summary>
	/// Indicates the base page.
	/// </summary>
	public AnalyzePage BasePage { get; set; } = null!;


	/// <inheritdoc/>
	public void ClearTabPageData() => SummaryTable.ItemsSource = null;

	/// <inheritdoc/>
	public void UpdateTabPageData(LogicalSolverResult analysisResult)
		=> SummaryTable.ItemsSource = AnalysisResultTableRow.CreateListFrom(analysisResult);
}
