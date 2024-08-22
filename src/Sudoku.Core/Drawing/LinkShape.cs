namespace Sudoku.Drawing;

/// <summary>
/// Represents a link shape.
/// </summary>
public enum LinkShape
{
	/// <summary>
	/// Indicates the link is inside a chain.
	/// </summary>
	Chain,

	/// <summary>
	/// Indicates the link is between two cells.
	/// </summary>
	Cell,

	/// <summary>
	/// Indicates the link is inside a conjugate pair.
	/// </summary>
	ConjugatePair
}
