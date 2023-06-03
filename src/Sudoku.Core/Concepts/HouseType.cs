namespace Sudoku.Concepts;

/// <summary>
/// Represents a <see href="https://sunnieshine.github.io/Sudoku/terms/house">house type</see>.
/// </summary>
public enum HouseType : byte
{
	/// <summary>
	/// Indicates the house type is a block.
	/// </summary>
	Block,

	/// <summary>
	/// Indicates the house type is a row.
	/// </summary>
	Row,

	/// <summary>
	/// Indicates the house type is a column.
	/// </summary>
	Column
}
