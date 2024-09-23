namespace Sudoku.Analytics.Primitives;

/// <summary>
/// Represents a mode that describes the puzzle can be solved with what kind of pencilmark-displaying rule applied.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
/// <completionlist cref="PencilmarkVisibilities"/>
[Flags]
public enum PencilmarkVisibility
{
	/// <summary>
	/// Indicates placeholder.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the technique can be applied to direct mode.
	/// </summary>
	Direct = 1 << 0,

	/// <summary>
	/// Indicates the technique can be applied to partial-marking mode.
	/// </summary>
	PartialMarking = 1 << 1,

	/// <summary>
	/// Indicates the technique can be applied to full-marking mode.
	/// </summary>
	FullMarking = 1 << 2
}
