namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines a summary page.
/// </summary>
public sealed partial class Summary : Page, IAnalyzerTab
{
	/// <summary>
	/// Initializes a <see cref="Summary"/> instance.
	/// </summary>
	public Summary() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;

	/// <summary>
	/// Indicates the analysis result.
	/// </summary>
	[AutoDependencyProperty]
	public partial AnalysisResult? AnalysisResult { get; set; }


	[Callback]
	private static void AnalysisResultPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is not (Summary page, { NewValue: var rawValue and (null or Sudoku.Analytics.AnalysisResult) }))
		{
			return;
		}

		page.SummaryTable.ItemsSource = rawValue is AnalysisResult value ? SummaryViewBindableSource.CreateListFrom(value) : null;
	}
}
