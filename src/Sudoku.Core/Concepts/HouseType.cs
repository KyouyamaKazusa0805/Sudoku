namespace Sudoku.Concepts;

/// <summary>
/// Represents a house type.
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
