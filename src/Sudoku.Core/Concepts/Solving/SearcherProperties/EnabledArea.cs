namespace Sudoku.Concepts.Solving.SearcherProperties;

/// <summary>
/// Represents the data about where a step searcher can be enabled and used.
/// </summary>
[Flags]
public enum EnabledArea : byte
{
	/// <summary>
	/// Indicates all modes are disabled.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	None = 0,

	/// <summary>
	/// Indicates the default mode (Sudoku Explainer or Hodoku mode).
	/// </summary>
	Default = 1,

	/// <summary>
	/// Indicates the gathering mode (All steps will be found here).
	/// </summary>
	Gathering = 1 << 1,

	/// <summary>
	/// Indicates the experimental function can use the step searcher.
	/// </summary>
	Experimental = 1 << 2
}
