namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Represents a mode that describes an area where a step searcher can be invoked.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
[Flags]
public enum StepSearcherRunningArea
{
	/// <summary>
	/// Indicates the step searcher is disabled.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the mode is normal searching.
	/// </summary>
	Searching = 1 << 0,

	/// <summary>
	/// Indicates the mode is collecting.
	/// </summary>
	Collecting = 1 << 1
}
