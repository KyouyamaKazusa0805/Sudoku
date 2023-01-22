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


	private async void AnalyzeButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var puzzle = BasePage.SudokuPane.Puzzle;
		if (!puzzle.IsValid())
		{
			return;
		}

		AnalyzeButton.IsEnabled = false;

		ClearTabPageData();

		var solver = ((App)Application.Current).RunningContext.Solver;
		var analysisResult = await Task.Run(() => { lock (BasePage.AnalyzeSyncRoot) { return solver.Solve(puzzle); } });

		AnalyzeButton.IsEnabled = true;

		switch (analysisResult)
		{
			case { IsSolved: true }:
			{
				SummaryTable.ItemsSource = AnalysisResultTableRow.CreateListFrom(analysisResult);

				break;
			}
#if false
			case
			{
				WrongStep: _,
				FailedReason: _,
				UnhandledException: WrongStepException { CurrentInvalidGrid: _ }
			}:
			{
				break;
			}
			case { FailedReason: _, UnhandledException: _ }:
			{
				break;
			}
#endif
		}
	}
}
