namespace Sudoku.Behaviors;

/// <summary>
/// Represents a difference type.
/// </summary>
[IntroducedSince(3, 4)]
public enum DiffType
{
	/// <summary>
	/// Indicates two grids are same.
	/// </summary>
	NothingChanged,

	/// <summary>
	/// Indicates given digits are added.
	/// </summary>
	AddGiven,

	/// <summary>
	/// Indicates modifiable digits are added.
	/// </summary>
	AddModifiable,

	/// <summary>
	/// Indicates candidate digits are added.
	/// </summary>
	AddCandidate,

	/// <summary>
	/// Indicates given digits are removed.
	/// </summary>
	RemoveGiven,

	/// <summary>
	/// Indicates modifiable digits are removed.
	/// </summary>
	RemoveModifiable,

	/// <summary>
	/// Indicates candidate digits are removed.
	/// </summary>
	RemoveCandidate,

	/// <summary>
	/// Indicates the second grid is the reset grid of the first one.
	/// </summary>
	Reset,
}
