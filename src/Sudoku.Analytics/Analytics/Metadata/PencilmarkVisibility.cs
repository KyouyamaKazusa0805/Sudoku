namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents a mode that describes the puzzle can be solved with what kind of pencilmark-displaying rule applied.
/// </summary>
[Flags]
public enum PencilmarkVisibility
{
	/// <summary>
	/// Indicates the technique can be applied to direct mode.
	/// </summary>
	Direct = 1,

	/// <summary>
	/// Indicates the technique can be applied to indirect mode.
	/// </summary>
	Indirect = 1 << 1
}
