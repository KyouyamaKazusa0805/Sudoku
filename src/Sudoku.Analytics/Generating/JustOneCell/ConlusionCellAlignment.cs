namespace Sudoku.Generating;

/// <summary>
/// Represents a rule that describes for the just-one-cell puzzles only produce conclusions in row 5, column 5 and block 5.
/// </summary>
public enum ConlusionCellAlignment
{
	/// <summary>
	/// Indicates conclusion cell can be everywhere.
	/// </summary>
	NotLimited,

	/// <summary>
	/// Indicates conclusion cell can only be inside row 5, column 5 and block 5.
	/// </summary>
	CenterHouse,

	/// <summary>
	/// Indicates conclusion cell can only be inside block 5.
	/// </summary>
	CenterBlock,

	/// <summary>
	/// Indicates the conclusion cell must be <c>r5c5</c>.
	/// </summary>
	CenterCell
}
