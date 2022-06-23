#if AUTHOR_FEATURE_CELL_MARKS

namespace Sudoku.UI.Models;

/// <summary>
/// Indicates the cell mark info that describes a single cell.
/// </summary>
[AutoDeconstruction(nameof(CellIndex), nameof(ShapeKindRawValue))]
internal partial struct CellMarkInfo
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

#endif