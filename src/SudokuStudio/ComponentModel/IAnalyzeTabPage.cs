namespace SudokuStudio.ComponentModel;

/// <summary>
/// Defines a page that is used for displaying analysis result for a puzzle.
/// </summary>
public interface IAnalyzeTabPage
{
	/// <summary>
	/// Indicates the parent page of the current tab.
	/// </summary>
	public abstract AnalyzePage BasePage { get; set; }

	/// <summary>
	/// Gets or sets the analysis result, updating the current tab page using this property.
	/// </summary>
	/// <value>The value you want to update. If <see langword="null"/>, clears the page data.</value>
	public abstract AnalyzerResult? AnalysisResult { get; set; }
}
