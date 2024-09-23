namespace Sudoku.Concepts;

/// <summary>
/// Represents a direction between two adjacent cells.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
/// <seealso cref="FlagsAttribute"/>
[Flags]
public enum AdjacentCellDirection
{
	/// <summary>
	/// Indicates the default value.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the direction is up.
	/// </summary>
	Up = 1 << 0,

	/// <summary>
	/// Indicates the direction is down.
	/// </summary>
	Down = 1 << 1,

	/// <summary>
	/// Indicates the direction is left.
	/// </summary>
	Left = 1 << 2,

	/// <summary>
	/// Indicates the direction is right.
	/// </summary>
	Right = 1 << 3
}
