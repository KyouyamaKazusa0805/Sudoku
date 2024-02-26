namespace Sudoku.Strategying;

/// <summary>
/// Represents a between rule.
/// </summary>
public enum BetweenRule
{
	/// <summary>
	/// Indicates both left and right are open (not included).
	/// </summary>
	BothOpen,

	/// <summary>
	/// Indicates only the left is open, and the right is closed.
	/// </summary>
	LeftOpen,

	/// <summary>
	/// Indicates only the right is open, and the left is closed.
	/// </summary>
	RightOpen,

	/// <summary>
	/// Indicates both left and right are closed (included).
	/// </summary>
	BothClosed
}
