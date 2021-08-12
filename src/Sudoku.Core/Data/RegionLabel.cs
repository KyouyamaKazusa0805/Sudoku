namespace Sudoku.Data;

/// <summary>
/// Indicates the region label.
/// </summary>
public enum RegionLabel : byte
{
	/// <summary>
	/// Indicates the block.
	/// </summary>
	Block,

	/// <summary>
	/// Indicates the row.
	/// </summary>
	Row,

	/// <summary>
	/// Indicates the column.
	/// </summary>
	Column
}
