namespace Sudoku.Concepts;

/// <summary>
/// Represents an option that controls the link collecting rules.
/// </summary>
public enum LinkOption
{
	/// <summary>
	/// Indicates link collecting method only collects for node lying in an intersection.
	/// </summary>
	Intersection,

	/// <summary>
	/// Indicates link collecting method only collects for node lying in one row or one column.
	/// </summary>
	Line,

	/// <summary>
	/// Indicates link collecting method only collects for node lying in one house.
	/// </summary>
	House,

	/// <summary>
	/// Indicates link collecting method collects for all possible nodes, regardless of position.
	/// </summary>
	All
}
