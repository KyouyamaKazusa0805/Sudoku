namespace SudokuStudio.Views.Interaction;

/// <summary>
/// Defines a page that is used for displaying analysis result for a puzzle.
/// </summary>
public interface IAnalyzeTabPage
{
	/// <summary>
	/// To clear tab page data.
	/// </summary>
	void ClearTabPageData();

	/// <summary>
	/// To update tab page data via the specified <see cref="LogicalSolverResult"/> instance.
	/// </summary>
	/// <param name="analysisResult">The instance to be displayed.</param>
	void UpdateTabPageData(LogicalSolverResult analysisResult);
}
