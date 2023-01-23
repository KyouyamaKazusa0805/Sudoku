namespace SudokuStudio.Views.Interaction;

/// <summary>
/// Defines a page that is used for displaying analysis result for a puzzle.
/// </summary>
public interface IAnalyzeTabPage
{
	/// <summary>
	/// Gets or sets the analysis result, updating the current tab page using this property.
	/// </summary>
	/// <value>The value you want to update. If <see langword="null"/>, clears the page data.</value>
	LogicalSolverResult? AnalysisResult { get; set; }
}
