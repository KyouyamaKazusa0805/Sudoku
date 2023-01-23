namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines the solving path page.
/// </summary>
public sealed partial class SolvingPath : Page, IAnalyzeTabPage
{
	/// <summary>
	/// Indicates the analysis result.
	/// </summary>
	private LogicalSolverResult? _analysisResult;


	/// <summary>
	/// Initializes a <see cref="SolvingPath"/> instance.
	/// </summary>
	public SolvingPath() => InitializeComponent();


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

			SolvingPathList.ItemsSource = value?.Steps.ToList();
		}
	}
}
