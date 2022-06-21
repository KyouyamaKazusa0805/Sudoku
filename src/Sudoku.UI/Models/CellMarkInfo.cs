namespace Sudoku.UI.Models;

/// <summary>
/// Indicates the cell mark info that describes a single cell.
/// </summary>
internal struct CellMarkInfo
{
	/// <summary>
	/// Indicates the cell index.
	/// </summary>
	public required int CellIndex { get; set; }

	/// <summary>
	/// Indicates the shape kind raw value.
	/// </summary>
	public required int ShapeKindRawValue { get; set; }
}
