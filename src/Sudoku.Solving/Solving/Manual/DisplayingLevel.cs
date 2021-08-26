namespace Sudoku.Solving.Manual;

/// <summary>
/// Indicates the difficulty level of the step.
/// This enumeration type is used for the displaying of the step information list.
/// </summary>
[Closed]
public enum DisplayingLevel : byte
{
	/// <summary>
	/// Indicates the level is none.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	None = 0,

	/// <summary>
	/// Indicates the level is easy (Level A).
	/// </summary>
	A,

	/// <summary>
	/// Indicates the level is medium (Level B).
	/// </summary>
	B,

	/// <summary>
	/// Indicates the level is hard (Level C).
	/// </summary>
	C,

	/// <summary>
	/// Indicates the level is fiendish (Level D).
	/// </summary>
	D,

	/// <summary>
	/// Indicates the level is nightmare (Level E).
	/// </summary>
	E
}
