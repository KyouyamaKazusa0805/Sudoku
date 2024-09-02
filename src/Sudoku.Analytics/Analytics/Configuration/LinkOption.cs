namespace Sudoku.Analytics.Configuration;

/// <summary>
/// Represents an option that controls the link collecting rules.
/// </summary>
public enum LinkOption
{
	/// <summary>
	/// Indicates the link collecting method doesn't check for the current link type,
	/// i.e. it will be skipped to be searched.
	/// </summary>
	None,

	/// <summary>
	/// Indicates link collecting method only collects for node lying in an intersection.
	/// </summary>
	Intersection,

	/// <summary>
	/// Indicates link collecting method only collects for node lying in one house.
	/// </summary>
	House,

	/// <summary>
	/// Indicates link collecting method collects for all possible nodes, regardless of position.
	/// </summary>
	All
}
