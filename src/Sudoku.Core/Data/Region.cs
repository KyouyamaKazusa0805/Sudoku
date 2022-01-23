namespace Sudoku.Data;

/// <summary>
/// Defines a region type.
/// </summary>
public enum Region : byte
{
	/// <summary>
	/// Indicates the region type is a block.
	/// </summary>
	Block,

	/// <summary>
	/// Indicates the region type is a row.
	/// </summary>
	Row,

	/// <summary>
	/// Indicates the region type is a column.
	/// </summary>
	Column
}
