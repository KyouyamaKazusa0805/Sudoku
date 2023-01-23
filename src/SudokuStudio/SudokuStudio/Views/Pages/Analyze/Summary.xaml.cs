namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines a summary page.
/// </summary>
public sealed partial class Summary : Page, IAnalyzeTabPage
{
	/// <summary>
	/// Indicates the analysis result.
	/// </summary>
	private LogicalSolverResult? _analysisResult;


	/// <summary>
	/// Initializes a <see cref="Summary"/> instance.
	/// </summary>
	public Summary() => InitializeComponent();


	/// <summary>
	/// Indicates the base page.
	/// </summary>
	public AnalyzePage BasePage { get; set; } = null!;

	/// <inheritdoc/>
	public LogicalSolverResult? AnalysisResult
	{
		get => _analysisResult;

		set
		{
			if (_analysisResult == value)
			{
				return;
			}

			_analysisResult = value;

			SummaryTable.ItemsSource = value is null ? null : AnalysisResultTableRow.CreateListFrom(value);
		}
	}
}
