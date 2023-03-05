namespace SudokuStudio.Models;

/// <summary>
/// Defines a data used for routing in analyzer tabs.
/// </summary>
internal sealed class AnalyzeTabPageData
{
	/// <summary>
	/// Indicates the header.
	/// </summary>
	public required string Header { get; init; }

	/// <summary>
	/// Indicates the icon source.
	/// </summary>
	public required IconSource IconSource { get; init; }

	/// <summary>
	/// Indicates the page instance.
	/// </summary>
	public required IAnalyzeTabPage TabPage { get; init; }
}
