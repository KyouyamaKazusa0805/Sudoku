namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents a rule that describes for the just-one-cell puzzles only produce conclusions in row 5, column 5 and block 5.
/// </summary>
public enum GridAlignment
{
	/// <summary>
	/// Indicates the rule is not limited.
	/// </summary>
	NotLimited,

	/// <summary>
	/// Indicates the rule is limited in row 5, column 5 and block 5. However, the conclusion can be outside the block 5
	/// to display other positions like some cells adjacent to grid borders.
	/// </summary>
	CenterHouses,

	/// <summary>
	/// Indicates the rule is limited in only block 5. No matter what technique used, the conclusion must be inside the block 5.
	/// </summary>
	CenterBlock,

	/// <summary>
	/// Indicates the conclusion can only be placed in <c>r5c5</c>.
	/// </summary>
	OnlyCenterCell
}
