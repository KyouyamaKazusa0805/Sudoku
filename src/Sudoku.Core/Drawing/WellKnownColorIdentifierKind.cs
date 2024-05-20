namespace Sudoku.Drawing;

/// <summary>
/// Represents a kind of well-known <see cref="ColorIdentifier"/> instance.
/// </summary>
/// <seealso cref="ColorIdentifier"/>
public enum WellKnownColorIdentifierKind
{
	/// <summary>
	/// Indicates the normal color.
	/// </summary>
	Normal,

	/// <summary>
	/// Indicates the first auxiliary color.
	/// </summary>
	Auxiliary1,

	/// <summary>
	/// Indicates the second auxiliary color.
	/// </summary>
	Auxiliary2,

	/// <summary>
	/// Indicates the third auxiliary color.
	/// </summary>
	Auxiliary3,

	/// <summary>
	/// Indicates the assignment color.
	/// </summary>
	Assignment,

	/// <summary>
	/// Indicates the overlapped assignment color.
	/// </summary>
	OverlappedAssignment,

	/// <summary>
	/// Indicates the elimination color.
	/// </summary>
	Elimination,

	/// <summary>
	/// Indicates the exo-fin color.
	/// </summary>
	Exofin,

	/// <summary>
	/// Indicates the endo-fin color.
	/// </summary>
	Endofin,

	/// <summary>
	/// Indicates the cannibalism color.
	/// </summary>
	Cannibalism,

	/// <summary>
	/// Indicates the link color.
	/// </summary>
	Link,

	/// <summary>
	/// Indicates the first ALS recorded.
	/// </summary>
	AlmostLockedSet1,

	/// <summary>
	/// Indicates the second ALS recorded.
	/// </summary>
	AlmostLockedSet2,

	/// <summary>
	/// Indicates the third ALS recorded.
	/// </summary>
	AlmostLockedSet3,

	/// <summary>
	/// Indicates the fourth ALS recorded.
	/// </summary>
	AlmostLockedSet4,

	/// <summary>
	/// Indicates the fifth ALS recorded.
	/// </summary>
	AlmostLockedSet5
}
