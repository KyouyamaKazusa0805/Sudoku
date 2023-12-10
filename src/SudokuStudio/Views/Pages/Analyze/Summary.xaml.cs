using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sudoku.Analytics;
using SudokuStudio.BindableSource;
using SudokuStudio.ComponentModel;

namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines a summary page.
/// </summary>
[DependencyProperty<AnalyzerResult>("AnalysisResult?", DocSummary = "Indicates the analysis result.")]
public sealed partial class Summary : Page, IAnalyzeTabPage
{
	/// <summary>
	/// Initializes a <see cref="Summary"/> instance.
	/// </summary>
	public Summary() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;


	[Callback]
	private static void AnalysisResultPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is not (Summary page, { NewValue: var rawValue and (null or AnalyzerResult) }))
		{
			return;
		}

		page.SummaryTable.ItemsSource = rawValue is AnalyzerResult value ? SummaryViewBindableSource.CreateListFrom(value) : null;
	}
}
