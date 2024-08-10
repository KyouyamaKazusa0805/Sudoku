namespace Sudoku.Analytics;

/// <summary>
/// Represents a mode that describes the puzzle can be solved with what kind of pencilmark-displaying rule applied.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
[Flags]
public enum PencilmarkVisibility
{
	/// <summary>
	/// Indicates the technique can be applied to direct mode.
	/// </summary>
	Direct = 1 << 0,

	/// <summary>
	/// Indicates the technique can be applied to indirect mode.
	/// </summary>
	Indirect = 1 << 1
}
